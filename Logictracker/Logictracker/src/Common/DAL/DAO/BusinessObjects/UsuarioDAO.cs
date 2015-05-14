using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <summary>
    /// User data access class.
    /// </summary>
    public class UsuarioDAO : GenericDAO<Usuario>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public UsuarioDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the user associated to the specified user name.
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <returns></returns>
        public Usuario GetByNombreUsuario(string nombreUsuario)
        {
            return  (Usuario) Session.CreateCriteria(typeof (Usuario)).Add(Restrictions.Eq("NombreUsuario", nombreUsuario)).Add(Restrictions.IsNull("FechaBaja")).UniqueResult();
        }

        /// <summary>
        /// Finds the application user assigned to the specified user name and password.
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Usuario FindForLogin(string nombre, string password)
        {
            var dc =
                DetachedCriteria.For<Usuario>()
                    .Add(Restrictions.Eq("NombreUsuario", nombre))
                    .Add(Restrictions.Eq("Clave", GetMd5(password)))
                    .Add(Restrictions.IsNull("FechaBaja"))
                    .SetMaxResults(1)
                    .SetProjection(Projections.Property("Id"));

            var crit = Session.CreateCriteria<Usuario>().Add(Subqueries.PropertyIn("Id", dc));

            var result = crit.UniqueResult<Usuario>();
            return result;
        }

        /// <summary>
        /// Deletes the user associated to the specified id.
        /// </summary>
        /// <param name="usuario"></param>
        public override void Delete(Usuario usuario)
        {
            if (usuario == null) return;

            usuario.FechaBaja = DateTime.UtcNow;
            usuario.Entidad.Baja = true;

            SaveOrUpdate(usuario);

        }

        /// <summary>
        /// Finds all active users.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Usuario> FindAll()
        {
            var dc = DetachedCriteria.For<Usuario>()
                .Add(Restrictions.IsNull(Projections.Property<Usuario>(u => u.FechaBaja)))
                .SetProjection(Projections.Property<Usuario>(u=> u.Id));

            return Session.CreateCriteria<Usuario>()
                .Add(Subqueries.PropertyIn("Id", dc)).List<Usuario>().ToList();
        }

        /// <summary>
        /// Saves the specified user hashing or not its password.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignorePassword"></param>
        public void SaveOrUpdate(Usuario obj, bool ignorePassword)
        {
            if (!ignorePassword) obj.Clave = GetMd5(obj.Clave);

            if (obj.Id.Equals(0)) obj.FechaAlta = DateTime.UtcNow;

            SaveOrUpdate(obj);
        }

        /// <summary>
        /// Updates only personal information about the user.
        /// </summary>
        /// <param name="obj"></param>
        public void UpdatePersonalData(Usuario obj) { base.SaveOrUpdate(obj); }

        /// <summary>
        /// Finds all user that the specified user can view and edit.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Usuario> FindByUsuario(Usuario user)
        {
            return (from Usuario u in FindAll()
                    where user.Tipo >= u.Tipo
                          && (user.Coches.IsEmpty || (!u.Coches.IsEmpty && user.Coches.ContainsAll(u.Coches)))
                          && (user.Transportistas.IsEmpty || (!u.Transportistas.IsEmpty && user.Transportistas.ContainsAll(u.Transportistas)))
                          && (user.Lineas.IsEmpty || (!u.Lineas.IsEmpty && user.Lineas.ContainsAll(u.Lineas)))
                          && (user.Empresas.IsEmpty || (!u.Empresas.IsEmpty && user.Empresas.ContainsAll(u.Empresas)))
                    orderby user.NombreUsuario
                    select u).ToList();
        }

        /// <summary>
        /// Enables access for the specified user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="horasHabilitado"></param>
        public void HabilitarUsuario(Usuario user, int horasHabilitado)
        {
            user.Inhabilitado = false;

            if (horasHabilitado > 0) user.FechaExpiracion = DateTime.UtcNow.AddHours(horasHabilitado);
            else user.FechaExpiracion = null;
        }

        /// <summary>
        /// Denegates acces to the specified user.
        /// </summary>
        /// <param name="user"></param>
        public void InhabilitarUsuario(Usuario user)
        {
            user.Inhabilitado = true;
            user.FechaExpiracion = null;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the md5 value of the givenn string.
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        private static string GetMd5(string strSource)
        {
            var encoding = new ASCIIEncoding();

            var ary = encoding.GetBytes(strSource);

            return GetMd5(ary);
        }

        /// <summary>
        /// Gets the md5 value of the given byte array.
        /// </summary>
        /// <param name="ary"></param>
        /// <returns></returns>
        private static string GetMd5(byte[] ary)
        {
            var md5 = new MD5CryptoServiceProvider();

            var hash = md5.ComputeHash(ary);
            var hashValue = "";

            foreach (var e in hash)
                if (e <= 15) hashValue += "0" + e.ToString("X");
                else hashValue += e.ToString("X");

            return hashValue;
        }

        #endregion
    }
}