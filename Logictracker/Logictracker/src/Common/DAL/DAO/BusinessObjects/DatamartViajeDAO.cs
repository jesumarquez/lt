using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.ReportObjects.Datamart;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class DatamartViajeDAO : GenericDAO<DatamartViaje>
    {
//        public DatamartViajeDAO(ISession session) : base(session) { }

        public void DeleteRecords(int idViaje)
        {
            var registros = GetRecords(idViaje);
            foreach (var registro in registros) Delete(registro);
        }

        public List<DatamartViaje> GetRecords(int idViaje)
        {
            return Query.Where(dm => dm.Viaje.Id == idViaje).ToList();
        }

        public List<DatamartViaje> GetList(int[] idsViajes)
        {
            return Query.Where(dm => idsViajes.Contains(dm.Viaje.Id))
                        .ToList();
        }
    }
}