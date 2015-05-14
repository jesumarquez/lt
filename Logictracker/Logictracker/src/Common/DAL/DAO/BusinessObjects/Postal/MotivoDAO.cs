#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Postal;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Postal
{
    /// <summary>
    /// Reasons data access class.
    /// </summary>
    public class MotivoDAO : GenericDAO<Motivo>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public MotivoDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds all clients.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Motivo> FindAll() { return Session.Query<Motivo>().Where(reason => reason.FechaBaja == null).ToList(); }

        public Motivo FindByCodigo(string codigo)
        {
			return Session.Query<Motivo>().Where(m => m.Codigo == codigo).SafeFirstOrDefault();
        }

        /// <summary>
        /// Deletes the specified reason.
        /// </summary>
        /// <param name="motivo"></param>
        public override void Delete(Motivo motivo)
        {
            if (motivo == null) return;

            motivo.FechaModificacion = DateTime.UtcNow;
            motivo.FechaBaja = DateTime.UtcNow;

            SaveOrUpdate(motivo);
        }

        /// <summary>
        /// Gets the reasons associated to the specified distributor.
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public IEnumerable<Motivo> GetReasons(Distribuidor distributor)
        {
            var reasons = new List<Motivo>();

            var clientes = Session.Query<Ruta>().Where(route => route.CodigoDistribuidor == distributor.Codigo).Select(route => route.Cliente).Distinct().ToList();

            if (clientes.Count.Equals(0)) return reasons;

            var reasonsToAdd = clientes.Select(cliente => FindByClient(cliente)).SelectMany(clientReasons => clientReasons.Where(clientReason => !reasons.Contains(clientReason)));

            reasons.AddRange(reasonsToAdd);

            return reasons.Where(reason => !reason.FechaBaja.HasValue).ToList();
        }

        /// <summary>
        /// Gets the reasons associate dto the specified client.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        private IEnumerable<Motivo> FindByClient(Int32 cliente)
        {
            return Session.Query<Motivo>().ToList();
        }

        #endregion
    }
}
