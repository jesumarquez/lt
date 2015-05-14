#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:PerfilDropDownCheckList runat=\"server\"></{0}:PerfilDropDownCheckList>")]
    public class PerfilDropDownCheckList : DropDownCheckListBase
    {
        #region Overrides of DropDownCheckListBase

        public override Type Type { get { return typeof (Perfil); } }

        protected override void Bind()
        {
            BindingManager.BindProfiles(this);
        }

        #endregion
    }
}
