using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.SecurityObjects;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.BaseClasses.Util;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.Documentos;
using Logictracker.Web.Documentos.Helpers;
using Logictracker.Web.Documentos.Interfaces;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_DocumentList : BaseUserControl, IDocumentView, IGridded<Documento>
    {
        #region Properties

        private VsProperty<int> TipoDocumento { get { return this.CreateVsProperty("TipoDocumento", -1); } }
        private VsProperty<int> Transportista { get { return this.CreateVsProperty("Transportista", -1); } }
        private VsProperty<int> Coche { get { return this.CreateVsProperty("Coche", -1); } }
        private VsProperty<int> Empleado { get { return this.CreateVsProperty("Empleado", -1); } }
        private VsProperty<int> Equipo { get { return this.CreateVsProperty("Equipo", -1); } }

        private VsProperty<bool> EditEnabled { get { return this.CreateVsProperty("EditEnabled", true); } }

        private VsProperty<int> TipoDocumentoEdit { get { return this.CreateVsProperty("TipoDocumentoEdit", -1); } }
        private VsProperty<int> DocumentoEdit { get { return this.CreateVsProperty("DocumentoEdit", -1); } }

        protected GridUtils<Documento> GridUtils { get; set; }
        protected ScriptHelper ScriptHelper;
        protected IPresentStrategy Presenter;

        private Module _permission;
        protected Module Permission { get { return _permission ?? (_permission = WebSecurity.GetUserModuleByRef("DOCUMENTO")); } }

        public bool OnlyForVehicles
        {
            get { return cbTipoDocumento.OnlyForVehicles; }
            set { cbTipoDocumento.OnlyForVehicles = value;}
        }
        public bool OnlyForEmployees
        {
            get { return cbTipoDocumento.OnlyForEmployees; }
            set { cbTipoDocumento.OnlyForEmployees = value; }
        }
        public bool OnlyForEquipment
        {
            get { return cbTipoDocumento.OnlyForEquipment; }
            set { cbTipoDocumento.OnlyForEquipment = value; }
        }
        public bool OnlyForTransporter
        {
            get { return cbTipoDocumento.OnlyForTransporter; }
            set { cbTipoDocumento.OnlyForTransporter = value; }
        }

        #endregion

        #region Page Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Permission == null) Visible = false;

            GridUtils = new GridUtils<Documento>(grid, this);
            GridUtils.RowDataBound += GridUtilsRowDataBound;
            if (Permission == null || Permission.Edit)
            {
                GridUtils.SelectedIndexChanged += GridUtilsSelectedIndexChanged;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GridUtils.GenerateColumnIndices();

            if (multiDocumentos.ActiveViewIndex == 1)
            {
                DocumentContainer.EnableViewState = false;
                var tipoDocumento = DAOFactory.TipoDocumentoDAO.FindById(TipoDocumentoEdit.Get());
                Presenter = GetPresentStrategy(tipoDocumento);
                Presenter.CrearForm();
                DocumentContainer.EnableViewState = true;
            }

            if(!IsPostBack)
            {
                btBorrar.OnClientClick = string.Concat("return confirm('", CultureManager.GetString("SystemMessages", "CONFIRM_OPERATION"), "');");

                btNuevo.Visible = cbTipoDocumento.Visible = Permission == null || Permission.Add;

                if (OnlyForVehicles && Coche.Get() <= 0) Visible = false;
                if (OnlyForEmployees && Empleado.Get() <= 0) Visible = false;
                if (OnlyForEquipment && Equipo.Get() <= 0) Visible = false;
                if (OnlyForTransporter && Transportista.Get() <= 0) Visible = false;
            }
        } 
        #endregion

        #region Binding
        public void LoadDocumentos(int tipoDocumento, int transportista, int coche, int empleado, int equipo)
        {
            TipoDocumento.Set(tipoDocumento);
            Transportista.Set(transportista);
            Coche.Set(coche);
            Empleado.Set(empleado);
            Equipo.Set(equipo);

            BindDocumentos();
        }
        public void BindDocumentos()
        {
            GridUtils.Bind();
        }
        public void ClearDocumentos()
        {
            Data = new List<Documento>(0);
            GridUtils.Bind();
        }
        #endregion

        #region Events

        #region Grid Events

        protected void GridUtilsRowDataBound(object sender, RowEventArgs<Documento> e)
        {
            var doc = e.DataItem;
            if (doc == null) return;

            e.Event.Row.Cells[0].Text = doc.TipoDocumento.Nombre;
            e.Event.Row.Cells[1].Text = doc.Fecha.ToString("dd/MM/yyyy");
            e.Event.Row.Cells[2].Text = doc.Vencimiento.HasValue ? doc.Vencimiento.Value.ToString("dd/MM/yyyy") : "";
            e.Event.Row.Cells[3].Text = doc.Codigo;
            e.Event.Row.Cells[4].Text = doc.Descripcion;


            if (doc.Vencimiento.HasValue)
            {
                var dias = Convert.ToInt32(doc.Vencimiento.Value.Subtract(DateTime.UtcNow).TotalDays);
                if (dias < 0) e.Event.Row.BackColor = Color.Red;
                else if (dias < doc.TipoDocumento.SegundoAviso) e.Event.Row.BackColor = Color.Yellow;
                else e.Event.Row.BackColor = Color.Green;
            }
        }

        protected void GridUtilsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (grid.SelectedIndex < 0) return;
            var id = (int)grid.DataKeys[grid.SelectedIndex].Value;
            var doc = DAOFactory.DocumentoDAO.FindById(id);

            LoadDocument(doc);
        }

        #endregion


        protected void BtNuevoClick(object sender, EventArgs e) { NewDocument(DAOFactory.TipoDocumentoDAO.FindById(cbTipoDocumento.Selected)); }
        protected void BtCancelClick(object sender, EventArgs e) { CloseDocument(); }
        protected void BtGuardarClick(object sender, EventArgs e) { SaveDocument(); }
        protected void BtBorrarClick(object sender, EventArgs e) { DeleteDocument(); }

        #endregion

        #region Actions (New, Load, Save, Cancel, Delete)
        public void NewDocument(TipoDocumento tipoDocumento)
        {
            TipoDocumentoEdit.Set(tipoDocumento.Id);
            DocumentoEdit.Set(-1);

            Presenter = GetPresentStrategy(tipoDocumento);
            Presenter.CrearForm();

            SetCurrentValues(tipoDocumento);

            DisableCombos();
            multiDocumentos.SetActiveView(viewDocument);

            btBorrar.Visible = false;
        }

        public void LoadDocument(Documento documento)
        {
            Presenter = GetPresentStrategy(documento.TipoDocumento);

            Presenter.CrearForm();
            Presenter.SetValores(documento);
            DisableCombos();

            TipoDocumentoEdit.Set(documento.TipoDocumento.Id);
            DocumentoEdit.Set(documento.Id);
            multiDocumentos.SetActiveView(viewDocument);

            btBorrar.Visible = Permission == null || Permission.Delete;
        }
        private void SaveDocument()
        {
            var tipoDocumento = DAOFactory.TipoDocumentoDAO.FindById(TipoDocumentoEdit.Get());
            var documento = DocumentoEdit.Get() > 0 ? DAOFactory.DocumentoDAO.FindById(DocumentoEdit.Get()) : new Documento();

            try
            {
                var saver = GetSaverStrategy(tipoDocumento);
                saver.Save(documento);

                CloseDocument();
            }
            catch (Exception ex)
            {
                lblDocError.Text = ex.Message;
            }
        }
        private void CloseDocument()
        {
            BindDocumentos();
            multiDocumentos.SetActiveView(ViewList);
        } 

        private void DeleteDocument()
        {
            try
            {
                var documento = DAOFactory.DocumentoDAO.FindById(DocumentoEdit.Get());
                DAOFactory.DocumentoDAO.Delete(documento);
                CloseDocument();
            }
            catch (Exception ex)
            {
                lblDocError.Text = ex.Message;
            }
        }
        #endregion

        #region Document Control
        public void SetCurrentValues(TipoDocumento tipoDocumento)
        {
            ISecurable ent = null;
            if (Coche.Get() != -1) ent = DAOFactory.CocheDAO.FindById(Coche.Get());
            else if (Empleado.Get() != -1) ent = DAOFactory.EmpleadoDAO.FindById(Empleado.Get());
            else if (Equipo.Get() != -1) ent = DAOFactory.EquipoDAO.FindById(Equipo.Get());
            else if (Transportista.Get() != -1) ent = DAOFactory.TransportistaDAO.FindById(Transportista.Get());

            var doc = new Documento();
            if (ent != null)
            {
                doc.Empresa = ent.Empresa;
                doc.Linea = ent.Linea;
            }
            if (tipoDocumento.AplicarATransportista && Transportista.Get() != -1)
                doc.Transportista = DAOFactory.TransportistaDAO.FindById(Transportista.Get());
            if (tipoDocumento.AplicarAEquipo && Equipo.Get() != -1)
                doc.Equipo = DAOFactory.EquipoDAO.FindById(Equipo.Get());
            if (tipoDocumento.AplicarAEmpleado && Empleado.Get() != -1)
                doc.Empleado = DAOFactory.EmpleadoDAO.FindById(Empleado.Get());
            if (tipoDocumento.AplicarAVehiculo && Coche.Get() != -1)
                doc.Vehiculo = DAOFactory.CocheDAO.FindById(Coche.Get());

            Presenter.SetValores(doc);
        }

        public void DisableCombos()
        {
            EnableCombo(TipoDocumentoHelper.CONTROL_NAME_PARENTI07, Transportista.Get() == -1);
            EnableCombo(TipoDocumentoHelper.CONTROL_NAME_PARENTI03, Coche.Get() == -1);
            EnableCombo(TipoDocumentoHelper.CONTROL_NAME_PARENTI09, Empleado.Get() == -1);
            EnableCombo(TipoDocumentoHelper.CONTROL_NAME_PARENTI19, Equipo.Get() == -1);
            EnableCombo(TipoDocumentoHelper.CONTROL_NAME_PARENTI01, false);
            EnableCombo(TipoDocumentoHelper.CONTROL_NAME_PARENTI02, false);
        }

        private void EnableCombo(string cbName, bool enable)
        {
            var cb = Presenter.GetControlFromView(cbName) as DropDownList;
            if (cb != null) cb.Enabled = enable;
        } 
        #endregion

        #region Document Strategies
        protected IPresentStrategy GetPresentStrategy(TipoDocumento tipoDocumento)
        {
            var strategyFactory = GetStrategyFactory(tipoDocumento.Strategy);
            return strategyFactory != null
                       ? strategyFactory.GetPresentStrategy(tipoDocumento, this, DAOFactory)
                       : new GenericPresenter(tipoDocumento, this, DAOFactory);
        }

        protected ISaverStrategy GetSaverStrategy(TipoDocumento tipoDocumento)
        {
            var strategyFactory = GetStrategyFactory(tipoDocumento.Strategy);
            return strategyFactory != null
                       ? strategyFactory.GetSaverStrategy(tipoDocumento, this, DAOFactory)
                       : new GenericSaver(tipoDocumento, this, DAOFactory);
        }

        protected IStrategyFactory GetStrategyFactory(string className)
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

        public Control DocumentContainer { get { return panelContainer; } }

        public bool Enabled
        {
            get { return EditEnabled.Get(); }
            set { EditEnabled.Set(value); }
        }

        public void RegisterScript(string key, string script)
        {
            if (ScriptHelper == null) ScriptHelper = new ScriptHelper(this);
            ScriptHelper.RegisterStartupScript(key, script);
        }

        public ClientScriptManager ClientScript
        {
            get { return Page.ClientScript; }
        }

        #endregion

        #region Implementation of IGridded<DocumentoVo>

        public C1GridView Grid { get { return grid; } }

        private List<Documento> _data;
        public List<Documento> Data
        {
            get
            {
                if (Permission == null || !Permission.View) return new List<Documento>(0);
                return _data ?? (_data = DAOFactory.DocumentoDAO.FindBy(TipoDocumento.Get(), Transportista.Get(), Coche.Get(), Empleado.Get(), Equipo.Get()).Cast<Documento>().ToList());
            }
            set { _data = value; }
        }

        public string SearchString { get; set; }

        public StateBag StateBag { get { return ViewState; } }

        public int PageSize { get { return 20; } }

        public bool SelectableRows { get { return true; } }

        public bool MouseOverRowEffect { get { return true; } }

        public OutlineMode GridOutlineMode { get { return OutlineMode.None; } }

        public bool HasTotalRow { get { return false; } }

        #endregion
    }
}
