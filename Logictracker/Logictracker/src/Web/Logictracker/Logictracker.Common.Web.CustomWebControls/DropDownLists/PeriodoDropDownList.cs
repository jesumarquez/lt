using System;
using System.Collections.Specialized;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:PeriodoDropDownList ID=\"PeriodoDropDownList1\" runat=\"server\"></{0}:PeriodoDropDownList>")]
    public class PeriodoDropDownList : DropDownListBase, IPeriodoAutoBindeable
    {
        private const string BackCommand = "back";
        private const string FwdCommand = "fwd";
        public override Type Type
        {
            get { return typeof(Periodo); }
        }

        protected override void Bind()
        {
            BindingManager.BindPeriodo(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Page.RegisterRequiresPostBack(this);
            var scriptManager = ScriptManager.GetCurrent(Page);
            if (scriptManager != null) scriptManager.RegisterAsyncPostBackControl(this);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            var pbk = Page.ClientScript.GetPostBackEventReference(this, BackCommand, true);
            var pfw = Page.ClientScript.GetPostBackEventReference(this, FwdCommand, true);
            writer.Write("<table><tr><td>");
            writer.Write("<a href=\"javascript:" + pbk + ";\">&lt;&lt;</a>");
            writer.Write("</td><td style=\"text-align: center;\">");
            writer.Write(Year.ToString());
            writer.Write("</td><td style=\"text-align: right;\">");
            writer.Write("<a href=\"javascript:" + pfw + ";\">&gt;&gt;</a>");
            writer.Write("</td></tr><tr><td colspan='3'>");
            base.Render(writer);
            writer.Write("</td></tr></table>");
        }

        //<asp:Label runat="server" ID="Label1"  Font-Bold="true" Text="Periodo"></asp:Label>
        //                    &nbsp;&nbsp;&nbsp;
        //                    <asp:LinkButton ID="btAnioDown" runat="server" CommandName="Down" OnCommand="ChangeAnio">&lt;&lt;</asp:LinkButton>                
        //                    <asp:Label runat="server" ID="lblAnioPeriodo"  Text=""></asp:Label>                   
        //                    <asp:LinkButton ID="btAnioUp" runat="server" CommandName="Up" OnCommand="ChangeAnio">&gt;&gt;</asp:LinkButton>                
        //                    <br />
        public int Year
        {
            get { return (int) (ViewState["Year"] ?? DateTime.UtcNow.Year); }
            set { ViewState["Year"] = value; }
        }

        private string _postedValue = string.Empty;
        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (postCollection["__EVENTTARGET"] != UniqueID) return false;
            _postedValue = postCollection["__EVENTARGUMENT"];
            if (_postedValue == BackCommand || _postedValue == FwdCommand) return true;
            return base.LoadPostData(postDataKey, postCollection);
        }
        protected override void RaisePostDataChangedEvent()
        {
            if (_postedValue == BackCommand)
            {
                Year--;
                Bind();
            }
            if (_postedValue == FwdCommand)
            {
                Year++;
                Bind();
            }
            OnSelectedIndexChanged(EventArgs.Empty);           
        }
    }
}
