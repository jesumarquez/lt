<%@ Application Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="HibernatingRhinos.Profiler.Appender.NHibernate" %>
<%@ Import Namespace="Logictracker.DAL.DAO.BusinessObjects.Auditoria" %>
<%@ Import Namespace="Logictracker.DAL.NHibernate" %>
<%@ Import Namespace="Logictracker.DatabaseTracer.Core" %>
<%@ Import Namespace="Logictracker.DatabaseTracer.Enums" %>
<%@ Import Namespace="Logictracker.Security" %>
<%@ Import Namespace="Logictracker.Types.SecurityObjects" %>
<%@ Import Namespace="NHibernate" %>

<script RunAt="server">

    /// <summary>Hold all the extensions we treat as static</summary>
    static HashSet<string> allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".js", ".css", ".png", ".image", ".gif", ".axd", ".swf", "ashx"
    };

    /// <summary>Is this a request for a static file?</summary>
    /// <param name="request">The HTTP request instance to extend.</param>
    /// <returns>True if the request is for a static file on disk, false otherwise.</returns>
    public static bool IsStaticFile(HttpRequest request)
    {
        string fileOnDisk = request.PhysicalPath;
        if (string.IsNullOrEmpty(fileOnDisk))
        {
            return false;
        }

        string extension = Path.GetExtension(fileOnDisk);

        return allowedExtensions.Contains(extension);
    }    
    
    private void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        STrace.Module = LogModules.LogictrackerWeb.GetDescription();

        NHibernateProfiler.Initialize();
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

        return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest"));
    }

    private void Application_Error(object sender, EventArgs e)
    {
        
        if (HttpContext.Current == null ||
            HttpContext.Current.Session == null) return;

        if (HttpContext.Current.Request !=null && IsAjaxRequest(HttpContext.Current.Request))
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
        using (ISession session = SessionHelper.OpenSession())
        {
                var auditDao = new LoginAuditDAO();

                UserSessionData userSessionData = WebSecurity.GetAuthenticatedUser(Session);

                if (userSessionData == null) return;

                auditDao.CloseUserSession(userSessionData.Id);
        }
    }

</script>
