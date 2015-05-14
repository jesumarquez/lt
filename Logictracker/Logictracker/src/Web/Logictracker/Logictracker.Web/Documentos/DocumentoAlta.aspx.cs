#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos;
using Logictracker.Web.Documentos.Interfaces;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Documentos
{
    public partial class Documentos_DocumentoAlta : SecuredAbmPage<Documento>, IDocumentView
    {
        #region Private Properties

        private IPresentStrategy presenter;
        private ISaverStrategy saver;

        #endregion

        #region Protected Properties

        protected bool ViewOnly { get { return ViewState["ViewOnly"] != null; } }

        protected override string VariableName { get { return "DOC_DOCUMENTOS"; } }

        protected override string RedirectUrl { get { return "DocumentoLista.aspx?t=" + Request.QueryString["t"]; } }

        #endregion

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        
            ToolBar.AsyncPostBacks = false;

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
                presenter = new GenericPresenter(tipoDoc, this, DAOFactory);
                saver = new GenericSaver(tipoDoc, this, DAOFactory);    
            }
        
        }

        public IStrategyFactory GetStrategyFactory(string className)
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

        protected void Page_Load(object sender, EventArgs e)
        {
            presenter.CrearForm();
            if(!IsPostBack && !EditMode) presenter.SetDefaults();
            if(ViewOnly)
            {
                Enabled = false;
                ToolBar.Visible = false;
            }
        }

        protected override void Bind() { presenter.SetValores(EditObject); }

        protected override void OnDelete() { if (EditMode) DAOFactory.DocumentoDAO.Delete(EditObject); }

        protected override void OnSave() { saver.Save(EditObject); }

        protected override string GetRefference() { return "DOCUMENTO"; }

        #endregion

        #region Implementation of IDocumentView

        public Control DocumentContainer
        {
            get { return PanelForm; }
        }

        public void RegisterScript(string key, string script)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), key, script, true);
        }

        public bool Enabled
        {
            get { return PanelForm.Enabled; }
            set { PanelForm.Enabled = value; }
        }

        #endregion
    }
}
