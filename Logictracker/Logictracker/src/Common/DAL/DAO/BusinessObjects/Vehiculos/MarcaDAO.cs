using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Vehiculos;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects.Vehiculos
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class MarcaDAO : GenericDAO<Marca>
    {
//        public MarcaDAO(ISession session) : base(session) { }



        #region Get Methods

        public Marca GetByDescripcion(int empresa, int linea, string descripcion)
        {
            return Query.FilterEmpresa(Session, new[]{empresa})
                .FilterLinea(Session, new[]{-1}, new[]{-1}).FirstOrDefault(m => m.Descripcion.Equals(descripcion) && !m.Baja);
        }

        public List<Marca> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(m => !m.Baja)
                .ToList();
        }

        #endregion

        #region Override Methods

        public override void Delete(Marca obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        } 

        #endregion

        #region Other Methods

        public bool ExistsDescripcion(int empresa, int linea, string descripcion)
        {
            return Query.FilterEmpresa(Session, new[] { empresa })
                       .FilterLinea(Session, new[] { empresa }, new[] { linea }).Count(m => m.Descripcion.Equals(descripcion) && !m.Baja) > 0;
        } 

        #endregion       
    }
}