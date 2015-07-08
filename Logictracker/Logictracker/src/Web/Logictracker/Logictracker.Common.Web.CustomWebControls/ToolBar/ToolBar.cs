using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;

namespace Logictracker.Web.CustomWebControls.ToolBar
{
    /// <summary>
    /// Custom action toolbar control.
    /// </summary>
    [ToolboxData("<{0}:ToolBar ID=\"ToolBar1\" runat=\"server\"></{0}:ToolBar>")]
    public class ToolBar : Panel, INamingContainer
    {
        public const string ToolBarButtonNewId = "__toolbarbutton_new";
        public const string ToolBarButtonPrintId = "__toolbarbutton_print";
        public const string ToolBarButtonDuplicateId = "__toolbarbutton_duplicate";
        public const string ToolBarButtonSaveId = "__toolbarbutton_save";
        public const string ToolBarButtonDeleteId = "__toolbarbutton_delete";
        public const string ToolBarButtonListId = "__toolbarbutton_list";
        public const string ToolBarButtonImportId = "__toolbarbutton_import";
        public const string ToolbarButtonSplitId = "__toolbarbutton_split";
        public const string ToolbarButtonRegenerateId = "__toolbarbutton_regenerate";
        public const string ToolbarButtonCsvId = "__toolbarbutton_csv";
        public const string ToolbarButtonExcelId = "__toolbarbutton_excel";
        public const string ToolBarButtonMapId = "__map";
        public const string ToolBarButtonEventId = "__event";
        public const string ToolBarButtonScheduleId = "__toolbarbutton_schedule";
        public const string ToolBarButtonSendReportId = "__toolbarbutton_sendreport";
        public const string ToolBarButtonStartAllId = "__toolbarbutton_startall";
        public const string ToolBarButtonStartId = "__toolbarbutton_start";
        public const string ToolBarButtonCancelId = "__toolbarbutton_cancel";
        public const string ToolBarButtonEditId = "__toolbarbutton_edit";

        public const string ButtonCommandNameCsv = "CSV";
        public const string ButtonCommandNameExcel = "Excel";
        public const string ButtonCommandNameNew = "New";
        public const string ButtonCommandNameDuplicate = "Duplicate";
        public const string ButtonCommandNameDelete = "Delete";
        public const string ButtonCommandNameImport = "Import";
        public const string ButtonCommandNameStartAll = "StartAll";
        public const string ButtonCommandNameSplit = "Split";
        public const string ButtonCommandNamePrint = "Print";
        public const string ButtonCommandNameSave = "Save";
        public const string ButtonCommandNameOpen = "Open";
        public const string ButtonCommandNameView = "View";
        public const string ButtonCommandNameEvent = "Event";
        public const string ButtonCommandNameSchedule = "Schedule";
        public const string ButtonCommandNameSendReport = "SendReport";
        public const string ButtonCommandNameRegenerate = "Regenerate";
        public const string ButtonCommandNameCancel = "Cancel";
        public const string ButtonCommandNameStart = "Start";
        public const string ButtonCommandNameEdit = "Edit";
        
        #region Public Properties

        public UpdatePanel UpdatePanel { get { return Parent.FindControl(string.Format("{0}__updatepanel", ID)) as UpdatePanel; } }

        /// <summary>
        /// Defines if the nice little toolbar will do postaback Sync or Async.
        /// </summary>
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        [Localizable(true)]
        public bool AsyncPostBacks
        {
            get { return (bool)(ViewState["AsyncPostBacks"] ?? true); }
            set
            {
                ViewState["AsyncPostBacks"] = value;

                if (value) MakePostbackAsync();
                else MakePostbackSync();
            }
        }
        /// <summary>
        /// Associated titlebar text for displaying as the title of the page.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get { return ((String)ViewState["Text"] ?? String.Empty); }
            set { ViewState["Text"] = value; }
        }

        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public string ResourceName
        {
            get { return ViewState["ResourceName"] != null ? ViewState["ResourceName"].ToString() : string.Empty; }
            set { ViewState["ResourceName"] = value; }
        }

        /// <summary>
        /// The name of the specific variable wanted form the resource manager.
        /// </summary>
        [Category("Custom Resources")]
        public string VariableName
        {
            get { return ViewState["VariableName"] != null ? ViewState["VariableName"].ToString() : string.Empty; }
            set { ViewState["VariableName"] = value; }
        }

