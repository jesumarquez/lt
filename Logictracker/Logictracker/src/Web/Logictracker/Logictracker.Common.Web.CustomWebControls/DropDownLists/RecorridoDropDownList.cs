using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:RecorridoDropDownList ID=\"RecorridoDropDownList1\" runat=\"server\"></{0}:SubSistemasDropDownList>")]
    public class RecorridoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Recorrido); } }

        protected override void Bind()
        {
            BindingManager.BindRecorrido(this);
        }
    }
}