using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico.TimeTracking;
using NHibernate;
using System.Collections.Generic;
using System;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.TimeTracking
{
    public class EventoViajeDAO : GenericDAO<EventoViaje>
    {
//        public EventoViajeDAO(ISession session) : base(session) { }

        public EventoViaje FindFirstInicioFin(int vehiculo, DateTime? desde, DateTime? hasta)
        {
            var q = Query.Where(e => e.Vehiculo.Id == vehiculo && ((e.EsInicio && !e.EsEntrada) || (e.EsFin && e.EsEntrada)));
            if(desde.HasValue)
            {
                q = q.Where(e => e.Fecha >= desde).OrderBy(e => e.Fecha);
            }
            else if (hasta.HasValue)
            {
                q = q.Where(e => e.Fecha <= hasta).OrderByDescending(e => e.Fecha);
            }
            return q.FirstOrDefault();
        }
        public List<EventoViaje> FindInicioFin(int vehiculo, DateTime desde, DateTime hasta)
        {
            return Query
                .Where(e => e.Vehiculo.Id == vehiculo && e.Fecha >= desde && e.Fecha <= hasta && ((e.EsInicio && !e.EsEntrada) || (e.EsFin && e.EsEntrada)))
                .ToList();
        }

        public List<EventoViaje> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> vehiculos, IEnumerable<int> empleados, DateTime desde, DateTime hasta)
        {
            var all = new[] {-1};
            var q = Query.FilterVehiculo(Session, empresas, lineas, all, all, all, all, vehiculos)
                         .Where(e => e.Fecha >= desde && e.Fecha <= hasta);

            if (!QueryExtensions.IncludesAll(empleados))
                q = q.FilterEmpleado(Session, empresas, lineas, all, all, empleados);
            
            return q.ToList();
        }
    }
}
