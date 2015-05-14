using System;
using System.Web.UI;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:CentroDeCostosDropDownList runat=\"server\"></{0}:CentroDeCostosDropDownList>")]
    public class CentroDeCostosDropDownList : DropDownListBase
    {
        #region Public Method

        public override Type Type
        {
            get { return typeof (CentroDeCostos); }
        }

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_SIN_CENTRO"); } }

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindCentroDeCostos(this);
        }

        #endregion
    }
}