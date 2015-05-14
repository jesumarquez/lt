using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas
{
    /// <summary>
    /// GeoRef types data access class.
    /// </summary>
    public class TipoReferenciaGeograficaDAO: GenericDAO<TipoReferenciaGeografica>
    {
//        public TipoReferenciaGeograficaDAO(ISession session) : base(session) { }


        #region Find Methods

        /// <summary>
        /// Gets the georef type associated to the specified description.
        /// </summary>
        /// <param name="empresas"></param>
        /// <param name="lineas"></param>
        /// <param name="descripcion"></param>
        /// <returns></returns>
        public TipoReferenciaGeografica FindByDescripcion(IEnumerable<int> empresas, IEnumerable<int> lineas, string descripcion)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(t => !t.Baja).FirstOrDefault(t => t.Descripcion == descripcion);
        }

        /// <summary>
        /// Gets the georef type associated to the specified code.
        /// </summary>
        /// <param name="empresas"></param>
        /// <param name="lineas"></param>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public TipoReferenciaGeografica FindByCodigo(IEnumerable<int> empresas, IEnumerable<int> lineas, string codigo)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                 .FilterLinea(Session, empresas, lineas, null)
                 .Where(t => !t.Baja)
                 .Where(t => t.Codigo == codigo)
                 .Cacheable()
                 .FirstOrDefault();
        }

        #endregion

        #region Get Methods

        public TipoReferenciaGeografica GetByCodigo(IEnumerable<int> empresas, IEnumerable<int> lineas, string codigo)
        {
            return Query.FilterEmpresa(Session, empresas)
                 .FilterLinea(Session, empresas, lineas)
                 .Where(t => t.Codigo == codigo)
                 .Cacheable()
                 .FirstOrDefault();
        }

        public List<TipoReferenciaGeografica> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                 .FilterLinea(Session, empresas, lineas)
                 .Where(t=>!t.Baja)
                 .Cacheable()
                 .ToList();
        } 
        
        #endregion

        #region Override Methods

        public override void Delete(TipoReferenciaGeografica type)
        {
            type.Baja = true;
            SaveOrUpdate(type);
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Determines if the specified georef type id is assigned to any georef.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasChilds(int id)
        {
            var geoRefDao = new ReferenciaGeograficaDAO();
            return geoRefDao.FindList(new[] {-1}, new[] {-1}, new[] {id}).Any();
        }

        #endregion

    }
}
