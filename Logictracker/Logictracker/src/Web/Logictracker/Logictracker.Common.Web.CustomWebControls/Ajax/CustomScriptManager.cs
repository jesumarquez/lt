#region Usings

using System;
using System.Web.UI;

#endregion

namespace Logictracker.Web.CustomWebControls.Ajax
{
    /// <summary>
    /// Custom Web Script Manager for solving multi-browser compatibility issues.
    /// </summary>
    [ToolboxData("<{0}:CustomScriptManager ID=\"CustomScriptManager1\" runat=\"server\"></{0}:CustomScriptManager>")]
    public class CustomScriptManager : ScriptManager
    {
        private static string _webKitHack;
        private string WebKitHack {
            get
            {
                return _webKitHack ?? (_webKitHack = Page.ClientScript.GetWebResourceUrl(GetType(), "Logictracker.Web.CustomWebControls.Ajax.WebKitHack.js"));
            }
        }

        #region Protected Methods

        /// <summary>
        /// Load scripts to solve multi-browser compatibility issues.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnableScriptGlobalization = true;
            EnablePartialRendering = true;
            ScriptMode = ScriptMode.Release;
            AsyncPostBackTimeout = 595;
            Scripts.Add(new ScriptReference(WebKitHack));
        }

        #endregion
    }
}