using System.Web.Mvc;
using Logictracker.Security;
using Logictracker.Web.Controllers;

namespace Logictracker.Web.Filters
{
    public class MVCAuthorizeFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //base.OnAuthorization(filterContext);

            var funcController = filterContext.Controller as IFunctionController;
            var controller = filterContext.Controller as BaseFunctionController;

            var module = WebSecurity.GetUserModuleByRef(funcController.GetRefference().ToUpper());

            if (module == null)
                filterContext.Result = new RedirectResult("~/SinAcceso.aspx");
            else
                controller.Module = module;
        }
    }
}