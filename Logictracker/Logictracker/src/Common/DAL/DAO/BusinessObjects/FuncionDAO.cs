#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class FuncionDAO : GenericDAO<Funcion>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public FuncionDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds all functions ordered by its system.
        /// </summary>
        /// <returns></returns>
        public IList FindAllByUsuarioOrderBySistema(Usuario user)
        {
            var userAvailableFunctions = new List<int>();

            foreach(Perfil p in user.Perfiles)
            {
                foreach (var f in from MovMenu f in p.Funciones where !userAvailableFunctions.Contains(f.Id) select f)
                    userAvailableFunctions.Add(f.Funcion.Id);
            }

            return (from Funcion f in Session.Query<Funcion>().ToList()
                    where f.FechaBaja == null
                    && (user.Perfiles.IsEmpty()|| userAvailableFunctions.Contains(f.Id))
                    orderby f.Sistema.Orden , f.Sistema.Descripcion , f.Modulo , f.Descripcion
                    select f).ToList();
        }

        /// <summary>
        /// Gets all function associated to the specified system id.
        /// </summary>
        /// <param name="sistema"></param>
        /// <returns></returns>
        public IQueryable<Funcion> GetBySistema(int sistema)
        {
            return Session.Query<Funcion>()
                .Where(f => f.Sistema.Id == sistema && f.FechaBaja == null);
        }

        public IList FindAllOrderBySistema()
        {
            return FindAll()
                .Where(f => f.FechaBaja == null)
                .OrderBy(f => f.Sistema.Orden)
                .ThenBy(f => f.Sistema.Descripcion)
                .ThenBy(f => f.Modulo)
                .ThenBy(f => f.Descripcion)
                .ToList();
        }

        /// <summary>
        /// Deletes the specified function.
        /// </summary>
        /// <param name="funcion"></param>
        /// <returns></returns>
        public override void Delete(Funcion funcion)
        {
            if (funcion == null) return;

            funcion.FechaBaja = DateTime.UtcNow.ToShortDateString();

            SaveOrUpdate(funcion);
        }

        #endregion
    }
}