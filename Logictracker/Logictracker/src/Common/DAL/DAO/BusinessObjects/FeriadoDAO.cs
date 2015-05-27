#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class FeriadoDAO : GenericDAO<Feriado>
    {
        #region Constructor

    	/// <summary>
    	/// Instanciates a new data access class using the provided nhibernate sessions.
    	/// </summary>
    	/// <param name="session"></param>
//    	public FeriadoDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methdos

        /// <summary>
        /// Find all messages for the givenn location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Feriado> FindByEmpresaYLineaAndUser(Empresa empresa, Linea linea, Usuario user)
        {
            var emp = empresa != null ? empresa.Id : linea != null && linea.Empresa != null ? linea.Empresa.Id : -1;
            var lin = linea != null ? linea.Id : -1;

            return FindByEmpresaYLinea(emp, lin, user);
        }

        /// <summary>
        /// Determines if the giveen date is a holiday or not.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool EsFeriado(Empresa empresa, Linea linea, DateTime date)
        {
            var feriados = FindByEmpresaYLineaAndUser(empresa, linea, null).Select(feriado => feriado.Fecha.DayOfYear);

            return feriados.Contains(date.DayOfYear);
        }

        #endregion

        #region Private Methods

        private List<Feriado> FindByEmpresaYLinea(int empresa, int linea, Usuario user)
        {
            var feriados = Session.Query<Feriado>().Where(f =>
                (linea > 0 && (f.Linea.Id == linea || (f.Linea == null && (f.Empresa.Id == empresa || f.Empresa == null))))
                    || (linea <= 0 && (empresa <= 0 || (empresa > 0 && (f.Empresa == null || f.Empresa.Id == empresa))))).ToList();

            if (user == null) return feriados;

            return (from f in feriados
                    where ((f.Empresa == null && f.Linea == null) || (f.Empresa != null && f.Linea == null && (user.Empresas.IsEmpty()|| user.Empresas.Contains(f.Empresa)))
                        || (f.Linea != null && (user.Lineas.IsEmpty() || user.Lineas.Contains(f.Linea))))
                    select f).ToList();
        }

        #endregion 
    }
}
