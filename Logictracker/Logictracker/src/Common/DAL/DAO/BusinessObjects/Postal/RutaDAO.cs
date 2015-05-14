#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Postal;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Postal
{
    /// <summary>
    ///     Routes items data access class.
    /// </summary>
    public class RutaDAO : GenericDAO<Ruta>
    {
        #region Constructors

        /// <summary>
        ///     Instanciates a new route data access class using the givenn nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public RutaDAO(ISession session) : base(session) { }

        #endregion
        private readonly Dictionary<string, Distribuidor> _cacheDistribuidores = new Dictionary<string, Distribuidor>();

        private Distribuidor GetDistribuidorByCodigo(string codigoDistribuidor)
        {
            if (!_cacheDistribuidores.ContainsKey(codigoDistribuidor))
            {
                var distributorsDao = new DistribuidorDAO();
                _cacheDistribuidores.Add(codigoDistribuidor, distributorsDao.FindByPtmCode(codigoDistribuidor));
            }
            return _cacheDistribuidores[codigoDistribuidor];
        }

        #region Public Methods

        /// <summary>
        ///     Gets the distributors assigned to current available routes.
        /// </summary>
        /// <returns></returns>
        public List<GrupoRuta> GetAvailableDistributors(int days)
        {
            DateTime from = DateTime.UtcNow.AddDays(-days);
            List<Ruta> availableDistributors =
                Session.Query<Ruta>()
                    .Where(ruta => ruta.Estado <= 2 && ruta.Dispositivo == null && !ruta.FechaBaja.HasValue && (days == 0 || ruta.FechaModificacion > from))
                    .ToList();

            return (from ruta in availableDistributors
                let distribuidor = GetDistribuidorByCodigo(ruta.CodigoDistribuidor)
                where !distribuidor.FechaBaja.HasValue && !String.IsNullOrEmpty(distribuidor.Usuario) && !String.IsNullOrEmpty(distribuidor.Clave)
                select new GrupoRuta {Distribuidor = distribuidor, NumeroRuta = ruta.NumeroRuta}).Distinct().ToList();
        }

        /// <summary>
        ///     Gets the currently assigned an sincronized distributors.
        /// </summary>
        /// <returns></returns>
        public List<GrupoRuta> GetAssignedDistributors(int days)
        {
            DateTime from = DateTime.UtcNow.AddDays(-days);
            List<Ruta> assignedDistributors =
                Session.Query<Ruta>()
                    .Where(ruta => ruta.Estado <= 2 && ruta.Dispositivo != null && !ruta.FechaBaja.HasValue && (days == 0 || ruta.FechaModificacion > from))
                    .ToList();

            return (from ruta in assignedDistributors
                let distribuidor = GetDistribuidorByCodigo(ruta.CodigoDistribuidor)
                where !distribuidor.FechaBaja.HasValue && !String.IsNullOrEmpty(distribuidor.Usuario) && !String.IsNullOrEmpty(distribuidor.Clave)
                select new GrupoRuta {Distribuidor = distribuidor, NumeroRuta = ruta.NumeroRuta}).Distinct().ToList();
        }

        /// <summary>
        ///     Gets the currently assigned an sincronized distributors.
        /// </summary>
        /// <returns></returns>
        public List<GrupoRuta> GetDeletedDistributors(int days)
        {
            DateTime from = DateTime.UtcNow.AddDays(-days);
            List<Ruta> assignedDistributors =
                Session.Query<Ruta>()
                    .Where(ruta => ruta.Estado <= 2 && ruta.Dispositivo == null && ruta.FechaBaja.HasValue && (days == 0 || ruta.FechaModificacion > from))
                    .ToList();

            return (from ruta in assignedDistributors
                let distribuidor = GetDistribuidorByCodigo(ruta.CodigoDistribuidor)
                where !distribuidor.FechaBaja.HasValue && !String.IsNullOrEmpty(distribuidor.Usuario) && !String.IsNullOrEmpty(distribuidor.Clave)
                select new GrupoRuta {Distribuidor = distribuidor, NumeroRuta = ruta.NumeroRuta}).Distinct().ToList();
        }

        public void UnassignRoutes(GrupoRuta grupo)
        {
            using (SmartTransaction transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    UnassignRoutesWithoutTransaction(grupo);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof (RutaDAO).FullName, ex, "Exception in UnassignRoutes(GrupoRuta) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof (RutaDAO).FullName, ex, "Exception in UnassignRoutes(GrupoRuta)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof (RutaDAO).FullName, ex2, "Exception in UnassignRoutes(GrupoRuta) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        ///     Unassings all open routes associated to the givenn distributor.
        /// </summary>
        /// <param name="grupo"></param>
        protected void UnassignRoutesWithoutTransaction(GrupoRuta grupo)
        {
            List<Ruta> routes =
                Session.Query<Ruta>()
                    .Where(
                        route =>
                            route.CodigoDistribuidor == grupo.Distribuidor.Codigo && route.NumeroRuta == grupo.NumeroRuta && route.Estado <= 2 &&
                            route.Dispositivo != null && !route.FechaBaja.HasValue)
                    .ToList();

            foreach (Ruta route in routes)
            {
                route.Dispositivo = null;
                route.Distribuidor = null;
                route.Estado = 0;

                SaveOrUpdate(route);
            }
        }

        public void DeleteRoutes(GrupoRuta grupo)
        {
            using (SmartTransaction transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    DeleteRoutesWithoutTransaction(grupo);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof (RutaDAO).FullName, ex, "Exception in DeleteRoutes(GrupoRuta) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof (RutaDAO).FullName, ex, "Exception in DeleteRoutes(GrupoRuta)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof (RutaDAO).FullName, ex2, "Exception in DeleteRoutes(GrupoRuta) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        protected void DeleteRoutesWithoutTransaction(GrupoRuta grupo)
        {
            List<Ruta> routes =
                Session.Query<Ruta>()
                    .Where(
                        route =>
                            route.CodigoDistribuidor == grupo.Distribuidor.Codigo && route.NumeroRuta == grupo.NumeroRuta && route.Estado <= 2 &&
                            route.Dispositivo == null && !route.FechaBaja.HasValue)
                    .ToList();

            foreach (Ruta route in routes)
            {
                route.FechaBaja = DateTime.UtcNow;

                SaveOrUpdate(route);
            }
        }

        public void UndeleteRoutes(GrupoRuta grupo)
        {
            using (SmartTransaction transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    UndeleteRoutesWithoutTransaction(grupo);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof (RutaDAO).FullName, ex, "Exception in UndeleteRoutes(GrupoRuta) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof (RutaDAO).FullName, ex, "Exception in UndeleteRoutes(GrupoRuta)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof (RutaDAO).FullName, ex2, "Exception in UndeleteRoutes(GrupoRuta) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        protected void UndeleteRoutesWithoutTransaction(GrupoRuta grupo)
        {
            List<Ruta> routes =
                Query.Where(
                    route =>
                        route.CodigoDistribuidor == grupo.Distribuidor.Codigo && route.NumeroRuta == grupo.NumeroRuta && route.Estado <= 2 &&
                        route.Dispositivo == null && route.FechaBaja.HasValue).ToList();

            foreach (Ruta route in routes)
            {
                route.FechaBaja = null;

                SaveOrUpdate(route);
            }
        }

        /// <summary>
        ///     Gets the routes associated to the specified distributor.
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public IEnumerable<Ruta> GetRoutes(GrupoRuta distributor)
        {
            return
                Session.Query<Ruta>()
                    .Where(
                        route =>
                            route.CodigoDistribuidor == distributor.Distribuidor.Codigo && route.NumeroRuta == distributor.NumeroRuta && route.Estado <= 2 &&
                            route.Dispositivo == null && !route.FechaBaja.HasValue)
                    .ToList();
        }

        public Ruta GetRouteByPieza(string pieza)
        {
            return Session.Query<Ruta>().Where(route => route.Pieza == pieza).OrderByDescending(route => route.NumeroRuta).First();
        }

        #endregion
    }
}