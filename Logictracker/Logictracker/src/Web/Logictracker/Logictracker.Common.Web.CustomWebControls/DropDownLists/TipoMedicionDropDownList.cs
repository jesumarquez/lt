using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoMedicionDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TipoMedicion); } }

        protected override void Bind()
        {
            BindingManager.BindTiposMedicion(this);
        }
    }
}