using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.SecurityObjects;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Web.CustomWebControls.Binding;

namespace Logictracker.Web.CustomWebControls.BaseControls
{
    public abstract class DropDownListBase : DropDownList, IAutoBindeable
    {
        #region Private Properties 

        /// <summary>
        /// Parents Dictionary.
        /// </summary>
        private readonly Dictionary<Type, IAutoBindeable> _parents = new Dictionary<Type, IAutoBindeable>();

        /// <summary>
        /// Class for handling data binding.
        /// </summary>
        private BindingManager _bindingManager;

        private TextBox _filter;
        private bool _parentsLoaded;
        #endregion

        #region Public Properties

        /// <summary>
        /// The selected value of the entity beeing edited for this T of drop down.
        /// </summary>
        public int EditValue
        {
            private get { return ViewState["EditValue"] == null ? NullValue : Convert.ToInt32(ViewState["EditValue"]); }
            set { ViewState["EditValue"] = value; }
        }

        /// <summary>
        /// A comma separeted string indicating the control parents.
        /// </summary>
        [Category("Custom Properties")]
        public string ParentControls
        {
            get { return ViewState["ParentControls"] == null ? string.Empty : ViewState["ParentControls"].ToString(); }
            set { ViewState["ParentControls"] = value; }
        }
        /// <summary>
        /// Secures the control
        /// </summary>
        [Category("Custom Resources")]
        public string SecureRefference
        {
            get { return ViewState["SecureRefference"] != null ? ViewState["SecureRefference"].ToString() : string.Empty; }
            set { ViewState["SecureRefference"] = value; }
        }

        [Category("Custom Properties")]
        public string FilterControl
        {
            get { return ViewState["FilterControl"] == null ? string.Empty : ViewState["FilterControl"].ToString(); }
            set { ViewState["FilterControl"] = value; }
        }
        /// <summary>
        /// All value constant.
        /// </summary>
        public int AllValue { get { return -1; } }

        public int NoneValue { get { return -2; } }

        public virtual string AllItemsName { get { return CultureManager.GetControl("DDL_ALL_ITEMS"); } }

        public virtual string NoneItemsName { get { return CultureManager.GetControl("DDL_NONE"); } }

        /// <summary>
        /// Null value constant.
        /// </summary>
        public int NullValue { get { return int.MinValue; } }

        /// <summary>
        /// Determines wither to add the all item or not when binding.
        /// </summary>
        public bool AddAllItem
        {
            get { return ViewState["AddAllItem"] == null ? false : Convert.ToBoolean(ViewState["AddAllItem"]); }
            set { ViewState["AddAllItem"] = value; }
        }

        /// <summary>
        /// Determines wither to add the all item or not when binding.
        /// </summary>
        public bool AddNoneItem
        {
            get { return ViewState["AddNoneItem"] == null ? false : Convert.ToBoolean(ViewState["AddNoneItem"]); }
            set { ViewState["AddNoneItem"] = value; }
        }

        /// <summary>
        /// Associated data T of the DropDownlist.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// The logged in user.
        /// </summary>
        public UserSessionData Usuario { get { return WebSecurity.AuthenticatedUser; } }
        
        /// <summary>
        /// Selected Value.
        /// </summary>
        public int Selected { get { return SelectedValue != string.Empty ? int.Parse(SelectedValue) : 0; } }

        /// <summary>
        /// Get a list of selected values.
        /// </summary>
        public List<int> SelectedValues { get { return new List<int>{Selected}; } }

        public List<string> SelectedStringValues { get { return new List<string> { SelectedValue }; } }

        /// <summary>
        /// Determines if the option group should be considered on data binding.
        /// </summary>
        [Category("Custom Properties")]
        public bool UseOptionGroup
        {
            get { return (bool)(ViewState["UseOptionGroup"] ?? false); }
            set { ViewState["UseOptionGroup"] = value; }
        }

        /// <summary>
        /// Determines if data assigned to all parents should be considered on data binding.
        /// </summary>
        public bool FilterMode
        {
            get { return (bool)(ViewState["FilterMode"] ?? true); }
            set { ViewState["FilterMode"] = value; }
        }

        /// <summary>
        /// Class for handling data bindings.
        /// </summary>
        public BindingManager BindingManager { get { return _bindingManager ?? (_bindingManager = new BindingManager()); } }

        /// <summary>
        /// Determines if the control performs autopostbacks.
        /// </summary>
        [DefaultValue(true)] // So the control can remember in session its selected value.
        public override bool AutoPostBack
        {
            get { return ViewState["AutoPostBack"] == null ? true : Convert.ToBoolean(ViewState["AutoPostBack"]); }
            set { ViewState["AutoPostBack"] = value; }
        }

