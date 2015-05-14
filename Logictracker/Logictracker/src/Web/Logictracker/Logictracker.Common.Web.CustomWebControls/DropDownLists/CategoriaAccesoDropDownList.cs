using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class CategoriaAccesoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Tarjeta); } }

        protected override void Bind() { BindingManager.BindCategoriaAcceso(this); }
    }
}