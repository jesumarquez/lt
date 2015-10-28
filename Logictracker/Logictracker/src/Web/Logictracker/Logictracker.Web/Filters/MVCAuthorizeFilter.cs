using Logictracker.Security;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Logictracker.Web.Controllers;

namespace Logictracker.Web.Filters
{
    public class MVCAuthorizeFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var funcController = filterContext.Controller as IFunctionController;
            var controller = filterContext.Controller as BaseFunctionController;

            var module = WebSecurity.GetUserModuleByRef(funcController.ReferenceName);
            controller.Module = module;

            if (module == null)
            {
                filterContext.Result = new RedirectResult("~/SinAcceso.aspx");
            }
        }
    }
}