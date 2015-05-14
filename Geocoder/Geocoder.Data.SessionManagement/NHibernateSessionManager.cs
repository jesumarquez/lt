using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Web;
namespace Geocoder.Data.SessionManagement
{
	public sealed class NHibernateSessionManager
	{
		private class Nested
		{
			internal static readonly NHibernateSessionManager NHibernateSessionManager;
			static Nested()
			{
				NHibernateSessionManager.Nested.NHibernateSessionManager = new NHibernateSessionManager();
			}
		}
		private const string TRANSACTION_KEY = "CONTEXT_TRANSACTIONS";
		private const string SESSION_KEY = "CONTEXT_SESSIONS";
		private Hashtable sessionFactories = new Hashtable();
		public static NHibernateSessionManager Instance
		{
			get
			{
				return NHibernateSessionManager.Nested.NHibernateSessionManager;
			}
		}
		private Hashtable ContextTransactions
		{
			get
			{
				Hashtable result;
				if (this.IsInWebContext())
				{
					if (HttpContext.Current.Items["CONTEXT_TRANSACTIONS"] == null)
					{
						HttpContext.Current.Items["CONTEXT_TRANSACTIONS"] = new Hashtable();
					}
					result = (Hashtable)HttpContext.Current.Items["CONTEXT_TRANSACTIONS"];
				}
				else
				{
					if (CallContext.GetData("CONTEXT_TRANSACTIONS") == null)
					{
						CallContext.SetData("CONTEXT_TRANSACTIONS", new Hashtable());
					}
					result = (Hashtable)CallContext.GetData("CONTEXT_TRANSACTIONS");
				}
				return result;
			}
		}
		private Hashtable ContextSessions
		{
			get
			{
				Hashtable result;
				if (this.IsInWebContext())
				{
					if (HttpContext.Current.Items["CONTEXT_SESSIONS"] == null)
					{
						HttpContext.Current.Items["CONTEXT_SESSIONS"] = new Hashtable();
					}
					result = (Hashtable)HttpContext.Current.Items["CONTEXT_SESSIONS"];
				}
				else
				{
					if (CallContext.GetData("CONTEXT_SESSIONS") == null)
					{
						CallContext.SetData("CONTEXT_SESSIONS", new Hashtable());
					}
					result = (Hashtable)CallContext.GetData("CONTEXT_SESSIONS");
				}
				return result;
			}
		}
		private NHibernateSessionManager()
		{
		}
		private ISessionFactory GetSessionFactoryFor(string sessionFactoryConfigPath)
		{
			if (string.IsNullOrEmpty(sessionFactoryConfigPath))
			{
				throw new ArgumentException("sessionFactoryConfigPath may not be null nor empty", "sessionFactoryConfigPath");
			}
			ISessionFactory sessionFactory = (ISessionFactory)this.sessionFactories[sessionFactoryConfigPath];
			if (sessionFactory == null)
			{
				if (!File.Exists(sessionFactoryConfigPath))
				{
					throw new FileNotFoundException("The config file at '" + sessionFactoryConfigPath + "' could not be found");
				}
				Configuration configuration = new Configuration();
				configuration.Configure(sessionFactoryConfigPath);
				sessionFactory = configuration.BuildSessionFactory();
				if (sessionFactory == null)
				{
					throw new InvalidOperationException("cfg.BuildSessionFactory() returned null.");
				}
				this.sessionFactories.Add(sessionFactoryConfigPath, sessionFactory);
			}
			return sessionFactory;
		}
		public void RegisterInterceptorOn(string sessionFactoryConfigPath, IInterceptor interceptor)
		{
			ISession session = (ISession)this.ContextSessions[sessionFactoryConfigPath];
			if (session != null && session.IsOpen)
			{
				throw new CacheException("You cannot register an interceptor once a session has already been opened");
			}
			this.GetSessionFrom(sessionFactoryConfigPath, interceptor);
		}
		public ISession GetSessionFrom(string sessionFactoryConfigPath)
		{
			return this.GetSessionFrom(sessionFactoryConfigPath, null);
		}
		private ISession GetSessionFrom(string sessionFactoryConfigPath, IInterceptor interceptor)
		{
			ISession session = (ISession)this.ContextSessions[sessionFactoryConfigPath];
			if (session == null)
			{
				if (interceptor != null)
				{
					session = this.GetSessionFactoryFor(sessionFactoryConfigPath).OpenSession(interceptor);
				}
				else
				{
					session = this.GetSessionFactoryFor(sessionFactoryConfigPath).OpenSession();
				}
				this.ContextSessions[sessionFactoryConfigPath] = session;
			}
			if (session == null)
			{
				throw new NullReferenceException("session was null");
			}
			return session;
		}
		public void CloseSessionOn(string sessionFactoryConfigPath)
		{
			ISession session = (ISession)this.ContextSessions[sessionFactoryConfigPath];
			if (session != null && session.IsOpen)
			{
				session.Flush();
				session.Close();
			}
			this.ContextSessions.Remove(sessionFactoryConfigPath);
		}
		public ITransaction BeginTransactionOn(string sessionFactoryConfigPath)
		{
			ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
			if (transaction == null)
			{
				transaction = this.GetSessionFrom(sessionFactoryConfigPath).BeginTransaction();
				this.ContextTransactions.Add(sessionFactoryConfigPath, transaction);
			}
			return transaction;
		}
		public void CommitTransactionOn(string sessionFactoryConfigPath)
		{
			ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
			try
			{
				if (this.HasOpenTransactionOn(sessionFactoryConfigPath))
				{
					transaction.Commit();
					this.ContextTransactions.Remove(sessionFactoryConfigPath);
				}
			}
			catch (HibernateException)
			{
				this.RollbackTransactionOn(sessionFactoryConfigPath);
				throw;
			}
		}
		public bool HasOpenTransactionOn(string sessionFactoryConfigPath)
		{
			ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
			return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
		}
		public void RollbackTransactionOn(string sessionFactoryConfigPath)
		{
			ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
			try
			{
				if (this.HasOpenTransactionOn(sessionFactoryConfigPath))
				{
					transaction.Rollback();
				}
				this.ContextTransactions.Remove(sessionFactoryConfigPath);
			}
			finally
			{
				this.CloseSessionOn(sessionFactoryConfigPath);
			}
		}
		private bool IsInWebContext()
		{
			return HttpContext.Current != null;
		}
	}
}
