using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:SubCentroDeCostosDropDownList runat=\"server\"></{0}:SubCentroDeCostosDropDownList>")]
    public class SubCentroDeCostosDropDownList : DropDownListBase
    {
           #region Public Method

        public override Type Type
        {
            get { return typeof (SubCentroDeCostos); }
        }

        protected override void Bind()
        {
            BindingManager.BindSubCentroDeCostos(this);
        }

        #endregion 
    }
}