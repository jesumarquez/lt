using System;
using System.Web.UI;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists.Tickets
{
    [ToolboxData("<{0}:EstadosDropDownList ID=\"EstadosDropDownList1\" runat=\"server\" />")]
    [Serializable]
    public class EstadosDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Int32); } }
        protected override void Bind() { BindingManager.BindEstadosPedido(this); }
    }
}
