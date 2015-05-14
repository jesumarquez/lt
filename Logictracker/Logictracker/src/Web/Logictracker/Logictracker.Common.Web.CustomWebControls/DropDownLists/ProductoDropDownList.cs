using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ProductoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Producto); } }

        protected override void Bind()
        {
            BindingManager.BindProductos(this);
        }
    }
}