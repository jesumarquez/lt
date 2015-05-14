#region Usings

using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

#endregion

namespace Logictracker.Web.CustomWebControls.Input
{
    [Themeable(true)]
    public class DynamicHelp : Control, INamingContainer
    {
        public string TargetControlID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string CssClass { get; set; }
        public string TitleCssClass { get; set; }
        public PopupControlPopupPosition Position { get; set; }
        public Unit Width { get; set; }
        protected override void CreateChildControls()
        {
            var panel = new Panel { ID = ClientID + "_panel", CssClass = CssClass, Width = Width };
            panel.Controls.Add(new Literal {Text = string.Format("<span class='{0}'>{1}</span>", TitleCssClass, Title)});
            panel.Controls.Add(new Literal { Text = string.Format("<span>{0}</span>",  Text) });
            var popup = new PopupControlExtender
                            {
                                ID = ClientID + "_popupextender",
                                TargetControlID = TargetControlID,
                                PopupControlID = panel.ID,
                                Position = Position
                            };

            Controls.Add(panel);
            Controls.Add(popup);

        }
    }
}
