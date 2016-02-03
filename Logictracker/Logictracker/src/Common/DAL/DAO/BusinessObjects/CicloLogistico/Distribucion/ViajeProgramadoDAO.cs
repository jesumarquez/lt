using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion
{
    public class ViajeProgramadoDAO : GenericDAO<ViajeProgramado>
    {
        public ViajeProgramado FindByCodigo(int empresa, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FirstOrDefault(t => t.Codigo == codigo);
        }

        public List<ViajeProgramado> GetList(IEnumerable<int> empresas, IEnumerable<int> transportistas)
        {
            var q = Query.FilterEmpresa(Session, empresas);                         

            if (!QueryExtensions.IncludesAll(transportistas))
                q = q.FilterTransportista(Session, empresas, new[] { -1 }, transportistas);

            return q.ToList();
        }
    }
}
