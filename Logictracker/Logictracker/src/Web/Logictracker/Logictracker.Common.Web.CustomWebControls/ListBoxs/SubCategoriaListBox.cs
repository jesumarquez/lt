using System;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class SubCategoriaListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(Subcategoria); } }

        protected override void Bind() { BindingManager.BindSubCategoria(this); }
    }
}