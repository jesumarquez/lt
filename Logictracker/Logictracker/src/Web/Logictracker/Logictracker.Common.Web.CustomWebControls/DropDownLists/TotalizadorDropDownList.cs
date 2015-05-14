using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TotalizadorDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Coche.Totalizador); } }

        protected override void Bind()
        {
            BindingManager.BindTotalizador(this);
        }
    }
}