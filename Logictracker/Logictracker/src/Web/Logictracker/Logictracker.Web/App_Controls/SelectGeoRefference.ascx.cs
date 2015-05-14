using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_SelectGeoRefference : BaseUserControl
    {
        public event EventHandler DireccionSelected;

        protected void OnDireccionSelected()
        {
            if (DireccionSelected != null) DireccionSelected(this, EventArgs.Empty);
        }
        public int Selected { get { return SelectedId; } }

        public string ParentControls
        {
            get { return (string) ViewState["ParentControls"]; }
            set { ViewState["ParentControls"] = value; }
        }

        public int IdEmpresa
        {
            get { return (int) (ViewState["IdEmpresa"] ?? -1); }
            set { ViewState["IdEmpresa"] = value; }
        }
        public int IdLinea
        {
            get { return (int)(ViewState["IdLinea"] ?? -1); }
            set { ViewState["IdLinea"] = value; }
        }

        protected int SelectedId
        {
            get { return (int)(ViewState["Selected"]?? -1); }
            set { ViewState["Selected"] = value; }
        }

        private void LoadParents()
        {
            if (string.IsNullOrEmpty(ParentControls)) return;

            foreach (var parent in ParentControls.Split(','))
            {
                var control = FindParent(parent.Trim(), Page.Controls);
                if (control == null) continue;
                var iab = control as IAutoBindeable;
                var ipc = control as IParentControl;
                if (iab != null)
                {
                    if (iab.Type == typeof(Empresa)) IdEmpresa = iab.Selected;
                    else if (iab.Type == typeof(Linea)) IdLinea = iab.Selected;
                }
                else  if(ipc != null)
                {
                    foreach (var bindeable in ipc.ParentControls)
                    {
                        if (bindeable.Type == typeof(Empresa)) IdEmpresa = bindeable.Selected;
                        else if (bindeable.Type == typeof(Linea)) IdLinea = bindeable.Selected;
                    }
                }
            }
        }

        private static Control FindParent(string parent, ControlCollection controls)
        {
            if (controls == null) return null;

            foreach (Control control in controls)
            {
                if (!string.IsNullOrEmpty(control.ID) && control.ID.Equals(parent)) return control;
                var cnt = FindParent(parent, control.Controls);
                if (cnt != null) return cnt;
            }
            return null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cbTipoReferenciaGeografica.ParentControls = ParentControls;
        }
        protected override void  OnLoad(EventArgs e)
        {
            LoadParents();
            base.OnLoad(e);
        }

        protected void CbTipoReferenciaGeograficaSelectedIndexChanged(object sender, EventArgs e)
        {
            var georef = DAOFactory.ReferenciaGeograficaDAO.GetList(new[]{IdEmpresa}, new[]{IdLinea}, new[]{cbTipoReferenciaGeografica.Selected});

            var referencias = (from r in georef where (IdLinea > 0 || r.Linea == null) && (IdEmpresa > 0 || r.Empresa == null) select r).ToList();

            cbReferenciaGeografica.DataSource = referencias;
            cbReferenciaGeografica.DataBind();

            updControl.Update();
        }

        protected void BtSelectClick(object sender, EventArgs e)
        {
            if (cbReferenciaGeografica.SelectedIndex < 0) return;
            var referencia = DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32((string) cbReferenciaGeografica.SelectedValue));
            SetReferencia(referencia);
        }
        protected void BtSearchClick(object sender, EventArgs e)
        {
            var descripcion = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(descripcion)) return;

            var referencias = DAOFactory.ReferenciaGeograficaDAO.GetByDescripcion(new[] { IdEmpresa }, new[] { IdLinea }, descripcion);

            SetResults(referencias);
        }

        protected void SetResults(IList<ReferenciaGeografica> result)
        {
            if (result.Count == 0) return;
            if (result.Count == 1) SetReferencia(result[0]);
            else
            {
                cbResults.Items.Clear();
                foreach (var vo in result) cbResults.Items.Add(new ListItem(vo.Descripcion, vo.Id.ToString("#0")));
                SetResultView(true);
                SwitchView(3);
                updControl.Update();
            }
        }
        protected void BtAceptarClick(object sender, EventArgs e)
        {
            if (cbResults.SelectedIndex < 0) return;
            SetReferencia(DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32((string) cbResults.SelectedValue)));
        }
        protected void BtCancelarClick(object sender, EventArgs e)
        {
            SwitchView(0);
            SetResultView(false);
        }
        public void SetReferencia(ReferenciaGeografica referencia)
        {
            if (referencia != null)
            {
                SelectedId = referencia.Id;
                lblReferencia.Text = referencia.Descripcion;
                SwitchView(0);
            }
            SetResultView(false);
            updControl.Update();
            OnDireccionSelected();
        }
        protected void SetResultView(bool state)
        {
            tabSelect.Enabled =
                tabSearch.Enabled =
                tabReferencia.Enabled = !state;
            tabResult.Enabled = state;
            updControl.Update();
        }
        public Unit Width
        {
            get { return panelControl.Width; }
            set { panelControl.Width = tab.Width = value; }
        }
        public Unit Height
        {
            get { return panelControl.Height; }
            set { panelControl.Height = tab.Height = value; }
        }
        protected void SwitchView(int idx)
        {
            tab.ActiveTabIndex = idx;
        }
    }
}
