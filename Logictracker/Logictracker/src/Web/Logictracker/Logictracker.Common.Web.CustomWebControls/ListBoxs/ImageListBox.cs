#region Usings

using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    [ToolboxData("<{0}:ImageListBox ID=\"ImageListBox1\" runat=server></{0}:ImageListBox>")]
    public class ImageListBox : ListBox
    {
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "scroll");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.IsEmpty ? "60px" : Height.ToString());
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.IsEmpty ? "auto" : Width.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            if(!string.IsNullOrEmpty(CssClass)) writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            else AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0");
            writer.AddStyleAttribute("border-spacing", "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_items");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            var script = Page.ClientScript.GetWebResourceUrl(GetType(), "Logictracker.Web.CustomWebControls.ListBoxs.ImageListBox.js");
            Page.ClientScript.RegisterStartupScript(typeof(string), "ImageListBoxClass",
                string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", script), false);

            //base.RenderControl(writer);
            RenderBeginTag(writer);
            for (var i = 0; i < Items.Count; i++)
            {
                RenderItem(writer, Items[i], i);
            }
            RenderEndTag(writer);


            Page.ClientScript.RegisterStartupScript(typeof(string), "ImageListBox_" + ClientID, "new ImageListBox('" + ClientID + "');", true);

        }

        protected virtual void RenderItem(HtmlTextWriter writer, ListItem item, int itemIndex)
        {
            var text = item.Text;
            var value = item.Value;
            var image = item.Attributes["ImageUrl"];

            writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "default");
            writer.RenderBeginTag("tr");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "1%");
            writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
            writer.RenderBeginTag("td");
            writer.Write(string.Format("<input type='hidden' value='{0}' id='{1}' name='{1}' />", value, ClientID + "_items_" + itemIndex));
            if(!string.IsNullOrEmpty(image)) writer.Write(string.Format("<img src='{0}' />", image));
            writer.RenderEndTag();
            writer.RenderBeginTag("td");
            writer.Write(text);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}
