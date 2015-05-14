using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Sync;
using Logictracker.Utils;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <summary>
    /// RFID cards data acces class.
    /// </summary>
    public class TarjetaDAO : GenericDAO<Tarjeta>
    {
//        public TarjetaDAO(ISession session) : base(session) { }

        public List<Tarjeta> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .ToList();
        }

        public Tarjeta FindByNumero(IEnumerable<int> empresas, string numero)
        {
            return Query.FilterEmpresa(Session, empresas)
                .Where(t => t.Numero == numero)
                .SafeFirstOrDefault();
        }
        
        public Tarjeta FindByPin(IEnumerable<int> empresas, string pin)
        {
            return Query.FilterEmpresa(Session, empresas)
                .Where(t => t.Pin == pin)
				.SafeFirstOrDefault();
        }

        public Tarjeta FindByPinHexa(IEnumerable<int> empresas, string pinHexa)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .Where(t => t.PinHexa == pinHexa)
                        .SafeFirstOrDefault();
        }
        
        public Tarjeta FindByCodigoAcceso(IEnumerable<int> empresas, int codigoAcceso)
        {
            return Query.FilterEmpresa(Session, empresas)
                .Where(t => t.CodigoAcceso == codigoAcceso)
				.SafeFirstOrDefault();
        }
        
        public IEnumerable<Tarjeta> GetTarjetasLibres(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            var empleadoDao = new EmpleadoDAO();

            var todas = GetList(empresas, lineas);
            var asignadas = empleadoDao.FindActivosConTarjetas(empresas, lineas).Select(x=>x.Tarjeta.Id);

            return todas.Where(card => !asignadas.Contains(card.Id)).OrderBy(tarjeta => tarjeta.Numero);
        }

        public List<Tarjeta> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Cacheable()
                .ToList();
        }

        public override void SaveOrUpdate(Tarjeta obj)
        {
            base.SaveOrUpdate(obj);
            EnqueueSync(obj, OutQueue.Queries.Molinete, OutQueue.Operations.Tarjeta);
        }
    }
}
