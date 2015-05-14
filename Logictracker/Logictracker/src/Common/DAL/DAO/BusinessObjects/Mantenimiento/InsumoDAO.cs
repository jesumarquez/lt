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
    public class InsumoDAO : GenericDAO<Insumo>
    {
//        public InsumoDAO(ISession session) : base(session) { }

        public Insumo FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .Where(c => c.Codigo == code)
                        .Cacheable()
                        .FirstOrDefault();
        }
        
        public Insumo GetByDescripcion(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposInsumo, string descripcion)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .FilterTipoInsumo(Session, empresas, lineas, tiposInsumo).FirstOrDefault(i => i.Descripcion == descripcion);
        }

        public List<Insumo> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposInsumo)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTipoInsumo(Session, empresas, lineas, tiposInsumo)
                        .ToList();
        }

        public bool IsCodeUnique(int empresa, int linea, int tipoInsumo, int idInsumo, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                       .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                       .FilterTipoInsumo(Session, new[] { empresa }, new[] { linea }, new[] { tipoInsumo }, null)
                       .Where(m => m.Id != idInsumo).FirstOrDefault(m => m.Codigo == code) == null;
        }
    }
}
