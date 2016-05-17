#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class Reportes_CombustibleEnPozos_Conciliaciones : SecuredAbmPage<Movimiento>
    {
        #region Protected Properties

        protected override string VariableName { get { return "INGRESO_CONCILIACIONES"; } }

        protected override string RedirectUrl { get { return "ConciliacionesLista.aspx"; } }
    
        protected override string GetRefference() { return "CONCILIACIONES"; }

        #endregion

        #region Protected Methods

        #region Toolbar Overrides

        protected override bool DeleteButton { get { return false; } }

        protected override bool DuplicateButton { get { return false; } }

        #endregion

        protected override void ValidateSave()
        {
            if (ddlMotivoConciliacion.Selected <= 0) ThrowMustEnter("MOTIVO_CONCILIACION");
            if (String.IsNullOrEmpty(txtObservacion.Text)) ThrowMustEnter("REM_OBS");
            if (cbTanque.Selected <= 0) ThrowMustEnter("TANQUE");
        }

        protected override void OnSave()
        {
            EditObject.Volumen = npVolumen.Value;
            EditObject.Observacion = txtObservacion.Text;
            EditObject.Fecha = dpFecha.SelectedDate.GetValueOrDefault();
            EditObject.TipoMovimiento = DAOFactory.TipoMovimientoDAO.FindById(ddlTipoMov.Selected);
            EditObject.FechaIngresoABase = DateTime.Now;
            EditObject.Estado = 1;
            EditObject.Tanque = DAOFactory.TanqueDAO.FindById(cbTanque.Selected);
            var motivo = DAOFactory.MotivoConciliacionDAO.FindById(ddlMotivoConciliacion.Selected);
            EditObject.Motivo = motivo;
            if (motivo.Asignable)
            {
                EditObject.Coche = DAOFactory.CocheDAO.FindById(ddlMovil.Selected);
            }
            DAOFactory.MovimientoDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnDelete() { }

        protected override void Bind()
        {
            npVolumen.Value = EditObject.Volumen;
            txtObservacion.Text = EditObject.Observacion;
            txtObservacion.Text = EditObject.Observacion;
            npVolumen.Enabled = false;
            txtObservacion.Enabled = false;
            dpFecha.SelectedDate = EditObject.Fecha;
            dpFecha.Enabled = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!EditMode || EditObject.Motivo != null) return;
            ddlMotivoConciliacion.Items.Clear();
            ddlMovil.Items.Clear();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) dpFecha.SetDate();
        }


        #region Initial Bindings

        protected void ddlMotivoConciliacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMovil.Enabled = ddlMotivoConciliacion.Selected > 0 && DAOFactory.MotivoConciliacionDAO.FindById(ddlMotivoConciliacion.Selected).Asignable ? true: false;
        }
        
        protected void ddlMotivoConciliacion_PreBind(object sender, EventArgs e)
        {
            if (!EditMode) return;

            if (EditObject.Motivo != null) ddlMotivoConciliacion.EditValue = EditObject.Motivo.Id;

            ddlMotivoConciliacion.Enabled = false;
        }

        protected void ddlTipoMov_PreBind(object sender, EventArgs e)
        {
            if (!EditMode) return;
            ddlTipoMov.EditValue = EditObject.TipoMovimiento != null ? EditObject.TipoMovimiento.Id : ddlTipoMov.AllValue;
            ddlTipoMov.Enabled = false;
        }

        protected void ddlMovil_PreBind(object sender, EventArgs e)
        {
            if (!EditMode) return;

            ddlMovil.EditValue =  (EditObject.Motivo != null && EditObject.Motivo.Asignable) ? EditObject.Coche.Id :  ddlMovil.AllValue;
            ddlMovil.Enabled = false;
        }

        protected void cbEmpresa_PreBind(object sender, EventArgs e)
        {
            if (!EditMode) return;
            cbEmpresa.EditValue = EditObject.Tanque.Equipo != null ? EditObject.Tanque.Equipo.Empresa != null ? EditObject.Tanque.Equipo.Empresa.Id : ddlTipoMov.AllValue : ddlTipoMov.AllValue ;
            cbEmpresa.Enabled = false;
        }

        protected void cbLinea_PreBind(object sender, EventArgs e)
        {
            if (!EditMode) return;
            cbLinea.EditValue = EditObject.Tanque.Equipo != null ? EditObject.Tanque.Equipo.Linea != null ? EditObject.Tanque.Equipo.Linea.Id : ddlTipoMov.AllValue : ddlTipoMov.AllValue;
            cbLinea.Enabled = false;
        }

        protected void cbEquipo_PreBind(object sender, EventArgs e)
        {
            if (!EditMode) return;
            cbEquipo.EditValue = EditObject.Tanque.Equipo != null ? EditObject.Tanque.Equipo.Id : ddlTipoMov.AllValue;
            cbEquipo.Enabled = false;
        }

        protected void cbTanque_PreBind(object sender, EventArgs e)
        {
            if (!EditMode) return;
            cbTanque.EditValue = EditObject.Tanque.Id;
            cbTanque.Enabled = false;
        }

        #endregion

        #endregion
    }
}
