using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
namespace Geocoder.Data.SessionManagement
{
	public abstract class AbstractDAO<T, TIdT>
	{
		private readonly Type _persitentType = typeof(T);
		private readonly string _sessionFactoryConfigPath;
		protected ISession NHibernateSession
		{
			get
			{
				return NHibernateSessionManager.Instance.GetSessionFrom(this._sessionFactoryConfigPath);
			}
		}
		protected AbstractDAO(string sessionFactoryConfigPath)
		{
			if (string.IsNullOrEmpty(sessionFactoryConfigPath))
			{
				throw new ArgumentException("sessionFactoryConfigPath may not be null nor empty", "sessionFactoryConfigPath");
			}
			this._sessionFactoryConfigPath = sessionFactoryConfigPath;
		}
		public virtual T GetById(TIdT id, bool shouldLock)
		{
			T result;
			if (shouldLock)
			{
				result = (T)((object)this.NHibernateSession.Load(this._persitentType, id, LockMode.Upgrade));
			}
			else
			{
				result = (T)((object)this.NHibernateSession.Load(this._persitentType, id));
			}
			return result;
		}
		public virtual List<T> GetAll()
		{
			return this.GetByCriteria(new ICriterion[0]);
		}
		protected virtual List<T> GetByCriteria(params ICriterion[] criterion)
		{
			ICriteria criteria = this.NHibernateSession.CreateCriteria(this._persitentType);
			for (int i = 0; i < criterion.Length; i++)
			{
				ICriterion expression = criterion[i];
				criteria.Add(expression);
			}
			return criteria.List<T>() as List<T>;
		}
		protected virtual List<T> GetByExample(T exampleInstance, params string[] propertiesToExclude)
		{
			ICriteria criteria = this.NHibernateSession.CreateCriteria(this._persitentType);
			Example example = Example.Create(exampleInstance);
			for (int i = 0; i < propertiesToExclude.Length; i++)
			{
				string name = propertiesToExclude[i];
				example.ExcludeProperty(name);
			}
			criteria.Add(example);
			return criteria.List<T>() as List<T>;
		}
		public virtual T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude)
		{
			List<T> byExample = this.GetByExample(exampleInstance, propertiesToExclude);
			if (byExample.Count > 1)
			{
				throw new NonUniqueResultException(byExample.Count);
			}
			return (byExample.Count > 0) ? byExample[0] : default(T);
		}
		public virtual T Save(T entity)
		{
			this.NHibernateSession.Save(entity);
			return entity;
		}
		public virtual T SaveOrUpdate(T entity)
		{
			this.NHibernateSession.SaveOrUpdate(entity);
			return entity;
		}
		public virtual void Delete(T entity)
		{
			this.NHibernateSession.Delete(entity);
		}
		public virtual void BeginTransaction()
		{
			NHibernateSessionManager.Instance.BeginTransactionOn(this._sessionFactoryConfigPath);
		}
		public virtual void CommitChanges()
		{
			if (NHibernateSessionManager.Instance.HasOpenTransactionOn(this._sessionFactoryConfigPath))
			{
				NHibernateSessionManager.Instance.CommitTransactionOn(this._sessionFactoryConfigPath);
			}
			else
			{
				NHibernateSessionManager.Instance.GetSessionFrom(this._sessionFactoryConfigPath).Flush();
			}
		}
		public virtual void RollbackChanges()
		{
			if (NHibernateSessionManager.Instance.HasOpenTransactionOn(this._sessionFactoryConfigPath))
			{
				NHibernateSessionManager.Instance.RollbackTransactionOn(this._sessionFactoryConfigPath);
			}
		}
		public void Flush()
		{
			this.NHibernateSession.Flush();
		}
	}
}
