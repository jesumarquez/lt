#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Vehiculos
{
    public class TipoCocheDAO : GenericDAO<TipoCoche>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TipoCocheDAO(ISession session) : base(session) { }

        #endregion


        public TipoCoche FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] {empresa})
                .FilterLinea(Session, new[] {empresa}, new[] {linea})
                .FirstOrDefault(x => !x.Baja && x.Codigo == codigo);
        }

        #region Public Methods

        /// <summary>
        /// Finds all active vehicle types.
        /// </summary>
        /// <returns></returns>
        public override IQueryable<TipoCoche> FindAll()
        {
            return Session.Query<TipoCoche>()
                .Where(t => !t.Baja)
                .OrderBy(t => t.Descripcion)
                .Cacheable();
        }

        /// <summary>
        /// Finds all vehicle types associated to the specified location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IList<TipoCoche> FindByEmpresasAndLineas(List<int> empresa, List<int> linea, Usuario user)
        {
            var tipos = FindByEmpresasYLineas(empresa, linea);

            if (user == null || !user.AppliesFilters || (user.Empresas.Count.Equals(0) && user.Lineas.Count.Equals(0))) return tipos;

            var lineaDao = new LineaDAO();
            var lineas = lineaDao.GetList(new[] { -1 });
            var empresas = (from Linea l in lineas select l.Empresa);

            return (from TipoCoche tipo in tipos
                    where (tipo.Empresa == null || (user.Empresas.Count > 0 && user.Empresas.Contains(tipo.Empresa) || (empresas.Any() && empresas.Contains(tipo.Empresa))))
                         && (tipo.Linea == null || (lineas.ToList().Count > 0 && lineas.Contains(tipo.Linea)))
                    select tipo).ToList();
        }

        /// <summary>
        /// Deletes the vehicle type associated to the specified id.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override void Delete(TipoCoche type)
        {
            if (type == null) return;

            type.Baja = true;

            SaveOrUpdate(type);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds all vehicle types associated to the specified location and base.
        /// </summary>
        /// <param name="empresas"></param>
        /// <param name="lineas"></param>
        /// <returns></returns>
        private IList<TipoCoche> FindByEmpresasYLineas(ICollection<int> empresas, ICollection<int> lineas)
        {
            var lineaDao = new LineaDAO();

            foreach (var linea in lineas.Where(lin => lin > 0).Select(id => lineaDao.FindById(id)).Where(linea => linea.Empresa != null).Where(linea => !empresas.Contains(linea.Empresa.Id)))
                empresas.Add(linea.Empresa.Id);

            return Session.Query<TipoCoche>().Where(tipo => !tipo.Baja
                && (empresas.Contains(-1) || empresas.Contains(0) || tipo.Empresa == null || empresas.Contains(tipo.Empresa.Id))
                && (lineas.Contains(-1) || lineas.Contains(0) || (tipo.Linea == null && (tipo.Empresa == null || empresas.Contains(tipo.Empresa.Id))) 
                || (tipo.Linea != null && lineas.Contains(tipo.Linea.Id)))).ToList();
        }

        #endregion
    }
}