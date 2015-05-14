#region Usings

using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;

#endregion

namespace Logictracker.Web.Helpers.C1Helpers
{
    [ToolboxData("<{0}:C1ResourceTemplateColumn></{0}:C1ResourceTemplateColumn>")]
    public class C1ResourceTemplateColumn: C1TemplateField
    {
        #region Public Properties

        /// <summary>
        /// The name of the resource set wich contains the resource to be displayed.
        /// </summary>
        public string ResourceName
        {
            get { return (string)ViewState["ResourceName"]; }
            set { ViewState["ResourceName"] = value; }
        }

        /// <summary>
        /// The resource name to be displayed.
        /// </summary>
        public string VariableName
        {
            get { return (string)ViewState["VariableName"]; }
            set { ViewState["VariableName"] = value; }
        }

        /// <summary>
        /// Provides the text to be displayed as the header of the column.
        /// </summary>
        public override string HeaderText
        {
            get { return ResourceName != null ? CultureManager.GetString(ResourceName, VariableName) : base.HeaderText; }
            set { base.HeaderText = value; }
        }

        #endregion
    }
}
