using System.Linq;
using System.Security.Cryptography;
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
    public class ModeloDAO : GenericDAO<Modelo>
    {
//        public ModeloDAO(ISession session) : base(session) { }

        public Modelo FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] {empresa})
                .FilterLinea(Session, new[] {empresa}, new[] {linea})
                .FirstOrDefault(x => !x.Baja && x.Codigo == codigo);
        }

        public Modelo FindByDescripcion(int empresa, int linea, string descripcion)
        {
            return Query.FilterEmpresa(Session, new[] { empresa })
                .FilterLinea(Session, new[] { empresa }, new[] { linea })
                .FirstOrDefault(x => !x.Baja && x.Descripcion == descripcion);
        }

        #region Get Methods

        public List<Modelo> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .FilterMarca(Session, empresas, lineas, marcas)
                .Where(m=>!m.Baja)
                .ToList();
        } 

        #endregion

        #region Override Methods

        public override void Delete(Modelo obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        } 

        #endregion

        #region Other Methods

        public bool IsCodeUnique(int empresa, int linea, int marca, int idModelo, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                       .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                       .FilterMarca(Session, new[] { empresa }, new[] { linea }, new[] { marca })
                       .Where(m => !m.Baja)
                       .Where(m => m.Id != idModelo).FirstOrDefault(m => m.Codigo == code) == null;
        } 

        #endregion
    }
}