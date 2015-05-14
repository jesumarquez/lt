using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class ProveedorDAO : GenericDAO<Proveedor>
    {
//        public ProveedorDAO(ISession session) : base(session) { }

        #region Find Methods
        public Proveedor FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(c => c.Codigo == code)
                .Cacheable()
                .FirstOrDefault();
        }
        #endregion

        #region Get Methods

        public Proveedor GetByDescripcion(IEnumerable<int> empresas, IEnumerable<int> lineas, string descripcion)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas).FirstOrDefault(p => p.Descripcion == descripcion);
        }

        public List<Proveedor> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor)
        {
            var q = Query.FilterEmpresa(Session, empresas);

            if (!QueryExtensions.IncludesAll(lineas))
                q = q.FilterLinea(Session, empresas, lineas);
            if (!QueryExtensions.IncludesAll(tiposProveedor))
                q = q.FilterTipoProveedor(Session, empresas, lineas, tiposProveedor);

            return q.ToList();
        } 

        #endregion

        #region Other Methods

        public bool IsCodeUnique(int empresa, int linea, int idProveedor, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                       .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                       .Where(m => m.Id != idProveedor).FirstOrDefault(m => m.Codigo == code) == null;
        }

        #endregion
    }
}
