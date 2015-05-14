#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Tickets;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Tickets
{
    public class ServicioDAO: GenericDAO<Servicio>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="statelessSession"></param>
//        public ServicioDAO(ISession session) : base(session) { }

        #endregion

        public Servicio GetByCodigo(int empresa, int linea, string codigo)
        {
            var idempresa = GetIdEmpresa(empresa, linea);
            var idlinea = linea;

            var list = Session.Query<Servicio>().Where(c => c.Estado != Servicio.EstadoCancelado &&
                   c.Codigo == codigo &&
                    (c.Owner == null ||
                    ((empresa <= 0 || c.Owner.Empresa == null || c.Owner.Empresa.Id == idempresa) &&
                        (linea <= 0 || c.Owner.Linea == null || c.Owner.Linea.Id == idlinea)))
                ).ToList();
            return list.Count > 0 ? list[0] : null;
        }

        public List<Servicio> GetList(int empresa, int linea)
        {
            var idempresa = GetIdEmpresa(empresa, linea);
            var idlinea = linea;

            return Session.Query<Servicio>().Where(c => c.Estado != Servicio.EstadoCancelado &&
                    (c.Owner == null ||
                    ((empresa <= 0 || c.Owner.Empresa == null || c.Owner.Empresa.Id == idempresa) &&
                        (linea <= 0 || c.Owner.Linea == null || c.Owner.Linea.Id == idlinea)))
                ).ToList();
        }

        private int GetIdEmpresa(int empresa, int linea)
        {
            var idempresa = empresa;
            if (empresa <= 0 && linea > 0)
            {
                var lineaDao = new LineaDAO();
                var l = lineaDao.FindById(linea);
                idempresa = l.Empresa.Id;
            }
            return idempresa;
        }
    }
}
