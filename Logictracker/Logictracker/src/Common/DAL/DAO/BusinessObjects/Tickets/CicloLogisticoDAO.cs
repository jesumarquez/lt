using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Tickets
{
    public class CicloLogisticoDAO: GenericDAO<Types.BusinessObjects.Tickets.CicloLogistico>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public CicloLogisticoDAO(ISession session) : base(session) { }

        #endregion

        public Types.BusinessObjects.Tickets.CicloLogistico GetByCodigo(int empresa, int linea, string codigo)
        {
            var idempresa = GetIdEmpresa(empresa, linea);
            var idlinea = linea;

            var list = Session.Query<Types.BusinessObjects.Tickets.CicloLogistico>().Where(
                c => !c.Baja && c.Codigo == codigo &&
                    (c.Owner == null || 
                    ((empresa <= 0 || c.Owner.Empresa == null || c.Owner.Empresa.Id == idempresa) &&
                        (linea <= 0 || c.Owner.Linea == null || c.Owner.Linea.Id == idlinea)))
                ).ToList();
            return list.Count > 0 ? list[0] : null;
        }

        public List<Types.BusinessObjects.Tickets.CicloLogistico> GetList(int empresa, int linea)
        {
            var idempresa = GetIdEmpresa(empresa, linea);
            var idlinea = linea;

            return Session.Query<Types.BusinessObjects.Tickets.CicloLogistico>().Where(c => !c.Baja &&
                    (c.Owner == null || 
                    ((empresa <= 0 || c.Owner.Empresa == null || c.Owner.Empresa.Id == idempresa) &&
                        (linea <= 0 || c.Owner.Linea == null || c.Owner.Linea.Id == idlinea)))
                ).ToList();
        }
        public List<Types.BusinessObjects.Tickets.CicloLogistico> GetByType(int empresa, int linea, bool ciclos, bool estados)
        {
            var idempresa = GetIdEmpresa(empresa, linea);
            var idlinea = linea;

            return Session.Query<Types.BusinessObjects.Tickets.CicloLogistico>().Where(
                c => !c.Baja && ((ciclos  && c.EsCiclo == ciclos) || (estados && c.EsEstado == estados)) &&
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
