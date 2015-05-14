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
    public abstract class ListBoxBase : ListBox, IAutoBindeable
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

        #endregion

        #region Public Properties

        /// <summary>
        /// The selected values of the entity beeing edited.
        /// </summary>
        public List<int> EditValues
        {
            private get { return ViewState["EditValues"] == null ? new List<int> { NullValue } : (List<int>)ViewState["EditValues"]; }
            set { ViewState["EditValues"] = value; }
        }

        /// <summary>
        /// Determines if the option group should be considered for data binding.
        /// </summary>
        [Category("Custom Properties")]
        public bool UseOptionGroup
        {
            get { return (bool) (ViewState["UseOptionGroup"] ?? false); }
            set { ViewState["UseOptionGroup"] = value; }
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

        /// <summary>
        /// None value constant.
        /// </summary>
        public int NoneValue { get { return -2; } }

        public virtual string AllItemsName { get { return CultureManager.GetControl("DDL_ALL_ITEMS"); } }

        public virtual string NoneItemsName { get { return CultureManager.GetControl("DDL_NONE"); } }


        /// <summary>
        /// Null value constant.
        /// </summary>
        public int NullValue { get { return int.MinValue; } }

        /// <summary>
        /// The logged in user.
        /// </summary>
        public UserSessionData Usuario { get { return WebSecurity.AuthenticatedUser; } }

        /// <summary>
        /// Associated data T of the ListBox.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets first selected value.
        /// </summary>
        public int Selected { get { return SelectedValues.Count > 0 ? SelectedValues[0] : 0; } }

        /// <summary>
        /// Gets all selected values.
        /// </summary>
        public List<int> SelectedValues
        {
            get
            {
                return SelectedIndex < 0
                           ? new List<int>(1) {0}
                           : (from index in GetSelectedIndices()
                              select Items[index].Value.Contains("T-") || Items[index].Value.Contains("V-")
                                    ? Convert.ToInt32(Items[index].Value.Split('-')[1]) 
                                    : Convert.ToInt32(Items[index].Value))
                             .ToList();
            }
        }

        public List<string> SelectedStringValues { get { return SelectedIndex < 0 ? new List<string>(0){} : (from index in GetSelectedIndices() select Items[index].Value).ToList(); } }


        /// <summary>
        /// Determines if the "all item" should be binded.
        /// </summary>
        public bool AddAllItem { get { return false; } }

        /// <summary>
        /// Determines wither to add the all item or not when binding.
        /// </summary>
        public bool AddNoneItem
        {
            get { return ViewState["AddNoneItem"] == null ? false : Convert.ToBoolean(ViewState["AddNoneItem"]); }
            set { ViewState["AddNoneItem"] = value; }
        }

        /// <summary>
        /// Determines if the data binding should contemplate data ssigned to all parents.
        /// </summary>
        public bool FilterMode
        {
            get { return (bool)(ViewState["FilterMode"] ?? true); }
            set { ViewState["FilterMode"] = value; }
        }

        /// <summary>
        /// Class for handling data binding.
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

        #region Public Methods

        /// <summary>
        /// Gets the parent controls of the indicated T.
        /// </summary>
        /// <returns>The parent control if exist or null.</returns>
        public IAutoBindeable GetParent<T>()
        {
            var type = typeof (T);
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

            if (UseOptionGroup) li.Attributes.Add(OptionGroupTag, group.OptionGroupDescription);

            const String backgroundRepeat = "background-repeat: no-repeat;padding-left: 20px;";

            var imageUrl = !String.IsNullOrEmpty(group.ImageUrl) ? String.Format("background-image: url({0}); background-position: left center;", ResolveUrl(group.ImageUrl)) : String.Empty;
            var backgroundColor = !String.IsNullOrEmpty(group.BackgroundColor) ? String.Format("background-color: {0};", group.BackgroundColor) : String.Empty;
            var color = !String.IsNullOrEmpty(group.Color) ? String.Format("color: {0};", group.Color) : String.Empty;

            var style = String.Concat(backgroundRepeat, imageUrl, backgroundColor, color);

            if (!String.IsNullOrEmpty(color) || !String.IsNullOrEmpty(backgroundColor) || !String.IsNullOrEmpty(imageUrl)) li.Attributes.Add("style", style);

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

            if (UseOptionGroup) li.Attributes.Add(OptionGroupTag, group.OptionGroupDescription);

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
        /// Inverts selection of items.
        /// </summary>
        public void ToogleItems()
        {
            foreach (ListItem item in Items) item.Selected = !item.Selected;

            if (AutoPostBack) OnSelectedIndexChanged(EventArgs.Empty);
        }

        public void SelectAll()
        {
            foreach (ListItem item in Items) item.Selected = true;

            if (AutoPostBack) OnSelectedIndexChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Selects all the items with the givenn values.
        /// </summary>
        /// <param name="values"></param>
        public void SetSelectedIndexes(IEnumerable<int> values)
        {
            foreach (ListItem item in Items)
            {
                var valor = item.Value.Contains("T-") || item.Value.Contains("V-")
                                ? Convert.ToInt32(item.Value.Split('-')[1])
                                : Convert.ToInt32(item.Value);

                item.Selected = values.Contains(valor);
            }

            OnSelectedIndexChanged(EventArgs.Empty);
        }
        public void SetSelectedValues(IEnumerable<string> values)
        {
            foreach (ListItem item in Items)
            {
                var valor = item.Value.Contains("T-") || item.Value.Contains("V-")
                                ? item.Value.Split('-')[1]
                                : item.Value;

                item.Selected = values.Contains(valor);
            }

            OnSelectedIndexChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Custom data bidning.
        /// </summary>
        public override void DataBind()
        {
            Bind();

            SetSelectedValues();
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

            AddSelectedValuesToSession();
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

            GetInitialEditValues();

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
        private void LbSelectedIndexChanged(object sender, EventArgs e) { Bind(); }

        /// <summary>
        /// Adds the selected value to session.
        /// </summary>
        private void AddSelectedValuesToSession() { if (RememberSessionValues) Page.Session.Add(GetType().Name, SelectedValues); }

        /// <summary>
        /// Binds data, selects edited index and tells its child controls to rebind.
        /// </summary>
        private void DoBind()
        {
            Bind();

            SetSelectedValues();

            OnSelectedIndexChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Sets initial edited or selected index.
        /// </summary>
        private void SetSelectedValues()
        {
            if (EditValues[0].Equals(NullValue) && RememberSessionValues) GetSessionValues();

            SetSelectedIndexes(EditValues);

            EditValues = new List<int> {NullValue};
        }

        /// <summary>
        /// Gets the selected value for the control from session.
        /// </summary>
        private void GetSessionValues() { if (Page.Session[GetType().Name] != null) EditValues = (List<int>)Page.Session[GetType().Name]; }

        /// <summary>
        /// Gets the drop down initial edit value to be selected.
        /// </summary>
        private void GetInitialEditValues() { if (InitialBinding != null) InitialBinding(this, EventArgs.Empty); }

        /// <summary>
        /// Add Parent to Parents Dictionary DropDownList.
        /// </summary>
        /// <param name="ddl"></param>
        private void AddParent(IAutoBindeable ddl)
        {
            // Force postback only on parent controls. If the parent is a drop down list this is redundant, but it is usefull
            // when the parent is another list box. If any session functionality is required this setting should be treated as
            // in drop down list base class.
            ddl.AutoPostBack = true;

            _parents.Add(ddl.Type, ddl);

            ddl.SelectedIndexChanged += LbSelectedIndexChanged;
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
            if (!string.IsNullOrEmpty(FilterControl))
            {
                _filter = FindParent(FilterControl, Page.Controls) as TextBox;
                if (_filter != null)
                {
                    _filter.TextChanged += LbSelectedIndexChanged;
                    _filter.AutoPostBack = true;
                }
            }

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

        #region Option Group
        public const string OptionGroupTag = "optgroup";
        private const string OptionTag = "option";
        protected void RenderListItem(ListItem item, HtmlTextWriter writer)
        {
            if(item.Selected) writer.AddAttribute("selected", "selected");
            if (!string.IsNullOrEmpty(item.Value)) writer.AddAttribute("value", item.Value);
            foreach (string key in item.Attributes.Keys)
                if (key != OptionGroupTag) writer.AddAttribute(key, item.Attributes[key]);
            writer.RenderBeginTag(OptionTag);
            writer.Write(item.Text);
            writer.RenderEndTag();
        }
        protected void RenderOptionGroupBeginTag(string optionGroupName, HtmlTextWriter writer)
        {
            writer.AddAttribute("label", optionGroupName);
            writer.RenderBeginTag(OptionGroupTag);
        }
        protected void RenderOptionGroupEndTag(HtmlTextWriter writer)
        {
            writer.RenderEndTag();
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            var renderedOptionGroups = new List<String>();

            foreach (ListItem item in Items)
            {
                Page.ClientScript.RegisterForEventValidation(UniqueID, item.Value);

                if (item.Attributes[OptionGroupTag] == null) RenderListItem(item, writer);
                else
                {
                    var currentOptionGroup = item.Attributes[OptionGroupTag];

                    if (renderedOptionGroups.Contains(currentOptionGroup)) RenderListItem(item, writer);
                    else
                    {
                        if (renderedOptionGroups.Count > 0) RenderOptionGroupEndTag(writer);

                        RenderOptionGroupBeginTag(currentOptionGroup, writer);

                        renderedOptionGroups.Add(currentOptionGroup);

                        RenderListItem(item, writer);
                    }
                }
            }
            

            if (renderedOptionGroups.Count > 0) RenderOptionGroupEndTag(writer);
            /////////////////////////////////////////////
            //var count = Items.Count;
            //string tag;
            //string optgroupLabel;
            //if (count > 0)
            //{
            //    bool flag = false;
            //    for (int i = 0; i < count; i++)
            //    {
            //        tag = OptionTag;
            //        optgroupLabel = null;
            //        ListItem item = Items[i];
            //        if (item.Enabled)
            //        {
            //            if (item.Attributes != null && item.Attributes.Count > 0 && item.Attributes[OptionGroupTag] != null)
            //            {
            //                tag = OptionGroupTag;
            //                optgroupLabel = item.Attributes[OptionGroupTag];
            //            }
            //            writer.WriteBeginTag(tag);
            //            // NOTE(cboivin): Is optionGroup
            //            if (!string.IsNullOrEmpty(optgroupLabel))
            //            {
            //                writer.WriteAttribute("label", optgroupLabel);
            //            }
            //            else
            //            {
            //                if (item.Selected)
            //                {
            //                    if (flag)
            //                    {
            //                        this.VerifyMultiSelect();
            //                    }
            //                    flag = true;
            //                    writer.WriteAttribute("selected", "selected");
            //                }
            //                writer.WriteAttribute("value", item.Value, true);
            //                if (item.Attributes != null && item.Attributes.Count > 0)
            //                {
            //                    item.Attributes.Render(writer);
            //                }
            //                if (this.Page != null)
            //                {
            //                    this.Page.ClientScript.RegisterForEventValidation(this.UniqueID, item.Value);
            //                }
            //            }
            //            writer.Write('>');
            //            HttpUtility.HtmlEncode(item.Text, writer);
            //            writer.WriteEndTag(tag);
            //            writer.WriteLine();
            //        }
            //    }
            //}
        }

        protected override object SaveViewState()
        {
            object[] state = new object[this.Items.Count + 1];
            object baseState = base.SaveViewState();
            state[0] = baseState;
            bool itemHasAttributes = false;

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Attributes.Count > 0)
                {
                    itemHasAttributes = true;
                    object[] attributes = new object[this.Items[i].Attributes.Count * 2];
                    int k = 0;

                    foreach (string key in this.Items[i].Attributes.Keys)
                    {
                        attributes[k] = key;
                        k++;
                        attributes[k] = this.Items[i].Attributes[key];
                        k++;
                    }
                    state[i + 1] = attributes;
                }
            }

            if (itemHasAttributes)
                return state;
            return baseState;
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
                return;

            if (!(savedState.GetType().GetElementType() == null) &&
                (savedState.GetType().GetElementType().Equals(typeof(object))))
            {
                object[] state = (object[])savedState;
                base.LoadViewState(state[0]);

                for (int i = 1; i < state.Length; i++)
                {
                    if (state[i] != null)
                    {
                        object[] attributes = (object[])state[i];
                        for (int k = 0; k < attributes.Length; k += 2)
                        {
                            this.Items[i - 1].Attributes.Add
                                (attributes[k].ToString(), attributes[k + 1].ToString());
                        }
                    }
                }
            }
            else
            {
                base.LoadViewState(savedState);
            }
        } 
        #endregion

    }
}