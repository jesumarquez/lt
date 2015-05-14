#region Usings

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;

#endregion

namespace Logictracker.App_Controls
{
    public partial class App_Controls_IconPicker : BaseUserControl
    {
        public event EventHandler SelectedIconChange;
        private bool bind = false;

        public int Selected
        {
            get { return (int) (ViewState["Selected"] ?? 0); }
            set { ViewState["Selected"] = value; SetIcono(value);}
        }

        public string ParentControls
        {
            get { return (string)ViewState["ParentControls"]; }
            set { ViewState["ParentControls"] = value; }
        }

        public int IdEmpresa
        {
            get { return (int)(ViewState["IdEmpresa"] ?? -1); }
            set { if(value != IdEmpresa) bind = true;
                ViewState["IdEmpresa"] = value; }
        }
        public int IdLinea
        {
            get { return (int)(ViewState["IdLinea"] ?? -1); }
            set { if (value != IdLinea) bind = true; 
                ViewState["IdLinea"] = value; }
        }

        private void LoadParents()
        {
            if (string.IsNullOrEmpty(ParentControls)) return;

            foreach (var parent in ParentControls.Split(','))
            {
                var control = FindParent(parent.Trim(), Page.Controls);
                if (control == null) continue;
                var iab = control as IAutoBindeable;
                var ipc = control as IParentControl;
                if (iab != null)
                {
                    if (iab.Type == typeof(Empresa)) IdEmpresa = iab.Selected;
                    else if (iab.Type == typeof(Linea)) IdLinea = iab.Selected;
                }
                else if (ipc != null)
                {
                    foreach (var bindeable in ipc.ParentControls)
                    {
                        if (bindeable.Type == typeof(Empresa)) IdEmpresa = bindeable.Selected;
                        else if (bindeable.Type == typeof(Linea)) IdLinea = bindeable.Selected;
                    }
                }
            }
        }

        private static Control FindParent(string parent, ControlCollection controls)
        {
            if (controls == null) return null;

            foreach (Control control in controls)
            {
                if (!string.IsNullOrEmpty(control.ID) && control.ID.Equals(parent)) return control;
                var cnt = FindParent(parent, control.Controls);
                if (cnt != null) return cnt;
            }
            return null;
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadParents();
            if (bind) BindIconos();
            base.OnLoad(e);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                BindIconos();
                SetIcono(Selected);
            }
        }

        public void BindIconos()
        {
            var iconos = DAOFactory.IconoDAO.GetList(IdEmpresa, IdLinea);
            var filtered = from Icono i in iconos orderby i.Descripcion select i;
            gridIconos.DataSource = filtered;
            gridIconos.DataBind();
            updIconos.Update();

            if(Selected <= 0) return;
            var selected = iconos.FirstOrDefault(i => i.Id == Selected);
            if(selected == null) Selected = 0;
        }

        private void SetIcono(int value)
        {
            var iconPath = "";
            if (value > 0)
            {
                var icono = DAOFactory.IconoDAO.FindById(value);
                iconPath = string.Concat(IconDir, icono.PathIcono);
            }

            imgSelected.Style.Add("background-image", string.Concat("url('", iconPath, "')"));
            imgSelected.Style.Add("background-position", "center center");
            imgSelected.Style.Add("background-repeat", "no-repeat");
        }

        protected void gridIconos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int id;
                if (!int.TryParse(e.CommandArgument.ToString(), out id)) return;

                SetIcono(id);

                var change = Selected != id;
                Selected = id;
                if (change && SelectedIconChange != null) SelectedIconChange(this, EventArgs.Empty);
            }
        }
        protected void gridIconos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var icono = e.Item.DataItem as Icono;
            if (icono == null) return;
            var bic = e.Item.FindControl("Button1") as Button;
            if (bic == null) return;
            bic.CommandArgument = icono.Id.ToString();
            bic.Style.Add("background-image", string.Concat("url('", IconDir, icono.PathIcono, "')"));
            bic.Style.Add("background-position", "center center");
            bic.Style.Add("background-repeat", "no-repeat");

            var lbl = e.Item.FindControl("lblDescri") as Label;
            if (lbl == null) return;
            lbl.Text = icono.Descripcion.Replace(" ", "&nbsp;");

            var lit = e.Item.FindControl("litAconym") as Literal;
            if (lit == null) return;
            lit.Text = string.Concat("<acronym title='", icono.Descripcion, "'>");
        }
    }
}
