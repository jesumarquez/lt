using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTipoGeoRefAlta : SecuredAbmPage<TipoReferenciaGeografica>
    {
        protected override string RedirectUrl { get { return "TipoGeoRefLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_POI"; } }
        protected override string GetRefference() { return "TIPODOMICILIO"; }

        #region Protected Method

        /// <summary>
        /// On Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            BindTiposVehiculo();
        }

        /// <summary>
        /// Initial Binding
        /// </summary>
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null  ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);

            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            if (EditObject.Icono != null) { SelectIcon2.Selected = EditObject.Icono.Id; }

            chkControlaES.Checked = EditObject.ControlaEntradaSalida;
            chkControlaVelocidad.Checked = EditObject.ControlaVelocidad;
            chkInhibeAlarma.Checked = EditObject.InhibeAlarma;
            chkZonaRiesgo.Checked = EditObject.EsZonaDeRiesgo;
            chkExcluyeMonitor.Checked = EditObject.ExcluirMonitor;
            chkEsTaller.Checked = EditObject.EsTaller;
            chkEsControlAcceso.Checked = EditObject.EsControlAcceso;
            btnGenerar.Enabled = EditMode && chkEsControlAcceso.Checked;

            chkControlaPermanencia.Checked = EditObject.ControlaPermanencia;
            chkControlaPermanenciaEntrega.Checked = EditObject.ControlaPermanenciaEntrega;

            txtMaximaPermanencia.Enabled = chkControlaPermanencia.Checked;
            txtMaximaPermanenciaEntrega.Enabled = chkControlaPermanenciaEntrega.Checked;

            txtMaximaPermanencia.Text = EditObject.MaximaPermanencia.ToString("#0");
            txtMaximaPermanenciaEntrega.Text = EditObject.MaximaPermanenciaEntrega.ToString("#0");

            chkInicio.Checked = EditObject.EsInicio;
            chkIntermedio.Checked = EditObject.EsIntermedio;
            chkFin.Checked = EditObject.EsFin;

            ColorPicker.Color = EditObject.Color.HexValue;

            BindVelocidades();
        }

        protected override void OnDuplicate()
        {
            base.OnDuplicate();
            AddToolBarIcons();
        }

        protected override void AddToolBarIcons()
        {
            base.AddToolBarIcons();
            if(EditMode)
            {
                var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
                var pEmpresa = user.Empresas.Count > 0;
                var pLinea = user.Lineas.Count > 0;
                var eEmpresa = EditObject.Empresa != null;
                var eLinea = EditObject.Linea != null;
                if (!pEmpresa && !pLinea) return;
                if (!pLinea && eEmpresa) return;
                if (pLinea && eLinea) return;

                ToolBar.RemoveButton(ToolBar.ToolBarButtonDeleteId);
                ToolBar.RemoveButton(ToolBar.ToolBarButtonSaveId);
                ShowInfo("El nivel de acceso de este usuario no permite modificar este objeto");
            }
        }

        /// <summary>
        /// Saves current object.
        /// </summary>
        protected override void OnSave()
        {
            var oldIcon = EditObject.Icono;
            var newIcon = SelectIcon2.Selected > 0 ? DAOFactory.IconoDAO.FindById(SelectIcon2.Selected) : null;
            var iconChanged = oldIcon != newIcon;

            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Icono = newIcon;

            EditObject.ControlaEntradaSalida = chkControlaES.Checked;
            EditObject.ControlaVelocidad = chkControlaVelocidad.Checked;
            EditObject.InhibeAlarma = chkInhibeAlarma.Checked;
            EditObject.EsZonaDeRiesgo = chkZonaRiesgo.Checked;
            EditObject.ExcluirMonitor = chkExcluyeMonitor.Checked;
            EditObject.EsTaller = chkEsTaller.Checked;
            EditObject.EsControlAcceso = chkEsControlAcceso.Checked;

            EditObject.ControlaPermanencia = chkControlaPermanencia.Checked;
            EditObject.ControlaPermanenciaEntrega = chkControlaPermanenciaEntrega.Checked;
            EditObject.MaximaPermanencia = chkControlaPermanencia.Checked ? Convert.ToInt32((string) txtMaximaPermanencia.Text) : 0;
            EditObject.MaximaPermanenciaEntrega = chkControlaPermanenciaEntrega.Checked ? Convert.ToInt32((string) txtMaximaPermanenciaEntrega.Text) : 0;

            EditObject.EsInicio = chkInicio.Checked;
            EditObject.EsIntermedio = chkIntermedio.Checked;
            EditObject.EsFin = chkFin.Checked;

            EditObject.Color.HexValue = ColorPicker.Color;

            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = EditObject.Linea != null
                                     ? EditObject.Linea.Empresa
                                     : cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;

            if(EditObject.ControlaVelocidad) AddSpeedLimits();

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    DAOFactory.TipoReferenciaGeograficaDAO.SaveOrUpdate(EditObject);
                    if (EditMode && iconChanged) UpdateGeoRefIcons(oldIcon, newIcon);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private void UpdateGeoRefIcons(Icono oldIcon, Icono newIcon)
        {
            var refs = DAOFactory.ReferenciaGeograficaDAO.FindByTipo(EditObject.Id);
            foreach (var r in refs)
            {
                if (r.Icono != oldIcon) continue;
                r.Icono = newIcon;
                DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(r);
            }
        }

        /// <summary>
        /// Validates current object for saving.
        /// </summary>
        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text)) ThrowMustEnter("CODE");
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
            if (cbLinea.Selected.Equals(0)) ThrowMustEnter("LINEA");

            var bycodigo = DAOFactory.TipoReferenciaGeograficaDAO.FindByCodigo(
                new[] {cbEmpresa.Selected},
                new[] {cbLinea.Selected},
                String.IsNullOrEmpty(txtCodigo.Text) ? String.Empty : txtCodigo.Text.Trim());

            if (bycodigo != null && bycodigo.Id != EditObject.Id)
                ThrowDuplicated("CODE");

            var bydescripcion = DAOFactory.TipoReferenciaGeograficaDAO.FindByDescripcion(
                new[] {cbEmpresa.Selected},
                new[] {cbLinea.Selected},
                string.IsNullOrEmpty(txtDescripcion.Text) ? String.Empty : txtDescripcion.Text.Trim());

            if (bydescripcion != null && bydescripcion.Id != EditObject.Id)
                ThrowDuplicated("DESCRIPCION");

            if (chkControlaPermanencia.Checked)
                ValidateInt32(txtMaximaPermanencia.Text, "Labels", "MAXIMA_PERMANENCIA");

            if (chkControlaPermanenciaEntrega.Checked)
                ValidateInt32(txtMaximaPermanenciaEntrega.Text, "Labels", "MAXIMA_PERMANENCIA_ENTREGA");
        }

        /// <summary>
        /// Validates current object for deleting.
        /// </summary>
        private new void ValidateDelete()
        {
            if (DAOFactory.TipoReferenciaGeograficaDAO.HasChilds(EditObject.Id))
                throw new ApplicationException(CultureManager.GetError("CANT_DEL_TIPO_POI"));
        }
    
        /// <summary>
        /// Deletes current object.
        /// </summary>
        protected override void OnDelete()
        {
            ValidateDelete();
            DAOFactory.TipoReferenciaGeograficaDAO.Delete(EditObject);
        }

        #endregion

        #region Events

        protected void OwnerSelectedIndexChanged(object sender, EventArgs e)
        {
            BindTiposVehiculo();
        }

        protected void ChkControlaVelocidadCheckedChanged(object sender, EventArgs e)
        {
            //panelVelocidades.Visible = chkControlaVelocidad.Checked;
        }

        protected void ChkEsControlAccesoOnCheckedChanged(object sender, EventArgs e)
        {
            btnGenerar.Enabled = EditMode && chkEsControlAcceso.Checked;
        }

        protected void ChkControlaPermanenciaOnCheckedChanged(object sender, EventArgs e)
        {   
            txtMaximaPermanencia.Enabled = chkControlaPermanencia.Checked;
            if (!chkControlaPermanencia.Checked) txtMaximaPermanencia.Text = "0";
        }

        protected void ChkControlaPermanenciaEntregaOnCheckedChanged(object sender, EventArgs e)
        {
            txtMaximaPermanenciaEntrega.Enabled = chkControlaPermanenciaEntrega.Checked;
            if (!chkControlaPermanenciaEntrega.Checked) txtMaximaPermanenciaEntrega.Text = "0";
        }

        protected void BtnGenerarOnClick(object sender, EventArgs e)
        {
            var referencias = DAOFactory.ReferenciaGeograficaDAO.FindByTipo(EditObject.Id);
            DAOFactory.PuertaAccesoDAO.GenerarByGeocercas(referencias);

            Response.Redirect("~/Parametrizacion/PuertaAccesoLista.aspx");
        }

        #endregion

        #region Velocidades
        
        /// <summary>
        /// Vehicle types custom item data bound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridVelocidadesItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var tipo = e.Row.DataItem as TipoCoche;
            if (tipo == null) return;

            var own = string.Empty;
            if(cbLinea.Selected <= 0)
            {
                own = " (";
                if (cbEmpresa.Selected <= 0) own += (tipo.Empresa != null ? tipo.Empresa.RazonSocial : "Todos") + ", ";
                own += tipo.Linea != null ? tipo.Linea.Descripcion : "Todos";
                own += ")";
            }
            e.Row.Cells[0].Text = tipo.Descripcion + own;
        }

        /// <summary>
        /// Vehicles types data binding.
        /// </summary>
        private void BindTiposVehiculo()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var tipos = DAOFactory.TipoCocheDAO.FindByEmpresasAndLineas(cbEmpresa.SelectedValues, cbLinea.SelectedValues, user);

            gridVelocidades.DataSource = tipos;
            gridVelocidades.DataBind();
        }

        /// <summary>
        /// Binds associated speed per vehicle T limits.
        /// </summary>
        private void BindVelocidades()
        {
            if (!EditMode) return;

            var dic = new Dictionary<int, int>();

            foreach (TipoReferenciaVelocidad velo in EditObject.VelocidadesMaximas) dic.Add(velo.TipoVehiculo.Id, velo.VelocidadMaxima);

            for (var i = 0; i < gridVelocidades.Rows.Count; i++)
            {
                var id = (int)gridVelocidades.DataKeys[i].Value;

                if (!dic.ContainsKey(id)) continue;

                var txt = gridVelocidades.Rows[i].FindControl("txtVelocidadMaxima") as TextBox;

                if (txt != null) txt.Text = dic[id].ToString();
            }
        }

        /// <summary>
        /// Adds speeds limits per vehicle T to the current geocerca.
        /// </summary>
        private void AddSpeedLimits()
        {
            EditObject.VelocidadesMaximas.Clear();

            for (var i = 0; i < gridVelocidades.Rows.Count; i++)
            {
                var id = (int)gridVelocidades.DataKeys[i].Value;
                var txt = gridVelocidades.Rows[i].FindControl("txtVelocidadMaxima") as TextBox;
                var velo = 0;

                if (txt != null && (!int.TryParse(txt.Text, out velo) || velo == 0)) continue;

                var gv = new TipoReferenciaVelocidad { TipoReferenciaGeografica = EditObject, TipoVehiculo = DAOFactory.TipoCocheDAO.FindById(id), VelocidadMaxima = velo };

                EditObject.VelocidadesMaximas.Add(gv);
            }
        }
        
        #endregion
    }
}