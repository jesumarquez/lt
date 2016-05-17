using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Geocoder.Core.VO;
using Logictracker.Culture;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BaseControls;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_Consultas : BaseUserControl
    {
        public event EventHandler<DireccionEventArgs> DireccionAdded;
        public event EventHandler<DireccionEventArgs> DireccionRemoved;
        public event EventHandler<DireccionEventArgs> DireccionSelected;
        public event EventHandler Clear;
        public event EventHandler<DireccionSavedEventArgs> DireccionSaved;

        public string ParentControls
        {
            get { return (string)(ViewState["ParentControls"] ?? string.Empty); }
            set { ViewState["ParentControls"] = cbTipoGeoRef.ParentControls = value; }
        }

        private IList<DireccionVO> Direcciones
        {
            get { return (IList<DireccionVO>)(ViewState["Direcciones"] ?? new List<DireccionVO>()); }
            set { ViewState["Direcciones"] = value; }
        }

        public IList<DireccionVO> Selected
        {
            get { return (IList<DireccionVO>)(ViewState["Selected"] ?? new List<DireccionVO>()); }
            set { ViewState["Selected"] = value; }
        }

        public IList<int> SelectedId
        {
            get { return (IList<int>)(ViewState["SelectedId"] ?? new List<int>()); }
            set { ViewState["SelectedId"] = value; }
        }

        public IList<int> SavedId
        {
            get { return (IList<int>)(ViewState["SavedId"] ?? new List<int>()); }
            set { ViewState["SavedId"] = value; }
        }

        private int LastId
        {
            get { return (int)(ViewState["LastId"] ?? 0); }
            set { ViewState["LastId"] = value; }
        }

        public int AddSelected(DireccionVO direccion)
        {
            var id = LastId++;
            var sel = Selected;
            sel.Add(direccion);
            Selected = sel;
            var selId = SelectedId;
            selId.Add(id);
            SelectedId = selId;
            return id;
        }

        public void RemoveSelected(int index)
        {
            var sel = Selected;
            sel.RemoveAt(index);
            Selected = sel;
            var selId = SelectedId;
            selId.RemoveAt(index);
            SelectedId = selId;
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            var cbLinea = cbTipoGeoRef.GetParent<Linea>() as DropDownList;
            if(cbLinea != null) cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
            if (!IsPostBack) BindProvincias();
        }

        void cbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbTipoGeoRef.DataBind();
            updConsultas.Update();   
        }
        private void BindProvincias()
        {
            var provincias = GeocoderHelper.BuscarProvincias();

            cbProvincia.DataSource = provincias;
            cbProvincia.DataBind();
        }
        protected void btNormalSearch_Click(object sender, EventArgs e)
        {
            var calle = txtCalle.Text.Trim();
            int altura;

            if (!int.TryParse(txtAltura.Text.Trim(), out altura)) altura = -1;

            var esquina = txtEsquina.Text.Trim();
            var partido = txtPartido.Text.Trim();
            var provincia = Convert.ToInt32(cbProvincia.SelectedValue);

            if (string.IsNullOrEmpty(calle)) return;
            if (string.IsNullOrEmpty(esquina) && altura < 0) return;

            var result = GeocoderHelper.GetDireccion(calle, altura, esquina, partido, provincia);

            SetResults(result);
        }
        private void SetResults(IList<DireccionVO> result)
        {
            if (result.Count == 0) return;

            if (result.Count == 1) SetDireccion(result[0]);
            else
            {
                Direcciones = result;
                cbResults.Items.Clear();

                foreach (var vo in result) cbResults.Items.Add(vo.Direccion);

                SwitchView(1);
            }
        }
        public void SetDireccion(DireccionVO direccion)
        {
            var id = AddSelected(direccion);

            SwitchView(0);

            var sel = Selected;
            if (sel == null) return;

            BindGrid();

            txtCalle.Text = txtAltura.Text = txtEsquina.Text = txtPartido.Text = txtCodigo.Text = string.Empty;

            OnDireccionAdded(direccion, id);
        }
        private void BindGrid()
        {
            gridConsultas.DataSource = Selected;
            gridConsultas.DataBind();
            updGridConsultas.Update();
        }
        protected void btAceptar_Click(object sender, EventArgs e)
        {
            if (cbResults.SelectedIndex < 0) return;

            SetDireccion(Direcciones[cbResults.SelectedIndex]);
            Direcciones = null;
        }

        protected void btCancelar_Click(object sender, EventArgs e)
        {
            Direcciones = null;
            Selected = null;
            SwitchView(0);
        }
        public void AddFromLatLon(double lat, double lon)
        {
            var dir = GeocoderHelper.GetEsquinaMasCercana(lat, lon);
            if (dir == null) return;
            SetDireccion(dir);
        }
        protected void SwitchView(int index)
        {
            panelNormalSearch.Visible = index == 0;
            panelResult.Visible = index == 1;
        }

        protected void btLimpiar_Click(object sender, EventArgs e)
        {
            OnClear();
            Selected = null;
            BindGrid();
        }

        protected void btSaveGeoRef_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtCodigo.Text.Trim())) 
                    ThrowMustEnter("CODE");

                var sel = gridConsultas.SelectedIndex;

                if (sel < 0) throw new Exception(CultureManager.GetError("MUST_SELECT_QUERY"));

                var id = SelectedId[sel];

                var savedId = SavedId;

                if (savedId.Contains(id)) throw new Exception(CultureManager.GetError("QUERY_ALREADY_SAVED"));

                savedId.Add(id);
                SavedId = savedId;

                OnDireccionSaved(Selected[sel], id, cbTipoGeoRef.Selected, txtCodigo.Text.Trim());

                txtCodigo.Text = String.Empty;
                lblSaveStatus.Text = String.Empty;
            }
            catch (Exception ex) { lblSaveStatus.Text = ex.Message; }
        }

        protected void OnDireccionAdded(DireccionVO direccion, int index)
        {
            if (DireccionAdded == null) return;
            DireccionAdded(this, new DireccionEventArgs(direccion, index, chkPosicionar.Checked));
        }
        protected void OnDireccionRemoved(DireccionVO direccion, int index)
        {
            if (DireccionAdded == null) return;
            DireccionRemoved(this, new DireccionEventArgs(direccion, index, chkPosicionar.Checked));
        }
        protected void OnDireccionSelected(DireccionVO direccion, int index)
        {
            if (DireccionAdded == null) return;
            DireccionSelected(this, new DireccionEventArgs(direccion, index, chkPosicionar.Checked));
        }
        protected void OnClear()
        {
            if (Clear == null) return;
            Clear(this, EventArgs.Empty);
        }
        protected void OnDireccionSaved(DireccionVO direccion, int index, int idTipoReferenciaGeografica, string codigo)
        {
            if (DireccionSaved == null) return;
            DireccionSaved(this, new DireccionSavedEventArgs(direccion, index, idTipoReferenciaGeografica, codigo, chkPosicionar.Checked));
        }

        protected void gridConsultas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var dir = e.Row.DataItem as DireccionVO;
            if (dir == null) return;
            e.Row.Cells[1].Text = dir.Direccion;       
        }
        protected void gridConsultas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var sel = Convert.ToInt32(e.CommandArgument);
            if(e.CommandName == "Eliminar")
            {
                OnDireccionRemoved(Selected[sel], SelectedId[sel]);
                RemoveSelected(sel);
                BindGrid();
            }
        }
        protected void gridConsultas_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            var sel = e.NewSelectedIndex;
            OnDireccionSelected(Selected[sel], SelectedId[sel]);
        }
    }

    public class DireccionEventArgs: EventArgs
    {
        public DireccionVO Direccion { get; private set; }
        public int Index { get; private set; }
        public bool Posicionar { get; private set; }
        
        public DireccionEventArgs(DireccionVO direccion, int index, bool posicionar)
        {
            Direccion = direccion;
            Index = index;
            Posicionar = posicionar;
        }
    }
    public class DireccionSavedEventArgs: DireccionEventArgs
    {
        public int IdTipoReferenciaGeografica { get; private set; }
        public string Codigo { get; private set; }
        public DireccionSavedEventArgs(DireccionVO direccion, int index, int idTipoReferenciaGeografica, string codigo, bool posicionar)
            :base(direccion, index, posicionar)
        {
            IdTipoReferenciaGeografica = idTipoReferenciaGeografica;
            Codigo = codigo;
        }
    }
}