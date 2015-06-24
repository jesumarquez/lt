using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class PuntoEntregaDAO : GenericDAO<PuntoEntrega>
    {
//        public PuntoEntregaDAO(ISession session) : base(session) { }


        #region Find Methods

        public List<PuntoEntrega> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes)
        {
            return Query.FilterCliente(Session, empresas, lineas, clientes, null)
                .Where(p => !p.Baja)
                .Cacheable()
                .ToList();
        }
        public PuntoEntrega FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, string code)
        {
            return Query.FilterCliente(Session, empresas, lineas, clientes, null)
                .Where(c => !c.Baja)
                .Where(c => c.Codigo == code)
                .Cacheable()
				.SafeFirstOrDefault();
        }
        #endregion

        #region Get Methods

        public PuntoEntrega GetByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, string code)
        {
            return Query.FilterCliente(Session, empresas, lineas, clientes)
                .Where(p => !p.Baja)
                .Where(p => p.Codigo == code)
                .Cacheable()
				.SafeFirstOrDefault();
        }

        public List<PuntoEntrega> FindByCodes(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, IEnumerable<string> codes)
        {
            return Query.FilterCliente(Session, empresas, lineas, clientes)
                .Where(p => !p.Baja)
                .Where(p => codes.Contains(p.Codigo))
                .Cacheable()
                .ToList();
        }

        public List<PuntoEntrega> GetByCliente(int idCliente)
        {
            return Query.Where(p => p.Cliente.Id == idCliente && !p.Baja)
                        .Cacheable()
                        .ToList();
        }

        public List<PuntoEntrega> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes)
        {
            var q = Query.FilterCliente(Session, empresas, lineas, clientes)
                         .Where(p => !p.Baja);
            
            return q.Cacheable().ToList();
        }

        public List<PuntoEntrega> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, string prefixText)
        {
            var q = Query.FilterCliente(Session, empresas, lineas, clientes)
                         .Where(p => p.Descripcion.ToLower().Contains(prefixText))
                         .Where(p => !p.Baja);

            return q.Cacheable().ToList();
        }

        #endregion

        #region Override Methods

        public override void Delete(PuntoEntrega p)
        {
            if (p == null) return;
            p.Baja = true;
            SaveOrUpdate(p);
            if (p.ReferenciaGeografica != null)
            {
                var dao = new DAOFactory();
                dao.ReferenciaGeograficaDAO.DeleteGeoRef(p.ReferenciaGeografica.Id);
            }
        }

        #endregion

        #region Other Methods

        public void DeleteByCliente(int id)
        {
            foreach (PuntoEntrega punto in FindList(new[]{-1}, new[]{-1}, new[]{id}))
            {
                punto.Baja = true;
                Delete(punto);
            }
        }
        #endregion
    }
}
