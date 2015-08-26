#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;
using Logictracker.Web.Documentos.Interfaces;
using Logictracker.Web.Documentos.Mobile;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Mobile.Documentos
{
    public partial class MDocumentos_Mobile_DocumentoAlta : SecuredAbmPage<Documento>, IDocumentView
    {
        #region Private Properties

        private IPresentStrategy presenter;
        private ISaverStrategy saver;

        #endregion

        #region Protected Properties

        protected bool ViewOnly { get { return ViewState["ViewOnly"] != null; } }

        protected override InfoLabel LblInfo { get { return null; } }

        protected override string VariableName { get { return null; } }

        protected override ToolBar ToolBar { get { return null; } }

        protected override string RedirectUrl { get { return "MDocumentoLista.aspx?t=" + Request.QueryString["t"]; } }

        #endregion

        #region Private Methods

        private static IStrategyFactory GetStrategyFactory(string className)
        {
            if (string.IsNullOrEmpty(className)) return null;
            try
            {
                var t = Type.GetType(className, true);

                if (t == null) return null;

                var constInfo = t.GetConstructor(new Type[0]);

                if (constInfo == null) return null;

                return (IStrategyFactory)constInfo.Invoke(null);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Implementation of IDocumentView

        public Control DocumentContainer
        {
            get { return PanelForm; }
        }

        public void RegisterScript(string key, string script){}

        public bool Enabled
        {
            get { return PanelForm.Enabled; }
            set { PanelForm.Enabled = value; }
        }

        #endregion

        #region Protected Methods

        protected void btnDelete_Click(object sender, EventArgs e) { Delete(); }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Save();
            }
        
            catch(Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        protected void btnList_Click(object sender, EventArgs e) { Response.Redirect(String.Concat(ApplicationPath,"Mobile/Documentos/", RedirectUrl),false); }

        /// <summary>
        /// Creates the form and blocks the edition if its only for viewing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            presenter.CrearForm();
            if (ViewOnly)
            {
                Enabled = false;
            }
        }
        /// <summary>
        /// Calls the saver in order to Save the Document.
        /// </summary>
        protected override void OnSave() { saver.Save(EditObject); }

        protected override string GetRefference() { return "DOCUMENTO_MOBILE";}

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Overrides the NotLogged URL to redirect to mobile login.
        /// </summary>
        protected override string NotLoguedUrl
        {
            get
            {
                return string.Format("{0}/Mobile/Default.aspx", ApplicationPath);
            }
        }
        /// <summary>
        /// Calls the presenter in order to show the document
        /// </summary>
        protected override void Bind() { presenter.SetValores(EditObject); }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            int tipo;

            if (!int.TryParse(Request.QueryString["t"], out tipo)) return;

            if (Session["ViewOnly"] != null)
            {
                ViewState["ViewOnly"] = Session["ViewOnly"];
                Session.Remove("ViewOnly");
            }
            var tipoDoc = DAOFactory.TipoDocumentoDAO.FindById(tipo);

            var strategyFactory = GetStrategyFactory(tipoDoc.Strategy);
            if (strategyFactory != null)
            {
                presenter = strategyFactory.GetPresentStrategy(tipoDoc, this, DAOFactory);
                saver = strategyFactory.GetSaverStrategy(tipoDoc, this, DAOFactory);
            }
            else
            {
                presenter = new MobilePresenter(tipoDoc, this, DAOFactory);
                saver = new MobileSaver(tipoDoc, this, DAOFactory);
            }

        }

        /// <summary>
        /// Calls DAOFactory in order to delete if user is in EditMode.
        /// </summary>
        protected override void OnDelete() { if (EditMode) DAOFactory.DocumentoDAO.Delete(EditObject); }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            Theme = "";
        }

        #endregion
    }
}

