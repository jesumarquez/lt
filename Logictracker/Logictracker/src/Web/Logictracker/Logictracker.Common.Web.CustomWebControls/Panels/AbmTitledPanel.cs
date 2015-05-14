#region Usings

using System.Linq;
using System.Web.UI;

#endregion

namespace Logictracker.Web.CustomWebControls.Panels
{
    [Themeable(true)]
    public class AbmTitledPanel : TitledPanel
    {
        public string Align
        {
            get { return (string) (ViewState["Align"] ?? "right"); }
            set { ViewState["Align"] = value; }
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            var controls = Controls.OfType<Control>().Where(c => c.GetType() != typeof(LiteralControl) || ((c as LiteralControl) != null && !string.IsNullOrEmpty((c as LiteralControl).Text.Trim()))).ToList();
                
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
            writer.AddAttribute("cellspacing", "3px");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            for (var i = 0; i < controls.Count; i+=2)
            {
                var control = controls[i];
                var control2 = controls.Count > i + 1 ? controls[i + 1] : null;

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, Align);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                control.RenderControl(writer);
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                if(control2 != null) control2.RenderControl(writer);
                writer.RenderEndTag();
                writer.RenderEndTag();

            }
            writer.RenderEndTag();

            
        }
    }
}
