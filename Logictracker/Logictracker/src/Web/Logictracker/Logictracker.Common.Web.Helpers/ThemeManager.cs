#region Usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = System.Drawing.Image;

#endregion

namespace Logictracker.Web.Helpers
{
    /// <summary>
    /// Handles application global theaming.
    /// </summary>
    public static class ThemeManager
    {
        #region Public Methods

        /// <summary>
        /// Gets all the available themes for the application.
        /// </summary>
        /// <returns></returns>
        public static string[] GetThemes()
        {
            var themesDir = HttpContext.Current.Server.MapPath("~/App_Themes");

            var themes = Directory.GetDirectories(themesDir);

            var themeList = new List<string>();

            foreach (var theme in themes)
            {
                var dirName = theme.Substring(theme.LastIndexOf('\\') + 1);

                if (dirName.StartsWith(".")) continue;

                themeList.Add(dirName);
            }

            return themeList.ToArray();
        }

        /// <summary>
        /// Gets all available logos for the application.
        /// </summary>
        /// <returns></returns>
        public static string[] GetLogos()
        {
            var logosDir = HttpContext.Current.Server.MapPath("~/images/logos");

            var logos = new List<string>(Directory.GetFiles(logosDir, "*.gif", SearchOption.TopDirectoryOnly));

            logos.AddRange(Directory.GetFiles(logosDir, "*.jpg", SearchOption.TopDirectoryOnly));
            logos.AddRange(Directory.GetFiles(logosDir, "*.png", SearchOption.TopDirectoryOnly));

            return (from l in logos orderby l select l.Substring(l.LastIndexOf('\\') + 1)).ToArray();
        }

        /// <summary>
        /// Applyies the specified theme to the givenn page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="theme"></param>
        public static void ApplyTheme(Page page, string theme)
        {
            if (string.IsNullOrEmpty(theme)) theme = "Marinero";

            page.Theme = theme;
        }

        /// <summary>
        /// Applyies the specified logo to the givenn page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="logo"></param>
        public static void ApplyLogo(Page page, string logo)
        {
            if (string.IsNullOrEmpty(logo)) return;

            try
            {
                var file = page.ResolveUrl("~/images/logos/" + logo);
                var path = page.Server.MapPath(file);

                if (!File.Exists(path)) return;

                var lit = new Literal();

                using (var bmp = Image.FromFile(path))
                {
                    lit.Text = string.Format(
                        @"<style type='text/css'>
                        .Logo
                        {{
                            background-image: url({0});
                            background-position: center;
                            background-repeat: no-repeat;
                            min-width: {1}px;
                            height: {2}px;
                        }}
                    </style>",
                        file, bmp.Width, bmp.Height);
                }

                page.Controls.Add(lit);
            }
            catch
            {
            }
        }

        #endregion
    }
}