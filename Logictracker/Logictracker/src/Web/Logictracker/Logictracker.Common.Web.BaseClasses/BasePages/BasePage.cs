using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Web.CustomWebControls.Labels;
using System.Data.SqlClient;

namespace Logictracker.Web.BaseClasses.BasePages
{
    #region Public Enums

    /// <summary>
    /// Window open options enumarator.
    /// </summary>
    public enum WindowOptions { Status, Toolbar, Location, Menubar, Directories, Resizable, Scrollbars }

    #endregion

    #region Public Classes

    /// <summary>
    /// Summary description for BasePage
    /// </summary>
    public abstract class BasePage : Page
    {
        public const string NonBreakingSpace = "&nbsp;";
        public const string BreakLine = "<br/>";

        #region Private Properties

        /// <summary>
        /// Data access factory.
        /// </summary>
        private DAOFactory _daof;

        /// <summary>
        /// Report objects data access factory.
        /// </summary>
        private ReportFactory _reportf;

        #endregion

        #region Public Properties

        /// <summary>
        /// Data access factory singleton.
        /// </summary>
        public DAOFactory DAOFactory { get { return _daof ?? (_daof = new DAOFactory()); } }

        /// <summary>
        /// Report objects data access factory singleton.
        /// </summary>
        public ReportFactory ReportFactory { get { return _reportf ?? (_reportf = new ReportFactory(DAOFactory)); } }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the page title
        /// </summary>
        protected virtual string PageTitle { get { return string.Format("{0} - {1}", ApplicationTitle, Title); } }

        /// <summary>
        /// Application root.
        /// </summary>
        protected static string ApplicationPath { get { return Config.ApplicationPath; } }

        /// <summary>
        /// Images directory.
        /// </summary>
        protected static string ImagesDir { get { return Config.Directory.ImagesDir; } }

        /// <summary>
        /// Icons directory.
        /// </summary>
        protected static string IconDir { get { return Config.Directory.IconDir; } }

        /// <summary>
        /// Application temporary directory.
        /// </summary>
        protected static string TmpDir { get { return Config.Directory.TmpDir; } }

        /// <summary>
        /// Gets the application title.
        /// </summary>
        protected static string ApplicationTitle { get { return Config.ApplicationTitle; } }

        /// <summary>
        /// Fusion Chart XML definition directory.
        /// </summary>
        protected static string FusionChartDir { get { return Config.Directory.FusionChartDir; } }

        /// <summary>
        /// Gets the google maps script key according to the current host.
        /// </summary>
        protected static string GoogleMapsKey { get { return Config.Map.GoogleMapsKey; } }

        /// <summary>
        /// Error message label.
        /// </summary>
        protected abstract InfoLabel LblInfo { get; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Displays the givenn message as a info message using info label.
        /// </summary>
        /// <param name="text"></param>
        protected void ShowInfo(string text) { DisplayInfo(text); }

        /// <summary>
        /// Generic error handling.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();

            ShowError(error);
        }

        /// <summary>
        /// Registers google analytics code.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CallGoogleAnalytics();
        }

        protected virtual IEnumerable<GaCustomVar> GetGaCustomVars
        {
            get { return new GaCustomVar[0]; }
        }
        
        protected void CallGoogleAnalytics()
        {
            if (!IsPostBack)
            {
                var customVars = GetGaCustomVars.Select(v => string.Format("_gaq.push(['_setCustomVar', {0}, '{1}', '{2}', {3}]);", v.Index, v.Name, v.Value, v.Scope));

                var script = string.Format(@"
                var _gaq = _gaq || [];
                _gaq.push(['_setAccount', '{0}']);
                _gaq.push(['_setDomainName', 'none']);
                _gaq.push(['_setAllowLinker', true]);", Config.AnalyticsAccount);
                script = customVars.Aggregate(script, (current, customVar) => current + customVar);
                script += "_gaq.push(['_trackPageview']);";
                script +=
                    @"(function() {{
                    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
                    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
                    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
                }})();";

