using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.DAL.DAO.BaseClasses;

namespace Logictracker.Organizacion
{
    public partial class AltaPerfil : SecuredAbmPage<Perfil>
    {
        protected override string RedirectUrl { get { return "PerfilLista.aspx"; } }
        protected override string VariableName { get { return "SOC_PERFILES"; } }
        protected override string GetRefference() { return "PERFIL"; }

        #region Private Properties

        /// <summary>
        /// Determines if the page should redirect to home on save.
        /// </summary>
        private bool _home;

        /// <summary>
        /// A list of movmenus for the current profile.
        /// </summary>
        private List<MovMenu> _menues = new List<MovMenu>();

        /// <summary>
        /// Dictionary of function and profile mappings for the current profile being edited.
        /// </summary>
        private readonly Dictionary<int, MovMenu> _movmenus = new Dictionary<int, MovMenu>();

        private Dictionary<int, short> Orders
        {
            get { return (Dictionary<int, short>)(ViewState["FunctionOrders"] ?? new Dictionary<int, short>()); }
            set { ViewState["FunctionOrders"] = value; }
        }
        private Dictionary<int, bool[]> Access
        {
            get { return (Dictionary<int, bool[]>)(ViewState["Access"] ?? new Dictionary<int, bool[]>()); }
            set { ViewState["Access"] = value; }
        }
        private Dictionary<int, string> Sistemas
        {
            get { return (Dictionary<int, string>)(ViewState["Sistemas"] ?? new Dictionary<int, string>()); }
            set { ViewState["Sistemas"] = value; }
        }

        #endregion

        #region Protected Methods

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            GetGridSource();

            BindReorder();

            if(!IsPostBack) BindPermisos();
        }

        private void BindPermisos()
        {
            chkPermisos.Items.Clear();
            var asegurables = DAOFactory.AsegurableDAO.FindAll().ToList().OrderBy(a => a.GetDescripcion());
            foreach (var asegurable in asegurables)
            {
                if (WebSecurity.AuthenticatedUser.AccessLevel >= Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin || WebSecurity.IsSecuredAllowed(asegurable.Referencia))
                    chkPermisos.Items.Add(new ListItem(asegurable.GetDescripcion(), asegurable.Id.ToString("#0")));
            }
        }

        private void BindReorder()
        {
            panReorder.Controls.Clear();

            var orders = Orders;
            var access = Access;
            var sistemas = Sistemas;
            var d = new Dictionary<string, ReorderList>();
            var sysdone = new List<int>();

            foreach (var menu in _menues)
            {
                var key = GetSysKey(menu.Funcion);

                if (!Sistemas.ContainsKey(menu.Funcion.Id)) sistemas.Add(menu.Funcion.Id, key);

                if (!orders.ContainsKey(menu.Funcion.Id)) orders.Add(menu.Funcion.Id, menu.Orden);
                else menu.Orden = orders[menu.Funcion.Id];

                if (!access.ContainsKey(menu.Funcion.Id)) access.Add(menu.Funcion.Id, new[]{menu.Alta, menu.Modificacion, menu.Baja, menu.Consulta, menu.Reporte, menu.VerMapa});
                else
                {
                    menu.Alta = access[menu.Funcion.Id][0];
                    menu.Modificacion = access[menu.Funcion.Id][1];
                    menu.Baja = access[menu.Funcion.Id][2];
                    menu.Consulta = access[menu.Funcion.Id][3];
                    menu.Reporte = access[menu.Funcion.Id][4];
                    menu.VerMapa = access[menu.Funcion.Id][5];
                }

                if(!sysdone.Contains(menu.Funcion.Sistema.Id))
                {
                    panReorder.Controls.Add(new Literal { Text = string.Format("<table class='reorder_header'><tr><td>{0}</td><td style='width: 50px;'>Alta</td><td style='width: 50px;'>Mod.</td><td style='width: 50px;'>Baja</td><td style='width: 50px;'>Cons.</td><td style='width: 50px;'>Impr.</td><td style='width: 50px;'>Mapa</td></tr></table>", CultureManager.GetMenu(menu.Funcion.Sistema.Descripcion)) });
                }

                if (d.ContainsKey(key)) continue;

                var rol = new ReorderList
                              {
                                  ID = ("Reorder_" + key),
                                  AllowReorder = true,
                                  DragHandleAlignment = ReorderHandleAlignment.Left,
                                  SortOrderField = "Orden",
                                  DragHandleTemplate = ReorderList1.DragHandleTemplate,
                                  ItemTemplate = ReorderList1.ItemTemplate,
                              };

                rol.ItemReorder += RolItemReorder;
                rol.PostBackOnReorder = true;

                d.Add(key, rol);

                if(!string.IsNullOrEmpty(menu.Funcion.Modulo)) panReorder.Controls.Add(new Literal { Text = string.Format("<div><div class='reorder_subheader'>{0}</div>",  CultureManager.GetMenu(menu.Funcion.Modulo))});
                
                panReorder.Controls.Add(rol);

                if (!string.IsNullOrEmpty(menu.Funcion.Modulo)) panReorder.Controls.Add(new Literal { Text = "</div>" });

                sysdone.Add(menu.Funcion.Sistema.Id);
            }

            foreach (var pair in d)
            {
                int sys;
                string modulo;

                GetKeyValues(pair.Key, out sys, out modulo);

                pair.Value.DataSource = (from MovMenu m in _menues where m.Funcion.Sistema.Id == sys && m.Funcion.Modulo == modulo orderby m.Orden select m).ToList();
                pair.Value.DataBind();
            }

            Orders = orders;
            Access = access;
            Sistemas = sistemas;
        }

