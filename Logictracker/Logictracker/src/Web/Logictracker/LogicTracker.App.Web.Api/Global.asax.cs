using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Logictracker.DAL.NHibernate;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Spring.Web.Mvc;

namespace LogicTracker.App.Web.Api
{
    public class WebApiApplication : SpringMvcApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        protected override System.Web.Http.Dependencies.IDependencyResolver BuildWebApiDependencyResolver()
        {
            //get the 'default' resolver, populated from the 'main' config metadata
            var resolver = base.BuildWebApiDependencyResolver();

            //check if its castable to a SpringWebApiDependencyResolver
            var springResolver = resolver as SpringWebApiDependencyResolver;

            //if it is, add additional config sources as needed
            if (springResolver != null)
            {
                springResolver.AddChildApplicationContextConfigurationLocation("file://~/Config/objects.xml");
            }

            //return the fully-configured resolver
            return resolver;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            SessionHelper.CreateSession();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            SessionHelper.CloseSession();
        }
    }
}
