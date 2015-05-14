using System;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    public class TicketMantenimientoDAO : GenericDAO<TicketMantenimiento>
    {
//        public TicketMantenimientoDAO(ISession session) : base(session) { }

        public List<TicketMantenimiento> GetList(IEnumerable<int> talleres, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> vehiculos, DateTime desde, DateTime hasta)
        {
            var q = Query.FilterTaller(Session, talleres)
                         .FilterEmpresa(Session, empresas);

            if (!QueryExtensions.IncludesAll(vehiculos))
                q = q.FilterVehiculo(Session, empresas, lineas, new[]{-1}, new[]{-1}, new[]{-1}, new[]{-1}, vehiculos);
                
            return q.Where(t => t.FechaSolicitud >= desde 
                             && t.FechaSolicitud <= hasta)
                    .ToList();
        }

        public TicketMantenimiento GetByCode(int empresa, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] {empresa})
                        .FirstOrDefault(t => t.Codigo == codigo);
        }

        public bool IsCodeUnique(int id, int empresa, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FirstOrDefault(d => d.Id != id && d.Codigo == code) == null;
        }

        public bool IsUnique(int id, int taller, int vehiculo, short estado)
        {
            return Query.FilterTaller(Session, new[] {taller})
                        .FilterVehiculo(Session, new[] {-1}, new[] {-1}, new[] {-1}, new[] {-1}, new[] {-1}, new[] {-1}, new[] {vehiculo})
                        .FirstOrDefault(t => TicketMantenimiento.EstadosTicket.EstadosAbiertos.Contains(estado)
                                          && t.Id != id ) == null;
        }

        public TicketMantenimiento GetActive(int idCoche, int idGeocerca)
        {
            return Query.FirstOrDefault(t => t.Vehiculo.Id == idCoche
                                          && TicketMantenimiento.EstadosTicket.EstadosAbiertos.Contains(t.Estado)
                                          && t.Taller.ReferenciaGeografica != null 
                                          && t.Taller.ReferenciaGeografica.Id == idGeocerca);
        }
    }
}
