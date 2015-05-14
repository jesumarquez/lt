using System;
using System.Configuration;
using System.Web;

namespace Geocoder.Data.SessionManagement
{
	public class NHibernateSessionModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.BeginRequest += BeginTransaction;
			context.EndRequest += CommitAndCloseSession;
		}
		private void BeginTransaction(object sender, EventArgs e)
		{
		}
		private void CommitAndCloseSession(object sender, EventArgs e)
		{
			var openSessionInViewSection = GetOpenSessionInViewSection();
			try
			{
				foreach (SessionFactoryElement sessionFactoryElement in openSessionInViewSection.SessionFactories)
				{
					if (sessionFactoryElement.IsTransactional)
					{
						NHibernateSessionManager.Instance.CommitTransactionOn(sessionFactoryElement.FactoryConfigPath);
					}
				}
			}
			finally
			{
				foreach (SessionFactoryElement sessionFactoryElement in openSessionInViewSection.SessionFactories)
				{
					NHibernateSessionManager.Instance.CloseSessionOn(sessionFactoryElement.FactoryConfigPath);
				}
			}
		}
		private OpenSessionInViewSection GetOpenSessionInViewSection()
		{
			var openSessionInViewSection = ConfigurationManager.GetSection("nhibernateSettings") as OpenSessionInViewSection;
			if (openSessionInViewSection == null)
			{
				throw new ConfigurationErrorsException("The nhibernateSettings section was not found with ConfigurationManager.");
			}
			return openSessionInViewSection;
		}
		public void Dispose()
		{
		}
	}
}
