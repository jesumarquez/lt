using System;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class CategoriaAccesoListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(CategoriaAccesoListBox); } }

        protected override void Bind() { BindingManager.BindCategoriaAcceso(this); }
    }
}