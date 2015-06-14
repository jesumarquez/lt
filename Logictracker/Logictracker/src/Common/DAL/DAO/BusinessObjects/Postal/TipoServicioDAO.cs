#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Postal;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Postal
{
    public class TipoServicioDAO : GenericDAO<TipoServicio>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TipoServicioDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds all clients.
        /// </summary>
        /// <returns></returns>
        public override IQueryable<TipoServicio> FindAll()
        {
            return Session.Query<TipoServicio>()
                .Where(service => service.FechaBaja == null);
        }

        /// <summary>
        /// Deletes the specified service type.
        /// </summary>
        /// <param name="tipoServicio"></param>
        public override void Delete(TipoServicio tipoServicio)
        {
            if (tipoServicio == null) return;

            tipoServicio.FechaModificacion = DateTime.UtcNow;
            tipoServicio.FechaBaja = DateTime.UtcNow;

            SaveOrUpdate(tipoServicio);
        }

        /// <summary>
        /// Gets the service types associated to the specified distributor.
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public IEnumerable<TipoServicio> GetServiceTypes(Distribuidor distributor)
        {
            var results = new List<TipoServicio>();

            var services = Session.Query<Ruta>().Where(route => route.CodigoDistribuidor == distributor.Codigo).Select(route => route.CodigoTipoServicio).ToList();

            if (services.Count.Equals(0)) return results;

            var servicesToAdd = services.Select(service => FindByCode(service)).Where(serviceToAdd => serviceToAdd != null && !results.Contains(serviceToAdd));
            
            results.AddRange(servicesToAdd);

            return results.Where(service => !service.FechaBaja.HasValue).ToList();
        }

        /// <summary>
        /// Get a service type by its code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public TipoServicio FindByCode(String code) { return Session.Query<TipoServicio>().FirstOrDefault(service => service.Codigo == code); }

        #endregion
    }
}
