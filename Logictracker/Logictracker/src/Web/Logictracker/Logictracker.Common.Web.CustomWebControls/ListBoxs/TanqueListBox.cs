#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TanqueListBox : ListBoxBase, ITanqueAutoBindeable
    {
        #region Public Properties

        /// <summary>
        /// Gets the type associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(Tanque); } }

        /// <summary>
        /// Determines wither to add the "not employee" option.
        /// </summary>
        public bool AllowEquipmentBinding
        {
            get { return ViewState["AllowEquipmentBinding"] != null ? Convert.ToBoolean(ViewState["AllowEquipmentBinding"]) : false; }
            set { ViewState["AllowEquipmentBinding"] = value; }
        }

        public bool AllowBaseBinding
        {
            get { return ViewState["AllowBaseBinding"] != null ? Convert.ToBoolean(ViewState["AllowBaseBinding"]) : false; }
            set { ViewState["AllowBaseBinding"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Tank data binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTanque(this); }

        #endregion
    }
}

