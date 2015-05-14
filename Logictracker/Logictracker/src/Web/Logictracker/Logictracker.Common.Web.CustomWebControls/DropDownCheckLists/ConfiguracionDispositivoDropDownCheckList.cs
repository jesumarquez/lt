#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    /// <summary>
    /// Class for adding custom device configuration behaivour to a custom drop down check list.
    /// </summary>
    [ToolboxData("<{0}:ConfiguracionDispositivoDropDownCheckList runat=\"server\"></{0}:ConfiguracionDispositivoDropDownCheckList>")]
    public class ConfiguracionDispositivoDropDownCheckList : DropDownCheckListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the type associated to the custom drop down check list.
        /// </summary>
        public override Type Type { get { return typeof(ConfiguracionDispositivo); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Devices binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindConfigurations(this); }

        #endregion
    }
}