using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class PerfilDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Perfil); } }

        protected override void Bind()
        {
            BindingManager.BindProfiles(this);
        }
    }
}