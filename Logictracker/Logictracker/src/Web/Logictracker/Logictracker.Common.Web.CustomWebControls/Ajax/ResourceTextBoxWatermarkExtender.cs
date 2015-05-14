using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;
using Logictracker.Culture;

namespace Logictracker.Web.CustomWebControls.Ajax
{
    [ToolboxData("<{0}:ResourceTextBoxWatermarkExtender ID=\"ResourceTextBoxWatermarkExtender1\" runat=\"server\"></{0}:ResourceTextBoxWatermarkExtender>")]
    public class ResourceTextBoxWatermarkExtender : TextBoxWatermarkExtender
    {
        #region Protected Properties

        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public string ResourceName
        {
            get
            {
                return ViewState["ResourceName"] != null
                           ? ViewState["ResourceName"].ToString()
                           : string.Empty;
            }
            set
            {
                ViewState["ResourceName"] = value;
            }
        }

        /// <summary>
        /// The name of the specific variable wanted form the resource manager.
        /// </summary>
        [Category("Custom Resources")]
        public string VariableName
        {
            get
            {
                return ViewState["VariableName"] != null
                           ? ViewState["VariableName"].ToString()
                           : string.Empty;
            }
            set
            {
                ViewState["VariableName"] = value;
            }
        }

        #endregion

        public new string WatermarkText
        {
            get
            {
                return CultureManager.GetString(ResourceName, VariableName);
            }
            set
            {
                base.WatermarkText = value;
            }
        }
    }
}
