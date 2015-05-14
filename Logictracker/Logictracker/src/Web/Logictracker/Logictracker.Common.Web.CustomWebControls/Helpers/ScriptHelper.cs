#region Usings

using System.Web.UI;

#endregion

namespace Logictracker.Web.CustomWebControls.Helpers
{
    public class ScriptHelper
    {
        public Control Control {get; set;}
        public Page Page { get { return Control.Page; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control">Control to apply scripts</param>
        public ScriptHelper(Control control) { Control = control; }

        /// <summary>
        /// Registers a JavaScript embedded resurce
        /// </summary>
        /// <param name="resourceName"></param>
        public void RegisterJsResource(string resourceName)
        {
            var script = Page.ClientScript.GetWebResourceUrl(Control.GetType(), resourceName);
            RegisterStartupScript(resourceName, string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", script), false);
        }

        /// <summary>
        /// Registers a startup script with Page.ClienteScript or ScriptManager. Adds <script></script> tags.
        /// </summary>
        /// <param name="key">Script key</param>
        /// <param name="script">Script body</param>
        public void RegisterStartupScript(string key, string script) { RegisterStartupScript(key, script, true); }

        /// <summary>
        /// Registers a startup script with Page.ClienteScript or ScriptManager 
        /// </summary>
        /// <param name="key">Script key</param>
        /// <param name="script">Script body</param>
        /// <param name="addtags">Adds <script></script> tags if true</param>
        public void RegisterStartupScript(string key, string script, bool addtags)
        {
            var sm = ScriptManager.GetCurrent(Page);
            if(!Page.IsPostBack || sm == null || !sm.IsInAsyncPostBack) Page.ClientScript.RegisterStartupScript(typeof(string), key, script, addtags);
            else ScriptManager.RegisterStartupScript(Page, typeof(string), key, script, addtags); 
        }

        /// <summary>
        /// Registers a script inside a Sys.Application.add_init call from ASP.NET Ajax Client API
        /// </summary>
        /// <param name="key">Script key</param>
        /// <param name="script">Script body</param>
        public void RegisterClientOnLoad(string key, string script)
        {
            RegisterStartupScript(key, string.Concat("Sys.Application.add_init(function(){ ", script, " });"));
        }

        /// <summary>
        /// Prefixes the given key with the control's ClientID and an underscore.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>A string like [Control.ClientID]_[key] </returns>
        public string PrefixKey(string key) { return string.Concat(Control.ClientID, "_", key);}

        /// <summary>
        /// Gets the URL of an embedded resource
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public string GetWebResourceUrl(string resourceName)
        {
            return Page.ClientScript.GetWebResourceUrl(Control.GetType(), resourceName);
        }
    }
}
