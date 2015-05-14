#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:SonidoDropDownList runat=\"server\"></{0}:SonidoDropDownList>")]
    public class SonidoDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// The T associated to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof(Sonido); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sounds binding.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindSonidos(this);
        }

        #endregion
    }
}