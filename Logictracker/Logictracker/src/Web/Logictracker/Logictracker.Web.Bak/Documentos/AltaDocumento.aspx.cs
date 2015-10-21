#region Usings

using System;
using System.Web.UI;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.Documentos.Interfaces;
using Logictracker.Web.Documentos.Partes;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Documentos
{
    public partial class Documentos_AltaDocumento : SecuredAbmPage<Documento>, IDocumentView
    {
        #region Private Properties

        private IPresentStrategy presenter;
        private ISaveStrategy saver;
    
        #endregion

        #region Protected Properties

        protected int IdTipoDocumento
        {
            get
            { 
                int t;
                if (ViewState["IdTipoDocumento"] != null)
                    return Convert.ToInt32(ViewState["IdTipoDocumento"]);
                if(!int.TryParse(Request.QueryString["t"], out t)) return -1;
                return t;
            }
            set
            {
                ViewState["IdTipoDocumento"] = value;
            }
        }

        protected bool ShowSavedMessage
        {
            get
            {
                return Request.QueryString["s"] == "1";
            }
        }

        protected override string RedirectUrl { get { return "ListDocumento.aspx"; } }

        protected override string VariableName { get { return "COST_PARTES"; } }

        protected override string GetRefference() { return "PARTE"; }

        #endregion

        #region IDocumentView Members

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

        #region Protected Methods

        protected override void OnPreLoad(EventArgs e)
        {
            Response.CacheControl = "no-cache";
            if (IdTipoDocumento < 0)
                throw new ArgumentException(@"No se encontro el parametro", "Id Tipo Documento");

            var tipoDoc = DAOFactory.TipoDocumentoDAO.FindById(IdTipoDocumento);
            IdTipoDocumento = tipoDoc.Id;

            presenter = new PartePresentStrategy(tipoDoc, this, DAOFactory);
            saver = new ParteSaveStrategy(tipoDoc, this);

            presenter.CrearForm();

            if (EditMode)
            {
                presenter.SetValores(EditObject);
                if (Usuario.AccessLevel < Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin && EditObject.Valores[ParteCampos.EstadoControl].ToString() != "0") PanelForm.Enabled = false;
            }

            base.OnPreLoad(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if(ShowSavedMessage) ShowInfo("Se ha guardado con exito el parte");
        }

        protected override void Bind()
        {
        
        }

        protected override void OnDelete()
        {
            try
            {
                if (!PanelForm.Enabled) throw new ApplicationException("No se puede eliminar este parte");

                DAOFactory.DocumentoDAO.Delete(EditObject);
            }
            catch (Exception ex)
            {
                LblInfo.Text = ex.Message;

                throw;
            }
        }

        protected override void OnSave()
        {
            try
            {
                if(!PanelForm.Enabled) throw new ApplicationException("No se puede guardar este parte");
                saver.Save(EditObject, Usuario.Id, DAOFactory);
            }
            catch(Exception ex)
            {
                LblInfo.Text = ex.Message;
                STrace.Exception(GetType().FullName, ex);
                throw;
            }
        }

        protected override void AfterSave()
        {
            Response.Redirect("AltaDocumento.aspx?s=1&t=" + IdTipoDocumento,false);
        }

        #endregion
    }
}
