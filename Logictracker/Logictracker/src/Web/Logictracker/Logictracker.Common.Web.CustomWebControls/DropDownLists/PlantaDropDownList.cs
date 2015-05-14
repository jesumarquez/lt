#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:LocacionDropDownList runat=\"server\"></{0}:LocacionDropDownList>")]
    public class PlantaDropDownList : DropDownListBase
    {
        #region Public Properties
        
        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(Linea); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindPlanta(this); }

        #endregion
    }
}