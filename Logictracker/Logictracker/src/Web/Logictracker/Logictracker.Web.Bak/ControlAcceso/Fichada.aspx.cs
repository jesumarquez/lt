using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.ControlAcceso;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.Controls;
using Logictracker.Web.CustomWebControls.Buttons;
using AjaxControlToolkit;
using Image = System.Web.UI.WebControls.Image;

namespace Logictracker.ControlAcceso
{
    public partial class Fichada : SecuredGridReportPage<FichadaVo>
    {
        protected override string VariableName { get { return "AC_ADMINISTRADOR"; } }
        protected override string GetRefference() { return "AC_ADMINISTRADOR"; }
        public override bool MouseOverRowEffect { get { return true; } }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }
        public override bool SelectableRows { get { return false; } }
        protected override bool HideSearch { get { return true; } }
        protected override bool ExcelButton { get { return true; } }

        protected VsProperty<SearchData> FilterData { get { return this.CreateVsProperty<SearchData>("FilterData", null); } } 

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (C1GridViewRow row in Grid.Rows)
            {
                CreateRowTemplate(row);
            }
        }
        protected void SetContextKey()
        {
            var data = FilterData.Get();
            txtEditEmpleados.ContextKey = AutoCompleteTextBox.CreateContextKey(new[] { data.Empresa },
                                                                               new[] { data.Linea },
                                                                               data.TiposEmpleados.ToArray(),
                                                                               new[] { -1 },
                                                                               data.CentrosCostos.ToArray(),
                                                                               data.Departamentos.ToArray());
        }

        private bool changedEmpresa;
        protected void cbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            changedEmpresa = true;
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (changedEmpresa)
            {
                var now = DateTime.UtcNow;
                var periodoActual = Enumerable.FirstOrDefault<ListItem>((from ListItem li in cbPeriodo.Items
                                                                          let periodo = DAOFactory.PeriodoDAO.FindById(Convert.ToInt32((string) li.Value))
                                                                          where now >= periodo.FechaDesde && now < periodo.FechaHasta
                                                                          select li));
                if (periodoActual != null)
                {
                    cbPeriodo.SelectedIndex = -1;
                    periodoActual.Selected = true;
                    cbPeriodo_SelectedIndexChanged(cbPeriodo, e);
                }
            }
        }
        protected override List<FichadaVo> GetResults()
        {
            var data = SearchData.Create(this);
            var eventos = DAOFactory.EventoAccesoDAO.GetList(new[]{data.Empresa}, 
                                                             new[]{data.Linea}, 
                                                             data.CentrosCostos, 
                                                             data.TiposEmpleados, 
                                                             new[]{data.Empleado}, 
                                                             data.Puertas, 
                                                             data.Desde, 
                                                             data.Hasta);

            if (txtLegajo.Text.Trim() != string.Empty)
                eventos = eventos.Where(e => e.Empleado.Legajo.Trim().Equals(txtLegajo.Text.Trim())).ToList();

            var list = new List<FichadaVo>();
            FichadaVo current = null;
            int lastEmpleado = 0;
            foreach (var evento in eventos)
            {
                if(!chkEliminados.Checked && evento.Baja.HasValue) continue;
                if (lastEmpleado != evento.Empleado.Id) current = null;
                var fecha = evento.Fecha.ToDisplayDateTime();

                if (current == null)
                {
                    current = new FichadaVo(evento);
                    list.Add(current);
                }
                else
                {
                    if (current.HasSalida)
                    {
                        if (!evento.Entrada && fecha.Subtract(current.HoraSalida).TotalMinutes < data.Duracion)
                        {
                            current.SetEvento(evento);
                        }
                        else
                        {
                            current = new FichadaVo(evento);
                            list.Add(current);
                        }
                    }
                    else if (current.HasEntrada && evento.Entrada)
                    {
                        if (fecha.Subtract(current.HoraEntrada).TotalMinutes > data.Duracion)
                        {
                            current = new FichadaVo(evento);
                            list.Add(current);
                        }
                    }
                    else
                    {
                        current.SetEvento(evento);
                    }
                }

                lastEmpleado = evento.Empleado.Id;
            }

            if (chkHuerfanos.Checked) list = list.Where(f => !f.HasEntrada || !f.HasSalida).ToList();

            var module = Usuario.Modules[GetRefference()];
            if(module.Add) btNuevo.Visible = true;

            FilterData.Set(data);

            return list;
        }
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, FichadaVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");

            var cellEntrada = e.Row.Cells[GridUtils.GetColumnIndex(FichadaVo.IndexHoraEntrada)];
            if (dataItem.HasEntrada)
            {
                cellEntrada.Text = dataItem.HoraEntrada.ToString();
                CellEdit(cellEntrada, dataItem.IdEntrada.ToString(), true, dataItem.EntradaDeleted, dataItem.EntradaEdited);    
            }
            else
            {
                cellEntrada.Text = string.Empty;
            }

            var cellSalida = e.Row.Cells[GridUtils.GetColumnIndex(FichadaVo.IndexHoraSalida)];
            if (dataItem.HasSalida)
            {
                cellSalida.Text = dataItem.HoraSalida.ToString();
                CellEdit(cellSalida, dataItem.IdSalida.ToString(), false, dataItem.SalidaDeleted, dataItem.SalidaEdited);
            }
            else
            {
                cellSalida.Text = string.Empty;
            }
            var cellDuracion = e.Row.Cells[GridUtils.GetColumnIndex(FichadaVo.IndexDuracionJornada)];
            TimeSpan duracion;
            if(TimeSpan.TryParse(cellDuracion.Text, out duracion)) { cellDuracion.Text = FormatTimeSpan(duracion); }

            var cell = e.Row.Cells[GridUtils.GetColumnIndex(FichadaVo.IndexEdit)];
            EditButton(cell, dataItem.IdEntrada.ToString(), dataItem.IdSalida.ToString());
        }
        protected override string OnCustomFormatString(Logictracker.Web.BaseClasses.Util.AggregateHandler aggregate, Type type, double value)
        {
            if (type == typeof(TimeSpan)) { return FormatTimeSpan(TimeSpan.FromMinutes(value)); }
            return base.OnCustomFormatString(aggregate, type, value);
        }
        protected override string CustomExportFormat(Type type, object value)
        {
            TimeSpan duracion;
            if (type == typeof(TimeSpan) && TimeSpan.TryParse(value.ToString(), out duracion))
            {
                return FormatTimeSpan(duracion);
            }
            return base.CustomExportFormat(type, value);
        }
        private string FormatTimeSpan(TimeSpan value)
        {
            return string.Format("{0}:{1:00}", Math.Truncate(value.TotalHours), value.Minutes);
        }
        private void EditButton(TableCell cell, string idEntrada, string idSalida)
        {
            var module = Usuario.Modules[GetRefference()];
            var pan = cell.FindControl("panEdit") as Panel;
            
            if (pan == null && module.Edit)
            {
                pan = new Panel { ID = "panEdit" };
                var btEdit2 = new ResourceLinkButton { ID = "btEdit", ResourceName = "Controls", VariableName = "BUTTON_EDIT", CommandName="Editar", CommandArgument= idEntrada + ":" + idSalida, CausesValidation = false};
                pan.Controls.Add(btEdit2);
                cell.Controls.Add(pan);
            }
            var btEdit = cell.FindControl("btEdit") as IButtonControl;
            if (btEdit != null) btEdit.Command += btEdit_Command;
        }

        
        private void CellEdit(TableCell cell, string id, bool entrada, bool deleted, bool modified)
        {
            var idPanelText = entrada ? "panelText" : "panelText2";
            var idPanelButton = entrada ? "panelButton" : "panelButton2";
            var idExt = entrada ? "ext" : "ext2";
            var idBtAction = entrada ? "btAction" : "btAction2";

            var module = Usuario.Modules[GetRefference()];

            var panelText = cell.FindControl(idPanelText) as Panel;
            var panelButton = cell.FindControl(idPanelButton) as Panel;
            var ext = cell.FindControl(idExt) as DropDownExtender;
            
            if(panelText == null)
            {
                panelText = new Panel { ID = idPanelText };
                if (deleted)
                {
                    var img = new Image { ImageUrl = "~/images/delete.png", ImageAlign = ImageAlign.AbsBottom };
                    panelText.Controls.Add(img);
                    cell.ForeColor = Color.DarkRed;
                }
                else if (modified)
                {
                    var img = new Image { ImageUrl = "~/images/page_white_edit.png", ImageAlign = ImageAlign.AbsBottom };
                    panelText.Controls.Add(img);
                    cell.ForeColor = Color.DarkBlue;
                }

                panelText.Controls.Add(new Literal { Text = " " + cell.Text });
                cell.Controls.Add(panelText);
            }

            if (panelButton == null && module.Delete)
            {
                panelButton = new Panel { ID = idPanelButton };
                var btRestore = new Button { ID = idBtAction, CommandName = deleted ? "Restore" : "Delete", CommandArgument = id, Text = deleted ? "Restaurar" : "Eliminar", OnClientClick = string.Format("return confirm(\"{0}\");", CultureManager.GetSystemMessage("CONFIRM_OPERATION")) };
                panelButton.Controls.Add(btRestore);
                cell.Controls.Add(panelButton);
            }

            if (ext == null && module.Edit || module.Delete)
            {
                ext = new DropDownExtender { ID = idExt, TargetControlID = panelText.ID, DropDownControlID = panelButton.ID };
                cell.Controls.Add(ext);
            }

            var btAction = cell.FindControl(idBtAction) as Button;
            if(btAction != null) btAction.Command += btRestore_Command;
        }

        void btRestore_Command(object sender, CommandEventArgs e)
        {
            var id = Convert.ToInt32(e.CommandArgument);
            var evento = DAOFactory.EventoAccesoDAO.FindById(id);

            if (e.CommandName == "Restore")
            {
                evento.Baja = null;
                evento.Modificado = DateTime.Now;
                evento.Usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            }
            else if(e.CommandName == "Delete")
            {
                evento.Baja = DateTime.UtcNow;
                evento.Usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            }
            DAOFactory.EventoAccesoDAO.SaveOrUpdate(evento);
            
            ReBind();
        }
        
        void btEdit_Command(object sender, CommandEventArgs e)
        {           
            if(e.CommandName != "Editar") return;

            var ids = e.CommandArgument.ToString().Split(':');
            var idEntrada = Convert.ToInt32(ids[0]);
            var idSalida = Convert.ToInt32(ids[1]);

            Edit(idEntrada, idSalida);
        }
        private void Edit(int idEntrada, int idSalida)
        {
            popupEdit.Show();
            ResetFilters();
            ResetEditFilters();

            txtEditEmpleados.Enabled = true;

            if (idEntrada > 0)
            {
                var entrada = DAOFactory.EventoAccesoDAO.FindById(idEntrada);
                txtEditEmpleados.Selected = entrada.Empleado.Id;
                txtEditEmpleados.Text = entrada.Empleado.Entidad.Descripcion;
                cbEditPuertaEntrada.SetSelectedValue(entrada.Puerta.Id);
                dtEditHoraEntrada.SelectedDate = entrada.Fecha.ToDisplayDateTime();
                txtEditEmpleados.Enabled = false;
            }
            else
            {
                cbEditPuertaEntrada.SetSelectedValue(cbEditPuertaEntrada.NoneValue);
                dtEditHoraEntrada.SelectedDate = null;
            }
            if (idSalida > 0)
            {
                var salida = DAOFactory.EventoAccesoDAO.FindById(idSalida);
                txtEditEmpleados.Selected = salida.Empleado.Id;
                txtEditEmpleados.Text = salida.Empleado.Entidad.Descripcion;
                cbEditPuertaSalida.SetSelectedValue(salida.Puerta.Id);
                dtEditHoraSalida.SelectedDate = salida.Fecha.ToDisplayDateTime();
                txtEditEmpleados.Enabled = false;
            }
            else
            {
                cbEditPuertaSalida.SetSelectedValue(cbEditPuertaEntrada.NoneValue);
                dtEditHoraSalida.SelectedDate = null;
            }
            btAceptar.CommandArgument =idEntrada + ":" + idSalida;
            btAceptar.OnClientClick = string.Format("return confirm(\"{0}\");", CultureManager.GetSystemMessage("CONFIRM_OPERATION"));
        }
        protected void btAceptar_Command(object sender, CommandEventArgs e)
        {
            var ids = e.CommandArgument.ToString().Split(':');
            var idEntrada = Convert.ToInt32(ids[0]);
            var idSalida = Convert.ToInt32(ids[1]);

            var puertaEntrada = cbEditPuertaEntrada.Selected;
            var horaEntrada = dtEditHoraEntrada.SelectedDate;

            var puertaSalida = cbEditPuertaSalida.Selected;
            var horaSalida = dtEditHoraSalida.SelectedDate;

            UpdateEventoAcceso(idEntrada, puertaEntrada, horaEntrada, true);
            UpdateEventoAcceso(idSalida, puertaSalida, horaSalida, false);

            ReBind();
        }
        protected void btNuevo_Click(object sender, EventArgs e)
        {
            Edit(0, 0);
        }
        private bool UpdateEventoAcceso(int id, int puerta, DateTime? hora, bool entrada)
        {
            if (puerta <= 0 || !hora.HasValue) return false;

            hora = hora.Value.ToDataBaseDateTime();

            var evento = id > 0 ? DAOFactory.EventoAccesoDAO.FindById(id) : new EventoAcceso();
            var empleado = txtEditEmpleados.Selected;

            if (empleado <= 0) return false;
            if(id > 0 && evento.Puerta.Id == puerta && evento.Fecha == hora.Value) return false;

            if (id <= 0)
            {
                evento.Alta = DateTime.UtcNow;
                evento.Empleado = DAOFactory.EmpleadoDAO.FindById(empleado);
                evento.Entrada = entrada;
            }

            evento.Fecha = hora.Value;
            evento.Puerta = DAOFactory.PuertaAccesoDAO.FindById(puerta);
            evento.Modificado = DateTime.UtcNow;
            evento.Usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            DAOFactory.EventoAccesoDAO.SaveOrUpdate(evento);

            return true;
        }

        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            var idEntrada = Grid.DataKeys[row.RowIndex].Values[FichadaVo.IndexKeyIdEntrada].ToString();
            var cellEntrada = row.Cells[GridUtils.GetColumnIndex(FichadaVo.IndexHoraEntrada)];
            var entradaDeleted = Convert.ToBoolean(Grid.DataKeys[row.RowIndex].Values[FichadaVo.IndexKeyEntradaDeleted]);
            var entradaEdited = Convert.ToBoolean(Grid.DataKeys[row.RowIndex].Values[FichadaVo.IndexKeyEntradaEdited]);
            CellEdit(cellEntrada, idEntrada, true, entradaDeleted, entradaEdited);

            var idSalida = Grid.DataKeys[row.RowIndex].Values[FichadaVo.IndexKeyIdSalida].ToString();
            var cellSalida = row.Cells[GridUtils.GetColumnIndex(FichadaVo.IndexHoraSalida)];
            var salidaDeleted = Convert.ToBoolean(Grid.DataKeys[row.RowIndex].Values[FichadaVo.IndexKeySalidaDeleted]);
            var salidaEdited = Convert.ToBoolean(Grid.DataKeys[row.RowIndex].Values[FichadaVo.IndexKeySalidaEdited]);
            CellEdit(cellSalida, idSalida, false, salidaDeleted, salidaEdited);

            var cellEdit = row.Cells[GridUtils.GetColumnIndex(FichadaVo.IndexEdit)];
            EditButton(cellEdit, idEntrada, idSalida);
        }

        protected void cbPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbPeriodo.Selected <= 0) return;
            var periodo = DAOFactory.PeriodoDAO.FindById(cbPeriodo.Selected);
            dtDesde.SelectedDate = periodo.FechaDesde;
            dtHasta.SelectedDate = periodo.FechaHasta;
        }
        private void ReBind()
        {
            ResetFilters();
            Bind();
        }
        private void ResetFilters()
        {
            var data = FilterData.Get();
            if (data == null) return;
            cbEmpresa.SetSelectedValue(data.Empresa);
            cbLinea.SetSelectedValue(data.Linea);
            cbCentroDeCostos.SelectedValues = data.CentrosCostos;
            cbDepartamento.SelectedValues = data.Departamentos;
            cbTipoEmpleado.SelectedValues = data.TiposEmpleados;
            txtEmpleado.Text = data.TextEmpleado;
            txtEmpleado.Selected = data.Empleado;
            //cbEmpleado.SelectedValues = data.Empleados;
            cbPuerta.SelectedValues = data.Puertas;
            cbPeriodo.SetSelectedValue(data.Periodo);
            dtDesde.SelectedDate = data.Desde;
            dtHasta.SelectedDate = data.Hasta;
            txtDuracion.Text = data.Duracion.ToString("0");
            updFiltros.Update();
        }
        private void ResetEditFilters()
        {
            var data = FilterData.Get();
            if (data == null) return;
            SetContextKey();
            updEdit.Update();
        }

        [Serializable]
        public class SearchData
        {
            public int Empresa { get; set; }
            public int Linea { get; set; }
            public List<int> CentrosCostos { get; set; }
            public List<int> TiposEmpleados { get; set; }
            public List<int> Departamentos { get; set; }
            public int Empleado { get; set; }
            public string TextEmpleado { get; set; }
            public List<int> Puertas { get; set; }
            public int Periodo { get; set; }
            public DateTime Desde { get; set; }
            public DateTime Hasta { get; set; }
            public int Duracion { get; set; }

            public static SearchData Create(Fichada page)
            {
                int duracion;
                int.TryParse(page.txtDuracion.Text, out duracion);
                var sd = new SearchData
                             {
                                 Empresa = page.cbEmpresa.Selected,
                                 Linea = page.cbLinea.Selected,
                                 CentrosCostos = page.cbCentroDeCostos.SelectedValues,
                                 TiposEmpleados = page.cbTipoEmpleado.SelectedValues,
                                 Departamentos = page.cbDepartamento.SelectedValues,
                                 Empleado = page.txtEmpleado.Selected,
                                 TextEmpleado = page.txtEmpleado.Text,
                                 Puertas = page.cbPuerta.SelectedValues,
                                 Desde = SecurityExtensions.ToDataBaseDateTime(page.dtDesde.SelectedDate.Value),
                                 Hasta = SecurityExtensions.ToDataBaseDateTime(page.dtHasta.SelectedDate.Value),
                                 Duracion = duracion,
                                 Periodo = page.cbPeriodo.Selected
                             };

                return sd;
            }
        }
    }
}