using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class PrecintoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Precinto); } }
     
        protected override void Bind() { BindingManager.BindPrecinto(this); }
    }
}