using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Secured ABM Base Page.
    /// </summary>
    /// <typeparam name="T">The T associated to the ABM.</typeparam>
    public abstract class SecuredAbmPage<T> : ApplicationSecuredPage where T : class, IAuditable, new()
    {
        #region Private Properties

        private GenericDAO<T> _gdao;

        /// <summary>
        /// Object beeing edited.
        /// </summary>
        private T _editobj;

        /// <summary>
        /// Generic DAO Singleton Property.
        /// </summary>
        private GenericDAO<T> GDao { get { return _gdao ?? (_gdao = DAOFactory.GetGenericDAO<T>()); } }

        /// <summary>
        /// The id of the current object being edited.
        /// </summary>
        private int Id
        {
            get
            {
                if (Session["id"] != null)
                {
                    ViewState["id"] = Session["id"];
                    Session.Remove("id");
                }
                else if (GetIdFromQueryString)
                {
                    int id;
                    if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out id))
                        ViewState["id"] = id;
                }
                return ViewState["id"] != null ? Convert.ToInt32(ViewState["id"]) : 0;
            }
            set { ViewState["id"] = value; _editobj = null; }
        }

        #endregion

        #region Protected Properties

        #region MasterPage Controls

        protected BaseAbmMasterPage MasterListPage { get { return Master as BaseAbmMasterPage; } }

        protected UpdatePanel UpdatePanelToolbar { get { return ToolBar.UpdatePanel; } }

        protected virtual ToolBar ToolBar { get { return MasterListPage.ToolBar; } }

        protected override InfoLabel LblInfo { get { return MasterListPage.LblInfo; } }

        protected HtmlGenericControl LblId { get { return MasterListPage != null ? MasterListPage.LblId : null; } }

        protected void SetTabIndex(int index)
        {
            MasterListPage.SetTabIndex(index);
        }

        #endregion

        /// <summary>
        /// Determines if the page is in edit mode.
        /// </summary>
        protected bool EditMode { get { return Id > 0; } }

        protected virtual string ResourceName { get { return "Menu"; } }
        protected abstract string VariableName { get; }

        protected virtual bool AddButton { get { return true; } }
        protected virtual bool DeleteButton { get { return true;} }
        protected virtual bool DuplicateButton { get { return true; } }
        protected virtual bool SaveButton { get { return true; } }
        protected virtual bool ListButton { get { return true; } }
        protected virtual bool MapButton { get { return false; } }
        protected virtual bool EventButton { get { return false; } }
        protected virtual bool SplitButton { get { return false; } }
        protected virtual bool RegenerateButton { get { return false; } }
        protected virtual bool AnularButton { get { return false; } }

        protected virtual bool GetIdFromQueryString { get { return true; } }

        /// <summary>
        /// The list page associated to the abm.
        /// </summary>
        protected abstract string RedirectUrl { get; }

        /// <summary>
        /// The Object Being edited.
        /// </summary>
        protected T EditObject { get { return _editobj ?? (_editobj = Id > 0 ? GDao.FindById(Id) : new T()); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && EditMode)
            {
                var editOb = EditObject as IAuditable;
                if (LblId != null && editOb != null) LblId.InnerText = "Id: " + editOb.Id;

                Bind();
            }
            else if (LblId != null) LblId.InnerText = string.Empty;

            if(!IsPostBack)
            {
                var t = Request.QueryString["t"];
                int index;
                if(!string.IsNullOrEmpty(t) && int.TryParse(t, out index))
                {
                    SetTabIndex(index);
                }
            }
        }

        /// <summary>
        /// Adds action icons to the toolbar depending on user privileges.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreLoad(EventArgs e)
        {
            if (ToolBar != null)
            {
                AddToolBarIcons();

                ToolBar.ItemCommand += ToolbarItemCommand;

                ToolBar.ResourceName = ResourceName;

                ToolBar.VariableName = VariableName;
            }

            base.OnPreLoad(e);
        }

        /// <summary>
        /// Adds tooblbar icons according to user privileges.
        /// </summary>
        protected virtual void AddToolBarIcons()
        {
            var module = Module;

            ToolBar.Controls.Clear();

            if (module.Add)
            {
                if(AddButton) ToolBar.AddNewToolbarButton();

                if(DuplicateButton && EditMode) ToolBar.AddDuplicateToolbarButton();

                if (!module.Edit && SaveButton) ToolBar.AddSaveToolbarButton();
            }

            if (module.Edit && SaveButton) ToolBar.AddSaveToolbarButton();

            if (module.Delete && DeleteButton && EditMode) ToolBar.AddDeleteToolbarButton();

            if (module.Edit && AnularButton) ToolBar.AddCancelToolbarButton();

            if (ListButton) ToolBar.AddListToolbarButton();

            if (MapButton) ToolBar.AddMapToolbarButton();
            
            if (EventButton) ToolBar.AddEventToolBarButton();

            if (module.Add && SplitButton) ToolBar.AddSplitToolbarButton();

            if (module.Edit && RegenerateButton) ToolBar.AddRegenerateToolbarButton();

            ToolBar.Update();
        }

        protected virtual void UpdateToolbar()
        {
            AddToolBarIcons();
            
            if(UpdatePanelToolbar != null) UpdatePanelToolbar.Update();
        }

        /// <summary>
        /// Handles toolbar actions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ToolbarItemCommand(object sender, CommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case ToolBar.ButtonCommandNameSave: Save(); break;
                    case ToolBar.ButtonCommandNameDuplicate: Duplicate(); break;
                    case ToolBar.ButtonCommandNameOpen: Open(); break;
                    case ToolBar.ButtonCommandNameNew: New(); break;
                    case ToolBar.ButtonCommandNameDelete: Delete(); break;
                    case ToolBar.ButtonCommandNameView: ViewMap(); break;
                    case ToolBar.ButtonCommandNameEvent: ViewEvent(); break;
                    case ToolBar.ButtonCommandNameSplit: Split(); break;
                    case ToolBar.ButtonCommandNameRegenerate: Regenerate(); break;
                    case ToolBar.ButtonCommandNameCancel: Cancel(); break;
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        /// <summary>
        /// Deletes the current object being edited.
        /// </summary>
        protected void Delete()
        {
            try
            {
                ValidateDelete();

                if (EditMode) OnDelete();

                Open();
            }
            catch (Exception ex) { HandleError(ex); }
        }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        private void New()
        {
            Id = 0;
            UpdateToolbar();
            Response.Redirect(Request.Url.ToString());
        }

        /// <summary>
        /// Show the list of all existing objects of the same T.
        /// </summary>
        protected void Open() { Response.Redirect(RedirectUrl,false); }

        /// <summary>
        /// Duplicates the current object being edited.
        /// </summary>
        protected void Duplicate()
        {
            Id = 0;
            if (_editobj != null) _editobj.Id = 0;
            UpdateToolbar();
            ShowInfo(CultureManager.GetSystemMessage("DUPLICATED_ELEMENT"));

            OnDuplicate();
        }

        /// <summary>
        /// Saves any change made by the user to the current object being edited.
        /// </summary>
        protected void Save()
        {
            try
            {
                ValidateSave();

                OnSave();

                AfterSave();
            }
            catch (Exception ex) { HandleError(ex); }
        }

        /// <summary>
        /// Executed when Toolbar Map button is clicked
        /// </summary>
        protected virtual void ViewMap(){}

        protected virtual void ViewEvent() { }

        protected virtual void Split() { }

        protected virtual void Regenerate(){}

        protected virtual void Cancel() {}

        /// <summary>
        /// Bins the object being edited.
        /// </summary>
        protected abstract void Bind();

        /// <summary>
        /// Executes this method when deleting the current edited object.
        /// </summary>
        protected abstract void OnDelete();

        /// <summary>
        /// Executes this method when saving the current object.
        /// </summary>
        protected abstract void OnSave();

        /// <summary>
        /// Executes this method on saving after <see cref="OnSave"/>
        /// </summary>
        protected virtual void AfterSave() { Open(); }

        /// <summary>
        /// Validates the current edited object on pre save.
        /// </summary>
        protected virtual void ValidateSave() { }

        /// <summary>
        /// Validates the current edited object on pre delete.
        /// </summary>
        protected virtual void ValidateDelete() { }

        /// <summary>
        /// Executes this nethod wheb duplicating the selected object.
        /// </summary>
        protected virtual void OnDuplicate() { }

        protected bool CanEditEmpresa(int empresa)
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (user.Empresas.Count == 0) return true;

            var empresas = user.Empresas.OfType<Empresa>().Where(emp => emp.Id == empresa);

            return empresas.Any();
        }

        protected bool CanEditLinea(int linea)
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (user.Lineas.Count == 0) return true;

            var lineas = user.Lineas.OfType<Linea>().Where(emp => emp.Id == linea);

            return lineas.Any();
        }

        protected bool CanEditTransportista(int transportista)
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (user.Transportistas.Count == 0) return true;

            var transportistas = user.Transportistas.OfType<Transportista>().Where(emp => emp.Id == transportista);

            return transportistas.Any();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Error handling.
        /// </summary>
        /// <param name="ex"></param>
        private static void HandleError(Exception ex) { throw ex; }

        #endregion

        protected bool IsValidEmpresaLinea<TE>(TE obj, int idEmpresa, int idLinea) where TE : IHasEmpresa, IHasLinea
        {
            return IsValidEmpresa(obj, idEmpresa) && IsValidLinea(obj, idLinea);
        }
        protected bool IsValidEmpresa<TE>(TE obj, int idEmpresa) where TE:IHasEmpresa
        {
            if (obj.Empresa == null) return true;
            if (idEmpresa <= 0) return true;
            return obj.Empresa.Id == idEmpresa;
        }
        protected bool IsValidLinea<TE>(TE obj, int idLinea) where TE : IHasLinea
        {
            if (obj.Linea == null) return true;
            if (idLinea <= 0) return true;
            return obj.Linea.Id == idLinea;
        }

        #region Validation
        protected T ValidateDuplicated(T entity, string variableName)
        {
            return ValidateDuplicated(entity, "Labels", variableName);
        }
        protected T ValidateDuplicated(T entity, string resourceName, string variableName)
        {
            if (entity != null && entity.Id != EditObject.Id) ThrowDuplicated(resourceName, variableName);
            return entity;
        }
        #endregion
    }
}