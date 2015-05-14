using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico
{
    public class AgendaVehicularDAO : GenericDAO<AgendaVehicular>
    {
        public List<AgendaVehicular> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> vehiculos, DateTime desde, DateTime hasta)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterVehiculo(Session, empresas, lineas, new[]{-1}, new[]{-1},new[]{-1},new[]{-1}, vehiculos)
                        .Where(a => (a.FechaDesde >= desde && a.FechaDesde <= hasta) 
                                 || (a.FechaHasta >= desde && a.FechaHasta <= hasta)
                                 || (a.FechaDesde <= desde && a.FechaHasta >= hasta))
                        .ToList();
        }
    }
}
