using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class PuertaListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(PuertaAcceso); } }

        protected override void Bind() { BindingManager.BindPuertas(this); }
    }
}