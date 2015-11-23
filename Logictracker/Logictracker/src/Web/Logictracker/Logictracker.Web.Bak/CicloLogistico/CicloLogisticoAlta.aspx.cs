using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.CicloLogistico.Controls;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class CicloLogisticoAlta : SecuredAbmPage<Logictracker.Types.BusinessObjects.Tickets.CicloLogistico>
    {
        #region Constants

        private const string SinPermisos = "El usuario actual no tiene permisos para editar este elemento.";

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "CLOG_CICLOS_LOGISTICOS"; } }
        protected override string RedirectUrl { get { return "CicloLogisticoLista.aspx"; } }

        #endregion

        private List<int> Indices
        {
            get { return (ViewState["Indices"] ?? new List<int>()) as List<int>; }
            set { ViewState["Indices"] = value; }
        }

        #region Protected Methods

        protected void cbEmpresa_InitialBindig(object sender, EventArgs e) { if (EditMode && EditObject.Owner.Empresa != null) cbEmpresa.EditValue = EditObject.Owner.Empresa.Id; }
        protected void cbLinea_InitialBindig(object sender, EventArgs e) { if (EditMode && EditObject.Owner.Linea != null) cbLinea.EditValue = EditObject.Owner.Linea.Id; }

        protected override void OnPreLoad(EventArgs e)
        {
            LoadDetalles();
            base.OnPreLoad(e);
        }

        protected override void Bind()
        {
            var empresa = EditObject.Owner != null ? EditObject.Owner.Empresa != null ? EditObject.Owner.Empresa.Id : -1 : -1;
            var linea = EditObject.Owner != null ? EditObject.Owner.Linea != null ? EditObject.Owner.Linea.Id : -1 : -1;

            if (!CanEditEmpresa(empresa) || !CanEditLinea(linea)) ShowInfo(SinPermisos);

            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            chkCiclo.Checked = EditObject.EsCiclo;
            chkEstado.Checked = EditObject.EsEstado;
            txtCustom1.Text = EditObject.Custom1;
            txtCustom2.Text = EditObject.Custom2;
            txtCustom3.Text = EditObject.Custom3;

            BindDetalles(EditObject.Detalles.OfType<DetalleCiclo>().Where(detalle => !detalle.Baja).OrderBy(detalle => detalle.Orden).ToList());
        }

        private void BindDetalles(IEnumerable<DetalleCiclo> detalles)
        {
            foreach (var t in detalles)
            {
                var control = LoadControl("~/CicloLogistico/CicloDetalle.ascx") as ControlDetalleCiclo;

                if (control == null) continue;

                control.ID = "Detalle_" + AddIndex();
                control.ParentControls = "cbLinea";

                control.Deleted += control_Deleted;

                panelEstados.Controls.Add(control);

                control.SetDetalle(t);
            }
        }

        protected override void OnDelete()
        {
            EditObject.Baja = true;

            DAOFactory.CicloLogisticoDAO.SaveOrUpdate(EditObject);
        }

        protected override string GetRefference() { return "CICLO_LOGISTICO"; }

        protected override void OnSave()
        {
            var empresa = EditObject.Owner != null ? EditObject.Owner.Empresa != null ? EditObject.Owner.Empresa.Id : -1 : -1;
            var linea = EditObject.Owner != null ? EditObject.Owner.Linea != null ? EditObject.Owner.Linea.Id : -1 : -1;

            if (!CanEditEmpresa(empresa) || !CanEditLinea(linea)) throw new ApplicationException(SinPermisos);

            EditObject.Owner = new Owner();

            if (cbLinea.Selected > 0)
            {
                EditObject.Owner.Linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
                EditObject.Owner.Empresa = EditObject.Owner.Linea.Empresa;
            }
            else if (cbEmpresa.Selected > 0)
            {
                EditObject.Owner.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            }

            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Custom1 = txtCustom1.Text.Trim();
            EditObject.Custom2 = txtCustom2.Text.Trim();
            EditObject.Custom3 = txtCustom3.Text.Trim();
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.EsCiclo = chkCiclo.Checked;
            EditObject.EsEstado = chkEstado.Checked;

            var detalles = GetListDetalles();
            var current = EditObject.Detalles.OfType<DetalleCiclo>().ToDictionary(detalle => detalle.Id, detalle => detalle);

            foreach (var detalleCiclo in current.Values) detalleCiclo.Baja = true;

            foreach (var detalle in detalles)
            {
                if (current.ContainsKey(detalle.Id))
                {
                    var cur = current[detalle.Id];
                    cur.Codigo = detalle.Codigo;
                    cur.Descripcion = detalle.Descripcion;
                    cur.EstadoCicloLogistico = detalle.EstadoCicloLogistico;
                    cur.Mensaje = detalle.Mensaje;
                    cur.MensajeControl = detalle.MensajeControl;
                    cur.Minutos = detalle.Minutos;
                    cur.Obligatorio = detalle.Obligatorio;
                    cur.ObligatorioControl = detalle.ObligatorioControl;
                    cur.Orden = detalle.Orden;
                    cur.ReferenciaGeografica = detalle.ReferenciaGeografica;
                    cur.Repeticion = detalle.Repeticion;
                    cur.Tipo = detalle.Tipo;
                    cur.MensajeControl = detalle.MensajeControl;
                    cur.ObligatorioControl = detalle.ObligatorioControl;
                    cur.Duracion = detalle.Duracion;
                    cur.Baja = false;
                    //EditObject.Detalles.Add(cur);
                }
                else
                {
                    detalle.Id = 0;
                    detalle.CicloLogistico = EditObject;
                    EditObject.Detalles.Add(detalle);
                }
            }

            DAOFactory.CicloLogisticoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text.Trim())) throw new ApplicationException("Debe ingresar la descripcion");

            if (string.IsNullOrEmpty(txtCodigo.Text.Trim()))
                throw new ApplicationException("Debe ingresar el codigo");

            if (!chkCiclo.Checked && !chkEstado.Checked)
                throw new ApplicationException("El ciclo logistico debe ser de tipo Ciclo o Estado");

            var ciclo = DAOFactory.CicloLogisticoDAO.GetByCodigo(cbEmpresa.Selected, cbLinea.Selected, txtCodigo.Text.Trim());
            if (ciclo == null) return;
            if (!EditMode || (EditMode && !EditObject.Id.Equals(ciclo.Id)))
                throw new ApplicationException("Ya existe un ciclo con el mismo codigo");

        }

        protected void btAgregarEstado_Click(object sender, EventArgs e)
        {
            var index = AddIndex();
            var count = panelEstados.Controls.OfType<ControlDetalleCiclo>().Count();

            var control = LoadControl("~/CicloLogistico/CicloDetalle.ascx") as ControlDetalleCiclo;

            if (control == null) return;

            control.ID = "Detalle_" + index;
            control.ParentControls = "cbLinea";

            panelEstados.Controls.Add(control);

            control.Deleted += control_Deleted;
            control.SetDetalle(new DetalleCiclo { Id = -index, Orden = (short)(count + 1) });
        }

        #endregion

        #region Private Methods

        private void control_Deleted(object sender, EventArgs e)
        {
            var detalle = sender as ControlDetalleCiclo;
            var controls = panelEstados.Controls.OfType<ControlDetalleCiclo>().ToList();

            for (var i = 0; i < controls.Count; i++)
            {
                if (detalle != controls[i]) continue;

                panelEstados.Controls.Remove(detalle);

                Indices.RemoveAt(i);
            }

            UpdateOrders();
        }

        private int AddIndex()
        {
            var indices = Indices;
            var next = indices.Count == 0 ? 0 : indices[indices.Count - 1] + 1;

            indices.Add(next);
            Indices = indices;

            return next;
        }

        private void UpdateOrders()
        {
            short i = 1;

            foreach (var ctl in panelEstados.Controls.OfType<ControlDetalleCiclo>()) ctl.SetOrden(i++);
        }

        private void LoadDetalles()
        {
            foreach (var control in Indices.Select(t => CreateControl(t)))
            {
                control.Deleted += control_Deleted;

                panelEstados.Controls.Add(control);
            }
        }

        private ControlDetalleCiclo CreateControl(int t)
        {
            var control = LoadControl("~/CicloLogistico/CicloDetalle.ascx") as ControlDetalleCiclo;
            control.ID = "Detalle_" + t;
            control.ParentControls = "cbLinea";

            return control;
        }

        private IEnumerable<DetalleCiclo> GetListDetalles() { return panelEstados.Controls.OfType<ControlDetalleCiclo>().Select(ctrl => ctrl.Detalle).ToList(); }

        #endregion
    }
}