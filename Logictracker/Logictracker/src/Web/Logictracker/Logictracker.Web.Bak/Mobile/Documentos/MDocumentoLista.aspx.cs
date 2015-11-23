#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Mobile.Documentos
{
    public partial class Documentos_Mobile_MDocumentoLista : ApplicationSecuredPage
    {
        #region Protected Properties

        protected override InfoLabel LblInfo { get { return null; } }

        protected string RedirectUrl { get { return "Mobile/Documentos/MDocumentoAlta.aspx?t=" + cbTipoDocumento.SelectedValue; } }

        protected InfoLabel NotFound { get { return null; } }

        protected string InitialSortExpression { get { return "Descripcion"; } }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e) { }

        protected List<Documento> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var docs = DAOFactory.DocumentoDAO.FindByTipoAndUsuario(user, cbTipoDocumento.Selected, -1, -1);
            return (from Documento d in docs select d).ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) Bind();
        }

        protected void Bind()
        {
            grid.DataSource = GetListData();
            grid.DataBind();
        }

        protected override string GetRefference() { return "DOCUMENTO_MOBILE"; }

        protected void cbTipoDocumento_SelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack) Bind(); }

        protected void cbLinea_SelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack) Bind(); }

        protected void cbEmpresa_SelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack) Bind(); }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            Session["id"] = null;

            Response.Redirect(String.Concat(ApplicationPath,RedirectUrl),false);
        }
        protected void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var doc = e.Item.DataItem as Documento;
            var control = e.Item.FindControl("btDocumento") as LinkButton;
            if (control == null || doc == null) return;
            control.CommandArgument = doc.Id.ToString();
            control.Text = doc.Codigo;
        }

        protected void btDocumento_Command(object sender, CommandEventArgs e)
        {
            Session["id"] = Convert.ToInt32(e.CommandArgument);
            Response.Redirect(String.Concat(ApplicationPath, RedirectUrl),false);
        }
        #endregion

        #region Overriden Methods

        /// <summary>
        /// Overrides to redirect to Mobile Login Page.
        /// </summary>
        protected override string NotLoguedUrl
        {
            get
            {
                return string.Format("{0}/Mobile/Default.aspx", ApplicationPath);
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            //ApplySecurity();
            Theme = "";
        }

        #endregion


    }
}