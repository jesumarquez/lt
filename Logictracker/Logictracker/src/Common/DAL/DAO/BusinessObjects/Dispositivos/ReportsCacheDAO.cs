using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    /// <summary>
    /// Device data access class.
    /// </summary>
    public class ReportsCacheDAO : GenericDAO<ReportsCache>
    {
//		public ReportsCacheDAO(ISession session) : base(session) { }

		public bool ExistsValue(int dispositivo, string value)
		{
			return Query.Any(c => c.Dispositivo == dispositivo && c.Value == value);
		}
    }
}