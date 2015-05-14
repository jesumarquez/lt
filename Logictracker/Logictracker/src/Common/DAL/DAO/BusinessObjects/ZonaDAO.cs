using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class ZonaDAO : GenericDAO<Zona>
    {
//        public ZonaDAO(ISession session) : base(session) { }


        #region Find Methods
        public Zona FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .FirstOrDefault(x => x.Codigo == codigo && !x.Baja);
        }
        public Zona FindByPrioridad(int empresa, int linea, int prioridad)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .FirstOrDefault(x => x.Prioridad == prioridad && !x.Baja);
        }
        #endregion

        #region GetMethods
        public List<Zona> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZona)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTipoZona(Session, empresas, lineas, tiposZona)
                        .Where(z => !z.Baja)
                        .ToList();
        } 
        #endregion

        public override void Delete(Zona obj)
        {
            if (obj == null) return;

            obj.Baja = true;
            var geocercaDao = new ReferenciaGeograficaDAO();
            foreach(ReferenciaZona r in obj.Referencias)
            {
                geocercaDao.ClearGeocerca(r.ReferenciaGeografica.Id);
            }

            SaveOrUpdate(obj);
        }

        public List<ReferenciaGeografica> FilterAsignadas(int empresa, int linea, List<ReferenciaGeografica> referencias)
        {
            var ret = new List<ReferenciaGeografica>();
            ret.AddRange(referencias);

            foreach (var referencia in referencias)
            {
                var asig = Session.Query<ReferenciaZona>().Where(rz => rz.ReferenciaGeografica.Id == referencia.Id && !rz.Zona.Baja).ToList();

                if (asig.Any()) ret.Remove(referencia);
            }

            return ret;
        }
    }
}
