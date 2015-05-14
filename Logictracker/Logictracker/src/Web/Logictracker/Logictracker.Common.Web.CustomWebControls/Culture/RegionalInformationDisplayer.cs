using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Security;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Web.CustomWebControls.Culture
{
    public class RegionalInformationDisplayer : Panel, INamingContainer
    {
        #region Protected Methods

        protected override void CreateChildControls()
        {
            Controls.Clear();
            var ctl = new OnlineClock {ID = ClientID + "_clock"};
            Controls.Add(ctl);
        }

        /// <summary>
        /// Renders information about the user regional configuration.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            var user = WebSecurity.AuthenticatedUser;

            var gmtModifier = TimeSpan.FromHours(user.GmtModifier).ToString();

            gmtModifier = gmtModifier.Substring(0, gmtModifier.LastIndexOf(':'));

            if (!gmtModifier.StartsWith("-")) gmtModifier = string.Concat("+", gmtModifier);

            if (!string.IsNullOrEmpty(CssClass)) writer.AddAttribute("class", CssClass);
            if (!string.IsNullOrEmpty(Style.Value)) writer.AddAttribute("style", Style.Value);

            writer.AddAttribute("id", string.Concat(ClientID, "_div"));
            
            writer.RenderBeginTag("div");

            var url = GetFlagUrl(user.Culture.Name);

            writer.Write(string.Format(@"
                <table cellpadding='1'>
                    <tr align='left'>
                        <td>
                            <img id='{0}' runat='server' src='{1}' />
                        </td>
                        <td>
                            (GMT{2})
                        </td>
                        <td style='width: 1%;'>", string.Concat(ClientID, "_flag_img"), url, gmtModifier));

            foreach(Control ctl in Controls)ctl.RenderControl(writer);

            writer.Write(@"</td>
                    </tr>
                </table>");

            writer.RenderEndTag();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the url of the flag associated to the givenn culture.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        private string GetFlagUrl(string culture)
        {
            var file = culture.Length > 3 ? culture.Substring(culture.Length - 2).ToLower() : culture.ToLower();

            var res = "Logictracker.Web.CustomWebControls.Culture.flags.{0}.gif";

            return Page.ClientScript.GetWebResourceUrl(GetType(), string.Format(res, file));
        }

        #endregion
    }
}
