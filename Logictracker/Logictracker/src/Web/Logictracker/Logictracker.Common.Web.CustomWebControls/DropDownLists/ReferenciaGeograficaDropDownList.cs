#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ReferenciaGeograficaDropDownList: DropDownListBase
    {
        public override Type Type { get { return typeof (ReferenciaGeografica); } }

        protected override void Bind() { BindingManager.BindReferenciaGeografica(this); }
    }
}