        /// <summary>
        /// Css class to be applyied to the title text.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TitleCssClass
        {
            get { return ((String)ViewState["TitleCssClass"] ?? String.Empty); }
            set { ViewState["TitleCssClass"] = value; }
        }
        /// <summary>
        /// Css class to be applyied to the title text.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string IconCssClass
        {
            get { return ((String)ViewState["IconCssClass"] ?? String.Empty); }
            set { ViewState["IconCssClass"] = value; }
        }
        /// <summary>
        /// Css class to be applyied to the title text.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string BaseImageUrl
        {
            get { return ((String)ViewState["BaseImageUrl"] ?? "{0}"); }
            set { ViewState["BaseImageUrl"] = value; }
        }
        #endregion

        #region Public Events

        /// <summary>
        /// Item click command event handler.
        /// </summary>
        public event CommandEventHandler ItemCommand;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the givenn controls to the toolbar.
        /// </summary>
        /// <param name="childs"></param>
        public void AddControls(params Control[] childs) { foreach (var control in childs) Controls.Add(control); }

        /// <summary>
        /// Adds the new action button to the toolbar.
        /// </summary>
        public void AddNewToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonNewId, SkinID = "New", CommandName = ButtonCommandNameNew, Text = CultureManager.GetControl("BUTTON_NEW") });
        }

        /// <summary>
        /// Adds the import action button to the toolbar.
        /// </summary>
        public void AddImportToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonImportId, SkinID = "Import", CommandName = ButtonCommandNameImport, Text = CultureManager.GetControl("BUTTON_IMPORT") });
        }

        /// <summary>
        /// Adds the print action button to the toolbar.
        /// </summary>
        public void AddPrintToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonPrintId, SkinID = "Print", CommandName = ButtonCommandNamePrint, Text = CultureManager.GetControl("BUTTON_PRINT") });
        }

        /// <summary>
        /// Adds the duplicate action button to the toolbar.
        /// </summary>
        public void AddDuplicateToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonDuplicateId, SkinID = "Duplicate", CommandName = ButtonCommandNameDuplicate, Text = CultureManager.GetControl("BUTTON_DUPLICATE") });
        }

        /// <summary>
        /// Adds the save action button to the toolbar.
        /// </summary>
        public void AddSaveToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonSaveId, SkinID = "Save", CommandName = ButtonCommandNameSave, Text = CultureManager.GetControl("BUTTON_SAVE") });
        }

        /// <summary>
        /// Adds the delete action button to the toolbar.
        /// </summary>
        public void AddDeleteToolbarButton()
        {
            AddControls(new ToolBarButton
                            {
                                ID = ToolBarButtonDeleteId,
                                SkinID = "Delete",
                                CommandName = ButtonCommandNameDelete,
                                Text = CultureManager.GetControl("BUTTON_DELETE"),
                                OnClientClick = "return confirm('" + CultureManager.GetSystemMessage("CONFIRM_OPERATION") + "');"
                            });
        }

        /// <summary>
        /// Adds the open action button to the toolbar.
        /// </summary>
        public void AddListToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonListId, SkinID = "List", CommandName = ButtonCommandNameOpen, Text = CultureManager.GetControl("BUTTON_LIST") });
        }

        /// <summary>
        /// Adds the show map action button to the toolbar.
        /// </summary>
        public void AddMapToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonMapId, SkinID = "Map", CommandName = ButtonCommandNameView, Text = CultureManager.GetControl("BUTTON_MAP") });
        }

        public void AddEventToolBarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonEventId, SkinID = "Event", CommandName = ButtonCommandNameEvent, Text = CultureManager.GetControl("BUTTON_EVENT") });            
        }

        /// <summary>
        /// Adds the csv action button to the toolbar.
        /// </summary>
        public void AddCsvToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolbarButtonCsvId, SkinID = "CSV", CommandName = ButtonCommandNameCsv, Text = CultureManager.GetControl("BUTTON_CSV") });
        }

        /// <summary>
        /// Adds the csv action button to the toolbar.
        /// </summary>
        public void AddExcelToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolbarButtonExcelId, SkinID = "Excel", CommandName = ButtonCommandNameExcel, Text = CultureManager.GetControl("BUTTON_EXCEL") });
        }

        /// <summary>
        /// Adds the Split action button to the toolbar.
        /// </summary>
        public void AddSplitToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolbarButtonSplitId, SkinID = "Split", CommandName = ButtonCommandNameSplit, Text = CultureManager.GetControl("BUTTON_SPLIT"), OnClientClick = "return confirm('" + CultureManager.GetSystemMessage("TICKET_SPLIT_CONFIRM") + "');" });
        }

        /// <summary>
        /// Adds the Schedule action button to the toolbar.
        /// </summary>
        public void AddScheduleButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonScheduleId, SkinID = "Schedule", CommandName = ButtonCommandNameSchedule, Text = CultureManager.GetControl("BUTTON_SCHEDULE") });
        }

        /// <summary>
        /// Adds the SendReport action button to the toolbar.
        /// </summary>
        public void AddSendReportButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonSendReportId, SkinID = "SendReport", CommandName = ButtonCommandNameSendReport, Text = CultureManager.GetControl("BUTTON_SENDREPORT") });
        }

        /// <summary>
        /// Adds the Regenerate action button to the toolbar.
        /// </summary>
        public void AddRegenerateToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolbarButtonRegenerateId, SkinID = "Regenerate", CommandName = ButtonCommandNameRegenerate, Text = CultureManager.GetControl("BUTTON_REGENERATE"), OnClientClick = "return confirm('" + CultureManager.GetSystemMessage("TICKET_REGENERATE_CONFIRM") + "');" });
        }

        /// <summary>
        /// Adds the Cancel action button to the toolbar.
        /// </summary>
        public void AddCancelToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonCancelId, SkinID = "Cancel", CommandName = ButtonCommandNameCancel, Text = CultureManager.GetControl("BUTTON_ANULAR") });
        }
        
        /// <summary>
        /// Adds the Start All action button to the toolbar.
        /// </summary>
        public void AddStartAllToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonStartAllId, SkinID = "StartAll", CommandName = ButtonCommandNameStartAll, Text = CultureManager.GetControl("BUTTON_START_ALL_TICKETS"), OnClientClick = "return confirm('" + CultureManager.GetSystemMessage("TICKET_START_ALL_CONFIRM") + "');" });
        }

        /// <summary>
        /// Adds the Start action button to the toolbar.
        /// </summary>
        public void AddStartToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonStartId, SkinID = "Start", CommandName = ButtonCommandNameStart, Text = CultureManager.GetControl("BUTTON_START_TICKETS"), OnClientClick = "return confirm('" + CultureManager.GetSystemMessage("TICKET_START_CONFIRM") + "');" });
        }

        /// <summary>
        /// Adds a custom action button to the toolbar.
        /// </summary>
        /// <param name="id">The id of the custom button.</param>
        /// <param name="skin">The skin id to be assigned to the button.</param>
        /// <param name="text">The text of the button.</param>
        /// <param name="commandName">The associated button command name.</param>
        public void AddCustomToolbarButton(string id, string skin, string text, string commandName)
        {
            AddControls(new ToolBarButton { ID = id, SkinID = skin, Text = text, CommandName = commandName });
        }

        /// <summary>
        /// Adds a custom action button to the toolbar.
        /// </summary>
        /// <param name="id">The id of the custom button.</param>
        /// <param name="skin">The skin id to be assigned to the button.</param>
        /// <param name="text">The text of the button.</param>
        /// <param name="commandName">The associated button command name.</param>
        /// <param name="onClienClickString"></param>
        public void AddCustomToolbarButton(string id, string skin, string text, string commandName, string onClienClickString)
        {
            AddControls(new ToolBarButton { ID = id, SkinID = skin, Text = text, CommandName = commandName, OnClientClick = onClienClickString });
        }

        /// <summary>
        /// Adds a edit button to the toolbar.
        /// </summary>
        public void AddEditToolbarButton()
        {
            AddControls(new ToolBarButton { ID = ToolBarButtonEditId, SkinID = "Edit", CommandName = ButtonCommandNameEdit, Text = CultureManager.GetControl("BUTTON_EDIT") });
        }

        public void RemoveButton(string id)
        {
            var ctl = FindControl(id);
            if (ctl == null) return;
            Controls.Remove(ctl);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Render the content of all associated controls.
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            output.Write(string.Format("<div class='{0}'>", CssClass));
            output.Write("<table style='width: 100%;'><tr>");

            if (Controls.Count <= 0) output.Write("<td style='text-align: right;'>");
            else
            {
                output.Write("<td><table><tr>");
                foreach (Control control in Controls)
                {
                    output.Write("<td>");

                    control.RenderControl(output);

                    output.Write("</td>");
                }
                output.Write("</tr></table></td><td style='text-align: right;'>");
            }

            var textTitle = !string.IsNullOrEmpty(Text);
            var resourceTitle = !string.IsNullOrEmpty(ResourceName) && !string.IsNullOrEmpty(VariableName);

            if (resourceTitle || textTitle)
            {
                Image i = null;
                if (resourceTitle)
                {
                    i = new Image { ImageUrl = ResolveUrl(string.Format(BaseImageUrl, VariableName)), CssClass = IconCssClass };
                    AddControls(i);
                }
                var c = new Label { Text = textTitle ? Text : CultureManager.GetString(ResourceName, VariableName).Trim(), CssClass = TitleCssClass };
                AddControls(c);
                c.RenderControl(output);
                if (i != null) i.RenderControl(output);
            }
            output.Write("</td></tr></table></div>");
        }

        /// <summary>
        /// Triggers the item command event handler if any is associated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnItemCommand(object sender, CommandEventArgs e) { if (ItemCommand != null) ItemCommand(sender, e); }

        protected override void AddedControl(Control control, int index)
        {
            if (!(control is IButtonControl)) return;
            base.AddedControl(control, index);
            SetCommandEvent(control);
            if (Controls.Count == 1 && AsyncPostBacks) AsyncPostBacks = true;
        }

        /// <summary>
        /// Makes the toolbar behave as a sync post back control.
        /// </summary>
        protected void MakePostbackAsync()
        {
            if (Parent == null) return;

            var upd = Parent.FindControl(string.Format("{0}__updatepanel", ID)) as UpdatePanel;

            if (upd == null)
            {
                upd = new UpdatePanel { ID = string.Format("{0}__updatepanel", ID), UpdateMode = UpdatePanelUpdateMode.Conditional };
                Parent.Controls.Add(upd);
            }

            upd.Triggers.Clear();

            var trg = new AsyncPostBackTrigger { ControlID = ID, EventName = "ItemCommand" };

            upd.Triggers.Add(trg);
        }

        /// <summary>
        /// Makes the toolbar behaive as a async post back control.
        /// </summary>
        protected void MakePostbackSync()
        {
            if (Parent == null) return;

            var upd = Parent.FindControl(string.Format("{0}__updatepanel", ID)) as UpdatePanel;

            if (upd == null)
            {
                upd = new UpdatePanel { ID = string.Format("{0}__updatepanel", ID), UpdateMode = UpdatePanelUpdateMode.Conditional };
                Parent.Controls.Add(upd);
            }

            upd.Triggers.Clear();
            var trg = new PostBackTrigger { ControlID = ID };
            upd.Triggers.Add(trg);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the button command event as the handler of the comand event.
        /// </summary>
        /// <param name="control"></param>
        private void SetCommandEvent(Control control)
        {
            var button = control as IButtonControl;

            if (button == null) return;

            button.Command += ButtonCommand;
        }

        /// <summary>
        /// Triggers the on item comand event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCommand(object sender, CommandEventArgs e) { OnItemCommand(sender, e); }

        #endregion

        public void Update()
        {
            GetUpdatePanel().Update();
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.PreLoad += PagePreLoad;
        }

        void PagePreLoad(object sender, EventArgs e)
        {
            EncloseInUpdatePanel();
        }
        protected void EncloseInUpdatePanel()
        {
            var upd = GetUpdatePanel();
            upd.ContentTemplateContainer.Controls.Add(this);
        }
        protected UpdatePanel GetUpdatePanel()
        {
            if (Parent != null)
            {
                var upd = Parent.FindControl(string.Format("{0}__updatepanel__container", ID)) as UpdatePanel;
                if (upd != null) return upd;

                upd = new UpdatePanel { ID = string.Format("{0}__updatepanel__container", ID), UpdateMode = UpdatePanelUpdateMode.Conditional, ChildrenAsTriggers = false };

                var index = Parent.Controls.IndexOf(this);
                Parent.Controls.AddAt(index, upd);
                return upd;
            }
            return null;
        }
    }
}