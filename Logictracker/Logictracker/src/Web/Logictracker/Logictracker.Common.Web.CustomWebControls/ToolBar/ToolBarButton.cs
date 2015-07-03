using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Logictracker.Web.CustomWebControls.ToolBar
{
    /// <summary>
    /// Toolbar buttons image enum.
    /// </summary>
    public enum ToolBarButtonImages
    {
        None, 
        New, 
        Duplicate, 
        Delete, 
        Save, 
        Map, 
        Event, 
        Print, 
        List, 
        Csv, 
        Split, 
        Import, 
        Regenerate, 
        Schedule, 
        Start, 
        Anular,
        Excel,
        Edit,
        SendReport
    }

    /// <summary>
    /// Custom toolbar link button.
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ToolBarButton ID=\"ToolBarButton1\" runat=\"server\"></{0}:ToolBarButton>")]
    public class ToolBarButton : LinkButton
    {
        #region Public Properties

        /// <summary>
        /// Button image url.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ImageUrl
        {
            get { return (string)(ViewState["ImageUrl"] ?? string.Empty); }
            set { ViewState["ImageUrl"] = value; }
        }

        /// <summary>
        /// Defines image T associated to the button.
        /// </summary>
        public ToolBarButtonImages Image
        {
            get { return (ToolBarButtonImages)(ViewState["Image"] ?? ToolBarButtonImages.None); }
            set { ViewState["Image"] = value; }
        }

        /// <summary>
        /// Button background css class.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string BackCssClass
        {
            get { return (string)(ViewState["BackCssClass"] ?? String.Empty); }
            set { ViewState["BackCssClass"] = value; }
        }

        /// <summary>
        /// Button mouse over css class.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string OverCssClass
        {
            get { return (string)(ViewState["OverCssClass"] ?? string.Empty); }
            set { ViewState["OverCssClass"] = value; }
        }
        public override bool CausesValidation
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Render the control content and every child control content.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterStartupScript(typeof(string), "ToolbarOverAndOut", "var cwcToolBar = {};", true);
            Page.ClientScript.RegisterStartupScript(typeof(string), string.Format("ToolbarOverAndOut_{0}", ClientID), GetToolbarButtonScript(), true);

            writer.Write("<table cellspacing='0' cellpadding='0' style='height: 24px;'>");
            writer.Write("<tr>");
            writer.Write(string.Format("<td id='{0}_1' class='{1}' style='width:2px;background-position: left;background-repeat: no-repeat;'></td>", ClientID, BackCssClass));
            writer.Write(string.Format("<td id='{0}_2' class='{1}' style='background-position: center;background-repeat: no-repeat; padding-top:2px;padding-left: 5px; padding-right: 7px;' >", ClientID, BackCssClass));
            writer.AddAttribute("onmouseover", string.Format("cwcToolBar['{0}'].over('{0}');", ClientID));
            writer.AddAttribute("onmouseout", string.Format("cwcToolBar['{0}'].out('{0}');", ClientID));
            writer.AddStyleAttribute("text-decoration", "none");

            base.Render(writer);

            writer.Write("</td>");
            writer.Write(string.Format(string.Format("<td id='{0}_3' class='{1}' style='width:2px;background-position: right;background-repeat: no-repeat;'></td>", ClientID, BackCssClass)));
            writer.Write("</tr></table>");
        }

        /// <summary>
        /// Adds images to buttons.
        /// </summary>
        /// <param name="output"></param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(string.Format(string.Format("<img src='{0}' align='absmiddle' style='width: 16px;height: 16px; border: none;margin-right: 5px;' />", GetImageUrl(Image))));

            base.RenderContents(output);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the script associated to the toolbar custom button.
        /// </summary>
        /// <returns></returns>
        private string GetToolbarButtonScript()
        {
            return string.Format(
                @"cwcToolBar['{0}'] =
                {{
                overImage: '{1}',
                outImage: '{2}',
                changeback: function(button, image) 
                {{
                    document.getElementById(button + '_1').className = image;
                    document.getElementById(button + '_2').className = image;
                    document.getElementById(button + '_3').className = image;
                }},
                over: function(button){{this.changeback(button, this.overImage);}},
                out:  function(button){{this.changeback(button, this.outImage);}}
                }};", ClientID, OverCssClass, BackCssClass);
        }

        /// <summary>
        /// Gets the url associated to the givenn image T.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private string GetImageUrl(ToolBarButtonImages image)
        {
            string img;

            switch(image)
            {
                case ToolBarButtonImages.Delete: img = "delete.png"; break;
                case ToolBarButtonImages.Duplicate: img = "copy.png"; break;
                case ToolBarButtonImages.Map: img = "map.png"; break;
                case ToolBarButtonImages.Event: img = "event.png"; break;
                case ToolBarButtonImages.New: img = "add.png"; break;
                case ToolBarButtonImages.Print: img = "printer.png"; break;
                case ToolBarButtonImages.Save: img = "disk.png"; break;
                case ToolBarButtonImages.List: img = "list.png"; break;
                case ToolBarButtonImages.Csv: img = "csv.png"; break;
                case ToolBarButtonImages.Excel: img = "excel.png"; break;
                case ToolBarButtonImages.Import: img = "import.png"; break;
                case ToolBarButtonImages.Split: img = "spliter.png"; break;
                case ToolBarButtonImages.Regenerate: img = "regenerate.png"; break;
                case ToolBarButtonImages.Schedule: img = "schedule.png"; break;
                case ToolBarButtonImages.SendReport: img = "sendReport.png"; break;
                case ToolBarButtonImages.Start: img = "start.png"; break;
                case ToolBarButtonImages.Anular: img = "anular.png"; break;
                case ToolBarButtonImages.Edit: img = "edit.png"; break;
                default: return ResolveUrl(ImageUrl);
            }

            return Page.ClientScript.GetWebResourceUrl(GetType(), string.Format("Logictracker.Web.CustomWebControls.ToolBar.{0}", img));
        }

        #endregion
    }
}