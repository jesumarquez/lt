using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [Serializable]
    public class TiposMovimientoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(ConsumoCabecera.TiposMovimiento); } }
        
        protected override void Bind() { BindingManager.BindTiposMovimiento(this); }
    }
}