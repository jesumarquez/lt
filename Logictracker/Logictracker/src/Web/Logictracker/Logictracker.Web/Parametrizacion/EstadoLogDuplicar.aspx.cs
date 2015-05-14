#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_EstadoLogDuplicar : SessionSecuredPage
    {
        #region Protected Properties

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        #endregion

        #region Private Properties

        /// <summary>
        /// Messages ids to be duplicated.
        /// </summary>
        private List<int> MessagesIds
        {
            get
            {
                if (Session["MessagesIds"] != null)
                {
                    ViewState["MessagesIds"] = Session["MessagesIds"];
                    Session.Remove("MessagesIds");
                }
                return ViewState["MessagesIds"] != null ? (List<int>)ViewState["MessagesIds"] : new List<int>();
            }
        }

        /// <summary>
        /// Current origin company.
        /// </summary>
        private int CurrentCompany
        {
            get
            {
                if (Session["CurrentCompany"] != null)
                {
                    ViewState["CurrentCompany"] = (int)Session["CurrentCompany"];
                    Session.Remove("CurrentCompany");
                }
                return ViewState["CurrentCompany"] != null ? (int)ViewState["CurrentCompany"] : 0;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial page set up.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                if (CurrentCompany == 0 || MessagesIds.Count == 0) Response.Redirect("EstadoLogLista.aspx",false);
                var index = lbPlanta.Items.IndexOf(lbPlanta.Items.FindByValue(CurrentCompany.ToString()));
                if (index > -1) lbPlanta.Items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Duplicates the indicated messages for the selected companyes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDuplicate_Click(Object sender, EventArgs e)
        {
            try
            {
                var lineas = (from ListItem item in lbPlanta.Items where item.Selected select Convert.ToInt32(item.Value)).ToList();

                DAOFactory.EstadoDAO.DuplicateEstados(MessagesIds, lineas);

                Response.Redirect("EstadoLogLista.aspx", false);
            }
            catch (Exception ex) { infoLabel1.Text = ex.InnerException != null ? ex.InnerException.Message : ex.Message; }
        }

        #endregion
    }
}
