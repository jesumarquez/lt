using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class ConsumoDetalleDAO : GenericDAO<ConsumoDetalle>
    {
//        public ConsumoDetalleDAO(ISession session) : base(session) { }

        public List<ConsumoDetalle> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> tiposVehiculo, IEnumerable<int> vehiculos, IEnumerable<int> tiposEmpleado, IEnumerable<int> empleados, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores, IEnumerable<int> depositosOrigen, IEnumerable<int> depositosDestino, DateTime? desde, DateTime? hasta, IEnumerable<int> cabeceras)
        {
            return Query.FilterConsumoCabecera(Session, empresas, lineas, transportistas, departamentos, centrosDeCosto, tiposVehiculo, vehiculos, tiposEmpleado, empleados, tiposProveedor, proveedores, depositosOrigen, depositosDestino, desde, hasta, cabeceras)
                        .ToList();
        }

        public ConsumoDetalle GetPrimerDespacho(int vehiculoId, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return GetDespacho(vehiculoId, fechaDesde, fechaHasta, true);
        }

        public ConsumoDetalle GetUltimoDespacho(int vehiculoId, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return GetDespacho(vehiculoId, fechaDesde, fechaHasta, false);
        }

        private ConsumoDetalle GetDespacho(int vehiculoId, DateTime? fechaDesde, DateTime? fechaHasta, bool orderAsc)
        {
            var q = Query.Where(d => d.ConsumoCabecera.Vehiculo.Id == vehiculoId)
                         .Where(d => d.Insumo.TipoInsumo.DeCombustible);

            if (fechaDesde.HasValue) q = q.Where(d => d.ConsumoCabecera.Fecha > fechaDesde);
            if (fechaHasta.HasValue) q = q.Where(d => d.ConsumoCabecera.Fecha < fechaHasta);

            q = orderAsc ? q.OrderBy(d => d.ConsumoCabecera.Fecha) 
                         : q.OrderByDescending(d => d.ConsumoCabecera.Fecha);

            return q.FirstOrDefault();
        }

        public ConsumoDetalle GetLastByDepositoInsumo(int depositoId, int insumoId)
        {
            return Query.Where(c => c.ConsumoCabecera.DepositoDestino != null)
                        .Where(c => c.ConsumoCabecera.DepositoDestino.Id == depositoId)
                        .Where(c => c.Insumo.Id == insumoId)
                        .Where(c => c.ConsumoCabecera.Estado != ConsumoCabecera.Estados.Eliminado)
                        .OrderByDescending(c => c.ConsumoCabecera.Fecha)
                        .FirstOrDefault();
        }

        public List<ConsumoDetalle> GetStock(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositos, IEnumerable<int> insumos, DateTime desde)
        {
            return Query.FilterInsumo(Session, empresas, lineas, insumos)
                        .Where(d => d.ConsumoCabecera.Fecha >= desde)
                        .Where(d => depositos.Contains(d.ConsumoCabecera.Deposito.Id) 
                                 || depositos.Contains(d.ConsumoCabecera.DepositoDestino.Id))
                        .Where(c => c.ConsumoCabecera.Estado != ConsumoCabecera.Estados.Eliminado)
                        .OrderByDescending(d => d.ConsumoCabecera.Fecha)
                        .ToList();
        }

        public double GetDespachoByVehicle(DateTime desde, DateTime hasta, int vehicle)
        {
            var despachos = Query.Where(d => d.ConsumoCabecera.Fecha >= desde
                                          && d.ConsumoCabecera.Fecha <= hasta
                                          && d.ConsumoCabecera.Vehiculo != null
                                          && d.ConsumoCabecera.Vehiculo.Id == vehicle
                                          && d.Insumo.TipoInsumo.DeCombustible);

            return despachos.Any() ? despachos.Sum(d => d.Cantidad) : 0.0;
        }
    }
}
