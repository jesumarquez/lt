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

            var module = WebSecurity.GetUserModuleByRef(funcController.ReferenceName.ToUpper());

            if (module != null)
                controller.Module = module;
            else
                filterContext.Result = new RedirectResult("~/SinAcceso.aspx");
        }
    }
}