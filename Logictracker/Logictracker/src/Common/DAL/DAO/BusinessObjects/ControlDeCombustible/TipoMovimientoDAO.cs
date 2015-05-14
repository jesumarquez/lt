#region Usings

using System.Collections;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible
{
    public class TipoMovimientoDAO : GenericDAO<TipoMovimiento>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TipoMovimientoDAO(ISession session) : base(session) { }

        #endregion

        public TipoMovimiento GetByCode(string code)
        {
            return (from TipoMovimiento t in Session.Query<TipoMovimiento>().ToList()
                    where t.Codigo.Equals(code)
                    select t).First();
        }

        public IList FindAllUserAvaiable()
        {
            return (from TipoMovimiento t in Session.Query<TipoMovimiento>().ToList()
                 where t.Codigo.Equals("C") || t.Codigo.Equals("E")
                 select t).ToList();
        }
    }
}
