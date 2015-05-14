using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class PuertaAccesoDAO : GenericDAO<PuertaAcceso>
    {
//        public PuertaAccesoDAO(ISession session) : base(session){}

        public PuertaAcceso FindByCodigo(int empresa, int linea, short codigo)
        {
            return Query.FilterEmpresa(Session, new[]{empresa})
                .FilterLinea(Session, new[] { empresa }, new[] { linea })
                .SingleOrDefault(p => p.Codigo == codigo && !p.Baja);
        }

		public PuertaAcceso FindByVehiculo(int vehiculo)
		{
			return Query.Where(p => p.Vehiculo.Id == vehiculo && !p.Baja).SafeFirstOrDefault();
		}

        public List<PuertaAcceso> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(p => !p.Baja)
                .ToList();
        }

        public override void Delete(PuertaAcceso obj)
        {
            if (obj == null) return;

            obj.Baja = true;
            obj.Vehiculo = null;
            SaveOrUpdate(obj);
        }
    }
}
