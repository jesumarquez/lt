using System;
using System.ComponentModel;
using AjaxControlToolkit;
using Logictracker.Culture;

namespace Logictracker.Web.CustomWebControls.Input
{
    public class ResourceWatermarkExtender : TextBoxWatermarkExtender
    {
        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public string ResourceName
        {
            get { return ViewState["ResourceName"] != null ? ViewState["ResourceName"].ToString() : string.Empty; }
            set { ViewState["ResourceName"] = value; }
        }

        /// <summary>
        /// The name of the specific variable wanted form the resource manager.
        /// </summary>
        [Category("Custom Resources")]
        public string VariableName
        {
            get { return ViewState["VariableName"] != null ? ViewState["VariableName"].ToString() : string.Empty; }
            set { ViewState["VariableName"] = value; }
        }

        #region Protected Methods

        /// <summary>
        /// Gets the resource value and sets it as the label text.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            WatermarkText = CultureManager.GetString(ResourceName, VariableName);
        }

        #endregion
    }
}
