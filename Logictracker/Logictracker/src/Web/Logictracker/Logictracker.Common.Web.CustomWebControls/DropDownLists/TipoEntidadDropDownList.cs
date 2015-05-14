using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoEntidadDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TipoEntidad); } }

        protected override void Bind()
        {
            BindingManager.BindTiposEntidad(this);
        }
    }
}