using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class ConsumoCabeceraDAO : GenericDAO<ConsumoCabecera>
    {
//        public ConsumoCabeceraDAO(ISession session) : base(session) { }

        #region Find Methods

        public ConsumoCabecera FindByNroFactura(IEnumerable<int> empresas, IEnumerable<int> lineas, string nroFactura)
        {
            var vehiculos = new CocheDAO().GetList(empresas, lineas);
            return Query.Where(c => c.NumeroFactura == nroFactura)
                        .Where(c => c.Estado != ConsumoCabecera.Estados.Eliminado)
                        .Cacheable()
                        .ToList()
                        .FilterVehiculo(vehiculos, true, null)
                        .SafeFirstOrDefault();
        }
        
        private ConsumoCabecera FindPrevious(ConsumoCabecera consumo)
        {
            return Query.Where(t => t.Fecha < consumo.Fecha &&
                                    t.Estado != ConsumoCabecera.Estados.Eliminado &&
                                    t.Vehiculo.Id == consumo.Vehiculo.Id)
                        .OrderByDescending(t => t.Fecha)
                        .FirstOrDefault();
        }
        private IEnumerable<ConsumoCabecera> FindByProveedorDepositoYFactura(string factura, int idProveedor, int idDeposito)
        {
            return Query.Where(c => c.NumeroFactura == factura)
                        .Where(c => c.Estado != ConsumoCabecera.Estados.Eliminado)
                        .Where(c => (c.Proveedor != null && c.Proveedor.Id == idProveedor)
                                 || (c.Deposito != null && c.Deposito.Id == idDeposito))
                        .ToList();
        }

        public ConsumoCabecera FindByDatos(DateTime fecha, double kmDeclarados, string factura, int idVehiculo, int idProveedor, int idDeposito, int idDepositoDestino)
        {
            var q = Query.Where(c => c.Fecha == fecha
                                  && c.KmDeclarados == kmDeclarados
                                  && c.NumeroFactura == factura
                                  && c.Estado != ConsumoCabecera.Estados.Eliminado);

            if (idProveedor != -1 && idVehiculo != -1)
                q = q.Where(c => c.Proveedor.Id == idProveedor && c.Vehiculo.Id == idVehiculo);
            if (idProveedor != -1 && idDepositoDestino != -1)
                q = q.Where(c => c.Proveedor.Id == idProveedor && c.DepositoDestino.Id == idDepositoDestino);

            if (idDeposito != -1 && idVehiculo != -1)
                q = q.Where(c => c.Deposito.Id == idDeposito && c.Vehiculo.Id == idVehiculo);
            if (idDeposito != -1 && idDepositoDestino != -1)
                q = q.Where(c => c.Deposito.Id == idDeposito && c.DepositoDestino.Id == idDepositoDestino);

            return q.FirstOrDefault();
        }

        public double FindCostoVehiculo(DateTime desde, DateTime hasta, int idVehiculo)
        {
            var consumos = Query.Where(c => c.Fecha >= desde && c.Fecha < hasta
                                    && c.Vehiculo.Id == idVehiculo)
                                .Select(c => c.ImporteTotal)
                                .ToList();
            return consumos.Count == 0 ? 0 : consumos.Sum();
        }

        #endregion

        #region Get Methods

        public List<ConsumoCabecera> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> tiposVehiculo, IEnumerable<int> vehiculos, IEnumerable<int> tiposEmpleado, IEnumerable<int> empleados, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores, IEnumerable<int> depositosOrigen, IEnumerable<int> depositosDestino, DateTime? desde, DateTime? hasta)
        {
            var q = Query;

            if (!QueryExtensions.IncludesAll(empresas) || 
                !QueryExtensions.IncludesAll(lineas) || 
                !QueryExtensions.IncludesAll(departamentos) ||
                !QueryExtensions.IncludesAll(transportistas) || 
                !QueryExtensions.IncludesAll(centrosDeCostos) || 
                !QueryExtensions.IncludesAll(tiposVehiculo) || 
                !QueryExtensions.IncludesAll(vehiculos))
                q = q.FilterVehiculo(Session, empresas, lineas, transportistas, departamentos, centrosDeCostos, tiposVehiculo, vehiculos);

            if (!QueryExtensions.IncludesAll(empresas) || 
                !QueryExtensions.IncludesAll(lineas) || 
                !QueryExtensions.IncludesAll(tiposProveedor) || 
                !QueryExtensions.IncludesAll(proveedores))
                q = q.FilterProveedor(Session, empresas, lineas, tiposProveedor, proveedores);

            if (!QueryExtensions.IncludesAll(empresas) || 
                !QueryExtensions.IncludesAll(lineas) || 
                !QueryExtensions.IncludesAll(depositosOrigen))
                q = q.FilterDeposito(Session, empresas, lineas, depositosOrigen);

            if (!QueryExtensions.IncludesAll(empresas) || 
                !QueryExtensions.IncludesAll(lineas) || 
                !QueryExtensions.IncludesAll(depositosDestino))
                q = q.FilterDepositoDestino(Session, empresas, lineas, depositosDestino);

            if (!QueryExtensions.IncludesAll(empleados))
                q = q.FilterEmpleado(Session, empresas, lineas, transportistas, tiposEmpleado, empleados);

            q = q.Where(c => c.Estado != ConsumoCabecera.Estados.Eliminado);

            if (desde.HasValue) q = q.Where(c => c.Fecha >= desde);
            if (hasta.HasValue) q = q.Where(c => c.Fecha < hasta);
            
            var result = q.ToList();

            var emp = QueryExtensions.GetEmpresas(Session, empresas).Select(x=>x.Id);
            var lin = QueryExtensions.GetLineas(Session, empresas, lineas).Select(x=>x.Id);

            return result
                .Where(x => x.Vehiculo.Empresa == null || emp.Contains(x.Vehiculo.Empresa.Id))
                .Where(x => x.Vehiculo.Linea == null || lin.Contains(x.Vehiculo.Linea.Id))
                .ToList();
        }

        #endregion

        #region Other Methods

        public double GetDistance(int id)
        {
            var consumo = FindById(id);
            var consumoAnterior = FindPrevious(consumo);

            if (consumoAnterior == null) return 0;

            var cocheDao = new CocheDAO();
            return cocheDao.GetDistance(consumo.Vehiculo.Id, consumoAnterior.Fecha, consumo.Fecha);
        }

        public double GetKmDeclarados(int id)
        {
            var consumo = FindById(id);
            var consumoAnterior = FindPrevious(consumo);

            return consumoAnterior != null && consumoAnterior.KmDeclarados <= consumo.KmDeclarados 
                        ? consumo.KmDeclarados - consumoAnterior.KmDeclarados
                        : 0;
        }

        public bool IsFacturaUnique(string factura, int idProveedor, int idDeposito, int idConsumo)
        {
            var consumos = FindByProveedorDepositoYFactura(factura, idProveedor, idDeposito);
            return (consumos.Count() <= 1) && (consumos.Count() != 1 || consumos.ToArray()[0].Id == idConsumo);
        }

        #endregion
    }
}
