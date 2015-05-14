using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class ProductoDAO : GenericDAO<Producto>
    {
//        public ProductoDAO(ISession session) : base(session) { }

        public List<Producto> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterBocaDeCarga(Session, empresas, lineas, bocasDeCarga)
                        .Where(m => !m.Baja)
                        .ToList();
        }

        public Producto FindByCodigo(int empresa, int linea, int bocaDeCarga, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .FilterBocaDeCarga(Session, new[] { empresa }, new[] { linea }, new[] { bocaDeCarga }, null)
                        .Where(g=> !g.Baja)
                        .Where(g => g.Codigo == codigo)
						.SafeFirstOrDefault();
        }

        public override void Delete(Producto obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}