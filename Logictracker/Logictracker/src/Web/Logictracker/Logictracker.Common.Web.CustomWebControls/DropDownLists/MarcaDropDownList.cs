using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class MarcaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Marca); } }

        protected override void Bind()
        {
            BindingManager.BindMarcas(this);
        }
    }
}