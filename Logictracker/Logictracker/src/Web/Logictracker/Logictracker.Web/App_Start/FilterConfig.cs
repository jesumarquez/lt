using System.Web.Mvc;
using Logictracker.Web.Filters;

namespace Logictracker.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MVCAuthenticationFilter());
            filters.Add(new MVCAuthorizeFilter());
            
        }

       
    }
}