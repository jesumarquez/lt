using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class CentroDeCostosDAO : GenericDAO<CentroDeCostos>
    {
//        public CentroDeCostosDAO(ISession session) : base(session) { }

        public CentroDeCostos FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                .Where(l => !l.Baja && l.Codigo == codigo)
                .Cacheable()
                .FirstOrDefault();
        } 

        #region Public Methods
       
        public int GetTanque(int idCenCostos)
        {
            const string query = @"from Tanque t where t.CentroDeCostos.Id = :cenCostos";

            var result = from Tanque t in Session.CreateQuery(query).SetParameter("cenCostos", idCenCostos).List() select t;

            return result.First().Id;
        }

        public CentroDeCostos GetByCode(string code)
        {
            var list = Session.CreateCriteria(typeof (CentroDeCostos))
                .Add(Restrictions.Eq("Codigo", code))
                .List();

            return list.Count > 0 ? (CentroDeCostos) list[0] : null;
        }

        public List<CentroDeCostos> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterDepartamento(Session, empresas, lineas, departamentos)
                        .Where(cc => !cc.Baja)
                        .Cacheable()
                        .ToList();
        }

        public IEnumerable<CentroDeCostos> GetCentrosDeCostosPermitidosPorUsuario(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var userId = sessionUser != null ? sessionUser.Id : 0;

            var tableEmpresas = Ids2DataTable(empresas);
            var tableLineas = Ids2DataTable(lineas);
            var tableDeptos = Ids2DataTable(departamentos);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_CentroDeCostosDAO_GetCentrosDeCostosPermitidosPorUsuario] @empresasIds = :empresasIds, @lineasIds = :lineasIds, @deptosIds = :deptosIds, @userId = :userId;")
                              .AddEntity(typeof(CentroDeCostos))
                              .SetStructured("empresasIds", tableEmpresas)
                              .SetStructured("lineasIds", tableLineas)
                              .SetStructured("deptosIds", tableDeptos)
                              .SetInt32("userId", userId);
            var results = sqlQ.List<CentroDeCostos>();
            return results;
        }


        public IList FindByEmpresasAndLineas(List<int> empresas, List<int> lineas)
        {
            return Session.Query<CentroDeCostos>().Where(c => !c.Baja
                && (empresas.Contains(-1) || empresas.Contains(0) || c.Empresa == null || empresas.Contains(c.Empresa.Id))
                && (lineas.Contains(-1) || lineas.Contains(0) || (c.Linea == null && (c.Empresa == null || empresas.Contains(c.Empresa.Id)))
                    || (c.Linea != null && lineas.Contains(c.Linea.Id)))).ToList();
        }

        public IEnumerable FindByEmpresasYLineasAndUser(List<int> empresas, List<int> lineas, Usuario user)
        {
            var centros = FindByEmpresasAndLineas(empresas, lineas);

            if (user == null) return centros;

            return (from CentroDeCostos c in centros
                    where ((user.CentrosCostos.IsEmpty() || user.CentrosCostos.Contains(c)) && ((c.Empresa == null && c.Linea == null) 
                            || (c.Empresa != null && c.Linea == null && (user.Empresas.IsEmpty() || user.Empresas.Contains(c.Empresa)))
                            || (c.Linea != null && (user.Lineas.IsEmpty() || user.Lineas.Contains(c.Linea)) && (user.Empresas.IsEmpty() || user.Empresas.Contains(c.Linea.Empresa)))))
                    orderby c.Descripcion
                    select c
                    ).ToList();
        }

        public override void Delete(CentroDeCostos center)
        {
            if (center == null) return;

            center.Baja = true;

            SaveOrUpdate(center);
        }

        public List<CentroDeCostos> FindByCode(string code)
        {
            return Session.Query<CentroDeCostos>().Where(r => r.Codigo == code && !r.Baja).ToList();
        }

        public bool IsCodeUnique(string code, Empresa empresa, Linea linea, int currentId)
        {
            var sameCode = FindByCode(code)
                       .Where(p => !p.Baja && 
                                   ((p.Linea == null && p.Empresa == null)
                                   || (empresa == null && linea == null)
                                   || ((p.Linea == null || linea == null) && empresa != null && p.Empresa.Id == empresa.Id)
                                   || (p.Linea != null && linea != null && p.Linea.Id == linea.Id)));

            if (!sameCode.Any()) return true;
            if (sameCode.Count() > 1) return false;
            return sameCode.First().Id == currentId;
        }

        #endregion
    }
}
