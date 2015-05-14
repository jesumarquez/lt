#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible
{
    [ToolboxData("<{0}:TanqueDropDownList runat=\"server\"></{0}:TanqueDropDownList>")]
    public class TanqueDropDownList : DropDownListBase, ITanqueAutoBindeable
    {
        #region Public Properties

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
        /// Bind Tanque.
        /// </summary>
        protected override void Bind() { BindingManager.BindTanque(this); }

        #endregion
    }
}
