using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using AjaxControlToolkit;

namespace Logictracker.Web.CustomWebControls.Panels
{
    public class PopupPanel:Panel
    {
        protected ModalPopupExtender Extender;
        protected Panel Content;
        protected UpdatePanel UpdatePanel;
        public string CancelControlID
        {
            get
            {
                return (string) ViewState["CancelControlID"];
            }
            set
            {
                ViewState["CancelControlID"] = value;
            }
        }
        public string BehaviorID
        {
            get
            {
                EnsureChildControls();
                return Extender.BehaviorID;
            }
            set
            {
                EnsureChildControls(); 
                Extender.BehaviorID = value;
            }
        }

        public void Show()
        {
            EnsureChildControls();
            Extender.Show();
            UpdatePanel.Update();
        }
        public void Hide()
        {
            EnsureChildControls();
            Extender.Hide();
            UpdatePanel.Update();
        }

        private bool creatingChilds = false;
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            creatingChilds = true;
            UpdatePanel = new UpdatePanel { ID="__updPopup"+ClientID, UpdateMode = UpdatePanelUpdateMode.Conditional};

            var modal = new Panel { ID = "modelPanel" + ClientID };
            Content = new Panel { ID = "contentPanel" + ClientID, CssClass = CssClass };
            var dummy = new Panel { ID = "dummyPanel" + ClientID };
            Extender = new ModalPopupExtender
                               {
                                   TargetControlID = dummy.ID,
                                   PopupControlID = modal.ID,
                                   CancelControlID = CancelControlID,
                                   BackgroundCssClass = "disabled_back"
                               };

            modal.Controls.Add(Content);
            UpdatePanel.ContentTemplateContainer.Controls.Add(modal);
            UpdatePanel.ContentTemplateContainer.Controls.Add(dummy);
            UpdatePanel.ContentTemplateContainer.Controls.Add(Extender);

            Controls.Add(UpdatePanel);

            creatingChilds = false;
        }
        protected override void AddedControl(Control control, int index)
        {
            base.AddedControl(control, index);
            if (creatingChilds) return;
            EnsureChildControls();
            Controls.Remove(control);
            Content.Controls.Add(control);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            Content.MergeStyle(ControlStyle);
            Content.Width = Width;
            Content.Height = Height;
            Content.CssClass = CssClass;

            var tl = new HtmlGenericControl("div");
            tl.Attributes.Add("class", "PopupPanel_tl");
            Content.Controls.Add(tl);
            var tr = new HtmlGenericControl("div");
            tr.Attributes.Add("class", "PopupPanel_tr");
            Content.Controls.Add(tr);
            var bl = new HtmlGenericControl("div");
            bl.Attributes.Add("class", "PopupPanel_bl");
            Content.Controls.Add(bl);
            var br = new HtmlGenericControl("div");
            br.Attributes.Add("class", "PopupPanel_br");
            Content.Controls.Add(br);
            var l = new HtmlGenericControl("div");
            l.Attributes.Add("class", "PopupPanel_l");
            Content.Controls.Add(l);
            var r = new HtmlGenericControl("div");
            r.Attributes.Add("class", "PopupPanel_r");
            Content.Controls.Add(r);
            var t = new HtmlGenericControl("div");
            t.Attributes.Add("class", "PopupPanel_t");
            Content.Controls.Add(t);
            var b = new HtmlGenericControl("div");
            b.Attributes.Add("class", "PopupPanel_b");
            Content.Controls.Add(b);
            writer.RenderBeginTag("div");
            base.RenderContents(writer);
            writer.RenderEndTag();
        }
    }
}
