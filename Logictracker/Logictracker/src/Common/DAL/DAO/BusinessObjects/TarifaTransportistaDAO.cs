#region Usings

using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class TarifaTransportistaDAO: GenericDAO<TarifaTransportista>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TarifaTransportistaDAO(ISession session) : base(session) { }

        #endregion

        public TarifaTransportista GetTarifaParaCliente(int transportista, int cliente)
        {
            var tarifa = Session.CreateQuery("from TarifaTransportista t where t.Transportista.Id = :trans and t.Cliente.Id = :cli")
                    .SetParameter("trans", transportista)
                    .SetParameter("cli", cliente)
                    .List();

            return tarifa.Count > 0 ? tarifa[0] as TarifaTransportista : null;
        }
    }
}
