#region Usings

using System;
using System.Linq;
using System.Web.UI;

#endregion

namespace Logictracker.Web
{
    public static class ControlExtensions
    {
        public static Control GetControl(this Control control, string id)
        {
            return control.ID == id 
                ? control
                : control.Controls.OfType<Control>().Select(c=>c.GetControl(id)).Where(c =>  c != null).FirstOrDefault();
        }

        public static Control GetControlOnPage(this Control control, string id)
        {
            return control.Page.GetControl(id);
        }

        public static VsProperty<T> CreateVsProperty<T>(this Control control, string propertyName)
        {
            return new VsProperty<T>(control, propertyName);
        }
        public static VsProperty<T> CreateVsProperty<T>(this Control control, string propertyName, T defaultValue)
        {
            return new VsProperty<T>(control, propertyName, defaultValue);
        }
        public static SessionVsProperty<T> CreateSessionVsProperty<T>(this Control control, string propertyName)
        {
            return new SessionVsProperty<T>(control, propertyName);
        }
        public static SessionVsProperty<T> CreateSessionVsProperty<T>(this Control control, string propertyName, T defaultValue)
        {
            return new SessionVsProperty<T>(control, propertyName, defaultValue);
        }
        public static FullVsProperty<T> CreateFullVsProperty<T>(this Control control, string propertyName)
        {
            return new FullVsProperty<T>(control, propertyName);
        }
        public static FullVsProperty<T> CreateFullVsProperty<T>(this Control control, string propertyName, T defaultValue)
        {
            return new FullVsProperty<T>(control, propertyName, defaultValue);
        }
        /// <summary>
        /// Registers a JavaScript embedded resurce
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceName"></param>
        public static void RegisterJsResource(this Control control, string resourceName)
        {
            RegisterJsResource(control, resourceName, control.GetType());
        }

        /// <summary>
        /// Registers a JavaScript embedded resurce
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceName"></param>
        /// <param name="controlType"> </param>
        public static void RegisterJsResource(this Control control, string resourceName, Type controlType)
        {
            var script = control.Page.ClientScript.GetWebResourceUrl(controlType, resourceName);
            string csslink = string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", script);
            LiteralControl include = new LiteralControl(csslink);
            control.Page.Header.Controls.Add(include);
        }

        /// <summary>
        /// Registers a JavaScript embedded resurce
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceName"></param>
        public static void RegisterCssResource(this Control control, string resourceName)
        {
            RegisterCssResource(control, resourceName, control.GetType());
        }

        /// <summary>
        /// Registers a JavaScript embedded resurce
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceName"></param>
        /// <param name="controlType"> </param>
        public static void RegisterCssResource(this Control control, string resourceName, Type controlType)
        {
            var res = control.Page.ClientScript.GetWebResourceUrl(controlType, resourceName);
            RegisterCss(control, res);
        }
        public static void RegisterCss(this Control control, string url)
        {
            string csslink = "<link href='" + url + "' rel='stylesheet' type='text/css' />";
            LiteralControl include = new LiteralControl(csslink);
            control.Page.Header.Controls.Add(include);
        }
        
        /// <summary>
        /// Registers a startup script with Page.ClienteScript or ScriptManager. Adds <script></script> tags.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="key">Script key</param>
        /// <param name="script">Script body</param>
        public static void RegisterStartupJScript(this Control control, string key, string script)
        {
            RegisterStartupJScript(control, key, script, true);
        }

        /// <summary>
        /// Registers a startup script with Page.ClienteScript or ScriptManager 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="key">Script key</param>
        /// <param name="script">Script body</param>
        /// <param name="addtags">Adds <script></script> tags if true</param>
        public static void RegisterStartupJScript(this Control control, string key, string script, bool addtags)
        {
            var sm = ScriptManager.GetCurrent(control.Page);
            if (!control.Page.IsPostBack || sm == null || !sm.IsInAsyncPostBack) control.Page.ClientScript.RegisterStartupScript(typeof(string), key, script, addtags);
            else ScriptManager.RegisterStartupScript(control.Page, typeof(string), key, script, addtags); 
        }

        /// <summary>
        /// Registers a script inside a Sys.Application.add_init call from ASP.NET Ajax Client API
        /// </summary>
        /// <param name="control"></param>
        /// <param name="key">Script key</param>
        /// <param name="script">Script body</param>
        public static void RegisterClientOnLoad(this Control control, string key, string script)
        {
            RegisterStartupJScript(control, key, string.Concat("Sys.Application.add_init(function(){ ", script, " });"));
        }

        /// <summary>
        /// Gets the URL of an embedded resource
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string GetWebResourceUrl(this Control control, string resourceName)
        {
            return control.Page.ClientScript.GetWebResourceUrl(control.GetType(), resourceName);
        }
    }
}
