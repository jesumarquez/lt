#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class ClienteListBox : ListBoxBase
    {
        public override Type Type { get { return typeof (Cliente); } }

        protected override void Bind() { BindingManager.BindCliente(this); }
    }
}
