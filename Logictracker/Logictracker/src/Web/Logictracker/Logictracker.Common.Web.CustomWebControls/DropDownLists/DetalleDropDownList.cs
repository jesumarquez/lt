using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class DetalleDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Detalle); } }

        public bool BindPadres { get; set; }

        protected override void Bind()
        {
            BindingManager.BindDetalle(this);
        }
    }
}