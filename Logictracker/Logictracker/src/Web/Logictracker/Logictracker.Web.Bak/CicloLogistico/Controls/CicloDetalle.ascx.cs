#region Usings

using System;
using System.Linq;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.Wrappers.DropDownLists;
using Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects;

#endregion

namespace Logictracker.CicloLogistico.Controls
{
    public partial class ControlDetalleCiclo : BaseUserControl
    {
        public event EventHandler Deleted;

        #region Parents

        public string ParentControls
        {
            private get { return (string)ViewState["ParentControls"]; }
            set { ViewState["ParentControls"] = value; }
        }

        private int IdEmpresa
        {
            get { return (int)(ViewState["IdEmpresa"] ?? -1); }
            set { ViewState["IdEmpresa"] = value; }
        }

        private int IdLinea
        {
            get { return (int)(ViewState["IdLinea"] ?? -1); }
            set { ViewState["IdLinea"] = value; }
        }

        private IAutoBindeable GetParent<T>()
        {
            if (string.IsNullOrEmpty(ParentControls)) return null;

            foreach (var parent in ParentControls.Split(','))
            {
                var control = FindParent(parent.Trim(), Page.Controls);

                if (control == null) continue;

                var iab = control as IAutoBindeable;

                if (iab != null && iab.Type == typeof(T)) return iab;

                var ipc = control as IParentControl;

                if (ipc != null) foreach (var bindeable in ipc.ParentControls.Where(bindeable => bindeable.Type == typeof(T))) return bindeable;
            }

            return null;
        }

        private void LoadParents()
        {
            var emp = GetParent<Empresa>();
            var lin = GetParent<Linea>();

            if (emp != null) IdEmpresa = emp.Selected;

            if (lin == null) return;

            IdLinea = lin.Selected;

            lin.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
        }

        void cbLinea_SelectedIndexChanged(object sender, EventArgs e) { SwitchView(); }

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

        #endregion

        public DetalleCiclo Detalle
        {
            get
            {
                var id = (int)(ViewState["Detalle"] ?? 0);

                var detalle = new DetalleCiclo { Id = id };

                int itmp;

                detalle.Tipo = Convert.ToInt16(cbTipo.SelectedValue);

                detalle.ReferenciaGeografica = detalle.Tipo == DetalleCiclo.TipoEntradaPoi || detalle.Tipo == DetalleCiclo.TipoSalidaPoi
                    ? Convert.ToInt32(cbReferenciaGeografica.SelectedValue) > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32(cbReferenciaGeografica.SelectedValue))
                    : null : null;

                detalle.EstadoCicloLogistico = detalle.Tipo == DetalleCiclo.TipoCicloLogistico ? Convert.ToInt16(cbCicloLogistico.SelectedValue) > 0
                    ? DAOFactory.CicloLogisticoDAO.FindById(Convert.ToInt16(cbCicloLogistico.SelectedValue)) : null : null;

                detalle.Minutos = int.TryParse(txtMinutos.Text.Trim(), out itmp) ? itmp : 0;
                detalle.Codigo = txtCodigo.Text.Trim();
                detalle.Descripcion = txtDescripcion.Text.Trim();
                detalle.Repeticion = (short)(chkRepite.Checked ? 0 : 1);
                detalle.Duracion = int.TryParse(txtDuracion.Text, out itmp) ? itmp : 0;
                detalle.Obligatorio = chkObligatorio.Checked;

                var empresa = IdEmpresa > 0 ? DAOFactory.EmpresaDAO.FindById(IdEmpresa) : null;
                var linea = IdLinea > 0 ? DAOFactory.LineaDAO.FindById(IdLinea) : null;

                detalle.Mensaje = detalle.Tipo == DetalleCiclo.TipoEvento ? Convert.ToInt16(cbEventos.SelectedValue) > 0
                    ? DAOFactory.MensajeDAO.FindById(DAOFactory.MensajeDAO.GetByCodigo(cbEventos.SelectedValue, empresa, linea).Id) : null : null;

                detalle.MensajeControl = chkControlManual.Checked
                    ? DAOFactory.MensajeDAO.FindById(DAOFactory.MensajeDAO.GetByCodigo(cbMensajes.SelectedValue, empresa, linea).Id) : null;

                detalle.ObligatorioControl = chkControlManual.Checked ? chkControlManualObligatorio.Checked : false;

                detalle.Orden = Convert.ToInt16(lblOrden.Text);

                return detalle;
            }
            private set { ViewState["Detalle"] = value.Id; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadParents();
        }

