#region Usings

using System;
using System.Web.UI;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:LogTypesDropDownList runat=\"server\"></{0}:LogTypesDropDownList>")]
    public class LogTypeDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(LogTypes); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindLogTypes(this); }

        #endregion
    }
}
