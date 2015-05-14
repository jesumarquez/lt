using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class SubEntidadDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(SubEntidad); } }

        public string TipoMedicion { get; set; }

        protected override void Bind()
        {
            BindingManager.BindSubEntidades(this);
        }
    }
}