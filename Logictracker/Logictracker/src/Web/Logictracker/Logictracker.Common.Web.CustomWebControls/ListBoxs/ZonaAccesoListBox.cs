using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class ZonaAccesoListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(ZonaAcceso); } }

        protected override void Bind() { BindingManager.BindZonaAcceso(this); }
    }
}