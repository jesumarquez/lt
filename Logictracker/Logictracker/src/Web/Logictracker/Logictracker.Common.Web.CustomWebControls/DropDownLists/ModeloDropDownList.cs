using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ModeloDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Modelo); } }

        protected override void Bind()
        {
            BindingManager.BindModelos(this);
        }
    }
}