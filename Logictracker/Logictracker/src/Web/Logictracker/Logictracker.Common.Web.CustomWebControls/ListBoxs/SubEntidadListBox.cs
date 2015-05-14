using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public enum SubEntidadOptionGroupValue { Entidad } 

    public class SubEntidadListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(SubEntidad); } }

        public string TipoMedicion { get; set; }

        public SubEntidadOptionGroupValue OptionGroupProperty { get; set; }

        protected override void Bind() { BindingManager.BindSubEntidadesList(this); }
    }
}