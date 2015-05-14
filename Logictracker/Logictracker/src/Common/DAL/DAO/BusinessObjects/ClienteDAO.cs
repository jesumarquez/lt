using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class ClienteDAO : GenericDAO<Cliente>
    {
//        public ClienteDAO(ISession session) : base(session) { }


        #region Find Methods

        public Cliente FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(c => !c.Baja)
                .Where(c => c.Codigo == code)
                .Cacheable()
				.SafeFirstOrDefault();
        }

        #endregion

        #region Get Methods

        public Cliente GetByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(c => !c.Baja)
                .Where(c => c.Codigo == code)
                .Cacheable()
				.SafeFirstOrDefault();
        }

        public List<Cliente> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(c => !c.Baja)
                .Cacheable()
                .ToList();
        }

        #endregion


        #region Override Methods

        public override void Delete(Cliente cliente)
        {
            if (cliente == null) return;
            cliente.Baja = true;
            SaveOrUpdate(cliente);

            var dao = new DAOFactory();
            dao.PuntoEntregaDAO.DeleteByCliente(cliente.Id);
            dao.ReferenciaGeograficaDAO.DeleteGeoRef(cliente.ReferenciaGeografica.Id);
        }

        #endregion

    }
}