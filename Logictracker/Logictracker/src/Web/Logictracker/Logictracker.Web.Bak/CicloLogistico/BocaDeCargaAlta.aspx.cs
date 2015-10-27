using System;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class BocaDeCargaAlta : SecuredAbmPage<BocaDeCarga>
    {
        protected override string RedirectUrl { get { return "BocaDeCargaLista.aspx"; } }
        protected override string VariableName { get { return "CLOG_BOCADECARGA"; } }
        protected override string GetRefference() { return "BOCADECARGA"; }

        protected override void Bind()
        {
            if (EditObject.Linea != null)
            {
                cbEmpresa.SetSelectedValue(EditObject.Linea.Empresa.Id);
                cbLinea.SetSelectedValue(EditObject.Linea.Id);
            }

            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            txtRendimiento.Text = EditObject.Rendimiento.ToString();
            txtHorasLaborales.Text = EditObject.HorasLaborales.ToString();
            dtInicioActividad.SelectedTime = TimeSpan.FromMinutes(EditObject.HoraInicioActividad);
        }

        protected override void OnDelete() { DAOFactory.BocaDeCargaDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;

            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Rendimiento = Convert.ToInt32(txtRendimiento.Text.Trim());
            EditObject.HorasLaborales = Convert.ToInt32(txtHorasLaborales.Text.Trim());
            EditObject.HoraInicioActividad = Convert.ToInt32(dtInicioActividad.SelectedTime.Value.TotalMinutes);
            
            DAOFactory.BocaDeCargaDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbLinea.Selected, "PARENTI02");

            var codigo = ValidateEmpty(txtCodigo.Text, "CODE");

            var byCode = DAOFactory.BocaDeCargaDAO.GetByCode(cbLinea.Selected, codigo);
            ValidateDuplicated(byCode, "CODE");

            var descripcion = ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            var byDesc = DAOFactory.BocaDeCargaDAO.GetByDescripcion(cbLinea.Selected, descripcion);
            ValidateDuplicated(byDesc, "DESCRIPCION");

            var rendimiento = ValidateInt32(txtRendimiento.Text, "RENDIMIENTO");

            if(rendimiento < 0) ThrowInvalidValue("RENDIMIENTO");

            var horaslaborales = ValidateInt32(txtHorasLaborales.Text, "HORAS_LABORALES");

            if(horaslaborales < 0 || horaslaborales > 24) ThrowInvalidValue("HORAS_LABORALES");

            ValidateEmpty(dtInicioActividad.SelectedTime, "INICIO_ACTIVIDAD");
        }

    }
}
