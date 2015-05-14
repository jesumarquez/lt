#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists.Tickets
{
    [ToolboxData("<{0}:CicloLogisticoDropDownList ID=\"CicloLogisticoDropDownList1\" runat=\"server\" />")]
    [Serializable]
    public class CicloLogisticoDropDownList : DropDownListBase, ICicloLogisticoAutoBindeable
    {
        public override Type Type { get { return typeof (CicloLogistico); } }

        public bool AddEstados
        {
            get { return (bool) (ViewState["AddEstados"] ?? false); }
            set { ViewState["AddEstados"] = value; }
        }
        public bool AddCiclos
        {
            get { return (bool)(ViewState["AddCiclos"] ?? true); }
            set { ViewState["AddCiclos"] = value; }
        }
        protected override void Bind()
        {
            BindingManager.BindCiclosLogisticos(this);
        }
    }
}
