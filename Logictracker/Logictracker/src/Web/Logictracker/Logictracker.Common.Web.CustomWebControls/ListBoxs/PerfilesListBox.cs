#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class PerfilesListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(Perfil); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Profiles binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindProfiles(this); }

        #endregion
    }
}