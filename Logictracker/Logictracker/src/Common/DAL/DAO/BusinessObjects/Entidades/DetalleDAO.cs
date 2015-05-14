using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Utils;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public class DetalleDAO : GenericDAO<Detalle>
    {
//        public DetalleDAO(ISession session) : base(session) { }

        public List<Detalle> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipos, IEnumerable<int> detallesPadres)
        {
            var q = Query.FilterTipoEntidad(Session, empresas, lineas, tipos);

            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) 
             || !QueryExtensions.IncludesAll(tipos) || !QueryExtensions.IncludesAll(detallesPadres))
                q = q.FilterDetalle(Session, empresas, lineas, tipos, detallesPadres);
                        
            return  q.Where(d => !d.Baja)
                     .OrderBy(d => d.Orden)
                     .ToList();
        }

        public override void Delete(Detalle obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public Detalle FindByIdAndTipoEntidad(int id, int tipoEntidad)
        {
            return GetList(new[] { -1 },
                           new[] { -1 },
                           new[] { tipoEntidad },
                           new[] { -1 })
                          .Where(d => d.Id == id)
                          .SafeFirstOrDefault();
        }

        public bool IsOrdenUnique(int empresa, int linea, int tipoEntidad, int idDetalle, int orden)
        {
            return Query.FilterTipoEntidad(Session, new[] { empresa }, new[] { linea }, new[] { tipoEntidad })
                       .Where(d => d.Id != idDetalle).FirstOrDefault(d => d.Orden == orden) == null;
        }

        public bool IsNombreUnique(int empresa, int linea, int tipoEntidad, int idDetalle, string nombre)
        {
            return Query.FilterTipoEntidad(Session, new[] { empresa }, new[] { linea }, new[] { tipoEntidad })
                       .Where(d => d.Id != idDetalle).FirstOrDefault(d => d.Nombre == nombre) == null;
        }
    }
}