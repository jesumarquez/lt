#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible
{
    /// <summary>
    /// Tank data access class.
    /// </summary>
    public class TanqueDAO : GenericDAO<Tanque>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TanqueDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the tank associated to the givenn code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Tanque FindByCode(string code)
        {
            var list = (from Tanque t in Session.Query<Tanque>().ToList() where t.Codigo == code select t).ToList();

            return (list.Count > 0) ? list.First() : null;
        }

        public IList FindByEmpresaAndLinea(Empresa empresa, Linea linea,Usuario user)
        {
            var tanks = FindByEmpresaLinea(empresa, linea, user);

            return (from Tanque tank in tanks
                    where tank.Linea != null && tank.Equipo == null
                    select tank).ToList();
        }

        /// <summary>
        /// Gets all tanks associated to the givenn district and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IList FindByEmpresaLinea(Empresa empresa, Linea linea,Usuario user)
        {
            var lin = linea != null ? linea.Id : -1;
            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;

            return FindByEmpresaLinea(emp, lin,user);
        }

        /// <summary>
        /// Gets the last message associated to the speficied code for the current tank.
        /// </summary>
        /// <param name="tanque"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public EventoCombustible FindLastMessageByCode(int tanque, string code)
        {
            var list = (from EventoCombustible e in Session.Query<EventoCombustible>().ToList()
                        where e.Mensaje.Codigo == code && e.Tanque != null && e.Tanque.Id == tanque
                        orderby e.Fecha descending select e).ToList();

            return  list.Count > 0 ? list.First() : null;
        }

        /// <summary>
        /// Gets all tanks associated to the specified district and base that belong to the givenn equipment id.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="equipo"></param>
        /// <returns></returns>
        public IList FindByEquipo(Usuario user,Empresa empresa, Linea linea, int equipo)
        {
            var lin = linea != null ? linea.Id : -1;
            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;

            var tanks = FindPorEquipoByEmpresaLinea(emp, lin, user);

            return (from Tanque tank in tanks
                    where (equipo.Equals(-1) || (equipo.Equals(-2) && tank.Equipo == null) || (tank.Equipo != null && tank.Equipo.Id.Equals(equipo))) && tank.Linea == null
                    select tank).ToList();
        }

        /// <summary>
        /// NO BORRAR - Se usa en el Exportador de Despachos.
        /// </summary>
        /// <param name="linea"></param>
        /// <returns></returns>
        public Tanque FindMainTankByLinea(int linea)
        {
            var list = (from Tanque t in Session.Query<Tanque>().ToList() where t.Linea != null && t.Linea.Id == linea orderby t.Id select t).ToList();

            return list.Count > 0 ? list.First() : null;
        }

        public IEnumerable<Tanque> FindAllEnPlanta() { return (from Tanque t in FindAll() where t.Linea != null select t); }

        public IEnumerable<Tanque> FindAllEnEquipo() { return (from Tanque t in FindAll() where t.Equipo != null select t); }

        #endregion

        #region Private Methods

        private IList FindPorEquipoByEmpresaLinea (int empresa, int linea, Usuario user)
        {
            var dao = new DAOFactory();
            var equipos = dao.EquipoDAO.FindAllByUsuario(user);

            return (from Tanque t in Session.Query<Tanque>().ToList()
                        where t.Linea == null && t.Equipo != null && equipos.Contains(t.Equipo) &&
                               ((linea <= 0 || (t.Equipo.Linea != null && t.Equipo.Linea.Id == linea))
                               || (empresa <= 0 ||
                                (t.Equipo.Empresa != null &&
                                 t.Equipo.Empresa.Id == empresa && (t.Equipo.Linea == null || linea <= 0))))
                        orderby t.Descripcion
                        select t).ToList();
        }

        /// <summary>
        /// Gets all tanks associated to the specified district and base ids.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private IList FindByEmpresaLinea(int empresa, int linea,Usuario user)
        {
            return (from Tanque t in Session.Query<Tanque>().ToList()
                        where t.Linea != null && t.Equipo == null &&
                        (empresa <= 0 || (t.Linea.Empresa.Id == empresa )&& (user.Empresas.IsEmpty() || user.Empresas.Contains(t.Linea.Empresa))) 
                              && (linea <= 0 || (t.Linea.Id == linea && (user.Lineas.IsEmpty() || user.Lineas.Contains(t.Linea))))
                        orderby t.Descripcion
                        select t).ToList();
        }

        #endregion
    }
}
