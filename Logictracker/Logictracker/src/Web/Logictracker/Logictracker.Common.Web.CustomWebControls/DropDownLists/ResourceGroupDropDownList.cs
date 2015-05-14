#region Usings

using System;
using System.Web.UI.WebControls;
using Logictracker.Web.CustomWebControls.Binding;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    /// <summary>
    /// Drop down list for display application resource groups.
    /// </summary>
    public class ResourceGroupDropDownList : DropDownList
    {
        #region Private Properties

        /// <summary>
        /// Class for handling data bindings.
        /// </summary>
        private BindingManager _bindingManager;

        /// <summary>
        /// Class for handling data bindings.
        /// </summary>
        private BindingManager BindingManager { get { return _bindingManager ?? (_bindingManager = new BindingManager()); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Resource groups data binding.
        /// </summary>
        protected void Bind() { BindingManager.BindResourceGroup(this); }

        /// <summary>
        /// Automatic binding of the control
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected override void OnPagePreLoad(Object o, EventArgs e)
        {
            base.OnPagePreLoad(o, e);

            if (Page.IsPostBack) return;

            Bind();
        }

        #endregion
    }
}
