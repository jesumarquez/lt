using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace Logictracker.Web.Controls
{
    public class AutoCompleteTextBox : TextBox
    {
        private HiddenField _hidId;
        private HtmlGenericControl _clearButton;
        private HtmlGenericControl _listPlacement;
        private AutoCompleteExtender _autoComplete;
        private VsProperty<string> _servicePath { get { return this.CreateVsProperty<string>("ServicePath"); } }
        private VsProperty<string> _serviceMethod { get { return this.CreateVsProperty<string>("ServiceMethod"); } }
        private VsProperty<string> _parentControls { get { return this.CreateVsProperty<string>("ParentControls"); } }

        public event EventHandler SelectedIndexChanged;

        #region Properties

        public string ParentControls
        {
            get { return _parentControls.Get(); }
            set { _parentControls.Set(value); }
        }
        public string ServicePath
        {
            get { return _servicePath.Get(); }
            set { _servicePath.Set(value); }
        }
        public string ServiceMethod
        {
            get { return _serviceMethod.Get(); }
            set { _serviceMethod.Set(value); }
        }
        public string ContextKey
        {
            get
            {
                EnsureChildControls();
                return _autoComplete.ContextKey;
            }
            set
            {
                EnsureChildControls();
                _autoComplete.ContextKey = value;
            }
        }
        public int Selected
        {
            get
            {
                EnsureChildControls(); 
                return _hidId.Value != string.Empty ? int.Parse(_hidId.Value) : 0;
            }
            set
            {
                EnsureChildControls();
                _hidId.Value = value.ToString("#0");
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.PreLoad += OnPagePreLoad;
            TextChanged += AutoCompleteTextBoxTextChanged;
            EnsureChildControls(); 
        }

        protected void OnPagePreLoad(Object o, EventArgs e)
        {
            BindParents();
        }

        private void AutoCompleteTextBoxTextChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty);
        }

        private List<Control> _parents;
        protected List<Control> Parents
        {
            get
            {
                if(string.IsNullOrEmpty(ParentControls)) return new List<Control>(0);
                return _parents ?? (_parents = ParentControls.Split(',').Select(c => !string.IsNullOrEmpty(c.Trim()) ? this.GetControlOnPage(c.Trim()) : null).ToList());
            }
        }

        protected void BindParents()
        {
            foreach(var parent in Parents)
            {
                var listControl = parent as IAutoBindeableBase;
                if(listControl != null)
                {
                    listControl.SelectedIndexChanged += ParentChanged;
                }
            }
        }

        private void ParentChanged(object sender, EventArgs e)
        {
            Clear();
            var parentValues = Parents.Select(p => p == null ? "0" : string.Join(",", ((List<int>)(p.GetType().GetProperty("SelectedValues", typeof(List<int>)).GetValue(p, null))).Select(i => i.ToString("#0")).ToArray()));
            _autoComplete.ContextKey = string.Join("|", parentValues.ToArray());
        }

        public void Clear()
        {
            EnsureChildControls();
            _hidId.Value = "0";
            Text = string.Empty;
        }

        #region ChildControls & Render

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var script = string.Format(
@"function {0}_setID(source, eventargs) {{ document.getElementById('{2}').value = eventargs.get_value(); {3};}}
function {0}_clear(){{ $get('{1}').value = ''; $get('{2}').value = '';}}
$addHandler($get('{1}'), 'keypress', function(e){{if(e.charCode==13)e.rawEvent.preventDefault();}});",
            ID, ClientID, _hidId.ClientID, (AutoPostBack ? Page.ClientScript.GetPostBackEventReference(this, "") : ""));
            this.RegisterStartupJScript(ID + "_script", script);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _clearButton = new HtmlGenericControl("div") { ID = ID + "_clear" };
            _clearButton.Style.Add("background-image", "url(" + this.GetWebResourceUrl("Logictracker.Web.Controls.Resources.clear.png") + ")");
            _clearButton.Style.Add("width", "16px");
            _clearButton.Style.Add("height", "16px");
            _clearButton.Style.Add("position", "absolute");
            _clearButton.Style.Add("right", "0px");
            _clearButton.Style.Add("top", "0px");
            _clearButton.Attributes.Add("class", "menu_search_clear");
            _clearButton.Attributes.Add("onclick", ID + @"_clear();");
            Controls.Add(_clearButton);

            _listPlacement = new HtmlGenericControl("div") { ID = ID + "_listPlacement" };
            Controls.Add(_listPlacement);

            _autoComplete = new AutoCompleteExtender
                               {
                                   Enabled = true,
                                   TargetControlID = ID,
                                   ServicePath = ServicePath,
                                   ServiceMethod = ServiceMethod,
                                   MinimumPrefixLength = 1,
                                   CompletionSetCount = 20,
                                   CompletionInterval = 200,
                                   UseContextKey = true,
                                   OnClientItemSelected = ID + "_setID",
                                   CompletionListElementID = _listPlacement.ClientID
                               };
            Controls.Add(_autoComplete);

            _hidId = new HiddenField();
            Controls.Add(_hidId);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div style='display: inline; position: relative;'>");
            base.Render(writer);
            if(Enabled) _clearButton.RenderControl(writer);
            _listPlacement.RenderControl(writer);
            _autoComplete.RenderControl(writer);
            _hidId.RenderControl(writer);
            writer.Write("</div>");
        }

        #endregion

        public static string CreateContextKey(params int[][] values)
        {
            return string.Join("|", values.Select(v => string.Join(",", v.Select(i => i.ToString("#0")).ToArray())).ToArray());
        }
    }
}
