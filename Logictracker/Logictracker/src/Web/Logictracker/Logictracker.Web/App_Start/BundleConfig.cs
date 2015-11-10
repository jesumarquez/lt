using System.Web.Optimization;
using System.Web.UI;

namespace Logictracker.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // Order is very important for these files to work, they have explicit dependencies
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // Use the Development version of Modernizr to develop with and learn from. Then, when you’re
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

           
            // JQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-{version}.js"));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                            "~/Scripts/bootstrap.js"));

            // Angularjs
            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                            "~/Scripts/angular.js",
                            "~/Scripts/angular-route.js",
                            "~/Scripts/angular-resource.js"));
           
            // Kendo
            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                 //  "~/Scripts/kendo/2014.2.1008/jquery-2.1.1.js",
                "~/Scripts/kendo/2014.2.1008/kendo.all.min.js",
                "~/Scripts/kendo/2014.2.1008/kendo.aspnetmvc.min.js",
                "~/Scripts/kendo/kendo.custom.js"));
                    //"~/Scripts/kendo/kendo.modernizr.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/global").Include(
                "~/Scripts/kendo/2014.2.1008/cultures/kendo.culture.es-AR.min.js"));

            bundles.Add(new StyleBundle("~/bundles/css-kendo-ui").Include(
                "~/Content/kendo/2014.2.1008/kendo.common-bootstrap.min.css",
                "~/Content/kendo/2014.2.1008/kendo.bootstrap.min.css",
                "~/Content/kendo/2014.2.1008/kendo.dataviz.min.css",
                "~/Content/kendo/2014.2.1008/kendo.dataviz.bootstrap.min.css",
                "~/Content/kendo/kendo.custom.css",
                "~/Content/kendo/"
                ));

 
            // Logictracker Angular app
            bundles.Add(new ScriptBundle("~/bundles/angularjs-logictracker").Include(
                            "~/Scripts/app/app.js")
                            .IncludeDirectory("~/Scripts/app/","*.js",searchSubdirectories: true));

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "respond",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/respond.min.js",
                    DebugPath = "~/Scripts/respond.js",
                });
        }
    }
}