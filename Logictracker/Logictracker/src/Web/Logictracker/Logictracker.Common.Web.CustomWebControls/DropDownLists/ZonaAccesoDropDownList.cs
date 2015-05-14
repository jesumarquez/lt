using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ZonaAccesoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(ZonaAcceso); } }

        protected override void Bind()
        {
            BindingManager.BindZonaAcceso(this);
        }
    }
}