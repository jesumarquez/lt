<%@ WebHandler Language="C#" Class="ShowState" %>

using System;
using System.Web;
using System.Web.SessionState;

public class ShowState : IHttpHandler, IRequiresSessionState
{
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/html";
        if (context.Request.QueryString["op"] != null)
        {
            context.Response.ClearContent();
            context.Response.Write("<html><head>");
            var value = (string)(context.Session[context.Request.QueryString["op"]] ?? "");
            if(value.EndsWith("#END#"))
            {
                value = value.Substring(0, value.Length - 5);
            }
            else
{
    context.Response.Write("<meta http-equiv='refresh' content='2'>");
}
            
            context.Response.Write("</head><body>");
            context.Response.Write(value);
            context.Response.Write("</body></html>");
            context.Response.Flush();
            context.Response.End();
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}