        public void SetOrden(short orden) { lblOrden.Text = orden.ToString(); }

        public void SetDetalle(DetalleCiclo detalle)
        {
            Detalle = detalle;

            var lin = GetParent<Linea>();

            BindTipo(lin);

            cbTipo.SelectedValue = detalle.Tipo.ToString();

            BindTipoEventos(lin);

            cbTipoEvento.SelectedValue = detalle.Mensaje != null ? detalle.Mensaje.TipoMensaje.Id.ToString() : "-1";

            BindEventos(lin);

            cbEventos.SelectedValue = detalle.Mensaje != null ? detalle.Mensaje.Codigo : "-1";

            BindTipoReferenciaGeografica(lin);

            cbTipoReferenciaGeografica.SelectedValue = detalle.ReferenciaGeografica != null ? detalle.ReferenciaGeografica.TipoReferenciaGeografica.Id.ToString() : "-1";

            BindReferenciaGeografica(lin);

            cbReferenciaGeografica.SelectedValue = detalle.ReferenciaGeografica != null ? detalle.ReferenciaGeografica.Id.ToString() : "-1";

            BindCiclosLogisticos(lin);

            cbCicloLogistico.SelectedValue = detalle.EstadoCicloLogistico != null ? detalle.EstadoCicloLogistico.Id.ToString() : "-1";

            SwitchView();

            txtCodigo.Text = detalle.Codigo;
            txtDescripcion.Text = detalle.Descripcion;
            chkRepite.Checked = detalle.Repeticion != 1;
            txtDuracion.Text = detalle.Duracion.ToString();
            chkObligatorio.Checked = detalle.Obligatorio;
            lblOrden.Text = detalle.Orden.ToString();
            txtMinutos.Text = detalle.Minutos.ToString();

            cbTipoMensaje.Enabled = cbMensajes.Enabled = chkControlManualObligatorio.Enabled = chkControlManual.Checked = detalle.MensajeControl != null;

            if (detalle.MensajeControl != null) chkControlManualObligatorio.Checked = detalle.ObligatorioControl;

            BindTipoMensajes(lin);

            if (detalle.MensajeControl != null) cbTipoMensaje.SelectedValue = detalle.MensajeControl.TipoMensaje.Id.ToString();

            BindMensajes(lin);

            if (detalle.MensajeControl != null) cbMensajes.SelectedValue = detalle.MensajeControl.Codigo;
        }

        private void BindTipoMensajes(IAutoBindeable lin)
        {
            var cbTipoMensajeWrapper = new DropDownListBaseWrapper<TipoMensaje>(cbTipoMensaje);

            if (lin != null) cbTipoMensajeWrapper.AddParent<Linea>(lin);

            cbTipoMensajeWrapper.BindingManager.BindTipoMensaje(cbTipoMensajeWrapper);
        }

        private void BindTipo(IAutoBindeable lin)
        {
            var cbTipoWrapper = new DropDownListBaseWrapper<String>(cbTipo);

            cbTipoWrapper.BindingManager.BindTipoDetalleCiclo(cbTipoWrapper);

            if (lin != null) cbTipoWrapper.AddParent<Linea>(lin);
        }

        protected void cbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var detalle = Detalle;

            if (detalle != null)
            {
                detalle.Tipo = Convert.ToInt16(cbTipo.SelectedValue);

                Detalle = detalle;
            }

            SwitchView();
        }

