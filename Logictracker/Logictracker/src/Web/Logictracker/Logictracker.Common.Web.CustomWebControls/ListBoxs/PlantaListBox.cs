#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class PlantaListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(Linea); } }

        #endregion

        #region Protected Methods
        
        /// <summary>
        /// Companyes binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindPlanta(this); }

        #endregion
    }
}