#region Usings

using System.Collections;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <summary>
    /// Equipo data access class.
    /// </summary>
    public class EquipoDAO : GenericDAO<Equipo>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public EquipoDAO(ISession session) : base(session) { }

        #endregion


        #region Find Methods
        public Equipo FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] {empresa})
                .FilterLinea(Session, new[] {empresa}, new[] {linea})
                .FirstOrDefault(x => !x.Baja && x.Codigo == codigo);
        } 
        #endregion
        #region Public Methods

        /// <summary>
        /// Finds all Equipos.
        /// </summary>
        /// <returns></returns>
        public new IList FindAll() { return (from Equipo e in Session.Query<Equipo>().ToList() where !e.Baja orderby e.Descripcion ascending select e).ToList(); }

        /// <summary>
        /// Finds all equipos using the givenn location, base and client filters.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="cliente"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IList FindByEmpresaLineaYCliente(Empresa empresa, Linea linea, Cliente cliente, Usuario user)
        {
            var lin = linea != null ? linea.Id : -1;
            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;
            var cli = cliente != null ? cliente.Id : -1;

            var equipos = FindByEmpresaLineaYCliente(emp, lin, cli);
            return (from Equipo e in equipos where 
                (e.CentroDeCostos == null || user.CentrosCostos.IsEmpty()|| user.CentrosCostos.Contains(e.CentroDeCostos))
                        && ((e.Empresa == null && e.Linea == null)
                        || (e.Empresa != null && e.Linea == null && (user.Empresas.IsEmpty()|| user.Empresas.Contains(e.Empresa)))
                        || (e.Linea != null && (user.Lineas.IsEmpty()|| user.Lineas.Contains(e.Linea))))
                    select e).ToList();
        }

        /// <summary>
        /// Gets an Equipo by its code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Equipo GetByCode(string code)
        {
            var list = (from Equipo e in Session.Query<Equipo>().ToList() where e.Codigo == code select e).ToList();

            return list.Count.Equals(0) ? null : list.First();
        }

        public IList FindAllByUsuario(Usuario user)
        {
            return (from Equipo e in Session.Query<Equipo>().ToList()
                    where (e.CentroDeCostos == null || user.CentrosCostos.IsEmpty()|| user.CentrosCostos.Contains(e.CentroDeCostos))
                        && ((e.Empresa == null && e.Linea == null)
                        || (e.Empresa != null && e.Linea == null && (user.Empresas.IsEmpty()|| user.Empresas.Contains(e.Empresa)))
                        || (e.Linea != null && (user.Lineas.IsEmpty()|| user.Lineas.Contains(e.Linea))))
                    select e).ToList();
        }

        /// <summary>
        /// Deletes the equipo associated to the specified id.
        /// </summary>
        /// <param name="equip"></param>
        /// <returns></returns>
        public override void Delete(Equipo equip)
        {
            var dao = new DAOFactory();

            equip.Baja = true;

            SaveOrUpdate(equip);

            dao.ReferenciaGeograficaDAO.DeleteGeoRef(equip.ReferenciaGeografica.Id);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds all equipos using the givenn location, base and client filters.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        private IList FindByEmpresaLineaYCliente(int empresa, int linea, int cliente)
        {//
            var equipos = FindByEmpresaYLinea(empresa, linea);

            return cliente <= 0 ? equipos : (from Equipo equipo in equipos where equipo.Cliente == null || equipo.Cliente.Id.Equals(cliente) select equipo).ToList();
        }

        /// <summary>
        /// Finds all equipos by location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        private IList FindByEmpresaYLinea(int empresa, int linea)
        {
            return (from Equipo e in Session.Query<Equipo>().ToList()
                    where !e.Baja
                          && (empresa <= 0 || e.Empresa == null || e.Empresa.Id == empresa)
                          && (linea <= 0 || e.Linea == null || e.Linea.Id == linea)
                    select e).ToList();
        }

        #endregion
    }
}