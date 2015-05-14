using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    public class PrecintoDAO : GenericDAO<Precinto>
    {
//        public PrecintoDAO(ISession session) : base(session) { }


        public List<Precinto> GetList()
        {
            return Query.Where(p => !p.Baja)
                        .ToList();
        }

        public List<Precinto> GetPrecintosLibres()
        {
            var dispositivoDao = new DispositivoDAO();
            var precintosAsignados = dispositivoDao.FindAll()
                                                   .Where(c => c.Precinto != null)
                                                   .Select(c => c.Precinto.Id);
            
            return GetList().Where(p => !precintosAsignados.Contains(p.Id))
                            .ToList();
        }

        public override void Delete(Precinto obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public bool IsCodeUnique(int idPrecinto, string code)
        {
            return Query.FirstOrDefault(g => g.Id != idPrecinto
                                             && g.Codigo == code
                                             && g.Baja == false) == null;
        }
    }
}