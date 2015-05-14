using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class UnidadMedidaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(UnidadMedida); } }

        protected override void Bind()
        {
            BindingManager.BindUnidadMedida(this);
        }
    }
}