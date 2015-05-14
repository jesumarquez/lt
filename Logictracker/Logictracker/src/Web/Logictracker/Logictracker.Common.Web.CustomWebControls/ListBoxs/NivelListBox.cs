using System;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class NivelListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(Nivel); } }

        protected override void Bind() { BindingManager.BindNivel(this); }
    }
}