#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:SubSistemasDropDownList runat=\"server\"></{0}:SubSistemasDropDownList>")]
    public class SubSistemaDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(Sistema); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindSubSistema(this);
        }

        #endregion
    }
}