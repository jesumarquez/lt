using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class DepartamentoDAO: GenericDAO<Departamento>
    {
//        public DepartamentoDAO(ISession session) : base(session) { }

        public Departamento FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[]{empresa}, null)
                .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                .Where(l => !l.Baja && l.Codigo == codigo)
                .Cacheable()
                .FirstOrDefault();
        }

        #region Get Methods

        public List<Departamento> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(l => !l.Baja)
                .Cacheable()
                .ToList();
        }

        #endregion

        #region Override Methods

        public override void Delete(Departamento departamento)
        {
            if (departamento == null) return;
            departamento.Baja = true;
            SaveOrUpdate(departamento);
        }

        #endregion

        
    }
}