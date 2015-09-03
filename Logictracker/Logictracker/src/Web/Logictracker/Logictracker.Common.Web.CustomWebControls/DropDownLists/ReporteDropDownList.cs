using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ReporteDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(int); } }
        protected override void Bind() { BindingManager.BindReporte(this); }
    }
}