using System;
using System.Web;
using System.Web.Mvc;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Web
{
    public static class HtmlExtensions
    {
        public static IHtmlString ResourceLabel(this HtmlHelper helper, string resourceName = "", string variableName = "", string secureRefference = "")
        {
            var visible = WebSecurity.IsSecuredAllowed(secureRefference);
            return !visible ? new HtmlString(String.Empty) : new HtmlString(CultureManager.GetString(resourceName, variableName).Trim(' '));
        }

    }
}