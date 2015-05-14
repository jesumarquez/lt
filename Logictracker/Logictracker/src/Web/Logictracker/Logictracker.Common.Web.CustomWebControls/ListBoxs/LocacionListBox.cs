#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class LocacionListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(Empresa); } }

        protected override void Bind() { BindingManager.BindLocacion(this); }
    }
}
