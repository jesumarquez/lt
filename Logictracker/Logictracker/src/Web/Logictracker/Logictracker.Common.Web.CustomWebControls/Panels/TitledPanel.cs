using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Web.CustomWebControls.Panels
{
    [Themeable(true)]
    public class TitledPanel : Panel
    {
        private bool HasOverflow { get { return overflowy != "none"; } }
        private string overflowx 
        {
            get
            {
                return ScrollBars == ScrollBars.Auto || ScrollBars == ScrollBars.Vertical ? "auto"
                       : ScrollBars == ScrollBars.Both || ScrollBars == ScrollBars.Horizontal ? "scroll" : "none";
            }
        }
        private string overflowy
        {
            get
            {
                return ScrollBars == ScrollBars.Auto || ScrollBars == ScrollBars.Horizontal ? "auto"
                   : ScrollBars == ScrollBars.Both || ScrollBars == ScrollBars.Vertical ? "scroll" : "none";
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Width.IsEmpty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
            if (!HasOverflow && !Height.IsEmpty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, TitleCssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(!string.IsNullOrEmpty(TitleResourceName) ? CultureManager.GetString(TitleResourceName, TitleVariableName): Title);
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, BodyCssClass);
            writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, overflowx);
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, overflowy);

            if (HasOverflow && !Height.IsEmpty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderContents(writer);

            writer.RenderEndTag();

            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderEndTag();
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            foreach (Control control in Controls)
                control.RenderControl(writer);
        }



        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public string TitleResourceName
        {
            get { return ViewState["TitleResourceName"] != null ? ViewState["TitleResourceName"].ToString() : string.Empty; }
            set { ViewState["TitleResourceName"] = value; }
        }

        /// <summary>
        /// The name of the specific variable wanted form the resource manager.
        /// </summary>
        [Category("Custom Resources")]
        public string TitleVariableName
        {
            get { return ViewState["TitleVariableName"] != null ? ViewState["TitleVariableName"].ToString() : string.Empty; }
            set { ViewState["TitleVariableName"] = value; }
        }

        public string Title
        {
            get { return ViewState["Title"] != null ? ViewState["Title"].ToString() : string.Empty; }
            set { ViewState["Title"] = value; }
        }

        public string TitleCssClass
        {
            get { return ViewState["TitleCssClass"] != null ? ViewState["TitleCssClass"].ToString() : string.Empty; }
            set { ViewState["TitleCssClass"] = value; }
        }
        public string BodyCssClass
        {
            get { return ViewState["BodyCssClass"] != null ? ViewState["BodyCssClass"].ToString() : string.Empty; }
            set { ViewState["BodyCssClass"] = value; }
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

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Visible = WebSecurity.IsSecuredAllowed(SecureRefference);
        }
    }
}
