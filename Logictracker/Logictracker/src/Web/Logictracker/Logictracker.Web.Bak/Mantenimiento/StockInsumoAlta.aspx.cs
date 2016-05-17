using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class StockInsumoAlta : SecuredAbmPage<Stock>
    {
        protected override string RedirectUrl { get { return "StockInsumoLista.aspx"; } }
        protected override string VariableName { get { return "MAN_STOCK_INSUMO"; } }
        protected override string GetRefference() { return "MAN_STOCK_INSUMO"; }

        protected override bool AddButton { get { return false; } }
        protected override bool DeleteButton { get { return false; } }
        protected override bool DuplicateButton { get { return false; } }
        
        protected override void Bind()
        {
            cbDeposito.SetSelectedValue(EditObject.Deposito != null ? EditObject.Deposito.Id : cbDeposito.NoneValue);
            cbInsumo.SetSelectedValue(EditObject.Insumo != null ? EditObject.Insumo.Id : cbInsumo.NoneValue);
            txtCapacidadMaxima.Text = EditObject.CapacidadMaxima.ToString("#0.00");
            txtPuntoReposicion.Text = EditObject.PuntoReposicion.ToString("#0.00");
            txtStockCritico.Text = EditObject.StockCritico.ToString("#0.00");
            txtStockActual.Text = EditObject.Cantidad.ToString("#0.00");
            chkAlarmaActiva.Checked = EditObject.AlarmaActiva;
        }

        protected override void OnDelete() { }

        protected override void OnSave()
        {
            EditObject.CapacidadMaxima = Convert.ToDouble((string) txtCapacidadMaxima.Text);
            EditObject.PuntoReposicion = Convert.ToDouble((string) txtPuntoReposicion.Text);
            EditObject.StockCritico = Convert.ToDouble((string) txtStockCritico.Text);
            EditObject.AlarmaActiva = chkAlarmaActiva.Checked;

            DAOFactory.StockDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateDouble(txtCapacidadMaxima.Text, "CAPACIDAD_MAXIMA");
            ValidateDouble(txtPuntoReposicion.Text, "PUNTO_REPOSICION");
            ValidateDouble(txtStockCritico.Text, "NIVEL_CRITICO");
        }
    }
}
