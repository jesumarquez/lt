using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterConsumoCabecera<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> tiposVehiculo, IEnumerable<int> vehiculos, IEnumerable<int> tiposEmpleado, IEnumerable<int> empleados, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores, IEnumerable<int> depositosOrigen, IEnumerable<int> depositosDestino, DateTime? desde, DateTime? hasta, IEnumerable<int> cabeceras)
            where TQuery : IHasConsumoCabecera
        {
            var consumoDetallesU = GetConsumoCabecera(session, empresas, lineas, transportistas, departamentos, centrosDeCosto, tiposVehiculo, vehiculos, tiposEmpleado, empleados, tiposProveedor, proveedores, depositosOrigen, depositosDestino, desde, hasta, cabeceras);

            if (consumoDetallesU != null) q = q.Where(d => consumoDetallesU.Contains(d.ConsumoCabecera));

            return q;
        }

        private static List<ConsumoCabecera> GetConsumoCabecera(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> tiposVehiculo, IEnumerable<int> vehiculos, IEnumerable<int> tiposEmpleado, IEnumerable<int> empleados, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores, IEnumerable<int> depositosOrigen, IEnumerable<int> depositosDestino, DateTime? desde, DateTime? hasta, IEnumerable<int> cabeceras)
        {
            if (empresas == null && lineas == null && IncludesAll(cabeceras)) return null;

            var dao = new ConsumoCabeceraDAO();
            return dao.GetList(empresas, lineas, transportistas, departamentos, centrosDeCosto, tiposVehiculo, vehiculos, tiposEmpleado, empleados, tiposProveedor, proveedores, depositosOrigen, depositosDestino, desde, hasta);
        }
    }
}
