using System;
using System.IO;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker
{
    public partial class Error : SessionSecuredPage
    {
        #region Protected Properties

        /// <summary>
        /// Info messages label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return null; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets page display according to the exception recieved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            SetImage();

            if (Session["Error"] == null) return;

            var exception = (Exception) Session["Error"];
            var lastPage = Session["LastVisitedPage"] != null ? (string) Session["LastVisitedPage"] : String.Empty;

            if (!NotifyError(exception, lastPage)) lblMailOK.Text = CultureManager.GetError("ERROR_MAIL_FAILED");

            PrintError(exception, lastPage);

            Session["Error"] = null;
            Session["LastVisitedPAge"] = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the error page backgroung image.
        /// </summary>
        private void SetImage()
        {
            try
            {
                var files = Directory.GetFiles(Server.MapPath("~/images"), "error_*.jpg");
                var rnd = new Random((int)DateTime.Now.Ticks);
                var img = rnd.Next(files.Length * 4);

                imgError.ImageUrl = "~/images/error_" + (img < files.Length ? img : 0) + ".jpg";
            }
            catch { imgError.ImageUrl = "~/images/error_0.jpg"; }
        }

        /// <summary>
        /// Prints the error if the User is "Administrador del Sistema".
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="lastPage"></param>
        private void PrintError(Exception exception,string lastPage)
        {
            try
            {
                if (Usuario == null || Usuario.AccessLevel < Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin) return;

                lblDisplayError.Text = GetErrorMessage(exception, lastPage);
            }
            catch (Exception e) { lblDisplayError.Text = e.Message; }
        }

        #endregion
    }
}
