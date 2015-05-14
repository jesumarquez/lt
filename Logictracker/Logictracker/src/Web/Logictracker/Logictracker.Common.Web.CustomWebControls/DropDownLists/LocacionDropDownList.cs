#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:LocacionDropDownList runat=\"server\"></{0}:LocacionDropDownList>")]
    public class LocacionDropDownList : DropDownListBase, IAutoBindeable
    {
        #region Public Properties 
        
        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(Empresa); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindLocacion(this); }
     
        #endregion
    }
}