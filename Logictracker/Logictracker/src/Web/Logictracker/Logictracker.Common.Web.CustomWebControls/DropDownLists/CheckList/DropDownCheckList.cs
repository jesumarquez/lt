using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Urbetrack.Common.Web.CustomWebControls.DropDownLists.CheckList
{
    [ParseChildren(true, "Items")]
    public class DropDownCheckList : Control, INamingContainer
    {
        [Bindable(true)]
        public event EventHandler SelectedIndexChanged;

        private CheckBoxList CheckBox;
        private Label Label;
        private DropDownExtender DropDownExtender;

        public Unit Width
        {
            get { return (Unit)(ViewState["Width"]?? new Unit()); }
            set { ViewState["Width"] = value; }
        }

        public bool AutoPostback
        {
            get { return (bool)(ViewState["AutoPostback"] ?? false); }
            set { ViewState["AutoPostback"] = value; }
        }

        public DropDownCheckList()
        {
            Items = new ListItemCollection();
        }

        protected override void CreateChildControls()
        {
            Label = new Label
                        {
                            ID = ClientID + "_label",
                            Text = "",
                            Width = Width,
                            Height = new Unit("17px"),
                            BackColor = Color.White,
                            BorderStyle = BorderStyle.Solid,
                            BorderWidth = new Unit("1px"),
                            BorderColor = Color.Gray,
                        };
            Label.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
            Label.Style.Add(HtmlTextWriterStyle.PaddingTop, "3px");
            Label.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");


            var panel = new Panel
                            {
                                ID = ClientID + "_panel",
                                Width = Width,
                                BackColor = Color.White,
                                BorderStyle = BorderStyle.Solid,
                                BorderWidth = new Unit("1px"),
                                BorderColor = Color.Gray,
                                ScrollBars = ScrollBars.Auto
                            };

            var panelAll = new Panel {ID = ClientID + "_panelAll", BackColor = Color.LightGray};
            panelAll.Font.Bold = true;
            panelAll.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            panelAll.Style.Add(HtmlTextWriterStyle.Padding, "3px");
            panelAll.Controls.Add(new Literal {Text = "Todos"});

            CheckBox = new CheckBoxList {ID = ClientID + "_list"};
            
            foreach (ListItem item in Items) CheckBox.Items.Add(item);

            DropDownExtender = new DropDownExtender
                                   {
                                       ID = ClientID + "_ddext", 
                                       TargetControlID = Label.ID, 
                                       DropDownControlID = panel.ID
                                   };

            panel.Controls.Add(panelAll);
            panel.Controls.Add(CheckBox);
            Controls.Add(Label);
            Controls.Add(panel);
            Controls.Add(DropDownExtender);

            panelAll.Attributes.Add("onclick", string.Format(
                @"event.stopPropagation();
                var label = $get('{0}');
                label.innerHTML = ''; 
                var el = this.parentNode.getElementsByTagName('input'); 
                var docheck = true;
                for(w in el) {{ if(el[w].checked) {{ docheck = false; break; }} }}
                for(w in el) {{ 
                    if(el[w].type == 'checkbox') {{ 
                        el[w].checked = docheck; 
                        if(docheck) 
                        {{
                            if(label.innerHTML != '') label.innerHTML += ', ';
                            label.innerHTML += el[w].nextSibling.innerHTML;
                        }}
                    }} 
                }}", Label.ClientID));

            CheckBox.Attributes.Add("onclick", string.Format(
                @"event.stopPropagation(); 
                var label = $get('{0}');
                label.innerHTML = ''; 
                var el = this.parentNode.getElementsByTagName('input'); 
                for(w in el) 
                    if(el[w].checked)
                    {{ 
                        if(label.innerHTML != '') label.innerHTML += ', ';
                        label.innerHTML += el[w].nextSibling.innerHTML;
                    }}
                ", Label.ClientID));
            CheckBox.SelectedIndexChanged += checkBox_SelectedIndexChanged;

            if (AutoPostback)
            {
                var pb = Page.ClientScript.GetPostBackEventReference(this, "");
                DropDownExtender.OnClientPopup =
                @"function(el, args) 
                { 
                    if(!el.lastSelection)
                        el.add_hiding(function(el,args){ if(el.lastSelection != $get('" + Label.ClientID + @"').innerHTML) " +pb+@";});
                    el.lastSelection = $get('" + Label.ClientID + @"').innerHTML;
                }";
            }
        }

        void checkBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged();
        }

        protected override void OnPreRender(EventArgs e)
        {
            Label.Text = string.Join(", ", SelectedTexts);
            base.OnPreRender(e);
        }

        [Bindable(true)]
        [Category("Settings: Value")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ListItemCollection Items { get; private set; }

        public string[] SelectedValues
        {
            get
            {
                EnsureChildControls();
                return (from ListItem item in CheckBox.Items where item.Selected select item.Value).ToArray();
            }
            set
            {
                EnsureChildControls();
                foreach (ListItem li in CheckBox.Items)
                    li.Selected = value.Contains(li.Value);
            }
        }

        protected string[] SelectedTexts
        {
            get
            {
                EnsureChildControls();
                return (from ListItem item in CheckBox.Items where item.Selected select item.Text).ToArray();
            }
        }

        protected void OnSelectedIndexChanged()
        {
            if (SelectedIndexChanged == null) return;
            SelectedIndexChanged(this, EventArgs.Empty);
        }
    }
}