        void RolItemReorder(object sender, ReorderListItemReorderEventArgs e)
        {
            var orders = Orders;
            var func = Convert.ToInt32(((HiddenField) e.Item.FindControl("hidIdFuncion")).Value);

            var changes = new Dictionary<int, short>();

            foreach (var pair in orders.Where(pair => Sistemas[func] == Sistemas[pair.Key]))
            {
                if (pair.Key == func) changes.Add(pair.Key, (short)e.NewIndex);
                else if (pair.Value > e.OldIndex && pair.Value <= e.NewIndex) changes.Add(pair.Key, (short) (pair.Value - 1));
                else if (pair.Value < e.OldIndex && pair.Value >= e.NewIndex) changes.Add(pair.Key, (short) (pair.Value + 1));
            }

            foreach (var pair in changes) orders[pair.Key] = pair.Value;

            Orders = orders;

            LoadAccess();
            BindReorder();
        }

        private void LoadAccess()
        {
            var access = Access;

            foreach (Control list in panReorder.Controls)
            {
                var rol = list as ReorderList;

                if (rol == null) continue;

                foreach (var item in rol.Items)
                {
                    var key = Convert.ToInt32(((HiddenField)item.FindControl("hidIdFuncion")).Value);

                    access[key] = new[]
                                      {
                                          ((CheckBox) item.FindControl("chkAlta")).Checked,
                                          ((CheckBox) item.FindControl("chkMod")).Checked,
                                          ((CheckBox) item.FindControl("chkBaja")).Checked,
                                          ((CheckBox) item.FindControl("chkConsulta")).Checked,
                                          ((CheckBox) item.FindControl("chkImprimir")).Checked,
                                          ((CheckBox) item.FindControl("chkMapa")).Checked
                                      };                
                }
            }

            Access = access;
        }
        /// <summary>
        /// Bins the current profile being edited.
        /// </summary>
        protected override void Bind()
        {
            txtDescripcion.Text = EditObject.Descripcion;

            LoadMovMenus();
            LoadPermisos();
        }

        private void LoadPermisos()
        {
            var asegurados = EditObject.Asegurados.Select(a => a.Asegurable.Id).ToList();
            foreach(ListItem item in chkPermisos.Items)
            {
                var id = Convert.ToInt32(item.Value);
                item.Selected = asegurados.Contains(id);
            }
        }

        /// <summary>
        /// Saves the current object being edited.
        /// </summary>
        protected override void OnSave()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            _home = (from Perfil perfil in user.Perfiles
                     from MovMenu menu in perfil.Funciones
                     where menu.Funcion.Ref.Equals("PERFIL")
                     select menu.Funcion).Any();

            EditObject.Descripcion = txtDescripcion.Text;

            AddFunctions();

            AddPermisos();

            DAOFactory.PerfilDAO.SaveOrUpdate(EditObject);

            AddPerfilToUser(user);
        
