#region Usings

using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Common
{
    public partial class CommonExportCsv : SessionSecuredPage
    {
        #region Protected Properties

        protected override InfoLabel LblInfo { get { return null; } }

        #endregion

        #region Private Properties

        /// <summary>
        /// File Name used for the CSV.
        /// </summary>
        private string FileName 
        {
            get 
            {

                if(Session["CSV_FILE_NAME"] != null)
                {
                    ViewState["CSV_FILE_NAME"] = Session["CSV_FILE_NAME"];

                    Session["CSV_FILE_NAME"] = null;
                }
                return (string) ViewState["CSV_FILE_NAME"] ?? String.Empty;
            }
        }

        /// <summary>
        /// File Name used for the CSV.
        /// </summary>
        private string Content
        {
            get
            {

                if (Session["CSV_EXPORT"] != null)
                {
                    ViewState["CSV_EXPORT"] = Session["CSV_EXPORT"];

                    Session["CSV_EXPORT"] = null;
                }
                return (string)ViewState["CSV_EXPORT"] ?? String.Empty;
            }
        }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName.Replace(' ','_').Trim() + ".csv");
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1252");
            Response.Charset = "utf-8";
            Response.Write(Content);
            Response.End();
        }

        #endregion
    }
}
