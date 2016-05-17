using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico
{
    public class DetalleStockVehicularDAO : GenericDAO<DetalleStockVehicular>
    {
        public List<DetalleStockVehicular> GetByStockAndCoche(int idStock, int idCoche)
        {
            return Query.Where(d => d.StockVehicular.Id != idStock
                                 && d.Vehiculo.Id == idCoche)
                        .ToList();
        }
    }
}
