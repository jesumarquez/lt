#region Usings

using System;
using Urbetrack.Common.Web.CustomWebControls.BaseControls;
using Urbetrack.Common.Web.CustomWebControls.Helpers;
using Urbetrack.Types.BusinessObjects;

#endregion

namespace Urbetrack.Common.Web.CustomWebControls.DropDownLists
{
    public class TipoGeocercaDropDownList: DropDownListBase
    {
        public override Type Type
        {
            get { return typeof (TipoGeocerca); }
        }

        protected override void Bind()
        {
            BindingManager.BindTipoGeocerca(this);
        }
    }
}
