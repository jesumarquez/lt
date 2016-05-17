using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Logictracker.Cache.Interfaces;
using Logictracker.DAL.DAO.BaseClasses.Interfaces;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.Auditoria;
using Logictracker.DAL.DAO.BusinessObjects.Sync;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static class Extension
    {
        public static bool IsEmpty<T>(this IEnumerable<T> collection) { return !collection.Any(); }

        public static bool ContainsAll<T>(this IEnumerable<T> superSet , IEnumerable<T> subSet)
        {
            return !subSet.Except(superSet).Any();
        }
    }

    #region Public Classes

    /// <summary>
    /// Generic DAO.
    /// </summary>
    /// <typeparam name="TDaotype">Persistant data type.</typeparam>
    public class GenericDAO<TDaotype> : IGenericDAO where TDaotype : IAuditable
    {
        #region Protected Properties

        /// <summary>
        /// Gets the type associated to the generic dao.
        /// </summary>
        protected static Type DaoType { get { return typeof(TDaotype); } }

        /// <summary>
        /// Gets a new criteria from the session.
        /// </summary>
        protected ICriteria Criteria { get { return Session.CreateCriteria(DaoType); } }

        protected IQueryable<TDaotype> Query { get { return Session.Query<TDaotype>(); } }

        /// <summary>
        /// NHibernate data access session accessor.
        /// </summary>
        protected ISession Session
        {
            get { return SessionHelper.Current; }
        }

        protected bool IsSecurable
        {
            get
            {
                return DaoType.GetInterface(typeof(ISecurable).FullName) != null;
            }
        }

        #endregion

        #region Constructors

        #endregion

        protected DataTable Ids2DataTable(IEnumerable<IDataIdentify> ids)
        {
            return Ids2DataTable(ids.Select(x => x.Id));
        }

        protected DataTable Ids2DataTable(IEnumerable<int> ids)
        {
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            foreach (var i in ids)
            {
                var r = table.NewRow();
                r["id"] = i;
                table.Rows.Add(r);
            }
            return table;
        }

        protected ISession GetCleanSession()
        {
            if (Session.Transaction != null && Session.Transaction.IsActive)
            {
                return GetNewSession();
            }

            return Session;
        }

        public ISession GetNewSession()
        {
            return SessionHelper.OpenSession();
        }

        #region Public Methods

        /// <summary>
        /// Busca todos
        /// </summary>
        /// <returns> </returns>
        public virtual IQueryable<TDaotype> FindAll() { return Session.Query<TDaotype>().Cacheable(); }

        /// <summary>
        /// Carga un objeto identificado x su Id en especial
        /// </summary>
        /// <param name="id"> Id del objeto</param>
        /// <returns> el objeto o null si no existe </returns>
        public virtual TDaotype FindById(int id) { return (TDaotype)Session.Load(typeof(TDaotype), id); }

        /// <summary>
        /// Deletes the object specified by its id using the givenn generic dao class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual void Delete(TDaotype obj)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    DeleteWithoutTransaction(obj);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Delete(TDaotype) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Delete(TDaotype)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex2, "Exception in Delete(TDaotype) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Deletes the object associated to the specified id.
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void DeleteWithoutTransaction(TDaotype obj)
        {
            var user = GetUser();
            var auditObj = obj as IAuditable;

            if (IsAuditable(auditObj, user)) DoDeleteAudit(obj, user);

            Session.Delete(string.Format("from {0} obj where obj.id = {1}", typeof(TDaotype), obj.Id));
        }

        /// <summary>
        /// Saves the speficied object.
        /// </summary>
        /// <param name="obj">The instance of the object to be saved.</param>
        /// <remarks>Opens and Commits a transaction for this operation doing an audit over the entity if indicated in the cong</remarks>
        public virtual void SaveOrUpdate(TDaotype obj)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    SaveOrUpdateWithoutTransaction(obj);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in SaveOrUpdate(TDaotype) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in SaveOrUpdate(TDaotype)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex2, "Exception in SaveOrUpdate(TDaotype) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }



        /// <summary>
        /// Saves the speficied object.
        /// </summary>
        /// <param name="obj">The instance of the object to be saved.</param>
        protected virtual void SaveOrUpdateWithoutTransaction(TDaotype obj)
        {
            var user = GetUser();
            var o = obj as IAuditable;

            if (IsAuditable(o, user)) DoSaveAudit(obj, o, user);
            else Session.SaveOrUpdate(obj);
        }

        /// <summary>
        /// Inserts into database the specified object whitin a transaction.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Save(TDaotype obj)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    SaveWithoutTransaction(obj);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Save(TDaotype) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Save(TDaotype)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex2, "Exception in Save(TDaotype) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Inserts into database the specified object whitout a transaction.
        /// </summary>
        /// <param name="obj"></param>
        protected void SaveWithoutTransaction(TDaotype obj) { Session.Save(obj); }

        /// <summary>
        /// Updates the specified object whitin a transaction.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Update(TDaotype obj)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    UpdateWithoutTransaction(obj);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Update(TDaotype) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Update(TDaotype)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex2, "Exception in Update(TDaotype) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Updates in database the specified object whitout a transaction.
        /// </summary>
        /// <param name="obj"></param>
        protected void UpdateWithoutTransaction(TDaotype obj) { Session.Update(obj); }

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the currently logged in user.
        /// </summary>
        /// <returns></returns>
        private Usuario GetUser()
        {
            var user = WebSecurity.AuthenticatedUser;

            if (user == null) return null;

            var usuarioDao = new UsuarioDAO();

            return usuarioDao.FindById(user.Id);
        }

        /// <summary>
        /// Checks if the current objects is allowed to be audited and a valid audit user is provided.
        /// </summary>
        /// <param name="auditObj"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool IsAuditable(IAuditable auditObj, Usuario user) { return auditObj != null && user != null && IsConfiguredAsAuditable(auditObj); }

        /// <summary>
        /// Determine if the givenn object is a auditable object.
        /// </summary>
        /// <param name="auditObj"></param>
        /// <returns></returns>
        private static bool IsConfiguredAsAuditable(IAuditable auditObj) { return AuditedClassesHelper.GetAuditedClasses().Contains(auditObj.TypeOf().ToString()); }

        /// <summary>
        /// Performs a Save Type audit into the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="o"></param>
        /// <param name="user"></param>
        private void DoSaveAudit(TDaotype obj, IAuditable o, Usuario user)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    DoSaveAuditWithoutTransaction(obj, o, user);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Update(TDaotype) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Update(TDaotype)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex2, "Exception in Update(TDaotype) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        private void DoSaveAuditWithoutTransaction(TDaotype obj, IAuditable o, Usuario user)
        {
            var auditDAO = new EntityAuditDAO();

            var oldObj = (o.Id != 0) ? FindById(o.Id) : default(TDaotype);

            /*Esto fue agregado porque sino se "colgaba" al entrar al metodo auditDAO.AuditSave*/
            var oldObj2 = (o.Id != 0) ? Session.Get(typeof(TDaotype).FullName, oldObj.Id) : null;

            Session.SaveOrUpdate(obj);

            auditDAO.AuditSave(obj, oldObj2, user);
        }


        private void DoDeleteAudit(TDaotype obj, Usuario user)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    DoDeleteAuditWithoutTransaction(obj, user);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Update(TDaotype) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex, "Exception in Update(TDaotype)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(GenericDAO<TDaotype>).FullName, ex2, "Exception in Update(TDaotype) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }

        }

        /// <summary>
        /// Performs a delete with audit info.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="user"></param>
        private void DoDeleteAuditWithoutTransaction(TDaotype obj, Usuario user)
        {
            var auditDAO = new EntityAuditDAO();
            auditDAO.AuditDelete(obj, user);
        }

        #endregion


        public void EnqueueSync<T>(T obj, string query, string operacion) where T : ISecurable, IAuditable
        {
            new OutQueueDAO().Enqueue(obj, query, operacion);
        }
    }

    #endregion
}