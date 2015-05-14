using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class TipoProveedorDAO : GenericDAO<TipoProveedor>
    {
//        public TipoProveedorDAO(ISession session) : base(session) { }

        #region Find Methods
        public TipoProveedor FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(c => !c.Baja)
                .Where(c => c.Codigo == code)
                .Cacheable()
                .FirstOrDefault();
        }  
        #endregion

        #region Get Methods

        public List<TipoProveedor> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(tp => !tp.Baja)
                        .ToList();
        } 

        #endregion

        #region Other Methods

        public bool IsCodeUnique(int empresa, int linea, int idTipoProveedor, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                       .FilterLinea(Session, new[] { empresa }, new[] { linea }, null).FirstOrDefault(m => m.Id != idTipoProveedor && m.Codigo == code && !m.Baja) == null;
        }

        #endregion

        public override void Delete(TipoProveedor obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}
