using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterCentroDeCostos<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos)
            where TQuery : IHasCentroDeCosto
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterCentroDeCostos(q, session, empresas, lineas, departamentos, centrosDeCostos, user);
        }
        public static IQueryable<TQuery> FilterCentroDeCostos<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, Usuario user)
            where TQuery : IHasCentroDeCosto
        {
            var empresasU = GetEmpresas(session, empresas);
            var lineasU = GetLineas(session, empresas, lineas);
            var departamentosU = GetDepartamentos(session, empresas, lineas, departamentos);
            var centrosDeCostosU = GetCentrosDeCosto(session, empresasU, lineasU, departamentosU, centrosDeCostos);

            var includesAll = IncludesAll(centrosDeCostos);
            var includesNone = IncludesNone(centrosDeCostos);

            return FilterCentroDeCostos(q, centrosDeCostosU, includesAll, includesNone, user);
        }

        public static IQueryable<TQuery> FilterCentroDeCostos<TQuery>(this IQueryable<TQuery> q, IQueryable<CentroDeCostos> centrosDeCostos, bool includesAll, bool includesNone, Usuario user)
            where TQuery : IHasCentroDeCosto
        {
            if (centrosDeCostos != null) q = q.Where(t => t.CentroDeCostos == null || centrosDeCostos.Contains(t.CentroDeCostos));

            var porCentroDeCostos = user != null && user.PorCentroCostos;

            if ((!includesNone && !includesAll) || porCentroDeCostos) q = q.Where(t => t.CentroDeCostos != null);

            return q;
        }
     
        public static IQueryable<CentroDeCostos> GetCentrosDeCosto(ISession session, IQueryable<Empresa> empresas, IQueryable<Linea> lineas, IQueryable<Departamento> departamentos, IEnumerable<int> centrosCosto)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var centroDeCostoDao = new CentroDeCostosDAO();

            if (empresas == null && lineas == null && (user == null || user.CentrosCostos.Count == 0) && (centrosCosto == null || centrosCosto.Contains(-1) || centrosCosto.Contains(0)))
                return null;

            if (centrosCosto == null) centrosCosto = new[] { -1 };

            var centroDeCostosU = (user != null && user.CentrosCostos.Count > 0
                                       ? user.CentrosCostos.AsQueryable()
                                       : centroDeCostoDao.FindAll()
                                  );
            if (empresas != null) centroDeCostosU = centroDeCostosU.FilterEmpresa(empresas);
            if (lineas != null) centroDeCostosU = centroDeCostosU.FilterLinea(lineas.ToList());
            if (departamentos != null) centroDeCostosU = centroDeCostosU.FilterDepartamento(departamentos);
            if (!IncludesAll(centrosCosto)) centroDeCostosU = centroDeCostosU.Where(l => centrosCosto.Contains(l.Id));

            return centroDeCostosU;
        } 
    }
}
