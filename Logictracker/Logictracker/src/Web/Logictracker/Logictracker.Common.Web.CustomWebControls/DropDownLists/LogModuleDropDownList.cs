#region Usings

using System;
using System.Web.UI;
using Logictracker.DatabaseTracer.Enums;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:LogModuleDropDownList runat=\"server\"></{0}:LogModuleDropDownList>")]
    public class LogModuleDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(LogModules); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindLogModules(this); }

        #endregion
    }
}
