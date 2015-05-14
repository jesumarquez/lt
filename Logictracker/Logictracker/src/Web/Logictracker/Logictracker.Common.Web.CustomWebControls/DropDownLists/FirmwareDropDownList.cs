#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class FirmwareDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated T.
        /// </summary>
        public override Type Type { get { return typeof(Firmware); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Devices binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindFirmwares(this); }

        #endregion
    }
}