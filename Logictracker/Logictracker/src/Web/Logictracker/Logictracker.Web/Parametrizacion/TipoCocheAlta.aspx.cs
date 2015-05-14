using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTipoCocheAlta : SecuredAbmPage<TipoCoche>
    {
        protected override string RedirectUrl { get { return "TipoCocheLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_VEHICULO"; } }
        protected override string GetRefference() { return "TIPOCOCHE"; }

        protected override void Bind()
        {
            ddlLocacion.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : ddlLocacion.AllValue);
            ddlPlanta.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : ddlPlanta.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            if (EditObject.IconoNormal != null) iconNormal.Selected = EditObject.IconoNormal.Id;
            if (EditObject.IconoAtraso != null) iconAtraso.Selected = EditObject.IconoAtraso.Id;
            if (EditObject.IconoAdelanto != null) iconAdelanto.Selected = EditObject.IconoAdelanto.Id;
            if (EditObject.IconoDefault != null) iconDeafult.Selected = EditObject.IconoDefault.Id;
            npMaxSpeed.Value = EditObject.MaximaVelocidadAlcanzable;
            npKilometros.Value = Convert.ToInt32(EditObject.KilometrosDiarios);
            npVelocidadPromedio.Value = EditObject.VelocidadPromedio;
            chkControlaKilometraje.Checked = EditObject.ControlaKilometraje;
            chkControlaTurno.Checked = EditObject.ControlaTurno;
            chkSeguimientoPersona.Checked = EditObject.SeguimientoPersona;
            txtCapacidad.Value = EditObject.Capacidad;
            npDesvioMinimo.Value = EditObject.DesvioMinimo;
            npDesvioMaximo.Value = EditObject.DesvioMaximo;
            chkNoEsVehiculo.Checked = EditObject.NoEsVehiculo;
            chkControlaConsumo.Checked = EditObject.AlarmaConsumo;
        }

        protected override void OnDelete() { DAOFactory.TipoCocheDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.IconoNormal = DAOFactory.IconoDAO.FindById(iconNormal.Selected);
            EditObject.IconoAdelanto = DAOFactory.IconoDAO.FindById(iconAdelanto.Selected);
            EditObject.IconoAtraso = DAOFactory.IconoDAO.FindById(iconAtraso.Selected);
            EditObject.IconoDefault = DAOFactory.IconoDAO.FindById(iconDeafult.Selected);
            EditObject.MaximaVelocidadAlcanzable = Convert.ToInt32((double) npMaxSpeed.Value);
            EditObject.Empresa = (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
            EditObject.Linea = (ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
            EditObject.KilometrosDiarios = Convert.ToInt32((double) npKilometros.Value);
            EditObject.VelocidadPromedio = Convert.ToInt32((double) npVelocidadPromedio.Value);
            EditObject.ControlaKilometraje = chkControlaKilometraje.Checked;
            EditObject.ControlaTurno = chkControlaTurno.Checked;
            EditObject.SeguimientoPersona = chkSeguimientoPersona.Checked;
            EditObject.Capacidad = Convert.ToInt32((double) txtCapacidad.Value);
            EditObject.DesvioMinimo = Convert.ToInt32((double) npDesvioMinimo.Value);
            EditObject.DesvioMaximo = Convert.ToInt32((double) npDesvioMaximo.Value);
            EditObject.NoEsVehiculo = chkNoEsVehiculo.Checked;
            EditObject.AlarmaConsumo = chkControlaConsumo.Checked;

            DAOFactory.TipoCocheDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text)) ThrowMustEnter("CODE");
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
            if (ddlPlanta.Selected.Equals(0)) ThrowMustEnter("LINEA");

            if (iconNormal.Selected <= 0) ThrowMustEnter("ICONO_ENHORA");
            if (iconAtraso.Selected <= 0) ThrowMustEnter("ICONO_ATRASO");
            if (iconAdelanto.Selected <= 0) ThrowMustEnter("ICONO_ADELANTO");
            if (iconDeafult.Selected <= 0) ThrowMustEnter("ICONO_DEFAULT");

            ValidateHigher(Convert.ToInt32((double) npDesvioMaximo.Value), Convert.ToInt32((double) npDesvioMinimo.Value), "VAL_MAYOR");
        }

        protected override void ValidateDelete()
        {
            var coches = DAOFactory.CocheDAO.FindList(new[] {-1}, new[] {-1}, new[] {EditObject.Id});

            if (coches != null && coches.Count > 0) 
                throw new Exception(CultureManager.GetError("ASSIGNED_VEHICLE_TYPE"));
        }
    }
}