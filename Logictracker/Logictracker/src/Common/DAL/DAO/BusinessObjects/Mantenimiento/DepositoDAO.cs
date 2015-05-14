using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class DepositoDAO : GenericDAO<Deposito>
    {
//        public DepositoDAO(ISession session) : base(session) { }

        public Deposito FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .Where(d => d.Codigo == code)
                        .Where(d => !d.Baja)
                        .Cacheable()
                        .FirstOrDefault();
        }

        public List<Deposito> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(d => !d.Baja)
                        .ToList();
        }

        public bool IsCodeUnique(int empresa, int linea, int idDeposito, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                       .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                       .Where(d => d.Id != idDeposito)
                       .Where(d => d.Codigo == code).FirstOrDefault(d => !d.Baja) == null;
        }

        public override void Delete(Deposito obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public Deposito GetByDescripcion(IEnumerable<int> empresas, IEnumerable<int> lineas, string descripcion)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(d => d.Descripcion == descripcion).FirstOrDefault(d => !d.Baja);
        }
    }
}
