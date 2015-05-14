using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using ExtExtenders;
using Logictracker.Culture;

namespace Logictracker.Web.CustomWebControls.LayoutRegions
{
    [ToolboxData("<{0}:ResourceLayoutRegion ID=\"ResourceLayoutRegion1\" runat=\"server\"></{0}:ResourceLayoutRegion>")]
    public class ResourceLayoutRegion : LayoutRegion
    {
        #region Public Properties

        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public string ResourceName
        {
            get { return ViewState[ClientID + "ResourceName"] != null ? ViewState[ClientID + "ResourceName"].ToString() : string.Empty; }
            set { ViewState[ClientID + "ResourceName"] = value; }
        }

        /// <summary>
        /// The name of the specific variable wanted form the resource manager.
        /// </summary>
        [Category("Custom Resources")]
        public string VariableName
        {
            get { return ViewState[ClientID + "VariableName"] != null ? ViewState[ClientID + "VariableName"].ToString() : string.Empty; }
            set { ViewState[ClientID + "VariableName"] = value; }
        }

        /// <summary>
        /// Gets the layout region title.
        /// </summary>
        public override string title
        {
            get { return ResourceName != null ? CultureManager.GetString(ResourceName, VariableName) : base.title; }
            set { base.title = value; }
        }

        /// <summary>
        /// Gets the script references.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ScriptReference> GetScriptReferences()
        {
            var reference = new ScriptReference();

            if (Page != null)
                reference.Path = Page.ClientScript.GetWebResourceUrl(typeof(LayoutRegion), "ExtExtenders.BorderLayout.Borderlayout.js");

            return new[] { reference };
        }

        #endregion
    }
}
