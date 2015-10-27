using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.Wrappers.DropDownLists;
using Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects;

namespace Logictracker.CicloLogistico.Controls
{
    public partial class ControlDetalleServicio : BaseUserControl
    {
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
                if (ipc != null)
                    foreach (var bindeable in ipc.ParentControls.Where(bindeable => bindeable.Type == typeof(T)))
                        return bindeable;

            }
            return null;
        }

        private void LoadParents()
        {
            var emp = GetParent<Empresa>();
            var lin = GetParent<Linea>();

            if (emp != null) IdEmpresa = emp.Selected;
            if (lin != null) IdLinea = lin.Selected;
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
        #endregion

        #region Control Wrappers
        private DropDownListBaseWrapper<TipoMensaje> _cbTipoEventoWrapper;
        private MensajeDropDownListWrapper _cbEventosWrapper;
        private DropDownListBaseWrapper<TipoReferenciaGeografica> _cbTipoReferenciaGeograficaWrapper;
        private DropDownListBaseWrapper<ReferenciaGeografica> _cbReferenciaGeograficaWrapper;

        private DropDownListBaseWrapper<TipoMensaje> cbTipoEventoWrapper { get { return _cbTipoEventoWrapper ?? (_cbTipoEventoWrapper = new DropDownListBaseWrapper<TipoMensaje>(cbTipoEvento)); } }
        private MensajeDropDownListWrapper cbEventosWrapper
        {
            get
            {
                if (_cbEventosWrapper == null)
                {
                    _cbEventosWrapper = new MensajeDropDownListWrapper(cbEventos);
                    _cbEventosWrapper.AddParent<TipoMensaje>(cbTipoEvento);
                }
                return _cbEventosWrapper;
            }
        }
        private DropDownListBaseWrapper<TipoReferenciaGeografica> cbTipoReferenciaGeograficaWrapper { get { return _cbTipoReferenciaGeograficaWrapper ?? (_cbTipoReferenciaGeograficaWrapper = new DropDownListBaseWrapper<TipoReferenciaGeografica>(cbTipoReferenciaGeografica)); } }
        private DropDownListBaseWrapper<ReferenciaGeografica> cbReferenciaGeograficaWrapper
        {
            get
            {
                if (_cbReferenciaGeograficaWrapper == null)
                {
                    _cbReferenciaGeograficaWrapper = new DropDownListBaseWrapper<ReferenciaGeografica>(cbReferenciaGeografica);
                    _cbReferenciaGeograficaWrapper.AddParent<TipoReferenciaGeografica>(cbTipoReferenciaGeografica);
                }
                return _cbReferenciaGeograficaWrapper;
            }
        }
        #endregion

        #region Events

        public event EventHandler Duplicating;
        public event EventHandler Duplicated;
        public event EventHandler DateChanged;
        #endregion

        #region ViewState Properties
        protected bool EsEstado
        {
            get { return (bool)(ViewState["EsEstado"] ?? false); }
            set { ViewState["EsEstado"] = value; }
        }
        public bool EsCiclo
        {
            get { return (bool)(ViewState["EsCiclo"] ?? false); }
            set { ViewState["EsCiclo"] = value; }
        }
        private int IdDetalleCiclo
        {
            get { return (int)(ViewState["IdDetalleCiclo"] ?? 0); }
            set { ViewState["IdDetalleCiclo"] = value; }
        }
        private int IdCiclo
        {
            get { return (int)(ViewState["IdCiclo"] ?? 0); }
            set { ViewState["IdCiclo"] = value; }
        }
        private int CountDetalles
        {
            get { return (int)(ViewState["CountDetalles"] ?? 0); }
            set { ViewState["CountDetalles"] = value; }
        }
        public int Duracion
        {
            get { return (int)(ViewState["Duracion"] ?? 0); }
            set { ViewState["Duracion"] = value; }
        }

        #endregion

        public List<DetalleServicio> Detalles
        {
            get
            {
                if (CountDetalles == 0)
                {
                    var det = Detalle;
                    return det == null ? new List<DetalleServicio>() : new List<DetalleServicio> { det };
                }
                var list = new List<DetalleServicio>();

                var controls = GetControls();
                foreach (var ctl in controls) { list.AddRange(ctl.Detalles); }

                return list;
            }
        }

        private DetalleServicio Detalle
        {
            get
            {
                LoadParents();
                var id = (int)(ViewState["Detalle"] ?? 0);

                if (IdDetalleCiclo == 0) return null;
                var detalleCiclo = GetDetalleCiclo();

                var detalle = new DetalleServicio
                                  {
                                      Id = id,
                                      Tipo = detalleCiclo.Tipo,
                                      DetalleCiclo = detalleCiclo,
                                      Estado = DetalleServicio.EstadoEnCurso,
                                      Obligatorio = chkObligatorio.Checked,
                                      Orden = Convert.ToInt16(lblOrden.Text),
                                      Programada = dtProgramada.SelectedDate.HasValue
                                              ? dtProgramada.SelectedDate.Value.ToDataBaseDateTime()
                                              : DateTime.MinValue
                                  };

                detalle.ReferenciaGeografica = detalle.Tipo == DetalleCiclo.TipoEntradaPoi || detalle.Tipo == DetalleCiclo.TipoSalidaPoi
                    ? cbReferenciaGeograficaWrapper.Selected > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(cbReferenciaGeograficaWrapper.Selected) : null
                    : null;

                if (detalle.Tipo == DetalleServicio.TipoEvento)
                {
                    var empresa = IdEmpresa > 0 ? DAOFactory.EmpresaDAO.FindById(IdEmpresa) : null;
                    var linea = IdLinea > 0 ? DAOFactory.LineaDAO.FindById(IdLinea) : null;

                    detalle.Mensaje = cbEventosWrapper.Selected > 0
                        ? DAOFactory.MensajeDAO.FindById(DAOFactory.MensajeDAO.GetByCodigo(cbEventosWrapper.Selected.ToString(), empresa, linea).Id) : null;
                }

                int itmp;

                detalle.Minutos = int.TryParse(txtMinutos.Text.Trim(), out itmp) ? itmp : 0;

                return detalle;
            }
            set { ViewState["Detalle"] = value.Id; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var lin = GetParent<Linea>();
            if (lin == null) return;

            cbTipoEventoWrapper.AddParent<Linea>(lin);
            cbTipoReferenciaGeograficaWrapper.AddParent<Linea>(lin);
        }

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btNuevo_Click(object sender, EventArgs e)
        {
            if (Duplicating != null) Duplicating(this, EventArgs.Empty);
            if (Duplicated != null) Duplicated(this, EventArgs.Empty);
        }

        private void CreateFromDetalleCiclo(DetalleCiclo detalle)
        {
            EsEstado = true;
            if (detalle.Tipo != DetalleCiclo.TipoCicloLogistico)
            {
                lblCodigo.Text = detalle.Codigo;
                lblDescripcion.Text = detalle.Descripcion;
                chkObligatorio.Checked = detalle.Obligatorio;
                panelControlManual.Visible = detalle.MensajeControl != null;
                lblObligatorioControl.Visible = detalle.ObligatorioControl;
                if (detalle.MensajeControl != null) lblMensaje.Text = detalle.MensajeControl.Descripcion;

                multiTipoEstado.SetActiveView(viewTipoEstadoNormal);
            }
            switch (detalle.Tipo)
            {
                case DetalleCiclo.TipoTiempo:
                    lblTipo.Text = "Tiempo";
                    multiEvento.SetActiveView(viewTiempo);
                    txtMinutos.Text = detalle.Minutos.ToString();
                    break;
                case DetalleCiclo.TipoEvento:
                    lblTipo.Text = "Evento";
                    multiEvento.SetActiveView(viewMensaje);

                    cbTipoEventoWrapper.BindingManager.BindTipoMensaje(cbTipoEventoWrapper);
                    if (detalle.Mensaje != null) cbTipoEvento.SelectedValue = detalle.Mensaje.TipoMensaje.Id.ToString();
                    cbEventosWrapper.BindingManager.BindMensajes(cbEventosWrapper);
                    if (detalle.Mensaje != null) cbEventos.SelectedValue = detalle.Mensaje.Codigo;
                    multiEvento.SetActiveView(viewMensaje);
                    break;
                case DetalleCiclo.TipoEntradaPoi:
                    lblTipo.Text = "Entrada GeoReferencia";

                    cbTipoReferenciaGeograficaWrapper.BindingManager.BindTipoReferenciaGeografica(cbTipoReferenciaGeograficaWrapper);
                    if (detalle.ReferenciaGeografica != null) cbTipoReferenciaGeografica.SelectedValue = detalle.ReferenciaGeografica.TipoReferenciaGeografica.Id.ToString();
                    cbReferenciaGeograficaWrapper.BindingManager.BindReferenciaGeografica(cbReferenciaGeograficaWrapper);
                    if (detalle.ReferenciaGeografica != null) cbReferenciaGeografica.SelectedValue = detalle.ReferenciaGeografica.Id.ToString();
                    multiEvento.SetActiveView(viewGeoRef);
                    break;
                case DetalleCiclo.TipoSalidaPoi:
                    lblTipo.Text = "Salida GeoReferencia";
                    cbTipoReferenciaGeograficaWrapper.BindingManager.BindTipoReferenciaGeografica(cbTipoReferenciaGeograficaWrapper);
                    if (detalle.ReferenciaGeografica != null) cbTipoReferenciaGeografica.SelectedValue = detalle.ReferenciaGeografica.TipoReferenciaGeografica.Id.ToString();
                    cbReferenciaGeograficaWrapper.BindingManager.BindReferenciaGeografica(cbReferenciaGeograficaWrapper);
                    if (detalle.ReferenciaGeografica != null) cbReferenciaGeografica.SelectedValue = detalle.ReferenciaGeografica.Id.ToString();
                    multiEvento.SetActiveView(viewGeoRef);
                    break;
                case DetalleCiclo.TipoCicloLogistico:
                    SetCiclo(detalle.EstadoCicloLogistico);
                    break;
            }

            dtProgramada.SelectedDate = DateTime.Now;

            IdDetalleCiclo = detalle.Id;

            if (detalle.Repeticion == 1) panelNuevo.Visible = false;
            else btNuevo.Text = "+ Agregar " + detalle.Descripcion;
        }

        public void SetDetalle(DetalleServicio detalle)
        {
            Detalle = detalle;
            chkObligatorio.Checked = detalle.Obligatorio;

            switch (detalle.Tipo)
            {
                case DetalleCiclo.TipoTiempo:
                    txtMinutos.Text = detalle.Minutos.ToString();
                    break;
                case DetalleCiclo.TipoEvento:
                    if (detalle.Mensaje != null)
                    {
                        var tipoEvento = detalle.Mensaje.TipoMensaje.Id.ToString();
                        var evento = detalle.Mensaje.Codigo;
                        if (cbTipoEvento.SelectedValue != tipoEvento) cbTipoEventoWrapper.BindingManager.BindTipoMensaje(cbTipoEventoWrapper);
                        cbTipoEvento.SelectedValue = tipoEvento;
                        if (cbEventos.SelectedValue != evento) cbEventosWrapper.BindingManager.BindMensajes(cbEventosWrapper);
                        cbEventos.SelectedValue = evento;
                    }
                    break;
                case DetalleCiclo.TipoEntradaPoi:
                case DetalleCiclo.TipoSalidaPoi:
                    if (detalle.ReferenciaGeografica != null)
                    {
                        var tipoGeoRef = detalle.ReferenciaGeografica.TipoReferenciaGeografica.Id.ToString();
                        var geoRef = detalle.ReferenciaGeografica.Id.ToString();
                        if (cbTipoReferenciaGeografica.SelectedValue != tipoGeoRef) cbTipoReferenciaGeograficaWrapper.BindingManager.BindTipoReferenciaGeografica(cbTipoReferenciaGeograficaWrapper);
                        cbTipoReferenciaGeografica.SelectedValue = tipoGeoRef;
                        cbReferenciaGeograficaWrapper.BindingManager.BindReferenciaGeografica(cbReferenciaGeograficaWrapper);
                        cbReferenciaGeografica.SelectedValue = geoRef;
                    }

                    break;
            }

            dtProgramada.SelectedDate = detalle.Programada.ToDisplayDateTime();
        }
        public void SetCiclo(Logictracker.Types.BusinessObjects.Tickets.CicloLogistico ciclo)
        {
            multiTipoEstado.SetActiveView(viewTipoEstadoCiclo);
            EsCiclo = true;
            if (ciclo == null) return;
            IdCiclo = ciclo.Id;

            var detalles = GetDetalles(ciclo);
            CountDetalles = detalles.Count;
            LoadDetalles();

            var controls = GetControls();

            for (var i = 0; i < detalles.Count && i < controls.Count; i++)
            {
                var detalle = detalles[i];
                var control = controls[i];
                control.CreateFromDetalleCiclo(detalle);

                control.Duplicating += control_Duplicar;
                control.DateChanged += control_DateChanged;
            }
            UpdateOrders();
        }
        public void SetDetalles(List<DetalleServicio> detalles)
        {
            if (!EsCiclo) return;

            var controls = GetControls();
            var c = 0;

            ControlDetalleServicio lastControl = null;
            var subDetalles = new List<DetalleServicio>();

            foreach (var detalleServicio in detalles)
            {
                var control = c < controls.Count ? controls[c] : lastControl;

                if (!control.EsCiclo && control != lastControl && control.IdDetalleCiclo == detalleServicio.DetalleCiclo.Id)
                {
                    control.SetDetalle(detalleServicio);
                    lastControl = control;
                    c++;
                }
                else if (lastControl != null && !lastControl.EsCiclo && lastControl.IdDetalleCiclo == detalleServicio.DetalleCiclo.Id)
                {
                    var dControl = DuplicarControl(lastControl);
                    dControl.SetDetalle(detalleServicio);
                    lastControl = dControl;
                }
                if (control.EsCiclo && control != lastControl && control.IdCiclo == detalleServicio.DetalleCiclo.CicloLogistico.Id)
                {
                    subDetalles.Add(detalleServicio);

                    if (control.CountDetalles == subDetalles.Count)
                    {
                        control.SetDetalles(subDetalles);
                        lastControl = control;
                        c++;

                        subDetalles = new List<DetalleServicio>();
                    }
                }
                else if (lastControl != null && lastControl.EsCiclo && lastControl.IdCiclo == detalleServicio.DetalleCiclo.CicloLogistico.Id)
                {
                    subDetalles.Add(detalleServicio);

                    if (lastControl.CountDetalles == subDetalles.Count)
                    {
                        var dControl = DuplicarControl(lastControl);
                        dControl.SetDetalles(subDetalles);
                        lastControl = dControl;
                        subDetalles = new List<DetalleServicio>();
                    }
                }
            }
        }

        void control_Duplicar(object sender, EventArgs e) { DuplicarControl(sender as ControlDetalleServicio); }

        private ControlDetalleServicio DuplicarControl(ControlDetalleServicio detalle)
        {
            CountDetalles = CountDetalles + 1;
            var detalleCiclo = DAOFactory.DetalleCicloDAO.FindById(detalle.IdDetalleCiclo);
            var control = new ControlDetalleServicio { ID = "_" + CountDetalles, ParentControls = ParentControls };
            var found = false;
            var interval = TimeSpan.Zero;
            ControlDetalleServicio prev = null;
            for (var i = 0; i < panelEstados.Controls.Count; i++)
            {
                var current = panelEstados.Controls[i] as ControlDetalleServicio;
                if (current == null) continue;
                if (!found && current != detalle) continue;


                var next = (i + 1 < panelEstados.Controls.Count ? panelEstados.Controls[i + 1] : null) as ControlDetalleServicio;
                var tmp = next != null ? next.GetFirstDate().Subtract(current.GetLastDate()) : TimeSpan.Zero;

                if (panelEstados.Controls[i] == detalle)
                {
                    panelEstados.Controls.AddAt(i + 1, control);
                    control.CreateFromDetalleCiclo(detalleCiclo);
                    control.PanelAddVisible = detalle.PanelAddVisible;
                    detalle.PanelAddVisible = false;

                    control.SetDate(detalle.GetNextDate());
                    i++;
                    current = control;
                    found = true;
                }
                else
                {
                    current.SetDate(prev.GetLastDate().Add(interval));
                }
                interval = tmp;
                prev = current;
            }

            UpdateOrders();

            // Reseteo los Id de los controles para que mantengan el orden correcto
            var allControls = GetControls();
            for (var i = 0; i < allControls.Count; i++) allControls[i].ID = "_" + i;


            return control;
        }
        private void UpdateOrders()
        {
            short i = 1;
            var controls = GetControls();
            foreach (var ctl in controls) ctl.SetOrden(i++);
        }

        public void SetOrden(short orden) { lblOrden.Text = orden.ToString(); }

        #region Date Date Date...

        public DateTime SetDate(DateTime date)
        {
            var d = date;
            if (EsCiclo) return panelEstados.Controls.OfType<ControlDetalleServicio>().Aggregate(d, (current, ctl) => ctl.SetDate(current));


            dtProgramada.SelectedDate = d;
            var detalle = GetDetalleCiclo();
            if (Duracion == 0) Duracion = detalle.Duracion;
            return d.AddMinutes(Duracion);
        }
        public DateTime GetFirstDate()
        {
            if (EsCiclo)
            {
                var first = panelEstados.Controls.OfType<ControlDetalleServicio>().FirstOrDefault();
                return first == null ? DateTime.Now : first.GetFirstDate();
            }

            var date = dtProgramada.SelectedDate;
            return date.HasValue ? date.Value : DateTime.Now;
        }
        public DateTime GetLastDate()
        {
            if (EsCiclo)
            {
                var last = panelEstados.Controls.OfType<ControlDetalleServicio>().LastOrDefault();
                return last == null ? DateTime.Now : last.GetLastDate();
            }

            var date = dtProgramada.SelectedDate;
            return date.HasValue ? date.Value : DateTime.Now;
        }
        public DateTime GetNextDate()
        {
            if (EsCiclo)
            {
                var last = panelEstados.Controls.OfType<ControlDetalleServicio>().LastOrDefault();
                return last.GetNextDate();
            }

            var detalle = GetDetalleCiclo();
            return GetLastDate().AddMinutes(detalle.Duracion);
        }
        #endregion

        public bool PanelAddVisible
        {
            get { return panelNuevo.Visible; }
            set { panelNuevo.Visible = value; }
        }

        public void LoadDetalles()
        {
            var count = CountDetalles;
            if (count == 0) return;
            ClearControls();

            //Hack para que me tome el primer elemento (?)
            AddControl(new LiteralControl());

            for (var i = 0; i < count; i++)
            {
                var control = new ControlDetalleServicio { ID = "_" + i, ParentControls = ParentControls };
                AddControl(control);
                control.Duplicating += control_Duplicar;
                control.DateChanged += control_DateChanged;
                control.LoadDetalles();
            }
            AddControl(new LiteralControl());
        }

        public void Clear()
        {
            CountDetalles = IdCiclo = IdDetalleCiclo = 0;
            ClearControls();
        }

        private void AddControl(Control control) { panelEstados.Controls.Add(control); }

        private void ClearControls() { panelEstados.Controls.Clear(); }

        public List<ControlDetalleServicio> GetControls() { return panelEstados.Controls.OfType<ControlDetalleServicio>().ToList(); }
        public List<ControlDetalleServicio> GetControlsEstado()
        {
            var list = new List<ControlDetalleServicio>();
            if (EsCiclo)
            {
                var controls = GetControls();
                foreach (var control in controls)
                {
                    list.AddRange(control.GetControlsEstado());
                }
            }
            else
            {
                list.Add(this);
            }
            return list;
        }

        private DetalleCiclo GetDetalleCiclo() { return DAOFactory.DetalleCicloDAO.FindById(IdDetalleCiclo); }

        private static List<DetalleCiclo> GetDetalles(Logictracker.Types.BusinessObjects.Tickets.CicloLogistico ciclo) { return ciclo.Detalles.OfType<DetalleCiclo>().OrderBy(det => det.Orden).ToList(); }
        protected void cbTipoEvento_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbEventosWrapper.BindingManager.BindMensajes(cbEventosWrapper);
        }
        protected void cbTipoReferenciaGeografica_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbReferenciaGeograficaWrapper.BindingManager.BindReferenciaGeografica(cbReferenciaGeograficaWrapper);
        }
        protected void dtProgramada_DateChanged(object sender, EventArgs e)
        {
            if (DateChanged != null) DateChanged(this, e);
        }

        protected void control_DateChanged(object sender, EventArgs e)
        {
            if (DateChanged != null) DateChanged(sender, e);
        }
    }
}