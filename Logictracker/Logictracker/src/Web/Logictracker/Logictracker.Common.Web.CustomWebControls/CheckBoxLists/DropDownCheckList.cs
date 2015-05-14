#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

#endregion

namespace Logictracker.Web.CustomWebControls.CheckBoxLists
{
    [ParseChildren(true, "Items"), Themeable(true)]
    public class DropDownCheckList : Control, INamingContainer
    {
        #region Private Const

        private const string StopPropagationScript = "if(window.event){if(!event)event=window.event;event.cancelBubble=true;}else if(event.stopPropagation)event.stopPropagation();";
        private const string GetLabelScript = "var l=$get('{0}');";
        private const string ResetLabelScript = "l.innerHTML = '';";
        private const string GetElementsScript = "var el=this.parentNode.getElementsByTagName('input');";
        private const string SetDoCheck = "var docheck = true;for(w in el){if(el[w].checked){docheck=false;break;}}";
        private const string IfCheckedAddToLabelScript = "if(el[w].checked){if(l.innerHTML!='')l.innerHTML+=', ';l.innerHTML+=el[w].nextSibling.innerHTML;}";
        private string SetAllStringScript { get {return string.Format("var t = 0; for(w in el)if(el[w].checked)t++; if(t==0)l.innerHTML = '{0}'; else if(t==el.length)l.innerHTML = '{1}';",Ninguno, Todos);}}
        private const string SetLabelTitleScript = "l.title = l.innerHTML;";

        #endregion

        #region Private Properties

        private CheckBoxList CheckBoxList;
        private Label Label;
        private DropDownExtender DropDownExtender;
        private Panel Panel;
        private Panel SelectAll;       


        #endregion

        #region Public Events

        [Bindable(true)]
        public event EventHandler SelectedIndexChanged; 

        #endregion

        #region Public Properties

        /// <summary>
        /// Lista de items
        /// </summary>
        [Bindable(true)]
        [Category("Settings: Value")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ListItemCollection Items { get{EnsureChildControls(); return CheckBoxList.Items;} private set{} }

        /// <summary>
        /// Texto que se muestra cuando estan todos los items seleccionados
        /// </summary>
        public string Todos
        {
            get { return (string)(ViewState["Todos"] ?? "(Todos)"); }
            set { ViewState["Todos"] = value; }
        }

        /// <summary>
        /// Texto que se muestra cuando no hay ningun item seleccionado
        /// </summary>
        public string Ninguno
        {
            get { return (string)(ViewState["Ninguno"] ?? "(Ninguno)"); }
            set { ViewState["Ninguno"] = value; }
        }

        /// <summary>
        /// Texto del boton Seleccionar Todos
        /// </summary>
        public string SelectAllText
        {
            get { return (string)(ViewState["SelectAllText"] ?? "Todos"); }
            set { ViewState["SelectAllText"] = value; }
        }

        /// <summary>
        /// Ancho del control
        /// </summary>
        public Unit Width
        {
            get { return (Unit)(ViewState["Width"] ?? new Unit()); }
            set { ViewState["Width"] = value; }
        }

        /// <summary>
        /// Altura del control
        /// </summary>
        public Unit Height
        {
            get { return (Unit)(ViewState["Height"] ?? new Unit("17px")); }
            set { ViewState["Height"] = value; }
        }

        /// <summary>
        /// Altura del panel dropdown. Default: 140px 
        /// </summary>
        public Unit DropDownHeight
        {
            get { return (Unit)(ViewState["DropDownHeight"] ?? new Unit("140px")); }
            set { ViewState["DropDownHeight"] = value; }
        }

        /// <summary>
        /// Autopostback
        /// </summary>
        public virtual bool AutoPostBack
        {
            get { return (bool)(ViewState["AutoPostback"] ?? false); }
            set { ViewState["AutoPostback"] = value; }
        }
        
        /// <summary>
        /// Estilo del control cerrado
        /// </summary>
        [Category("Appearance"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true)]
        public Style Style
        {
            get { return (Style)(ViewState["Style"] ?? new Style()); }
            set { ViewState["Style"] = value; }
        }
        
        /// <summary>
        /// Estilo del panel desplegable
        /// </summary>
        [Category("Appearance"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true)]
        public Style PanelStyle
        {
            get { return (Style)(ViewState["PanelStyle"] ?? new Style()); }
            set { ViewState["PanelStyle"] = value; }
        }

        /// <summary>
        /// Estilo del boton "Todos"
        /// </summary>
        [Category("Appearance"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true)]
        public Style SelectAllStyle
        {
            get { return (Style)(ViewState["SelectAllStyle"] ?? new Style()); }
            set { ViewState["SelectAllStyle"] = value; }
        }

        /// <summary>
        /// Lista de valores seleccionados
        /// </summary>
        public string[] SelectedValues
        {
            get
            {
                EnsureChildControls();
                return (from ListItem item in CheckBoxList.Items where item.Selected select item.Value).ToArray();
            }
            set
            {
                EnsureChildControls();
                foreach (ListItem li in CheckBoxList.Items)
                    li.Selected = value.Contains(li.Value);
            }
        }

        /// <summary>
        /// Lista de indices seleccionados
        /// </summary>
        public int[] SelectedIndices
        {
            get
            {
                EnsureChildControls();
                var sel = new List<int>();
                for (var i = 0; i < CheckBoxList.Items.Count; i++) if (CheckBoxList.Items[i].Selected) sel.Add(i);
                return sel.ToArray();
            }
            set
            {
                EnsureChildControls();
                foreach (var i in value) CheckBoxList.Items[i].Selected = true;
            }
        }

        #endregion

        #region Protected Properties
        protected string[] SelectedTexts
        {
            get
            {
                EnsureChildControls();
                return (from ListItem item in CheckBoxList.Items where item.Selected select item.Text).ToArray();
            }
        } 
        #endregion

        #region Constructor

        #endregion

        #region Protected Methods

        protected virtual void OnSelectedIndexChanged() { OnSelectedIndexChanged(EventArgs.Empty);}
        protected virtual void OnSelectedIndexChanged(EventArgs e) { if (SelectedIndexChanged != null) SelectedIndexChanged(this, e); }
        protected virtual void OnPagePreLoad(Object o,EventArgs e) {}

        #endregion

        #region Protected Overriden Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.PreLoad += Page_PreLoad;
        }

