#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Web.CustomWebControls.CheckBoxLists
{
    [ParseChildren(true, "Items"), Themeable(true)]
    public class CheckListBox: Control, INamingContainer
    {
        #region Private Properties

        private CheckBoxList CheckBoxList;

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
        public virtual ListItemCollection Items { get; private set; }

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
            get { return (Unit)(ViewState["Height"] ?? new Unit("140px")); }
            set { ViewState["Height"] = value; }
        }

        /// <summary>
        /// Autopostback
        /// </summary>
        public bool AutoPostback
        {
            get { return (bool)(ViewState["AutoPostback"] ?? false); }
            set { ViewState["AutoPostback"] = value; }
        }

        /// <summary>
        /// Estilo del control cerrado
        /// </summary>
        [Browsable(true),
        Category("Appearance"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true)]
        public Style Style
        {
            get { return (Style)(ViewState["Style"] ?? new Style()); }
            set { ViewState["Style"] = value; }
        }

        /// <summary>
        /// Estilo del boton "Todos"
        /// </summary>
        [Browsable(true),
        Category("Appearance"),
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

        public CheckListBox()
        {
            Items = new ListItemCollection();
        } 

        #endregion

        #region Protected Methods

        protected virtual void OnSelectedIndexChanged() { if (SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty); } 

        #endregion

        #region Protected Overriden Methods

        protected override void CreateChildControls()
        {
            //Creo los controles internos
            var panel = CreatePanel();
            var selectAllPanel = CreateSelectAllPanel();
            var listContainerPanel = CreateScrollPanel();
            CheckBoxList = CreateCheckBoxList();
            
            //Agrego los controles al padre correspondiente
            panel.Controls.Add(selectAllPanel);
            panel.Controls.Add(listContainerPanel);
            listContainerPanel.Controls.Add(CheckBoxList);
            Controls.Add(panel);

            var pb = AutoPostback ? Page.ClientScript.GetPostBackEventReference(this, ""): string.Empty;
            selectAllPanel.Attributes.Add("onclick",
                                @"var el=this.parentNode.getElementsByTagName('input');
                                var docheck = true;for(w in el){if(el[w].checked){docheck=false;break;}}
                                for(w in el)if(el[w].type=='checkbox')el[w].checked=docheck;" + pb);

            CheckBoxList.SelectedIndexChanged += CheckBoxSelectedIndexChanged;
        }
        #endregion

        #region Private Methods

        private Panel CreatePanel()
        {
            var panel = new Panel
            {
                ID = ClientID + "_panel",
                Width = Width
            };
            panel.ApplyStyle(Style);
            return panel;
        }

        private Panel CreateScrollPanel()
        {
            var panelScroll = new Panel
            {
                ID = ClientID + "_panelscroll",
                Height = Height,
                ScrollBars = ScrollBars.Auto
            };
            return panelScroll;
        }
        private Panel CreateSelectAllPanel()
        {
            var panelAll = new Panel { ID = ClientID + "_panelAll" };
            panelAll.ApplyStyle(SelectAllStyle);
            panelAll.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            //panelAll.Style.Add(HtmlTextWriterStyle.Padding, "3px");
            panelAll.Controls.Add(new Literal { Text = "Todos" });

            return panelAll;
        }
        private CheckBoxList CreateCheckBoxList()
        {
            var checkbox = new CheckBoxList { ID = ClientID + "_list", AutoPostBack = AutoPostback };

            foreach (ListItem item in Items) checkbox.Items.Add(item);

            return checkbox;
        }

        private void CheckBoxSelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(); }

        #endregion

    }
}