        /// <summary>
        /// Determines if the current control remembers its selected values in session.
        /// </summary>
        [DefaultValue(true)] // So the control can remember in session its selected value.
        public bool RememberSessionValues
        {
            get { return ViewState["RememberSessionValues"] == null ? true : Convert.ToBoolean(ViewState["RememberSessionValues"]); }
            set { ViewState["RememberSessionValues"] = value; }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Pre binding actions.
        /// </summary>
        [Category("Custom Events")]
        public event EventHandler InitialBinding;

        #endregion

        #region Public Method

        /// <summary>
        /// Gets the parent controls of the indicated T.
        /// </summary>
        /// <returns>The parent control if exist or null.</returns>
        public IAutoBindeable GetParent<T>()
        {
            if(!_parentsLoaded) LoadParents();
            var type = typeof(T);
            return _parents.ContainsKey(type) ? _parents[type] : _parents.Values.Select(parent => parent.GetParent<T>()).FirstOrDefault(par => par != null);
        }

        public int ParentSelected<T>()
        {
            var cb = GetParent<T>();
            return cb != null ? cb.Selected : -1;
        }
        public List<int> ParentSelectedValues<T>()
        {
            var cb = GetParent<T>();
            return cb != null ? cb.SelectedValues : new List<int> { -1 };
        }

        public string FilterText
        {
            get { return _filter != null ? _filter.Text.Trim() : string.Empty; }
        }

        /// <summary>
        /// Clear all items associated to the control.
        /// </summary>
        public void ClearItems() { Items.Clear(); }

        /// <summary>
        /// Adds a new item with the specified value.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="value">The associated value.</param>
        public void AddItem(string text, int value) { Items.Add(new ListItem(text, value.ToString())); }

        public void AddItem(string text, string value)
        {
            Items.Add(new ListItem(text, value));
        }

        /// <summary>
        /// Adds items with option groups.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="group"></param>
        public void AddItem(string text, int value, OptionGroupConfiguration group)
        {
            var li = new ListItem(text, value.ToString());

            if (UseOptionGroup) li.Attributes.Add("OptionGroup", group.OptionGroupDescription);

            if(!String.IsNullOrEmpty(group.Color) || !String.IsNullOrEmpty(group.BackgroundColor) || !String.IsNullOrEmpty(group.ImageUrl))
            {
                li.Attributes.Add("style",String.Concat("background-repeat: no-repeat;padding-left: 20px;",
                                        !String.IsNullOrEmpty(group.ImageUrl)
                                            ? "background-image: url(" +  ResolveUrl(group.ImageUrl) +
                                              "); background-position: left center;"
                                            : String.Empty,
                                        !String.IsNullOrEmpty(group.BackgroundColor)
                                            ? "background-color: " + group.BackgroundColor + ";"
                                            : String.Empty,
                                        !String.IsNullOrEmpty(group.Color)
                                            ? "color: " + group.Color + ";"
                                            : String.Empty));
            }
            
                
            Items.Add(li);
        }

        /// <summary>
        /// Adds a new item with the specified value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text">The text of the item.</param>
        /// <param name="value">The associated value.</param>
        public void AddItemAt(Int32 index, String text, Int32 value) { Items.Insert(index, new ListItem(text, value.ToString())); }

        /// <summary>
        /// Adds items with option groups.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="group"></param>
        public void AddItemAt(Int32 index, String text, Int32 value, OptionGroupConfiguration group)
        {
            var li = new ListItem(text, value.ToString());

            if (UseOptionGroup) li.Attributes.Add("OptionGroup", group.OptionGroupDescription);

            if (!String.IsNullOrEmpty(group.Color) || !String.IsNullOrEmpty(group.BackgroundColor) || !String.IsNullOrEmpty(group.ImageUrl))
                li.Attributes.Add("style", String.Concat("background-repeat: no-repeat;padding-left: 20px;",
                            !String.IsNullOrEmpty(group.ImageUrl) ? "background-image: url(" + ResolveUrl(group.ImageUrl) +
                                                                "); background-position: left center;"
                                                              : String.Empty,
                                                          !String.IsNullOrEmpty(group.BackgroundColor)
                                                              ? "background-color: " + group.BackgroundColor + ";"
                                                              : String.Empty,
                                                          !String.IsNullOrEmpty(group.Color)
                                                              ? "color: " + group.Color + ";"
                                                              : String.Empty));

            Items.Insert(index, li);
        }

        /// <summary>
        /// Sets the item with the givenn value as the selected item.
        /// </summary>
        /// <param name="value"></param>
        public void SetSelectedValue(int value)
        {
            SelectedIndex = Items.IndexOf(Items.FindByValue(value.ToString()));

            OnSelectedIndexChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Custom data bidning.
        /// </summary>
        public override void DataBind()
        {
            Bind();

            SetSelectedValue();
        }

        /// <summary>
        /// Removes the item associated to the specified value.
        /// </summary>
        /// <param name="value"></param>
        public void RemoveItem(int value)
        {
            var item = Items.FindByValue(value.ToString());

            if (item == null) return;

            Items.Remove(item);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets the selected value in session in order to be remembered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            AddSelectedValueToSession();
        }

        /// <summary>
        /// Initial binding of the control
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected override void OnPagePreLoad(Object o, EventArgs e)
        {
            base.OnPagePreLoad(o, e);

            LoadParents();

            if (Page.IsPostBack) return;

            GetInitialEditValue();

            DoBind();
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Visible = WebSecurity.IsSecuredAllowed(SecureRefference);
        }
        /// <summary>
        /// Binding logic.
        /// </summary>
        protected abstract void Bind();

        #endregion

        #region Private Methods

        /// <summary>
        /// Handler for rebinding control when the selected index of a parent changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdlSelectedIndexChanged(object sender, EventArgs e) { DoBind(); }

        /// <summary>
        /// Adds the selected value to session.
        /// </summary>
        private void AddSelectedValueToSession() { if (RememberSessionValues) Page.Session.Add(GetType().Name, Selected); }

        /// <summary>
        /// Binds data, selects edited index and tells its child controls to rebind.
        /// </summary>
        private void DoBind()
        {
            Bind();

            SetSelectedValue();

            OnSelectedIndexChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Sets initial edited or selected index.
        /// </summary>
        private void SetSelectedValue()
        {
            if (EditValue.Equals(NullValue) && RememberSessionValues) GetSessionValue();

            SelectedIndex = Items.IndexOf(Items.FindByValue(EditValue.ToString()));

            EditValue = NullValue;
        }

        /// <summary>
        /// Gets the selected value for the control from session.
        /// </summary>
        private void GetSessionValue() { if (Page.Session[GetType().Name] != null) EditValue = Convert.ToInt32(Page.Session[GetType().Name]); }

        /// <summary>
        /// Gets the drop down initial edit value to be selected.
        /// </summary>
        private void GetInitialEditValue() { if (InitialBinding != null) InitialBinding(this, EventArgs.Empty); }

        /// <summary>
        /// Add Parent to Parents Dictionary DropDownList.
        /// </summary>
        /// <param name="ddl"></param>
        public void AddParent(IAutoBindeable ddl)
        {
            if (_parents.ContainsKey(ddl.Type)) return;
            _parents.Add(ddl.Type, ddl);

            ddl.SelectedIndexChanged += DdlSelectedIndexChanged;
        }

        /// <summary>
        /// Loads control parents.
        /// </summary>
        private void LoadParents()
        {
            if (!string.IsNullOrEmpty(ParentControls))
            {

                foreach (var parent in ParentControls.Split(','))
                {
                    var control = FindParent(parent.Trim(), Page.Controls);

                    if (control == null) continue;

                    var autobind = control as IAutoBindeable;
                    var owner = control as IParentControl;

                    if (autobind != null) AddParent(autobind);
                    else if (owner != null) foreach (var ctl in owner.ParentControls) AddParent(ctl);
                }
            }
            if(!string.IsNullOrEmpty(FilterControl))
            {
                _filter = FindParent(FilterControl, Page.Controls) as TextBox;
                if(_filter != null)
                {
                    _filter.TextChanged += DdlSelectedIndexChanged;
                    _filter.AutoPostBack = true;
                }
            }
            _parentsLoaded = true;
        }

        /// <summary>
        /// Finds the control asociated to the givenn id.
        /// </summary>
        /// <param name="parent">The id of a parent control.</param>
        /// <param name="controls">A list of controls.</param>
        /// <returns>The parent control or null.</returns>
        private static Control FindParent(string parent, ControlCollection controls)
        {
            if (controls == null) return null;

            foreach (Control control in controls)
            {
                if (!string.IsNullOrEmpty(control.ID) && control.ID.Equals(parent)) return control;

                var cnt = FindParent(parent, control.Controls);

                if (cnt != null) return cnt;
            }

            return null;
        }

        #endregion
    }
}