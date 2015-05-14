using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TallerDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Taller); } }

        protected override void Bind() { BindingManager.BindTalleres(this); }
    }
}