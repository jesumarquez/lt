#region Usings

using System;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoDocumentoDropDownList : DropDownListBase, ITipoDocumentoAutoBindeable
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof(TipoDocumento); } }

        public bool OnlyForVehicles { get; set; }
        public bool OnlyForEmployees { get; set; }
        public bool OnlyForEquipment { get; set; }
        public bool OnlyForTransporter { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Vehicle types binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoDocumento(this); }

        #endregion
    }
}
