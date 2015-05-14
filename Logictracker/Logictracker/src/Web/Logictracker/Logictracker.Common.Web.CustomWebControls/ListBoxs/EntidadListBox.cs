using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class EntidadListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(EntidadPadre); } }

        protected override void Bind() { BindingManager.BindEntidades(this); }
    }
}