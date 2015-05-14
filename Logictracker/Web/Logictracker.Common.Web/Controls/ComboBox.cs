using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using C1.Web.UI.Controls.C1ComboBox;
using System.Collections;
using Logictracker.Culture;

namespace Logictracker.Web.Controls
{
    [ToolboxData("<{0}:ComboBox ID=\"ComboBox1\" runat=\"server\"></{0}:ComboBox>")]
    public class ComboBox : C1ComboBox, IAutoBindeable3
    {
        private readonly Dictionary<AutoBindingMode, IAutoBindeable3> _parents = new Dictionary<AutoBindingMode, IAutoBindeable3>();
        public const int AllItemsValue = -1;
        public const int NoneItemsValue = -2;
        public static string AllItemsName { get { return CultureManager.GetControl("DDL_ALL_ITEMS"); } }
        public static string NoneItemsName { get { return CultureManager.GetControl("DDL_NONE"); } }

        public new event EventHandler SelectedIndexChanged;

        protected void OnSelectedIndexChanged()
        {
            if (SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty);
        }

        [Category("Custom Properties")]
        public string ParentControls
        {
            get { return ViewState["ParentControls"] == null ? string.Empty : ViewState["ParentControls"].ToString(); }
            set { ViewState["ParentControls"] = value; }
        }

        public AutoBindingMode AutoBindingMode
        {
            get { return (AutoBindingMode) (ViewState["AutoBindingMode"] ?? AutoBindingMode.None); }
            set { ViewState["AutoBindingMode"] = value; }
        }
       
        public bool AddAllItem
        {
            get { return ViewState["AddAllItem"] != null && Convert.ToBoolean(ViewState["AddAllItem"]); }
            set { ViewState["AddAllItem"] = value; }
        }

        public bool AddNoneItem
        {
            get { return ViewState["AddNoneItem"] != null && Convert.ToBoolean(ViewState["AddNoneItem"]); }
            set { ViewState["AddNoneItem"] = value; }
        }

        public int Selected 
        {
            get { return SelectedValues.Any() ? SelectedValues.First() : AllItemsValue; } 
            set { SelectedValues = new[] {value}; }
        }
       
        public IEnumerable<int> SelectedValues
        {
            get { return SelectedItems.Select(it=>Convert.ToInt32(it.Value)); }
            set { SetSelectedValues(value != null ? value.Select(v=>v.ToString()) : new string[0]); }
        }
        private IEnumerable<string> _selectedValues = new string[0];
        private void SetSelectedValues(IEnumerable<string> values)
        {
            _selectedValues = values;
            foreach (var i in Items)
            {
                i.Selected = _selectedValues.Contains(i.Value);
            }
            if (!SelectedValues.Any() && Items.Count > 0) SelectedIndex = 0;
            OnSelectedIndexChanged();
        }


        #region Public Method

        public IAutoBindeable3 GetParent(AutoBindingMode mode)
        {
            return _parents.ContainsKey(mode) ? _parents[mode] : _parents.Values.Select(parent => parent.GetParent(mode)).FirstOrDefault(par => par != null);
        }

        public IEnumerable<int> ParentSelectedValues(AutoBindingMode mode)
        {
            var cb = GetParent(mode);
            return cb != null ? cb.SelectedValues : new List<int> { -1 };
        }

        public void Clear() { Items.Clear(); }

        public void AddItem(ComboBoxItem item)
        {
            Items.Add(new C1ComboBoxItem(item.Texts, item.Value.ToString(CultureInfo.InvariantCulture)));
        }

        public override void DataBind()
        {
            base.DataBind();
            SetSelectedValues(_selectedValues);
        }
        public void AutoBind()
        {
            AutoBindingManager.Bind(this);
            SetSelectedValues(_selectedValues);
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AutoPostBack = true;
            base.SelectedIndexChanged += ComboBoxSelectedIndexChanged;
            LoadParents();
        }

        void ComboBoxSelectedIndexChanged(object sender, C1ComboBoxEventArgs args)
        {
            if (SelectionMode == C1ComboBoxSelectionMode.Multiple)
            {
                var newValues = args.NewValue as ArrayList;
                var oldValues = args.OldValue as ArrayList;
                var newVal = newValues.Count > oldValues.Count ? Convert.ToInt32(Items[newValues.Cast<int>().First(v => !oldValues.Contains(v))].Value) : new int?();

                if(!SelectedValues.Any())
                {
                    Selected = AllItemsValue;
                }
                else if (newVal == AllItemsValue)
                {
                    Selected = AllItemsValue;
                }
                else if (newVal == NoneItemsValue)
                {
                    Selected = NoneItemsValue;
                }
                else
                {
                    if(SelectedValues.Contains(AllItemsValue))
                    {
                        SelectedValues = SelectedValues.Where(v => v != AllItemsValue);
                    }
                }
            }
            OnSelectedIndexChanged();
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            base.OnPagePreLoad(sender, e);
            if(!Page.IsPostBack)
            {
                AutoBind();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(!Page.IsPostBack || !Page.IsAsync)
            {
                RegisterResources();
            }
        }

        protected void RegisterResources()
        {
            this.RegisterJsResource("#k5.#j5.resources", typeof(C1ComboBox));
            
            this.RegisterCssResource("C1.Web.UI.Controls.C1ComboBox.VisualStyles.ArcticFox.styles.css", typeof(C1ComboBox));
            this.RegisterCssResource("C1.Web.UI.Controls.C1ComboBox.VisualStyles.Office2007Black.styles.css", typeof(C1ComboBox));
            this.RegisterCssResource("C1.Web.UI.Controls.C1ComboBox.VisualStyles.Office2007Blue.styles.css", typeof(C1ComboBox));
            this.RegisterCssResource("C1.Web.UI.Controls.C1ComboBox.VisualStyles.Office2007Silver.styles.css", typeof(C1ComboBox));
            this.RegisterCssResource("C1.Web.UI.Controls.C1ComboBox.VisualStyles.Vista.styles.css", typeof(C1ComboBox));
        }

        #region Parents

        public void AddParent(IAutoBindeable3 parent)
        {
            _parents.Add(parent.AutoBindingMode, parent);
            parent.SelectedIndexChanged += OnParentSelectedIndexChanged;
        }
        private void OnParentSelectedIndexChanged(object sender, EventArgs e)
        {
            AutoBind();
        }

        private void LoadParents()
        {
            if (!string.IsNullOrEmpty(ParentControls))
            {
                foreach (var parent in ParentControls.Split(','))
                {
                    var control = FindParent(parent.Trim(), Page.Controls);
                    if (control == null) continue;
                    var autobind = control as IAutoBindeable3;
                    if (autobind != null) AddParent(autobind);
                }
            }
        }

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
