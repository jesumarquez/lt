using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterViajeDistribucion<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centros, IEnumerable<int> subCentros, IEnumerable<int> vehiculos, IEnumerable<int> viajes)
            where TQuery : IHasViajeDistribucion
        {
            return FilterViajeDistribucion(q, session, empresas, lineas, transportistas, departamentos, centros, subCentros, vehiculos, viajes, null, null);
        }
        public static IQueryable<TQuery> FilterViajeDistribucion<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centros, IEnumerable<int> subCentros, IEnumerable<int> vehiculos, IEnumerable<int> viajes, DateTime? desde, DateTime? hasta)
            where TQuery : IHasViajeDistribucion
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterViajeDistribucion(q, session, empresas, lineas, transportistas, departamentos, centros, subCentros, vehiculos, viajes, desde, hasta, user);
        }
        public static IQueryable<TQuery> FilterViajeDistribucion<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centros, IEnumerable<int> subCentros, IEnumerable<int> vehiculos, IEnumerable<int> viajes, DateTime? desde, DateTime? hasta, Usuario user)
            where TQuery : IHasViajeDistribucion
        {
            var viajesU = GetViajeDistribucion(session, empresas, lineas, transportistas, departamentos, centros, subCentros, vehiculos, viajes, desde, hasta, user);
            if (viajesU != null) q = q.Where(c => c.Viaje == null || viajesU.Contains(c.Viaje));

            return q;
        }

        private static List<ViajeDistribucion> GetViajeDistribucion(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centros, IEnumerable<int> subCentros, IEnumerable<int> vehiculos, IEnumerable<int> viajes, DateTime? desde, DateTime? hasta)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetViajeDistribucion(session, empresas, lineas, transportistas, departamentos, centros, subCentros, vehiculos, viajes, desde, hasta, user);
        }
        private static List<ViajeDistribucion> GetViajeDistribucion(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centros, IEnumerable<int> subCentros, IEnumerable<int> vehiculos, IEnumerable<int> viajes, DateTime? desde, DateTime? hasta, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(viajes)) return null;

            var tipos = new[] {-1};

            var viajesQ = session.Query<ViajeDistribucion>()
                                 .FilterEmpresa(session, empresas, user)
                                 .FilterLinea(session, empresas, lineas, user);

            if (!IncludesAll(departamentos) || !IncludesAll(centros))
                viajesQ = viajesQ.FilterCentroDeCostos(session, empresas, lineas, departamentos, centros, user);
            if (!IncludesAll(subCentros))
                viajesQ = viajesQ.FilterSubCentroDeCostos(session, empresas, lineas, departamentos, centros, subCentros, user);
            if (!IncludesAll(transportistas) || !IncludesAll(departamentos) || !IncludesAll(centros) || !IncludesAll(vehiculos))
                viajesQ = viajesQ.FilterVehiculo(session, empresas, lineas, transportistas, departamentos, centros, tipos, vehiculos);

            if (desde.HasValue) viajesQ = viajesQ.Where(v => v.Inicio >= desde.Value);
            if (hasta.HasValue) viajesQ = viajesQ.Where(v => v.Inicio <= hasta.Value);

            var viajesU = viajesQ.Cacheable().ToList();

            if (!IncludesAll(viajes)) viajesU = viajesU.Where(l => viajes.Contains(l.Id)).ToList();

            return viajesU;
        }
    }
}
