using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TipoZonaAccesoListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(TipoZonaAcceso); } }

        protected override void Bind() { BindingManager.BindTipoZonaAcceso(this); }
    }
}