            ReloadUserFunctions();
        }

        /// <summary>
        /// Adds the profile to the current user if its a new one.
        /// </summary>
        /// <param name="user"></param>
        private void AddPerfilToUser(Usuario user)
        {
            if (!user.Perfiles.IsEmpty() && (from Perfil p in user.Perfiles where p.Id == EditObject.Id select p).Count().Equals(0))
            {
                user.Perfiles.Add(DAOFactory.PerfilDAO.FindById(EditObject.Id));
                DAOFactory.UsuarioDAO.SaveOrUpdate(user);
            }
        }

        protected override void AfterSave() { Response.Redirect(_home ? "../Home.aspx" : RedirectUrl,false); }

        /// <summary>
        /// Deletes the current object being edited.
        /// </summary>
        protected override void OnDelete() { DAOFactory.PerfilDAO.Delete(EditObject); } 

        /// <summary>
        /// Validates the current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (txtDescripcion.Text == string.Empty) throw new Exception(string.Format(CultureManager.GetError("MUST_ENTER_VALUE"), CultureManager.GetLabel("DESCRIPCION")));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets all the functions associated to the current profile being edited.
        /// </summary>
        private void LoadMovMenus()
        {
            if (EditMode && _movmenus.Count == 0)
                foreach (MovMenu mm in DAOFactory.PerfilDAO.FindMovMenuByProfile(new List<int> { EditObject.Id }))
                    _movmenus.Add(mm.Funcion.Id, mm);
        }

        /// <summary>
        /// Add the newly assigned functions to the current profile being edited.
        /// </summary>
        private void AddFunctions()
        {
            LoadMovMenus();
            EditObject.ClearFunciones();

            var index = new Dictionary<int, short>();
            foreach (Control list in panReorder.Controls)
            {
                var rol = list as ReorderList;
                if (rol == null) continue;
                foreach (var item in rol.Items)
                {
                    var key = Convert.ToInt32(((HiddenField)item.FindControl("hidIdFuncion")).Value);
                    var movMenu = _movmenus.ContainsKey(key)
                                      ? _movmenus[key]
                                      : new MovMenu {Perfil = EditObject, Funcion = DAOFactory.FuncionDAO.FindById(key)};

                    movMenu.Alta = ((CheckBox) item.FindControl("chkAlta")).Checked;
                    movMenu.Baja = ((CheckBox) item.FindControl("chkBaja")).Checked;
                    movMenu.Consulta = ((CheckBox) item.FindControl("chkConsulta")).Checked;
                    movMenu.Modificacion = ((CheckBox) item.FindControl("chkMod")).Checked;
                    movMenu.Reporte = ((CheckBox) item.FindControl("chkImprimir")).Checked;
                    movMenu.VerMapa = ((CheckBox) item.FindControl("chkMapa")).Checked;
                    movMenu.Perfil = EditObject;//DAOFactory.PerfilDAO.FindById(EditObject.Id);

                    if (!index.ContainsKey(movMenu.Funcion.Sistema.Id)) index.Add(movMenu.Funcion.Sistema.Id, 0);


                    movMenu.Orden = index[movMenu.Funcion.Sistema.Id]++;

                    if (movMenu.IsActive()) EditObject.AddFuncion(movMenu);
                }
            }
        }

        public void AddPermisos()
        {
            EditObject.Asegurados.Clear();
            foreach(ListItem item in chkPermisos.Items)
            {
                if(!item.Selected) continue;
                var id = Convert.ToInt32(item.Value);
                var asegurado = new AseguradoEnPerfil
                        {
                            Asegurable = DAOFactory.AsegurableDAO.FindById(id),
                            Perfil = EditObject,
                            Permitido = true
                        };     
                EditObject.Asegurados.Add(asegurado);
            }
        }

        /// <summary>
        /// Gets the grid data source.
        /// </summary>
        /// <returns></returns>
        private void GetGridSource()
        {

            LoadMovMenus();
            var bysis = new Dictionary<string, short>();

            var viewAll = WebSecurity.AuthenticatedUser.AccessLevel >= Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin;
            var functions = viewAll
                                ? DAOFactory.FuncionDAO.FindAllOrderBySistema()
                                : DAOFactory.FuncionDAO.FindAllByUsuarioOrderBySistema(DAOFactory.UsuarioDAO.FindById(Usuario.Id));

            _menues = (from Funcion f in functions
                       orderby f.Sistema.Orden, f.Modulo,
                           (_movmenus.ContainsKey(f.Id) ? _movmenus[f.Id].Orden : (short)9999),
                           f.Descripcion
                       select (_movmenus.ContainsKey(f.Id) ? _movmenus[f.Id] : new MovMenu { Funcion = f, Orden = 9999 }))
                .ToList();

            foreach (var menu in _menues)
            {
                var key = GetSysKey(menu.Funcion);
                if (!bysis.ContainsKey(key)) bysis.Add(key, 0);
                menu.Orden = bysis[key]++;
            }
        }

        private static string GetSysKey(Funcion funcion) { return funcion.Sistema.Id + "_" + funcion.Modulo; }

        private static void GetKeyValues(string key, out int idsys, out string modulo)
        {
            var idx = key.IndexOf('_');

            idsys = Convert.ToInt32(key.Substring(0, idx));
            modulo = key.EndsWith("_") ? string.Empty : key.Substring(idx + 1);
        }

        /// <summary>
        /// Reloads user funtions when editing user current profile.
        /// </summary>
        private void ReloadUserFunctions()
        {
            Usuario.SetModules(DAOFactory.PerfilDAO.FindMovMenuBySistema(Usuario.IdPerfiles));
            Usuario.SetSecurables(DAOFactory.PerfilDAO.GetAsegurables(Usuario.IdPerfiles));
        }

        #endregion
    }
}
