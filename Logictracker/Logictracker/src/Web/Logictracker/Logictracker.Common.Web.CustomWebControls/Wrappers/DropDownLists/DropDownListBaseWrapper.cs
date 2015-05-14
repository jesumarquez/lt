using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.SecurityObjects;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Web.CustomWebControls.Binding;

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists
{
    /// <summary>
    /// Wrrapper for decorating .net standart DropDownLists with custom DropDownLists behaivour.
    /// </summary>
    public class DropDownListBaseWrapper<T> : IAutoBindeable
    {
        #region Protected Properties

        /// <summary>
        /// .net DropDownList to be decorated.
        /// </summary>
        private readonly DropDownList _ddl;

        /// <summary>
        /// Lists of parents for the current DropDownList.
        /// </summary>
        private readonly Dictionary<Type, IAutoBindeable> _parents = new Dictionary<Type, IAutoBindeable>();

        /// <summary>
        /// Class for handling data binding.
        /// </summary>
        private BindingManager _bindingManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Decorates the givenn .net DropDownList with custom behaivour.
        /// </summary>
        /// <param name="dropDownList"></param>
        public DropDownListBaseWrapper(DropDownList dropDownList) : this(dropDownList, false) {}

        /// <summary>
        /// Decorates the givenn .net DropDownList with custom behaivour. Also add the "all item" option.
        /// </summary>
        /// <param name="dropDownList"></param>
        /// <param name="addAllItem"></param>
        public DropDownListBaseWrapper(DropDownList dropDownList, bool addAllItem)
        {
            _ddl = dropDownList;

            AddAllItem = addAllItem;

            AllValue = -1;
            NoneValue = -2;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the type associated to the DropDownList.
        /// </summary>
        public Type Type { get { return typeof(T); } }

        /// <summary>
        /// Gets or sets the autopostback behaivour for the DropDownList.
        /// </summary>
        public bool AutoPostBack
        {
            get { return _ddl.AutoPostBack; }
            set { _ddl.AutoPostBack = value; }
        }

        /// <summary>
        /// Gets the current selected value of the DropDownList.
        /// </summary>
        public int Selected { get { return _ddl == null || _ddl.SelectedIndex < 0 ? 0 : Convert.ToInt32(_ddl.SelectedValue); } }

        /// <summary>
        /// Gets a list of the current selected values.
        /// </summary>
        public List<int> SelectedValues { get { return new List<int> { Selected }; } }

        /// <summary>
        /// Gets the current user logged in.
        /// </summary>
        public UserSessionData Usuario { get { return WebSecurity.AuthenticatedUser; } }

        /// <summary>
        /// Determines if the "all item" should be binded to the DropDownList.
        /// </summary>
        public bool AddAllItem { get; set; }

        public bool AddNoneItem { get; set; }

        /// <summary>
        /// Determines if the binding should be grouped or not.
        /// </summary>
        public bool UseOptionGroup { get; set; }

        /// <summary>
        /// Determines if the bindings should considered data assigned to all parents.
        /// </summary>
        public bool FilterMode { get; set; }

        /// <summary>
        /// Class for handling data binding.
        /// </summary>
        public BindingManager BindingManager { get { return _bindingManager ?? (_bindingManager = new BindingManager()); } }

        /// <summary>
        /// Determines if the current control remembers its selected values in session.
        /// </summary>
        public bool RememberSessionValues { get { return false; } }

        public int AllValue { get; set; }

        public int NoneValue { get; set; }

        public virtual string AllItemsName { get { return CultureManager.GetControl("DDL_ALL_ITEMS"); } }

        public virtual string NoneItemsName { get { return CultureManager.GetControl("DDL_NONE"); } }


        #endregion

        #region Public Events

        /// <summary>
        /// OnSelectedIndexChanged triggered event.
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the IAutoBindeable parent control associated to the specified type.
        /// </summary>
        /// <returns></returns>
        public IAutoBindeable GetParent<T2>()
        {
            var type = typeof(T2);
            return _parents.ContainsKey(type) ? _parents[type] : _parents.Values.Select(parent => parent.GetParent<T2>()).FirstOrDefault(par => par != null);
        }

        public int ParentSelected<T2>()
        {
            var cb = GetParent<T2>();
            return cb != null ? cb.Selected : -1;
        }
        public List<int> ParentSelectedValues<T2>()
        {
            var cb = GetParent<T2>();
            return cb != null ? cb.SelectedValues : new List<int> { -1 };
        }
        public string FilterText
        {
            get { return null; }
        }

        /// <summary>
        /// Clear all DropDownList items.
        /// </summary>
        public void ClearItems() { _ddl.Items.Clear(); }

        /// <summary>
        /// Adds a new item to the DropDownList with the specified text and value.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        public void AddItem(string text, int value) { _ddl.Items.Add(new ListItem(text, value.ToString())); }

        public void AddItem(string text, string value)
        {
            _ddl.Items.Add(new ListItem(text, value));
        }

        /// <summary>
        /// Adds a new item with option group.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="group"></param>
        public void AddItem(string text, int value, OptionGroupConfiguration group)
        {
            var li = new ListItem(text, value.ToString());

            if (UseOptionGroup) li.Attributes.Add("OptionGroup", group.OptionGroupDescription);

            if (!String.IsNullOrEmpty(group.Color) || !String.IsNullOrEmpty(group.BackgroundColor) || !String.IsNullOrEmpty(group.ImageUrl))
                li.Attributes.Add("stryle", String.Concat("background-repeat: no-repeat;padding-left: 20px;",
                            !String.IsNullOrEmpty(group.ImageUrl) ? "background-image: url(" + group.ImageUrl +
                                                                "); background-position: left center;"
                                                              : String.Empty,
                                                          !String.IsNullOrEmpty(group.BackgroundColor)
                                                              ? "background-color: " + group.BackgroundColor + ";"
                                                              : String.Empty,
                                                          !String.IsNullOrEmpty(group.Color)
                                                              ? "color: " + group.Color + ";"
                                                              : String.Empty));
            _ddl.Items.Add(li);
        }

        /// <summary>
        /// Adds a new item with the specified value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text">The text of the item.</param>
        /// <param name="value">The associated value.</param>
        public void AddItemAt(Int32 index, String text, Int32 value) { _ddl.Items.Insert(index, new ListItem(text, value.ToString())); }

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
                            !String.IsNullOrEmpty(group.ImageUrl) ? "background-image: url(" + group.ImageUrl +
                                                                "); background-position: left center;"
                                                              : String.Empty,
                                                          !String.IsNullOrEmpty(group.BackgroundColor)
                                                              ? "background-color: " + group.BackgroundColor + ";"
                                                              : String.Empty,
                                                          !String.IsNullOrEmpty(group.Color)
                                                              ? "color: " + group.Color + ";"
                                                              : String.Empty));

            _ddl.Items.Insert(index, li);
        }

        /// <summary>
        /// Adds the speficied IAutoBindeable control to the list of parents.
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="parent"></param>
        public void AddParent<TParent>(IAutoBindeable parent) { _parents.Add(typeof(TParent), parent); }

        /// <summary>
        /// Adds the specified DropDownList wrapped as a DropDownListBaseWrapper to the list of parents.
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="parent"></param>
        public void AddParent<TParent>(DropDownList parent) { _parents.Add(typeof(TParent), new DropDownListBaseWrapper<TParent>(parent)); }

        /// <summary>
        /// Removes the item associated to the specified value.
        /// </summary>
        /// <param name="value"></param>
        public void RemoveItem(int value)
        {
            var item = _ddl.Items.FindByValue(value.ToString());

            if (item == null) return;

            _ddl.Items.Remove(item);
        }

        #endregion
    }
}