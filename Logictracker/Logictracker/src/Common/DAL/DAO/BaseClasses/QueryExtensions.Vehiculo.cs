using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate.Linq;
using NHibernate;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<Coche> FilterVehiculo(this IQueryable<Coche> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> tipos)
        {
            var vehiculosU = GetVehiculos(session, empresas, lineas, transportistas, departamentos, centrosDeCostos, tipos, new[] { -1 });

            if (vehiculosU != null) q = q.Where(t => vehiculosU.Contains(t));

            return q;
        }

        public static IQueryable<TQuery> FilterVehiculo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> tipos, IEnumerable<int> vehiculos)
            where TQuery : IHasVehiculo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var vehiculosU = GetVehiculos(session, empresas, lineas, transportistas, departamentos, centrosDeCostos, tipos, vehiculos);

            var includesAll = IncludesAll(vehiculos);

            return FilterVehiculo(q, vehiculosU, includesAll, user);
        }
       
        public static IQueryable<TQuery> FilterVehiculo<TQuery>(this IQueryable<TQuery> q, IQueryable<Coche> vehiculos, bool includesAll, Usuario user)
            where TQuery : IHasVehiculo
        {
            if (vehiculos != null)
            {
                var vehiculosList = vehiculos.ToList();

                q = q.Where(m => m.Vehiculo == null || vehiculosList.Contains(m.Vehiculo));
            }

            if (!includesAll) q = q.Where(m => m.Vehiculo != null);

            if (user != null && user.PorCoche)
            {
                var coches = user.Coches;
                q = q.Where(m => m.Vehiculo == null || coches.Contains(m.Vehiculo));
            }

            return q;
        }
        public static IEnumerable<TQuery> FilterVehiculo<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> tipos, IEnumerable<int> vehiculos)
            where TQuery : IHasVehiculo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var vehiculosU = GetVehiculos(session, empresas, lineas, transportistas, departamentos, centrosDeCostos, tipos, vehiculos);

            var includesAll = IncludesAll(vehiculos);

            return FilterVehiculo(q, vehiculosU, includesAll, user);
        }
        public static IEnumerable<TQuery> FilterVehiculo<TQuery>(this IEnumerable<TQuery> q, IQueryable<Coche> vehiculos, bool includesAll, Usuario user)
            where TQuery : IHasVehiculo
        {
            if (vehiculos != null)
            {
                var vehiculosList = vehiculos.ToList();

                q = q.Where(m => m.Vehiculo == null || vehiculosList.Contains(m.Vehiculo));
            }

            if (!includesAll) q = q.Where(m => m.Vehiculo != null);

            if (user != null && user.PorCoche)
            {
                var coches = user.Coches.OfType<Coche>().ToList();
                q = q.Where(m => coches.Contains(m.Vehiculo));
            }

            return q;
        }

        public static IQueryable<Coche> GetVehiculos(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> tipos, IEnumerable<int> vehiculos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var cocheDao = new CocheDAO();

            if (empresas == null && lineas == null && departamentos == null && tipos == null && transportistas == null && centrosDeCostos == null && (user == null || !user.PorCoche) && IncludesAll(vehiculos))
                return null;

            if (vehiculos == null) vehiculos = new[] { -1 };

            var empresasU = GetEmpresas(session, empresas);
            var lineasU = GetLineas(session, empresas, lineas);
            var tiposU = GetTipoVehiculo(session, empresas, lineas, tipos);
            var transportistasU = GetTransportistas(session, empresas, lineas, transportistas).ToList();
            var departamentosU = GetDepartamentos(session, empresas, lineas, departamentos);
            var centrosDeCostosU = GetCentrosDeCosto(session, empresasU, lineasU, departamentosU, centrosDeCostos);

            var vehiculosQ = (user != null && user.PorCoche
                                       ? user.Coches.AsQueryable().FilterEmpresa(empresasU).FilterLinea(lineasU).FilterTipoVehiculo(tiposU)
                                       : cocheDao.FindList(empresas, lineas, tipos)
                                  );
            if(transportistasU != null)
            {
                var includesNone = IncludesNone(transportistas);
                var includesAll = IncludesAll(transportistas);

                vehiculosQ = vehiculosQ.FilterTransportista(transportistasU, includesAll, includesNone, user);
            }
            if(centrosDeCostosU != null)
            {
                var includesNone = IncludesNone(centrosDeCostos);
                var includesAll = IncludesAll(centrosDeCostos);

                vehiculosQ = vehiculosQ.FilterCentroDeCostos(centrosDeCostosU, includesAll, includesNone, user);
            }

            var vehiculosU = vehiculosQ;

            if (!IncludesAll(vehiculos))
                vehiculosU = vehiculosU.Where(l => vehiculos.Contains(l.Id));

            return vehiculosU;
        }
        public static List<Coche> GetVehiculos(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> tipos, IEnumerable<int> vehiculos, Usuario user)
        {
            if (empresas == null && lineas == null && departamentos != null && transportistas == null && centrosDeCostos == null && IncludesAll(vehiculos)) return null;

            var vehiculoQ = session.Query<Coche>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user)
                .FilterTipoVehiculo(session, empresas, lineas, tipos)
                .FilterTransportista(session, empresas, lineas, transportistas, user)
                .FilterCentroDeCostos(session, empresas, lineas, departamentos, centrosDeCostos, user);

            var vehiculoU = vehiculoQ.Cacheable().ToList();

            if (!IncludesAll(vehiculos)) vehiculoU = vehiculoU.Where(l => vehiculos.Contains(l.Id)).ToList();
            if (!IncludesAll(transportistas) || !IncludesNone(transportistas)) vehiculoU = vehiculoU.Where(l => transportistas.Contains(l.Transportista.Id)).ToList();

            return vehiculoU;
        }
    }
}
