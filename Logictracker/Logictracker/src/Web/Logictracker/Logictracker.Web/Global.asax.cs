using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Logictracker.DatabaseTracer.Core;
using Logictracker.DatabaseTracer.Enums;
using Logictracker.DAL.DAO.BusinessObjects.Auditoria;
using Logictracker.DAL.NHibernate;
using Logictracker.Security;

namespace Logictracker.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            STrace.Module = LogModules.LogictrackerWeb.GetDescription();
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }



        /// <summary>Hold all the extensions we treat as static</summary>
        static readonly HashSet<string> allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".js", ".css", ".png", ".image", ".gif", ".axd", ".swf", "ashx"
        };

        /// <summary>Is this a request for a static file?</summary>
        /// <param name="request">The HTTP request instance to extend.</param>
        /// <returns>True if the request is for a static file on disk, false otherwise.</returns>
        public static bool IsStaticFile(HttpRequest request)
        {
            var fileOnDisk = request.PhysicalPath;
            if (string.IsNullOrEmpty(fileOnDisk))
            {
                return false;
            }

            var extension = Path.GetExtension(fileOnDisk);

            return allowedExtensions.Contains(extension);
        }

        private void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        public static bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers["X-Requested-With"] == "XMLHttpRequest"));
        }

        private void Application_Error(object sender, EventArgs e)
        {

            if (HttpContext.Current == null ||
                HttpContext.Current.Session == null) return;

            if (IsAjaxRequest(HttpContext.Current.Request))
            {
                SessionHelper.CloseSession();
                return;
            }

            HttpContext.Current.Session["Error"] = HttpContext.Current.Server.GetLastError();
            HttpContext.Current.Session["LastVisitedPage"] = HttpContext.Current.Request.Url.AbsolutePath;

            Response.Redirect(string.Concat(VirtualPathUtility.ToAbsolute("~"), "/Error.aspx"), false);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!IsStaticFile(Request))
            {
                SessionHelper.CreateSession();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (!IsStaticFile(Request))
            {
                SessionHelper.CloseSession();
            }
        }

        private void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        private void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            using (var session = SessionHelper.OpenSession())
            {
                var auditDao = new LoginAuditDAO();

                var userSessionData = WebSecurity.GetAuthenticatedUser(Session);

                if (userSessionData == null) return;

                auditDao.CloseUserSession(userSessionData.Id);
            }
        }

        /// Workaround para que funcione la Session de asp.net con Web.api y que no se rompa para todo el resto que no sea
        /// Wep.api
        private const string _WebApiPrefix = "api";
        private static string _WebApiExecutionPath = String.Format("~/{0}", _WebApiPrefix);

        protected void Application_PostAuthorizeRequest()  
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_WebApiExecutionPath);
        }
    }
}