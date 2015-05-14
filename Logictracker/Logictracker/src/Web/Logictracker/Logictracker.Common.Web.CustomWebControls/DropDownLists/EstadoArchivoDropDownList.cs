#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:LocacionDropDownList runat=\"server\"></{0}:LocacionDropDownList>")]
    public class EstadoArchivoDropDownList : DropDownListBase
    {
        #region Public Properties
        
        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(string); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindEstadoArchivo(this); }

        #endregion
    }
}