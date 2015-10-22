using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class OdometroAlta : SecuredAbmPage<Odometro>
    {
        protected override string VariableName { get { return "PAR_ODOMETROS"; } }
        protected override string RedirectUrl { get { return "OdometroLista.aspx"; } }
        protected override string GetRefference() { return "ODOMETROS"; }
        
        protected void ChkPorTiempoCheckedChanged(object sender, EventArgs e)
        {
            npDiasReferencia.Value = npDiasAlarma1.Value = npDiasAlarma2.Value = 0;
            npDiasReferencia.Enabled = npDiasAlarma1.Enabled = npDiasAlarma2.Enabled = chkPorTiempo.Checked;
        }
        protected void ChkPorKmCheckedChanged(object sender, EventArgs e)
        {
            npKilometrosReferencia.Value = npKmAlarma1.Value = npKmAlarma2.Value= 0;
            npKilometrosReferencia.Enabled = npKmAlarma1.Enabled = npKmAlarma2.Enabled = chkPorKm.Checked;
        }
        protected void ChkPorHorasCheckedChanged(object sender, EventArgs e)
        {
            npHorasReferencia.Value = npHorasAlarma1.Value = npHorasAlarma2.Value = 0;
            npHorasReferencia.Enabled = npHorasAlarma1.Enabled = npHorasAlarma2.Enabled = chkPorHoras.Checked;
        }

        protected override void Bind()
        {
            ddlDistrito.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : EditObject.Linea != null ? EditObject.Linea.Empresa.Id : ddlDistrito.AllValue);
            ddlBase.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : ddlBase.AllValue;
            cbInsumos.SetSelectedValue(EditObject.Insumo != null ? EditObject.Insumo.Id : cbInsumos.NoneValue);

            txtDescripcion.Text = EditObject.Descripcion;
            chkPorKm.Checked = EditObject.PorKm;
            chkPorTiempo.Checked = EditObject.PorTiempo;
            chkPorHoras.Checked = EditObject.PorHoras;
            chkEsIterativo.Checked = EditObject.EsIterativo;
            chkReseteable.Checked = EditObject.EsReseteable;
            npKilometrosReferencia.Value = Convert.ToInt32(EditObject.ReferenciaKm);
            npDiasReferencia.Value = EditObject.ReferenciaTiempo;
            npHorasReferencia.Value = EditObject.ReferenciaHoras;

            npKilometrosReferencia.Enabled = EditObject.PorKm;
            npDiasReferencia.Enabled = EditObject.PorTiempo;
            npHorasReferencia.Enabled = EditObject.PorHoras;

            npKmAlarma1.Value = Convert.ToInt32(EditObject.Alarma1Km);
            npDiasAlarma1.Value = EditObject.Alarma1Tiempo;
            npHorasAlarma1.Value = EditObject.Alarma1Horas;
            txtColorAlarma1.Color = EditObject.Alarma1RGB;

            npDiasAlarma1.Enabled = EditObject.PorTiempo;
            npKmAlarma1.Enabled = EditObject.PorKm;
            npHorasAlarma1.Enabled = EditObject.PorHoras;

            npKmAlarma2.Value = Convert.ToInt32(EditObject.Alarma2Km);
            npDiasAlarma2.Value = EditObject.Alarma2Tiempo;
            npHorasAlarma2.Value = EditObject.Alarma2Horas;
            txtColorAlarma2.Color = EditObject.Alarma2Rgb;

            npDiasAlarma2.Enabled = EditObject.PorTiempo;
            npKmAlarma2.Enabled = EditObject.PorKm;
            npHorasAlarma2.Enabled = EditObject.PorHoras;

            ddclTipos.SelectedValues = EditObject.TiposDeVehiculo.Cast<TipoCoche>().Select(tipo => tipo.Id).ToList();
            cbModelo.SetSelectedValue(cbModelo.AllValue);
            cbMarca.SetSelectedValue(cbMarca.AllValue);
        }

        protected override void OnDelete() { DAOFactory.OdometroDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.PorKm = chkPorKm.Checked;
            EditObject.PorTiempo = chkPorTiempo.Checked;
            EditObject.PorHoras = chkPorHoras.Checked;
            EditObject.EsIterativo = chkEsIterativo.Checked;
            EditObject.EsReseteable = chkReseteable.Checked;
            EditObject.ReferenciaKm = EditObject.PorKm ? npKilometrosReferencia.Value: 0;
            EditObject.ReferenciaTiempo = EditObject.PorTiempo ? (int)npDiasReferencia.Value : 0;
            EditObject.ReferenciaHoras = EditObject.PorHoras ? (int)npHorasReferencia.Value : 0;

            EditObject.Alarma1Km = EditObject.PorKm ? npKmAlarma1.Value : 0;
            EditObject.Alarma1Tiempo = EditObject.PorTiempo ? (int)npDiasAlarma1.Value: 0;
            EditObject.Alarma1Horas = EditObject.PorHoras ? (int)npHorasAlarma1.Value : 0;
            EditObject.Alarma1RGB = !string.IsNullOrEmpty(txtColorAlarma1.Color) ? txtColorAlarma1.Color : HexColorUtil.ColorToHex(Color.White).TrimStart('#');

            EditObject.Alarma2Km = EditObject.PorKm ? npKmAlarma2.Value: 0;
            EditObject.Alarma2Tiempo = EditObject.PorTiempo ?(int)npDiasAlarma2.Value: 0;
            EditObject.Alarma2Horas = EditObject.PorHoras ? (int)npHorasAlarma2.Value : 0;
            EditObject.Alarma2Rgb = !string.IsNullOrEmpty(txtColorAlarma2.Color) ? txtColorAlarma2.Color : HexColorUtil.ColorToHex(Color.White).TrimStart('#');

            var linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
            var empresa = ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : linea != null ? linea.Empresa : null;
            
            EditObject.Empresa = empresa;
            EditObject.Linea = linea;
            EditObject.Insumo = cbInsumos.Selected > 0 ? DAOFactory.InsumoDAO.FindById(cbInsumos.Selected) : null;
            
            var usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (ddclTipos.SelectedValues.Contains(-1)) 
                ddclTipos.ToogleItems();

            RefreshOdometerVehicleTypes();

            DAOFactory.OdometroDAO.SaveOrUpdate(EditObject, (IEnumerable<int>) VehiculoListBox.SelectedValues, usuario);
        }

        protected override void ValidateSave()
        {
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");

            if (ddclTipos.SelectedValues.Contains(0)) ThrowMustEnter("TIPOS_VEHICULO");
        }

        private void RefreshOdometerVehicleTypes()
        {
            EditObject.ClearVehicleTypes();

            foreach (var tipo in from int t in ddclTipos.SelectedValues select DAOFactory.TipoCocheDAO.FindById(t)) EditObject.AddVehicleType(tipo);
        }
    }
}