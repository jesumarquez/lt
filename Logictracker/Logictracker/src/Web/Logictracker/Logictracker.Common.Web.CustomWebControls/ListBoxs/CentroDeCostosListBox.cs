using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class CentroDeCostosListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(CentroDeCostos); } }

        protected override void Bind() { BindingManager.BindCentroDeCostos(this); }
    }
}