                var genericControl = new HtmlGenericControl("script");
                genericControl.Attributes.Add("type", "text/javascript");
                genericControl.Controls.Add(new Literal { Text = script });

                Header.Controls.Add(genericControl);
            }
        }

        /// <summary>
        /// Sets the page title.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = PageTitle;

            // Favicon
            var favicon = new HtmlGenericControl("link");
            favicon.Attributes.Add("rel", "shortcut icon");
            favicon.Attributes.Add("href", ResolveUrl("~/iconoLogictracker.png"));
            Header.Controls.Add(favicon);

            // Default Style
            var link = new HtmlGenericControl("link");
            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("href", ResolveUrl("~/App_Styles/default.css"));
            Header.Controls.AddAt(0, link);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the indicated url in a new window.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="winname"></param>
        public void OpenWin(string url, string winname) { OpenWin(url, winname, 0, 0, null); }

        /// <summary>
        /// Opens the indicated url in a new window using the givenn options.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="winname"></param>
        /// <param name="heigth"></param>
        /// <param name="width"></param>
        /// <param name="options"></param>
        public void OpenWin(string url, string winname, int heigth, int width, params WindowOptions[] options)
        {
            winname = winname.Replace(' ', '_');
            var windowOptions = GetWindowOptions(heigth, width, GetWindowsOptionList(options));

            var script = string.Format("window.open('{0}','{1}'", url, winname);

            if (!string.IsNullOrEmpty(windowOptions)) script = string.Concat(script, ",", windowOptions);

            script = string.Concat(script, ");");

            if (ScriptManager.GetCurrent(this) == null) ClientScript.RegisterStartupScript(typeof(String), Guid.NewGuid().ToString(), script, true);
            else ScriptManager.RegisterStartupScript(this, typeof(string), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// Custom error handling.
        /// </summary>
        public void ShowError(Exception ex)
        {
            if (LblInfo != null) DisplayError(ex);
            else
            {
                Session["Error"] = ex;
                Session["LastVisitedPage"] = Request.Url.AbsolutePath;

                Response.Redirect(string.Concat(ApplicationPath, "Error.aspx"));
            }
        }
        /// <summary>
        /// Custom error handling.
        /// </summary>
        public void ShowError(string text)
        {
            if (LblInfo != null) DisplayError(text);
            else
            {
                Session["Error"] = new ApplicationException(text);
                Session["LastVisitedPage"] = Request.Url.AbsolutePath;

                Response.Redirect(string.Concat(ApplicationPath, "Error.aspx"));
            }
        }

        public void ClearError()
        {
            if (LblInfo != null) DisplayError("");
            else
            {
                Session["Error"] = new ApplicationException("");
                Session["LastVisitedPage"] = Request.Url.AbsolutePath;

                Response.Redirect(string.Concat(ApplicationPath, "Error.aspx"));
            }
        }

        public void ShowResourceError(string variableName, params object[] param)
        {
            ShowError(string.Format(CultureManager.GetError(variableName), param));
        }

        public void RegisterExtJsStyleSheet()
        {
            ClientScript.RegisterStartupScript(typeof (string), "extjscss",
                string.Format(
            @"var fileref = document.createElement('link');
            fileref.setAttribute('rel', 'stylesheet');
            fileref.setAttribute('type', 'text/css');
            fileref.setAttribute('href', '{0}');
            document.getElementsByTagName('head')[0].appendChild(fileref);",
            Config.Monitor.GetExtExtendersCss(this)), true);        
        }

        #endregion

        protected void EmbedSoundPlayer()
        {
            var url = string.Concat(Config.Directory.SoundsDir, "FlashSound.swf");
            string embed = string.Format(@"<object classid=""clsid:d27cdb6e-ae6d-11cf-96b8-444553540000"" codebase=""http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0"" width=""1"" height=""1"" id=""FlashSound"" align=""middle""><param name=""allowScriptAccess"" value=""always"" /><param name=""movie"" value=""{0}"" /><param name=""quality"" value=""high"" /><param name=""bgcolor"" value=""#ffffff"" /><embed id=""FlashSound2"" src=""{0}"" quality=""high"" bgcolor=""#ffffff"" width=""1"" height=""1"" name=""FlashSound"" align=""middle"" allowScriptAccess=""always"" type=""application/x-shockwave-flash"" pluginspage=""http://www.macromedia.com/go/getflashplayer"" /></object>", url);
            var lit = new Literal {Text = embed};
            Controls.Add(lit);
        }
        protected void PlaySound(string filename)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "playSound", GetSoundScript(filename), true);
        }
        protected string GetSoundScript(string filename)
        {
            return string.Format(@"if($get('FlashSound').playSound) {{$get('FlashSound').playSound('{0}');}}
                            else {{$get('FlashSound2').playSound('{0}');}}",filename);
        }

        #region Private Methods

        /// <summary>
        /// Converts option parameters into a generics list.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static ICollection<WindowOptions> GetWindowsOptionList(IEnumerable options)
        {
            if (options == null) return new List<WindowOptions>();

            var optionsList = from WindowOptions option in options select option;

            return optionsList.ToList();
        }

        /// <summary>
        /// Gets the javascript window.open options based on the givenn window options.
        /// </summary>
        /// <param name="heigth"></param>
        /// <param name="width"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string GetWindowOptions(int heigth, int width, ICollection<WindowOptions> options)
        {
            var windowOptions = string.Empty;

            if (heigth > 0) windowOptions = string.Concat(windowOptions, string.Format("height={0},", heigth));
            if (width > 0) windowOptions = string.Concat(windowOptions, string.Format("width={0},", width));

            if (!string.IsNullOrEmpty(windowOptions))
                windowOptions = string.Concat(windowOptions, string.Format("directories={0},location={1},menubar={2},resizable={3},scrollbars={4},status={5},toolbar={6},",
                    GetWindowOptionValue(options, WindowOptions.Directories), GetWindowOptionValue(options, WindowOptions.Location), GetWindowOptionValue(options, WindowOptions.Menubar),
                    GetWindowOptionValue(options, WindowOptions.Resizable), GetWindowOptionValue(options, WindowOptions.Scrollbars), GetWindowOptionValue(options, WindowOptions.Status),
                    GetWindowOptionValue(options, WindowOptions.Toolbar)));

            return string.IsNullOrEmpty(windowOptions) ? string.Empty : string.Concat("'", windowOptions.TrimEnd(','), "'");
        }

        /// <summary>
        /// Gets the javascript option value associated to the givenn windows option.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private static string GetWindowOptionValue(ICollection<WindowOptions> options, WindowOptions option) { return options.Contains(option) ? "yes" : "no"; }

        /// <summary>
        /// Displays all exception messages.
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void DisplayError(Exception ex)
        {
            var message = ex.Message;

            var sqlEx = (ex.GetType() == typeof(SqlException) ? ex
                : ex.InnerException != null && ex.InnerException.GetType() == typeof(SqlException) ? ex.InnerException
                : null) as SqlException;

            if (sqlEx != null && sqlEx.Number == -2)
            {
                // catch del timeout
                message = CultureManager.GetError("QUERY_TIMEOUT");
            }
            else
            {
                while (ex.InnerException != null)
                {
                    message = string.Concat(message, " - ", ex.InnerException.Message);

                    ex = ex.InnerException;
                }
            }
            DisplayError(message);
        }

        /// <summary>
        /// Displays all exception messages.
        /// </summary>
		/// <param name="text"></param>
        protected virtual void DisplayError(String text)
        {
            LblInfo.Mode = InfoLabelMode.ERROR;
            LblInfo.Text = text;
            LblInfo.Visible = true;
        }

        /// <summary>
        /// Displays an info message.
        /// </summary>
        /// <param name="text"></param>
        private void DisplayInfo(string text)
        {
            if (LblInfo == null) return;

            LblInfo.Mode = InfoLabelMode.INFO;
            LblInfo.Text = text;
            LblInfo.Visible = true;
        }

        #endregion
    }

    #endregion
}