        protected override void CreateChildControls()
        {
            //Creo los controles internos
            Panel = CreateDropDownPanel();
            SelectAll = CreateSelectAllPanel();
            var listContainerPanel = CreateScrollPanel();
            Label = CreateLabel();
            CheckBoxList = CreateCheckBoxList();
            DropDownExtender = CreateDropDownExtender(Label.ID, Panel.ID);
            
            //Agrego los controles al padre correspondiente
            Panel.Controls.Add(SelectAll);
            Panel.Controls.Add(listContainerPanel);
            listContainerPanel.Controls.Add(CheckBoxList);
            Controls.Add(Label);
            Controls.Add(Panel);
            Controls.Add(DropDownExtender);


            // !!!
            // Los scripts no se pueden crear hasta que los controles esten asignados a la pagina con Controls.Add
            // porque no se puede calcular el ClientID


            // Script que selecciona y deselecciona todos los elementos
            // Evita que se cierre el dropdown y actualiza el label
            SelectAll.Attributes.Add("onclick", 
                string.Concat(  StopPropagationScript,
                                string.Format(GetLabelScript, Label.ClientID),
                                ResetLabelScript,
                                GetElementsScript,
                                SetDoCheck,
                                "for(w in el){if(el[w].type=='checkbox'){el[w].checked=docheck;",
                                IfCheckedAddToLabelScript,
                                "}}",
                                SetAllStringScript,
                                SetLabelTitleScript));

            // Script que actualiza el label con todos los elementos seleccionados
            // Evita que se cierre el dropdown
            CheckBoxList.Attributes.Add("onclick", 
                string.Concat(  StopPropagationScript,
                                string.Format(GetLabelScript, Label.ClientID),
                                ResetLabelScript,
                                GetElementsScript,
                                "for(w in el)",
                                IfCheckedAddToLabelScript,
                                SetAllStringScript,
                                SetLabelTitleScript));

            CheckBoxList.SelectedIndexChanged += CheckBoxSelectedIndexChanged;

       }

        protected override void OnPreRender(EventArgs e)
        {
            if (AutoPostBack)
            {
                var pb = Page.ClientScript.GetPostBackEventReference(this, "");

                // Script que hace el postback cuando se cierra el panel
                DropDownExtender.OnClientPopup =
                    string.Concat("function(elem,args){",
                                    string.Format(GetLabelScript, Label.ClientID),
                                    "if(!elem.lastSelection)elem.add_hiding(function(elem,args){if(elem.lastSelection!=l.innerHTML)",
                                    pb,
                                    ";});",
                                    "elem.lastSelection = l.innerHTML;",
                                    string.Format("var el=$get('{0}').parentNode.getElementsByTagName('input');", CheckBoxList.ClientID),
                                    SetAllStringScript,
                                    SetLabelTitleScript,
                                    "}");
            }
            Label.ApplyStyle(Style);
            Panel.ApplyStyle(PanelStyle);
            SelectAll.ApplyStyle(SelectAllStyle);
            Label.Text = SelectedTexts.Length  == 0 ? Ninguno : SelectedTexts.Length == Items.Count ? Todos : string.Join(", ", SelectedTexts);
            Label.ToolTip = Label.Text;
            base.OnPreRender(e);
        } 
        #endregion

        #region Private Methods

        private void Page_PreLoad(object sender, EventArgs e) { OnPagePreLoad(sender, e); }

        private Label CreateLabel()
        {
            var label = new Label
            {
                ID = ClientID + "_label",
                Width = Width,
                Height = Height
            };
            //label.Style.Add(HtmlTextWriterStyle.PaddingTop, "3px");
            
            label.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
            label.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
            return label;
        }

        private Panel CreateDropDownPanel()
        {
            var panel = new Panel
            {
                ID = ClientID + "_panel",
                Width = Width
            };
            
            return panel;
        }

        private Panel CreateScrollPanel()
        {
            var panelScroll = new Panel
            {
                ID = ClientID + "_panelscroll",
                Height = DropDownHeight,
                ScrollBars = ScrollBars.Auto
            };
            return panelScroll;
        }
        private Panel CreateSelectAllPanel()
        {
            var panelAll = new Panel { ID = ClientID + "_panelAll" };
            
            panelAll.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            //panelAll.Style.Add(HtmlTextWriterStyle.Padding, "3px");
            panelAll.Controls.Add(new Literal { Text = SelectAllText });

            return panelAll;
        }
        private CheckBoxList CreateCheckBoxList()
        {
            var checkbox = new CheckBoxList { ID = ClientID + "_list" };

            //foreach (ListItem item in Items) checkbox.Items.Add(item);

            return checkbox;
        }
        private DropDownExtender CreateDropDownExtender(string labelId, string panelId)
        {
            var dropDownExt = new DropDownExtender
            {
                ID = ClientID + "_ddext",
                TargetControlID = labelId,
                DropDownControlID = panelId
            };
            return dropDownExt;
        }

        private void CheckBoxSelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(); }

        #endregion

    }
}
