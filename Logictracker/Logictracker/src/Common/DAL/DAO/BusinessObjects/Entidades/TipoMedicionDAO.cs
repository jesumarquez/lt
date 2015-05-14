using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Entidades;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public class TipoMedicionDAO : GenericDAO<TipoMedicion>
    {
//        public TipoMedicionDAO(ISession session) : base(session) { }

        public List<TipoMedicion> GetList()
        {
            return Query.Where(m => !m.Baja)
                        .ToList();
        }

        public override void Delete(TipoMedicion obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public TipoMedicion FindByCode(string code)
        {
            return Query.FirstOrDefault(tm => !tm.Baja && tm.Codigo.Equals(code));
        }
    }
}