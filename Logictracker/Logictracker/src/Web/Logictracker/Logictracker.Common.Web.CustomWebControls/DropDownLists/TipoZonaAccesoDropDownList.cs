using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoZonaAccesoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TipoZonaAcceso); } }

        protected override void Bind()
        {
            BindingManager.BindTipoZonaAcceso(this);
        }
    }
}