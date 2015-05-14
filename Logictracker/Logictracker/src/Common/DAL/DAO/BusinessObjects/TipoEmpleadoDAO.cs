#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class TipoEmpleadoDAO : GenericDAO<TipoEmpleado>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TipoEmpleadoDAO(ISession session) : base(session) { }

        #endregion

        public override void Delete(TipoEmpleado obj)
        {
            if (obj == null) return;

            obj.FechaBaja = DateTime.UtcNow;
            obj.Baja = true;

            SaveOrUpdate(obj);
        }
        public TipoEmpleado FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                .Where(l => !l.Baja && l.Codigo == codigo)
                .Cacheable()
                .FirstOrDefault();
        }

        private IList FindByEmpresaAndLinea(int empresa, int linea)
        {
            var sql = "from TipoEmpleado t where t.Baja = 0";

            if (empresa > 0) sql = string.Concat(sql, " and (t.Empresa is null or t.Empresa.Id = :emp)");

            if (linea > 0) sql = string.Concat(sql, " and (t.Linea is null or t.Linea.Id = :lin)");

            var query = Session.CreateQuery(string.Concat(sql, " order by t.Descripcion"));

            if (empresa > 0) query = query.SetParameter("emp", empresa);

            if (linea > 0) query = query.SetParameter("lin", linea);

            return query.List();
        }

        public IList FindByEmpresaAndLinea(Empresa empresa, Linea linea, Usuario usuario)
        {
            var lin = linea != null ? linea.Id : -1;
            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;

            var tipos = FindByEmpresaAndLinea(emp, lin);

            if (usuario == null) return tipos;

            var daof = new DAOFactory();

            return (from TipoEmpleado t in tipos
                    where (t.Empresa == null && t.Linea == null) ||
                          daof.LineaDAO.GetList(new[] { -1 }).Contains(t.Linea) ||
                          (daof.EmpresaDAO.GetList().Contains(t.Empresa) && t.Linea == null)
                    select t).ToList();
        }
        public List<TipoEmpleado> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query
                .FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(t => !t.Baja)
                .ToList();
        }

        
    }
}
