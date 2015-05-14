using System;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace HandlerTest
{
    public partial class Tester
    {
        public Empresa Empresa { get { return cbEmpresa.SelectedItem as Empresa; } }
        public Linea Linea { get { return cbLinea.SelectedItem as Linea; } }
        public Coche Coche { get { return cbVehiculo.SelectedItem as Coche; } }
        public Dispositivo Dispositivo { get { var coche = Coche; return coche == null ? null : coche.Dispositivo; } }

        public void InitializeEntidades()
        {
            BindEmpresas();
        }

        protected void BindEmpresas()
        {
            cbEmpresa.DataSource = DaoFactory.EmpresaDAO.GetList().OrderBy(e=>e.RazonSocial).ToList();
            cbEmpresa.DisplayMember = "RazonSocial";
        }
        protected void BindLineas()
        {
            cbLinea.DataSource = DaoFactory.LineaDAO.GetList(new[] { Empresa != null ? Empresa.Id : -1 }).OrderBy(e => e.Descripcion).ToList();
            cbLinea.DisplayMember = "Descripcion";
        }
        protected void BindVehiculos()
        {
            var vehiculos = DaoFactory.CocheDAO.GetList(new[] { Empresa != null ? Empresa.Id : -1 }, new[] { Linea != null ? Linea.Id : -1 }).OrderBy(e => e.Interno).ToList();
            cbVehiculo.DataSource = vehiculos;
            cbVehiculo.DisplayMember = "Interno";

            cbVehiculosDatamart.DataSource = vehiculos;
            cbVehiculosDatamart.DisplayMember = "Interno";
        }

        private void cbEmpresa_SelectedIndexChanged(object sender, EventArgs e) { BindLineas(); }
        private void cbLinea_SelectedIndexChanged(object sender, EventArgs e) { BindVehiculos(); BindMessages(); }
        private void cbVehiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            GotoCurrent();
            CargarDistribucion();
        }

    }
}
