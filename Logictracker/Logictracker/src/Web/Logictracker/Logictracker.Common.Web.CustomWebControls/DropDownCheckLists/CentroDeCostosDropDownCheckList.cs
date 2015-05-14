using System;
using System.Web.UI;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:CentroDeCostosDropDownCheckList runat=\"server\"></{0}:CentroDeCostosDropDownCheckList>")]
    public class CentroDeCostosDropDownCheckList : DropDownCheckListBase
    {
        #region Public Properties

        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(CentroDeCostos); } }

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_SIN_CENTRO"); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindCentroDeCostos(this); }

        #endregion
    }
}
