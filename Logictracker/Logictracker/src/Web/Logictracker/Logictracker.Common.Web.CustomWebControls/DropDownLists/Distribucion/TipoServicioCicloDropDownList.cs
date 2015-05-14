using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists.Distribucion
{
    [ToolboxData("<{0}:TipoServicioCicloDropDownList ID=\"TipoServicioCicloDropDownList1\" runat=\"server\"></{0}:TipoServicioCicloDropDownList>")]
    [Serializable]
    public class TipoServicioCicloDropDownList:DropDownListBase
    {
        public override Type Type { get { return typeof(TipoServicioCiclo); } }
        protected override void Bind() { BindingManager.BindTipoServicioCiclo(this); }
    }
}
