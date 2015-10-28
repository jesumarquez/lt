using Logictracker.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace Logictracker.Web.Filters
{
    public class MVCAuthenticationFilter : IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (!WebSecurity.Authenticated)
            {
                filterContext.Result = new RedirectResult("Default.aspx?RedirectUrl=" + filterContext.HttpContext.Request.RawUrl);
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

        }
    }
}