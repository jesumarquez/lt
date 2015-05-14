#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:ClienteDropDownList ID=\"ClienteDropDownList1\" runat=\"server\"></{0}:ClienteDropDownList>")]
    [Serializable]
    public class ClienteDropDownList : DropDownListBase
    {
        #region Public Properties 
        
        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(Cliente); } }
        
        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindCliente( this ); }

        #endregion
    }
}