        private void SwitchView()
        {
            var lin = GetParent<Linea>();

            switch (Convert.ToInt16(cbTipo.SelectedValue))
            {
                case DetalleCiclo.TipoTiempo: multiEvento.SetActiveView(viewTiempo); break;
                case DetalleCiclo.TipoEvento:
                    multiEvento.SetActiveView(viewMensaje);

                    BindTipoEventos(lin);

                    BindEventos(lin);

                    break;
                case DetalleCiclo.TipoEntradaPoi:
                case DetalleCiclo.TipoSalidaPoi:
                    multiEvento.SetActiveView(viewGeoRef);

                    BindTipoReferenciaGeografica(lin);

                    BindReferenciaGeografica(lin);

                    break;
                case DetalleCiclo.TipoCicloLogistico:
                    multiEvento.SetActiveView(viewCiclo);

                    BindCiclosLogisticos(lin);

                    break;
            }

            updEvento.Update();
        }

        private void BindTipoReferenciaGeografica(IAutoBindeable lin)
        {
            var cbTipoReferenciaGeograficaWrapper = new DropDownListBaseWrapper<TipoReferenciaGeografica>(cbTipoReferenciaGeografica);

            if (lin != null) cbTipoReferenciaGeograficaWrapper.AddParent<Linea>(lin);

            cbTipoReferenciaGeograficaWrapper.BindingManager.BindTipoReferenciaGeografica(cbTipoReferenciaGeograficaWrapper);
        }

        private void BindCiclosLogisticos(IAutoBindeable lin)
        {
            var cbCicloLogisticoWrapper = new CicloLogisticoDropDownListWrapper(cbCicloLogistico) { AddCiclos = false, AddEstados = true };

            if (lin != null) cbCicloLogisticoWrapper.AddParent<Linea>(lin);

            cbCicloLogisticoWrapper.BindingManager.BindCiclosLogisticos(cbCicloLogisticoWrapper);
        }

        private void BindTipoEventos(IAutoBindeable lin)
        {
            var cbTipoEventoWrapper = new DropDownListBaseWrapper<TipoMensaje>(cbTipoEvento);

            if (lin != null) cbTipoEventoWrapper.AddParent<Linea>(lin);

            cbTipoEventoWrapper.BindingManager.BindTipoMensaje(cbTipoEventoWrapper);
        }

        protected void chkControlManual_CheckedChanged(object sender, EventArgs e)
        {
            cbTipoMensaje.Enabled = cbMensajes.Enabled = chkControlManualObligatorio.Enabled = chkControlManual.Checked;
        }

        protected void btEliminar_Click(object sender, EventArgs e) { if (Deleted != null) Deleted(this, EventArgs.Empty); }

        protected void cbTipoEvento_SelectedIndexChanged(object sender, EventArgs e) { BindEventos(GetParent<Linea>()); }

        private void BindEventos(IAutoBindeable lin)
        {
            var cbEventosWrapper = new MensajeDropDownListWrapper(cbEventos);

            cbEventosWrapper.AddParent<TipoMensaje>(cbTipoEvento);

            if (lin != null) cbEventosWrapper.AddParent<Linea>(lin);

            cbEventosWrapper.BindingManager.BindMensajes(cbEventosWrapper);
        }

        protected void cbTipoReferenciaGeografica_SelectedIndexChanged(object sender, EventArgs e) { BindReferenciaGeografica(GetParent<Linea>()); }

        private void BindReferenciaGeografica(IAutoBindeable lin)
        {
            var cbReferenciaGeograficaWrapper = new DropDownListBaseWrapper<ReferenciaGeografica>(cbReferenciaGeografica);

            cbReferenciaGeograficaWrapper.AddParent<TipoReferenciaGeografica>(cbTipoReferenciaGeografica);

            if (lin != null) cbReferenciaGeograficaWrapper.AddParent<Linea>(lin);

            cbReferenciaGeograficaWrapper.BindingManager.BindReferenciaGeografica(cbReferenciaGeograficaWrapper);
        }

        protected void cbTipoMensaje_SelectedIndexChanged(object sender, EventArgs e) { BindMensajes(GetParent<Linea>()); }

        private void BindMensajes(IAutoBindeable lin)
        {
            var cbMensajesWrapper = new MensajeDropDownListWrapper(cbMensajes);

            cbMensajesWrapper.AddParent<TipoMensaje>(cbTipoMensaje);

            if (lin != null) cbMensajesWrapper.AddParent<Linea>(lin);

            cbMensajesWrapper.BindingManager.BindMensajes(cbMensajesWrapper);
        }
    }
}
