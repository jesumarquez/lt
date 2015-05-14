using System;
using System.Web.UI;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:TransportistaDropDownCheckList runat=\"server\"></{0}:TransportistaDropDownCheckList>")]
    public class TransportistaDropDownCheckList : DropDownCheckListBase
    {
        #region Overrides of DropDownCheckListBase

        public override Type Type { get { return typeof(Transportista); } }

        public override string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_TRANSPORTISTA"); }
        }

        protected override void Bind()
        {
            BindingManager.BindTransportista(this);
        }

        #endregion
    }
}
