#region Usings

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Web.CustomWebControls.Binding;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:MenuResourcesDropDownList runat=\"server\"></{0}:MenuResourcesDropDownList>")]
    public class MenuResourcesDropDownList : DropDownList
    {
        /// <summary>
        /// Class for handling data bindings.
        /// </summary>
        private BindingManager _bindingManager;

        /// <summary>
        /// Class for handling data bindings.
        /// </summary>
        private BindingManager BindingManager { get { return _bindingManager ?? (_bindingManager = new BindingManager()); } }

        public bool FunctionMode
        {
            get { return (bool)(ViewState["ShowFunctions"] ?? true); }
            set { ViewState["ShowFunctions"] = value; }
        }

        public bool AddEmptyItem
        {
            get { return (bool)(ViewState["AddEmptyItem"] ?? true); }
            set { ViewState["AddEmptyItem"] = value; }
        }

        public void Bind() { BindingManager.BindMenuResources(this); }

        protected override void OnPagePreLoad(Object o, EventArgs e)
        {
            base.OnPagePreLoad(o, e);

            if (Page.IsPostBack) return;

            Bind();
        }
    }
}
