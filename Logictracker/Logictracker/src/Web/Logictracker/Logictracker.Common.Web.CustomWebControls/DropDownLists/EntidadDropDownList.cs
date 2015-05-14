using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class EntidadDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(EntidadPadre); } }

        protected override void Bind()
        {
            BindingManager.BindEntidades(this);
        }
    }
}