using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class PlanDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Plan); } }

        protected override void Bind()
        {
            BindingManager.BindPlan(this);
        }
    }
}