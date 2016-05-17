using System;
using System.Linq;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using System.Web.UI.WebControls;

namespace Logictracker.CicloLogistico
{
    public partial class StockVehicularAlta : SecuredAbmPage<StockVehicular>
    {
        protected override string RedirectUrl { get { return "StockVehicularLista.aspx"; } }
        protected override string VariableName { get { return "PAR_STOCK_VEHICULAR"; } }
        protected override string GetRefference() { return "PAR_STOCK_VEHICULAR"; }
        protected override bool DeleteButton { get { return false; } }
        
        private void BindNoAsignados()
        {
            lstNoAsignados.Items.Clear();

            var vehiculos = DAOFactory.CocheDAO.GetList(cbEmpresa.SelectedValues, new[]{-1}, cbTipoVehiculo.SelectedValues);

            if (vehiculos.Count() > 0)
            {
                var items = vehiculos.Select(v => new ListItem(v.Interno, v.Id.ToString("#0"))).OrderBy(l => l.Text).ToArray();
                lstNoAsignados.Items.AddRange(items);
            }
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            cbZona.SetSelectedValue(EditObject.Zona.Id);
            cbTipoVehiculo.SetSelectedValue(EditObject.TipoCoche.Id);

            panelTopLeft.Enabled = !EditMode;

            BindNoAsignados();
            lstAsignados.Items.Clear();
            
            if (EditObject.Detalles.Count > 0)
            {
                var detalles = EditObject.Detalles.Cast<DetalleStockVehicular>().OrderBy(d => d.Vehiculo.Interno);
                foreach (var detalle in detalles)
                {
                    var item = new ListItem(detalle.Vehiculo.Interno, detalle.Vehiculo.Id.ToString("#0"));
                    lstAsignados.Items.Add(item);
                    lstNoAsignados.Items.Remove(item);
                }
            }
        }

        protected void FiltrosOnSelectedIndexChanged(object sender, EventArgs e)
        {
            BindNoAsignados();
            lstAsignados.Items.Clear();
        }

        protected override void OnDelete() { }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Zona = DAOFactory.ZonaDAO.FindById(cbZona.Selected);
            EditObject.TipoCoche = DAOFactory.TipoCocheDAO.FindById(cbTipoVehiculo.Selected);
            
            EditObject.Detalles.Clear();
            foreach (ListItem item in lstAsignados.Items)
            {
                var coche = DAOFactory.CocheDAO.FindById(Convert.ToInt32(item.Value));
                var detalle = new DetalleStockVehicular();
                detalle.StockVehicular = EditObject;
                detalle.Vehiculo = coche;
                EditObject.Detalles.Add(detalle);

                EliminarAsignaciones(detalle);
            }
            
            DAOFactory.StockVehicularDAO.SaveOrUpdate(EditObject);
        }

        private void EliminarAsignaciones(DetalleStockVehicular detalle)
        {
            var idStockVehicular = detalle.StockVehicular.Id;
            var idCoche = detalle.Vehiculo.Id;

            var asignaciones = DAOFactory.DetalleStockVehicularDAO.GetByStockAndCoche(idStockVehicular, idCoche);

            foreach (var asignacion in asignaciones)
            {
                asignacion.StockVehicular.Detalles.Remove(asignacion);
                DAOFactory.StockVehicularDAO.SaveOrUpdate(asignacion.StockVehicular);
            }
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbZona.Selected, "PARENTI89");
            ValidateEntity(cbTipoVehiculo.Selected, "PARENTI17");
        }

        protected void BtnAgregarOnClick(object sender, EventArgs e)
        {
            var indices = lstNoAsignados.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstNoAsignados.Items[indice];
                lstAsignados.Items.Add(item);
                lstNoAsignados.Items.RemoveAt(indice);
            }

            lstAsignados.SelectedIndex = -1;
        }

        protected void BtnEliminarOnClick(object sender, EventArgs e)
        {
            var indices = lstAsignados.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstAsignados.Items[indice];
                lstNoAsignados.Items.Add(item);
                lstAsignados.Items.RemoveAt(indice);
            }

            lstNoAsignados.SelectedIndex = -1;
        }
       
        protected void cbEmpresa_PreBind(Object sender, EventArgs e) { if (EditMode) cbEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.NullValue; }
    }
}
