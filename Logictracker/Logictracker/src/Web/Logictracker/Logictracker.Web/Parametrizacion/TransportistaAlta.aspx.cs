using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.DAL.DAO.BaseClasses;

namespace Logictracker.Parametrizacion
{
    public partial class TransportistaAlta : SecuredAbmPage<Transportista>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_TRANSPORTISTAS"; } }
        protected override string RedirectUrl { get { return "TransportistaLista.aspx"; } }
        protected override string GetRefference() { return "TRANSP"; }

        #endregion

        #region Private Const Properties
        /// <summary>
        /// Tarifas for this transportista.
        /// </summary>
        private List<TarifaTransportista> Tarifas
        {
            get { return (List<TarifaTransportista>)(ViewState["Tarifas"] ?? new List<TarifaTransportista>()); }
            set { ViewState["Tarifas"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Page load and initial data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            chkExistente.Attributes.Add("onclick",
                                        string.Format(@"
                var panSel = $get('{0}'); var panNew = $get('{1}');
                if(panSel.style.display == 'none') {{ panSel.style.display = '';panNew.style.display = 'none';}}
                else {{ panNew.style.display = '';panSel.style.display = 'none';}}", panSelectGeoRef.ClientID, panNewGeoRef.ClientID));

            GetNewData();
        }

        /// <summary>
        /// Binds tarifas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RepeaterTarifas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var param = e.Item.DataItem as TarifaTransportista;

            var txtCorto = e.Item.FindControl("txtTarifaCorto") as TextBox;
            var txtLargo = e.Item.FindControl("txtTarifaLargo") as TextBox;
            var cbCliente = e.Item.FindControl("cbCliente") as DropDownList;
            var btEliminarTarifa = e.Item.FindControl("btEliminarTarifa") as LinkButton;

            if(btEliminarTarifa != null) btEliminarTarifa.OnClientClick = "return confirm('" + CultureManager.GetSystemMessage("CONFIRM_OPERATION") + "');";

            BindClientes(cbCliente);

            if (param == null) return;

            if (txtCorto != null) txtCorto.Text = param.TarifaTramoCorto.ToString();
            if (txtLargo != null) txtLargo.Text = param.TarifaTramoLargo.ToString();

            if (param.Cliente == null) return;

            if (cbCliente == null) return;

            var li = cbCliente.Items.FindByValue(param.Cliente.Id.ToString());

            if (li != null) li.Selected = true;
        }

        /// <summary>
        /// Adds a new tarifa.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btNuevaTarifa_Click(object sender, EventArgs e)
        {
            var par = Tarifas;

            double corto, largo;

            double.TryParse(txtTarifaCorto.Text, out corto);
            double.TryParse(txtTarifaLargo.Text, out largo);

            par.Add(new TarifaTransportista { TarifaTramoCorto = corto, TarifaTramoLargo = largo });

            Tarifas = par;

            BindTarifas();
        }

        /// <summary>
        /// Repert item command handling.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RepeaterTarifas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete") return;

            var idx = e.Item.ItemIndex;

            if (idx > EditObject.Tarifas.Count - 1)
            {
                var par = Tarifas;

                par.RemoveAt(idx - EditObject.Tarifas.Count);

                Tarifas = par;
            }
            else
            {
                TarifaTransportista toDel = null;

                var i = 0;

                foreach (TarifaTransportista tarifa in EditObject.Tarifas)
                {
                    if (i < idx) { i++; continue; }

                    toDel = tarifa;

                    break;
                }

                if (toDel != null)
                {
                    EditObject.Tarifas.Remove(toDel);
                    DAOFactory.TransportistaDAO.SaveOrUpdate(EditObject);
                }
            }

            BindTarifas();
        }

        /// <summary>
        /// Binds Initial Values
        /// </summary>
        protected override void Bind()
        {
            cbEmpresa.SelectedValue = EditObject.Empresa != null ? EditObject.Empresa.Id.ToString() : cbEmpresa.AllValue.ToString();
            cbLinea.SelectedValue = EditObject.Linea != null ? EditObject.Linea.Id.ToString() : cbLinea.AllValue.ToString();
            txtDescripcion.Text = EditObject.Descripcion;
            txtContacto.Text = EditObject.Contacto;
            txtMail.Text = EditObject.Mail;
            txtTelefono.Text = EditObject.Telefono;
            txtTarifaCorto.Text = EditObject.TarifaTramoCorto.ToString();
            txtTarifaLargo.Text = EditObject.TarifaTramoLargo.ToString();
            chkIdentificaChoferes.Checked = EditObject.IdentificaChoferes;

            SelectGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            EditEntityGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
        

            BindTarifas();

            DocumentList1.LoadDocumentos(-1, EditObject.Id, -1, -1, -1);
        }

        /// <summary>
        /// Validates current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            ValidateDouble(txtTarifaCorto.Text, "TRAMO_CORTO");

            ValidateDouble(txtTarifaLargo.Text, "TRAMO_LARGO");

            ValidateTarifas();

            var georef = EditEntityGeoRef1.GetNewGeoRefference();

            if (georef == null) ThrowMustEnter("DIRECCION");
        }

        private void ValidateTarifas()
        {
            var clientes = new List<Cliente>();

            foreach (RepeaterItem item in RepeaterTarifas.Items)
            {
                var txtCorto = item.FindControl("txtTarifaCorto") as TextBox;
                var txtLargo = item.FindControl("txtTarifaLargo") as TextBox;
                var cbCliente = item.FindControl("cbCliente") as DropDownList;

                if (txtCorto == null || txtLargo == null || cbCliente == null) continue;
                if (cbCliente.SelectedIndex < 0) ThrowMustEnter("CLIENTE");

                var cliente = cbCliente.SelectedItem.Text;
                clientes.Add(new Cliente { Id = Convert.ToInt32(cbCliente.SelectedValue), Descripcion = cliente });

                try
                {
                    ValidateDouble(txtCorto.Text, "TRAMO_CORTO");
                    ValidateDouble(txtLargo.Text, "TRAMO_LARGO");
                }
                catch
                {
                    ThrowError("TARIFA_INCORRECTA_CLIENTE", cliente);
                }
            }

            var duplicados = clientes.GroupBy(tarifa => new { tarifa.Id, tarifa.Descripcion })
                .Select(cliente => new { cliente.Key.Id, cliente.Key.Descripcion, Repeticiones = cliente.Count() })
                .Where(cliente => cliente.Repeticiones > 1)
                .ToList();

            if (duplicados.Count > 0) 
                ThrowError("CLIENTE_REPETIDO", duplicados.First().Descripcion);

        }
        /// <summary>
        /// Actions to do on delete
        /// </summary>
        protected override void OnDelete()
        {
            if (!EditObject.Coches.IsEmpty()) throw new ApplicationException(CultureManager.GetError("CANT_DEL_TRANSPORTISTA"));

            DAOFactory.TransportistaDAO.Delete(EditObject);
        }

        protected override void OnDuplicate()
        {
            base.OnDuplicate();
            DocumentList1.ClearDocumentos();
        }

        /// <summary>
        /// Saves the current modifications or the new Transportista
        /// </summary>
        protected override void OnSave()
        {
            var identificaChoferesChanged = EditObject.IdentificaChoferes != chkIdentificaChoferes.Checked;

            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Mail = txtMail.Text;
            EditObject.Telefono = txtTelefono.Text;
            EditObject.Contacto = txtContacto.Text;
            EditObject.TarifaTramoCorto = Convert.ToDouble(txtTarifaCorto.Text);
            EditObject.TarifaTramoLargo = Convert.ToDouble(txtTarifaLargo.Text);
            EditObject.IdentificaChoferes = chkIdentificaChoferes.Checked;
        
            var i = 0;

            foreach (TarifaTransportista tarifa in EditObject.Tarifas)
            {
                var item = RepeaterTarifas.Items[i];

                var txtCorto = item.FindControl("txtTarifaCorto") as TextBox;
                var txtLargo = item.FindControl("txtTarifaLargo") as TextBox;
                var cbCliente = item.FindControl("cbCliente") as DropDownList;

                if (cbCliente != null) tarifa.Cliente = DAOFactory.ClienteDAO.FindById(Convert.ToInt32(cbCliente.SelectedValue));
                if (txtCorto != null) tarifa.TarifaTramoCorto = Convert.ToDouble(txtCorto.Text);
                if (txtLargo != null) tarifa.TarifaTramoLargo = Convert.ToDouble(txtLargo.Text);

                i++;
            }

            foreach (var parametro in Tarifas)
            {
                parametro.Transportista = EditObject;
                EditObject.Tarifas.Add(parametro);
            }

            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : EditObject.Linea != null ? EditObject.Linea.Empresa : null;

            if (chkExistente.Checked)
            {
                EditObject.ReferenciaGeografica = SelectGeoRef1.Selected > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(SelectGeoRef1.Selected) : null;
            }
            else
            {
                EditObject.ReferenciaGeografica = EditEntityGeoRef1.GetNewGeoRefference();
                EditObject.ReferenciaGeografica.Empresa = EditObject.Empresa;
                EditObject.ReferenciaGeografica.Linea = EditObject.Linea;
                EditObject.ReferenciaGeografica.Descripcion = EditObject.Descripcion;
                EditObject.ReferenciaGeografica.Codigo = EditObject.Descripcion;
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(EditObject.ReferenciaGeografica);
                STrace.Trace("QtreeReset", "TransportistaAlta");
            }

            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            using (var tran = SmartTransaction.BeginTransaction())
            {
                try
                {
                    DAOFactory.TransportistaDAO.SaveOrUpdate(EditObject);

                    if (EditMode && identificaChoferesChanged) UpdateCochesIdentificaChofer(EditObject.IdentificaChoferes);

                    if (!EditMode &&
                        user.PorTransportista)
                    {
                        user.AddTransportista(DAOFactory.TransportistaDAO.FindById(EditObject.Id));
                        DAOFactory.UsuarioDAO.SaveOrUpdate(user);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }
        private void UpdateCochesIdentificaChofer(bool identificaChoferes)
        {
            var coches = DAOFactory.CocheDAO.GetList(new[] {-1}, new[] {-1}, new[] {-1}, new[] {EditObject.Id}, new[] {-1});
            foreach (var c in coches)
            {
                if (c.Transportista == null) continue;
                if (c.IdentificaChoferes == identificaChoferes) continue;
                c.IdentificaChoferes = identificaChoferes;
                DAOFactory.CocheDAO.SaveOrUpdate(c);
            }
        }

        protected void cbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditMode) return;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            if (linea != null)
            {
                chkIdentificaChoferes.Checked = linea.IdentificaChoferes;
            }
            else
            {
                var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                chkIdentificaChoferes.Checked = empresa != null ? empresa.IdentificaChoferes : false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets data of the tarifas assigned to this trnasportista.
        /// </summary>
        private void GetNewData()
        {
            var par = Tarifas;
            var totalItems = RepeaterTarifas.Items.Count;

            for (var i = 0; i < par.Count; i++)
            {
                var item = RepeaterTarifas.Items[totalItems - par.Count + i];
                var txtCorto = item.FindControl("txtTarifaCorto") as TextBox;
                var txtLargo = item.FindControl("txtTarifaLargo") as TextBox;
                var cbCliente = item.FindControl("cbCliente") as DropDownList;

                if (txtCorto == null || txtLargo == null || cbCliente == null) continue;
                if(cbCliente.SelectedIndex < 0) continue;

                var corto = txtCorto.Text.Trim().Replace(',', '.');
                var largo = txtLargo.Text.Trim().Replace(',', '.');

                double tarifaCorto, tarifaLargo;
                double.TryParse(corto, NumberStyles.Any, CultureInfo.InvariantCulture, out tarifaCorto);
                double.TryParse(largo, NumberStyles.Any, CultureInfo.InvariantCulture, out tarifaLargo);

                par[i].Cliente = DAOFactory.ClienteDAO.FindById(Convert.ToInt32(cbCliente.SelectedValue));
                par[i].TarifaTramoCorto = tarifaCorto;
                par[i].TarifaTramoLargo = tarifaLargo;

                Tarifas = par;
            }
        }

        /// <summary>
        /// Binds tarifas associated to this transportista.
        /// </summary>
        private void BindTarifas()
        {
            var pars = (from TarifaTransportista t in EditObject.Tarifas select t).ToList();

            pars.AddRange(Tarifas);

            RepeaterTarifas.DataSource = pars;

            RepeaterTarifas.DataBind();
        }

        /// <summary>
        /// Binds clientes assigned to this transportista.
        /// </summary>
        /// <param name="ddl"></param>
        private void BindClientes(BaseDataBoundControl ddl)
        {
            ddl.DataSource = DAOFactory.ClienteDAO.GetList(new[] {-1}, new[] {-1}).OrderBy(c => c.Descripcion);

            ddl.DataBind();
        }

        #endregion
    }
}
