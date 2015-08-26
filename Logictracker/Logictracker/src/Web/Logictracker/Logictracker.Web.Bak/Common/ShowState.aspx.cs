using System;

namespace Logictracker.Common
{
    public partial class Common_ShowState : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["op"] != null)
            {
                Response.ClearContent();
                Response.Write("<html><head></head><body>");
                Response.Write(Session[Request.QueryString["op"]] ?? "");
                Response.Flush();
                Response.Write("---</body></html>");
                Response.End();
            }
        }
    }
}
