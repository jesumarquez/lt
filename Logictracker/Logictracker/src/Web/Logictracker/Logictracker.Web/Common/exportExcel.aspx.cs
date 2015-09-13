#region Usings

using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Common
{
    public partial class CommonExportExcel : SessionSecuredPage
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

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName.Replace(' ','_').Trim() + ".xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1252");
            Response.Charset = "utf-8";
            Response.WriteFile(Tempfilename);
            Response.End();
        }

        protected string Tempfilename
        {
            get {
                if (Session["TMP_FILE_NAME"] != null)
                {
                    ViewState["TMP_FILE_NAME"] = Session["TMP_FILE_NAME"];

                    Session["TMP_FILE_NAME"] = null;
                }
                return (string)ViewState["TMP_FILE_NAME"] ?? String.Empty; 
            }
    
        }

        #endregion
    }
}
