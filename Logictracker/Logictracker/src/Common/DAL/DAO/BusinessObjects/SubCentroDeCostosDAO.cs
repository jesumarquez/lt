using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class SubCentroDeCostosDAO : GenericDAO<SubCentroDeCostos>
    {
//        public SubCentroDeCostosDAO(ISession session) : base(session) { }

        public SubCentroDeCostos FindByCodigo(int empresa, int linea, int departamento, int centroDeCostos, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .FilterCentroDeCostos(Session, new[] { empresa }, new[] { linea }, new[] { departamento}, new[] { centroDeCostos }, null)
                        .Where(scc => !scc.Baja && scc.Codigo == codigo)
                        .Cacheable()
                        .FirstOrDefault();
        } 

        public IEnumerable<SubCentroDeCostos> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos, IEnumerable<int> centroDeCostos)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterCentroDeCostos(Session, empresas, lineas, departamentos, centroDeCostos)
                        .Where(cc => !cc.Baja)
                        .Cacheable()
                        .ToList();
        }

        public override void Delete(SubCentroDeCostos center)
        {
            if (center == null) return;

            center.Baja = true;

            SaveOrUpdate(center);
        }

        public bool IsCodeUnique(string code, int empresa, int linea, int centroDeCostos, int currentId)
        {
            var sameCode = Query.FilterEmpresa(Session, new[] {empresa}, null)
                                .FilterLinea(Session, new[] {empresa}, new[] {linea}, null)
                                .FilterCentroDeCostos(Session, new[] {empresa}, new[] {linea}, new[] {-1}, new[] {centroDeCostos})
                                .Where(scc => !scc.Baja && scc.Codigo == code && scc.Id != currentId);

            return !sameCode.Any();
        }
    }
}
