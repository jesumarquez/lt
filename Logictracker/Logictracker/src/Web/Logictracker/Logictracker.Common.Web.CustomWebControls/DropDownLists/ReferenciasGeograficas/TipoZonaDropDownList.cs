using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists.ReferenciasGeograficas
{
    public class TipoZonaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TipoCoche); } }

        protected override void Bind() { BindingManager.BindTipoZona(this); }
    }
}