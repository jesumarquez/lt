#region Usings

using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    public class CicloLogisticoDropDownListWrapper: DropDownListBaseWrapper<CicloLogistico>, ICicloLogisticoAutoBindeable
    {
        public CicloLogisticoDropDownListWrapper(DropDownList dropDownList) : base(dropDownList)
        {
        }

        public CicloLogisticoDropDownListWrapper(DropDownList dropDownList, bool addAllItem) : base(dropDownList, addAllItem)
        {
        }

        public bool AddEstados {get;set;}

        public bool AddCiclos { get; set; }
    }
}
