using System;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class CategoriaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Categoria); } }

        public int TipoProblema { get; set; }

        protected override void Bind()
        {
            BindingManager.BindCategoria(this);
        }
    }
}