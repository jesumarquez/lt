using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class TransportistaDAO : GenericDAO<Transportista>
    {
//        public TransportistaDAO(ISession session) : base(session) { }

        public Transportista FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .Where(l => !l.Baja && l.Codigo == codigo)
                        .Cacheable()
                        .FirstOrDefault();
        }

        public List<Transportista> FindByCodigos(int empresa, int linea, IEnumerable<string> codigos)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .Where(p => !p.Baja && codigos.Contains(p.Codigo))
                        .Cacheable()
                        .ToList();
        }

        #region Get Methods

        public List<Transportista> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTransportista(Session, empresas, lineas)
                        .Where(t => !t.Baja)
                        .OrderBy(t => t.Descripcion)
                        .Cacheable()
                        .ToList();
        }

        public IEnumerable<Transportista> GetTransportistasPermitidosPorUsuario(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var userId = sessionUser != null ? sessionUser.Id : 0;

            var tableEmpresas = Ids2DataTable(empresas);
            var tableLineas = Ids2DataTable(lineas);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_TransportistaDAO_GetTransportistasPermitidosPorUsuario] @empresasIds = :empresasIds, @lineasIds = :lineasIds, @userId = :userId;")
                              .AddEntity(typeof(Transportista))
                              .SetStructured("empresasIds", tableEmpresas)
                              .SetStructured("lineasIds", tableLineas)
                              .SetInt32("userId", userId);
            var results = sqlQ.List<Transportista>();
            return results;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Saves or updates the givenn transportista.
        /// </summary>
        /// <param name="obj"></param>
        public override void SaveOrUpdate(Transportista obj)
        {
            if (obj.Id == 0)
            {
                obj.ReferenciaGeografica.Empresa = obj.Empresa;
                obj.ReferenciaGeografica.Linea = obj.Linea;
                Session.Evict(obj.ReferenciaGeografica);
            }
            base.SaveOrUpdate(obj);
        }

        /// <summary>
        /// Deletes the specified transportista.
        /// </summary>
        /// <param name="transportista"></param>
        /// <returns></returns>
        public override void Delete(Transportista transportista)
        {
            if (transportista == null) return;
            transportista.Baja = true;
            SaveOrUpdate(transportista);
            var dao = new DAOFactory();
            dao.ReferenciaGeograficaDAO.DeleteGeoRef(transportista.ReferenciaGeografica.Id);
        }

        #endregion
    }
}