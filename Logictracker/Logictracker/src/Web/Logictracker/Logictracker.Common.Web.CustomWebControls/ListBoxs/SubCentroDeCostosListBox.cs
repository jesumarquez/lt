using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class SubCentroDeCostosListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(SubCentroDeCostos); } }

        protected override void Bind() { BindingManager.BindSubCentroDeCostos(this); }
    }
}