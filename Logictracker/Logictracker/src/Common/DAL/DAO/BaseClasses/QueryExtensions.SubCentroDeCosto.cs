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
        public static IQueryable<TQuery> FilterSubCentroDeCostos<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> subCentrosDeCostos)
            where TQuery : IHasSubCentroDeCosto
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterSubCentroDeCostos(q, session, empresas, lineas, departamentos, centrosDeCostos, subCentrosDeCostos, user);
        }
        public static IQueryable<TQuery> FilterSubCentroDeCostos<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> subCentrosDeCostos, Usuario user)
            where TQuery : IHasSubCentroDeCosto
        {
            var empresasU = GetEmpresas(session, empresas);
            var lineasU = GetLineas(session, empresas, lineas);
            var departamentosU = GetDepartamentos(session, empresas, lineas, departamentos);
            var centrosDeCostosU = GetCentrosDeCosto(session, empresasU, lineasU, departamentosU, centrosDeCostos);
            var subCentrosDeCostosU = GetSubCentrosDeCosto(session, empresasU, lineasU, departamentosU, centrosDeCostosU, subCentrosDeCostos);

            var includesAll = IncludesAll(subCentrosDeCostos);

            return FilterSubCentroDeCostos(q, subCentrosDeCostosU, includesAll, user);
        }

        public static IQueryable<TQuery> FilterSubCentroDeCostos<TQuery>(this IQueryable<TQuery> q, List<SubCentroDeCostos> subCentrosDeCostos, bool includesAll, Usuario user)
            where TQuery : IHasSubCentroDeCosto
        {
            if (subCentrosDeCostos != null) q = q.Where(t => t.SubCentroDeCostos == null || subCentrosDeCostos.Contains(t.SubCentroDeCostos));

            if (!includesAll) q = q.Where(t => t.SubCentroDeCostos != null);

            return q;
        }
        public static List<SubCentroDeCostos> GetSubCentrosDeCosto(ISession session, IEnumerable<Empresa> empresas, IEnumerable<Linea> lineas, IEnumerable<Departamento> departamentos, IEnumerable<CentroDeCostos> centrosCosto, IEnumerable<int> subCentrosDeCostos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var subCentroDeCostoDao = new SubCentroDeCostosDAO();

            if (empresas == null && lineas == null && (user == null || user.CentrosCostos.Count == 0) && (centrosCosto == null))
                return null;

            if (subCentrosDeCostos == null) subCentrosDeCostos = new[] { -1 };            

            var subCentrosDeCostosU = subCentroDeCostoDao.GetList(empresas.Select(e => e.Id), lineas.Select(l => l.Id), departamentos.Select(d => d.Id), centrosCosto.Select(c => c.Id));

            if (!IncludesAll(subCentrosDeCostos)) subCentrosDeCostosU = subCentrosDeCostosU.Where(l => subCentrosDeCostos.Contains(l.Id));

            return subCentrosDeCostosU.ToList();
        } 
    }
}
