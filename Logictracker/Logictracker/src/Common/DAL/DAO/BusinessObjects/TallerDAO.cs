using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class TallerDAO : GenericDAO<Taller>
    {
//        public TallerDAO(ISession session) : base(session) { }

        #region Get Methods

        public List<Taller> GetList()
        {
            return Query.Where(taller => !taller.Baja)
                        .Cacheable()
                        .ToList();
        }

        public List<Taller> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(taller => !taller.Baja)
                .Cacheable()
                .ToList();
        }

        public Taller GetByCode(string codigo)
        {
            return Query.FirstOrDefault(t => !t.Baja && t.Codigo == codigo);
        }

        #endregion

        #region Override Methods

        public override void Delete(Taller taller)
        {
            if (taller == null) return;

            taller.Baja = true;

            SaveOrUpdate(taller);
            
            var dao = new DAOFactory();
 
            dao.ReferenciaGeograficaDAO.DeleteGeoRef(taller.ReferenciaGeografica.Id);
        }

        #endregion
    }
}
