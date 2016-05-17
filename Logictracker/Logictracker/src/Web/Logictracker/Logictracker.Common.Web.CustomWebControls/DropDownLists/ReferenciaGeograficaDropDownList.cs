#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ReferenciaGeograficaDropDownList: DropDownListBase
    {
        public override Type Type { get { return typeof (ReferenciaGeografica); } }

        public bool ShowOnlyAccessControl
        {
            get { return (bool)(ViewState["ShowOnlyAccessControl"] ?? false); }
            set { ViewState["ShowOnlyAccessControl"] = value; }
        }

        protected override void Bind() { BindingManager.BindReferenciaGeografica(this); }
    }
}
