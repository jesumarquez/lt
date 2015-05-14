#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    public class ConfiguracionDispositivoDAO: GenericDAO<ConfiguracionDispositivo>
    {
        #region Constructor

    	/// <summary>
    	/// Instanciates a new data access class using the provided nhibernate sessions.
    	/// </summary>
    	/// <param name="session"></param>
//    	public ConfiguracionDispositivoDAO(ISession session) : base(session) { }

        #endregion

        public new IEnumerable<ConfiguracionDispositivo> FindAll() { return base.FindAll().ToList(); }
    }
}
