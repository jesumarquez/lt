using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico
{
    public class StockVehicularDAO : GenericDAO<StockVehicular>
    {
        public List<StockVehicular> GetList(IEnumerable<int> empresas, IEnumerable<int> zonas, IEnumerable<int> tiposVehiculo)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterZona(Session, empresas, new[] { -1 }, zonas)
                        .FilterTipoVehiculo(Session, empresas, new[] { -1 }, tiposVehiculo)
                        .ToList();
        }

        public StockVehicular GetByZonaAndTipoCoche(int idZona, int idTipoCoche)
        {
            return Query.Where(s => s.Zona.Id == idZona
                                 && s.TipoCoche.Id == idTipoCoche)
                        .FirstOrDefault();
        }
    }
}
