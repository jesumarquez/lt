using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Documentos;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Documentos
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class TipoDocumentoDAO: GenericDAO<TipoDocumento>
    {
//        public TipoDocumentoDAO(ISession session) : base(session) { }

        #region FindMethods

        public TipoDocumento FindByNombre(int empresa, int linea, string nombre)
        {
            return Query.FilterEmpresa(Session, new[] { empresa })
                .FilterLinea(Session, new[] { empresa }, new[] { linea })
                .FirstOrDefault(d => !d.Baja && d.Nombre == nombre);
        }

        public List<TipoDocumento> FindObligatorioVehiculo(int empresa, int linea)
        {
            return Query.FilterEmpresa(Session, new[]{empresa})
                .FilterLinea(Session, new[] { empresa }, new[] { linea })
                .Where(d => !d.Baja && d.AplicarAVehiculo && d.RequerirPresentacion)
                .ToList();
        }
        public List<TipoDocumento> FindObligatorioEmpleado(int empresa, int linea)
        {
            return Query.FilterEmpresa(Session, new[] { empresa })
                .FilterLinea(Session, new[] { empresa }, new[] { linea })
                .Where(d => !d.Baja && d.AplicarAEmpleado && d.RequerirPresentacion)
                .ToList();
        }
        #endregion

        #region GetMethods

        public List<TipoDocumento> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(d => !d.Baja)
                .ToList();
        }

        #endregion

        #region Override Methods

        public override void Delete(TipoDocumento obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Borra todos los valores de los documentos para el parametro dado
        /// </summary>
        /// <param name="id">El id del TipoDocumentoParametro</param>
        public void DeleteValoresParametro(int id)
        {
            Session.Delete("from DocumentoValor dv where dv.Parametro.Id = " + id);
        } 

        #endregion
    }
}
