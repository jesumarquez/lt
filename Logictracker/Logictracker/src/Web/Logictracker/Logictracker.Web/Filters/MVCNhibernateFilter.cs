using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Logictracker.DAL.NHibernate;
using NHibernate;
using NHibernate.Context;

namespace Logictracker.Web.Filters
{
    public class MvcNhibernateFilter : ActionFilterAttribute
    {
        public MvcNhibernateFilter()
        {
            SessionFactory = NHibernateHelper.SessionFactory;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var session = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
            session.BeginTransaction();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var session = SessionFactory.GetCurrentSession();
            var transaction = session.Transaction;
            if (transaction != null && transaction.IsActive)
            {
                transaction.Commit();
            }
            session = CurrentSessionContext.Unbind(SessionFactory);
            session.Close();
        }

        private ISessionFactory SessionFactory { get; set; }
    }
}