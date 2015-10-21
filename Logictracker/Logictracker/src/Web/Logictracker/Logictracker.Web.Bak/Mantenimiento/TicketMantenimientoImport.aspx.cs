using System;
using System.Collections.Generic;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class TicketMantenimientoImport : SecuredImportPage
    {
        private const string Reporte = "Procesado: {0} de {1}<br/> Importados: {2} ({3}%)<br/>Erroneos: {4} ({5}%))<br/>Tiempo: {6}<br/>";
        private const string Result = "{0};{1};{2}<br/>";
        protected override bool Redirect { get { return false; } }
        protected override string RedirectUrl { get { return "TicketMantenimientoLista.aspx"; } }
        protected override string VariableName { get { return "MANT_TICKET"; } }
        protected override string GetRefference() { return "MANT_TICKET"; }

        protected override List<FieldValue> GetMappingFields() { return Fields.List; }
        private static List<FieldValue> GetFieldsObligatorios() { return Fields.Obligatorios; }

        protected override void ValidateMapping()
        {
            var fields = GetFieldsObligatorios();
            for(var i = 1; i < fields.Count; i++)
            {
                var field = fields[i];
                if(!IsMapped(field.Value) && !HasDefault(field.Value)) throw new ApplicationException("Falta mapear " + field.Name);
            }
        }

        protected override void Import(List<ImportRow> rows)
        {
            var procesados = 0;
            var erroneos = 0;
            var index = 0;
            var start = DateTime.Now.Ticks;
            lblResult.Text = "";

            foreach (var row in rows)
            {
                try
                {
                    index++;

                    var codComercio = GetValue(row, Fields.CodComercio.Value).Trim();
                    var nombreComercio = GetValue(row, Fields.NombreComercio.Value).Trim();
                    if (codComercio == string.Empty)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "COD. TALLER = VACIO");
                        continue;
                    }
                    var taller = DAOFactory.TallerDAO.GetByCode(codComercio);
                    if (taller == null)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "TALLER INEXISTENTE: " + codComercio + " - " + nombreComercio);
                        continue;
                    }

                    var nroCliente = GetValue(row, Fields.NroCliente.Value).Trim();
                    var razonSocial = GetValue(row, Fields.RazonSocial.Value).Trim();
                    if (nroCliente == string.Empty)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "COD. CLIENTE = VACIO");
                        continue;
                    }
                    var empresa = DAOFactory.EmpresaDAO.FindByCodigo(nroCliente);
                    if (empresa == null)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "CLIENTE INEXISTENTE: " + nroCliente + " - " + razonSocial);
                        continue;
                    }

                    var patente = GetValue(row, Fields.Patente.Value).Trim();
                    if (patente == string.Empty)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "PATENTE = VACIO");
                        continue;
                    }
                    var vehiculo = DAOFactory.CocheDAO.FindByPatente(empresa.Id, patente);
                    if (vehiculo == null)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "VEHICULO INEXISTENTE: " + patente);
                        continue;
                    }

                    var empleado = GetValue(row, Fields.Empleado.Value).Trim();
                    var responsable = empleado != string.Empty
                                          ? DAOFactory.EmpleadoDAO.FindByLegajo(empresa.Id, -1, empleado)
                                          : null;

                    var descripcion = GetValue(row, Fields.Descripcion.Value).Trim();
                    if (descripcion == string.Empty)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "NO EXISTE DESCRIPCION");
                        continue;
                    }
                    var solicitud = GetValue(row, Fields.Solicitud.Value).Trim();
                    var nroPresupuesto = GetValue(row, Fields.NroPresupuesto.Value).Trim();
                    var monto = GetValue(row, Fields.Monto.Value).Trim();
                    double valor;
                    double.TryParse(monto, out valor);

                    var ingresoSolicitud = GetValue(row, Fields.IngresoSolicitud.Value).Trim();
                    if (ingresoSolicitud == string.Empty)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "SIN FECHA DE INGRESO DE SOLICITUD");
                        continue;
                    }
                    var dtIngresoSolicitud = DateTime.Parse(ingresoSolicitud).ToDataBaseDateTime();

                    var fechaTurno = GetValue(row, Fields.FechaTurno.Value).Trim();
                    DateTime dtFt;
                    var dtFechaTurno = DateTime.TryParse(fechaTurno, out dtFt) ? dtFt.ToDataBaseDateTime() : (DateTime?)null;

                    var fechaRecepcion = GetValue(row, Fields.FechaRecepcion.Value).Trim();
                    DateTime dtRec;
                    var dtFechaRecepcion = DateTime.TryParse(fechaRecepcion, out dtRec) ? dtRec.ToDataBaseDateTime() : (DateTime?)null;

                    var primerPresupuesto = GetValue(row, Fields.PrimerPresupuesto.Value).Trim();
                    var fechaPrimerPresupuesto = GetValue(row, Fields.FechaPrimerPresupuesto.Value).Trim();
                    DateTime dtPrimerPres;
                    var dtPromerPresupuesto = DateTime.TryParse(fechaPrimerPresupuesto, out dtPrimerPres) ? dtPrimerPres.ToDataBaseDateTime() : (DateTime?)null;

                    var fechaPresupuestada = GetValue(row, Fields.FechaPresupuestada.Value).Trim();
                    DateTime dtPres;
                    var dtFechaPresupuestada = DateTime.TryParse(fechaPresupuestada, out dtPres) ? dtPres.ToDataBaseDateTime() : (DateTime?)null;

                    var fechaRecotizacion = GetValue(row, Fields.FechaRecotizacion.Value).Trim();
                    DateTime dtRecot;
                    var dtFechaRecotizacion = DateTime.TryParse(fechaRecotizacion, out dtRecot) ? dtRecot.ToDataBaseDateTime() : (DateTime?)null;
                    
                    var fechaVerificacion = GetValue(row, Fields.FechaVerificacion.Value).Trim();
                    DateTime dtVerif;
                    var dtFechaVerificacion = DateTime.TryParse(fechaVerificacion, out dtVerif) ? dtVerif.ToDataBaseDateTime() : (DateTime?)null;

                    var fechaAprobacion = GetValue(row, Fields.FechaAprobacion.Value).Trim();
                    DateTime dtAprob;
                    var dtFechaAprobacion = DateTime.TryParse(fechaAprobacion, out dtAprob) ? dtAprob.ToDataBaseDateTime() : (DateTime?)null;

                    var fechaTrabajoTerminado = GetValue(row, Fields.FechaTrabajoTerminado.Value).Trim();
                    DateTime dtTrabajoTer;
                    var dtFechaTrabajoTerminado = DateTime.TryParse(fechaTrabajoTerminado, out dtTrabajoTer) ? dtTrabajoTer.ToDataBaseDateTime() : (DateTime?)null;

                    var fechaEntrega = GetValue(row, Fields.FechaEntrega.Value).Trim();
                    DateTime dtEnt;
                    var dtFechaEntrega = DateTime.TryParse(fechaEntrega, out dtEnt) ? dtEnt.ToDataBaseDateTime() : (DateTime?)null;

                    var fechaTrabajoAceptado = GetValue(row, Fields.FechaTrabajoAceptado.Value).Trim();
                    DateTime dtTrabajoAcep;
                    var dtFechaTrabajoAceptado = DateTime.TryParse(fechaTrabajoAceptado, out dtTrabajoAcep) ? dtTrabajoAcep.ToDataBaseDateTime() : (DateTime?)null;

                    var estSolicitud = GetValue(row, Fields.EstadoSolicitud.Value).Trim();
                    if (estSolicitud == string.Empty)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, "SIN ESTADO DE SOLICITUD");
                        continue;
                    }
                    var estadoSolicitud = -1;
                    switch (estSolicitud)
                    {
                        case "INGRESADA": estadoSolicitud = TicketMantenimiento.EstadosTicket.Ingresado; break;
                        case "APROBADA": estadoSolicitud = TicketMantenimiento.EstadosTicket.Aprobado; break;
                        case "TERMINADA": estadoSolicitud = TicketMantenimiento.EstadosTicket.Terminado; break;
                        case "TRABAJO ACEPTADO": estadoSolicitud = TicketMantenimiento.EstadosTicket.Aceptado; break;
                        case "TRABAJO NO ACEPTADO": estadoSolicitud = TicketMantenimiento.EstadosTicket.NoAceptado; break;
                        case "CANCELADA": estadoSolicitud = TicketMantenimiento.EstadosTicket.Cancelado; break;
                        default:
                            {
                                erroneos++;
                                Log("FILA ERRONEA", index, "ESTADO DE SOLICITUD NO ENCONTRADO: " + estadoSolicitud);
                                continue;
                            }
                    }

                    var estPresupuesto = GetValue(row, Fields.EstadoPresupuesto.Value).Trim();
                    var estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.SinPresupuesto;
                    switch (estPresupuesto)
                    {
                        case "TRABAJO ACEPT. CLIENTE": estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.AceptadoCliente; break;
                        case "APROBADO": estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.Aprobado; break;
                        case "PRESUPUESTADA": estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.Presupuestado; break;
                        case "PEDIDO RECOTIZADO": estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.Recotizado; break;
                        case "TRABAJO TERMINADO": estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.Terminado; break;
                        case "VERIF. SIN APROBAR": estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.VerificadoSinAprobar; break;
                        case "CANCELADO": estadoPresupuesto = TicketMantenimiento.EstadosPresupuesto.Cancelado; break;
                    }

                    var nivelComp = GetValue(row, Fields.NivelComplejidad.Value).Trim();
                    var nivelComplejidad = TicketMantenimiento.NivelesComplejidad.Baja;
                    switch (nivelComp)
                    {
                        case "Media": nivelComplejidad = TicketMantenimiento.NivelesComplejidad.Media; break;
                        case "Alta": nivelComplejidad = TicketMantenimiento.NivelesComplejidad.Alta; break;
                        case "Muy Alta": nivelComplejidad = TicketMantenimiento.NivelesComplejidad.MuyAlta; break;
                    }

                    var ticket = GetTicket(empresa, solicitud);

                    ticket.Taller = taller;
                    ticket.Vehiculo = vehiculo;
                    ticket.Empleado = responsable;
                    ticket.Descripcion = descripcion;
                    ticket.Presupuesto = nroPresupuesto;
                    ticket.Monto = valor;
                    ticket.FechaSolicitud = dtIngresoSolicitud;
                    ticket.FechaTurno = dtFechaTurno;
                    ticket.FechaRecepcion = dtFechaRecepcion;
                    ticket.PrimerPresupuesto = primerPresupuesto;
                    ticket.FechaPresupuestoOriginal = dtPromerPresupuesto;
                    ticket.FechaPresupuestada = dtFechaPresupuestada;
                    ticket.FechaRecotizacion = dtFechaRecotizacion;
                    ticket.FechaVerificacion = dtFechaVerificacion;
                    ticket.FechaAprobacion = dtFechaAprobacion;
                    ticket.FechaTrabajoTerminado = dtFechaTrabajoTerminado;
                    ticket.FechaEntrega = dtFechaEntrega;
                    ticket.FechaTrabajoAceptado = dtFechaTrabajoAceptado;
                    ticket.Estado = (short)estadoSolicitud;
                    ticket.EstadoPresupuesto = estadoPresupuesto;
                    ticket.NivelComplejidad = nivelComplejidad;

                    var historia = new HistoriaTicketMantenimiento
                                       {
                                           Codigo = ticket.Codigo,
                                           Descripcion = ticket.Descripcion,
                                           Empresa = ticket.Empresa,
                                           Empleado = ticket.Empleado,
                                           Estado = ticket.Estado,
                                           EstadoPresupuesto = ticket.EstadoPresupuesto,
                                           Fecha = DateTime.UtcNow,
                                           FechaAprobacion = ticket.FechaAprobacion,
                                           FechaEntrega = ticket.FechaEntrega,
                                           FechaPresupuestada = ticket.FechaPresupuestada,
                                           FechaPresupuestoOriginal = ticket.FechaPresupuestoOriginal,
                                           FechaRecepcion = ticket.FechaRecepcion,
                                           FechaRecotizacion = ticket.FechaRecotizacion,
                                           FechaSolicitud = ticket.FechaSolicitud,
                                           FechaTrabajoAceptado = ticket.FechaTrabajoAceptado,
                                           FechaTrabajoTerminado = ticket.FechaTrabajoTerminado,
                                           FechaTurno = ticket.FechaTurno,
                                           FechaVerificacion = ticket.FechaVerificacion,
                                           Monto = ticket.Monto,
                                           NivelComplejidad = ticket.NivelComplejidad,
                                           Presupuesto = ticket.Presupuesto,
                                           PrimerPresupuesto = ticket.PrimerPresupuesto,
                                           Taller = ticket.Taller,
                                           TicketMantenimiento = ticket,
                                           Usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id),
                                           Vehiculo = ticket.Vehiculo
                                       };

                    ticket.Historia.Add(historia);

                    DAOFactory.TicketMantenimientoDAO.SaveOrUpdate(ticket);

                    procesados++;
                }
                catch (Exception)
                {
                    erroneos++;
                    Log("FILA ERRONEA", index, string.Empty);
                }
            }
            const int totalWidth = 200;
            var percent = index*100/rows.Count;
            litProgress.Text =string.Format(
                    @"<div style='margin: auto; border: solid 1px #999999; background-color: #FFFFFF; width: {0}px; height: 10px;'>
                        <div style='background-color: #0000AA; width: {1}px; height: 10px; font-size: 8px; color: #CCCCCC;'>{2}%</div>
                    </div>",
                    totalWidth, percent*totalWidth/100, percent);
            lblDirs.Text = string.Format(Reporte,
                                         procesados + erroneos,
                                         rows.Count,
                                         procesados,
                                         rows.Count > 0 ? (procesados*100.0/rows.Count).ToString("0.00") : "0.00",
                                         erroneos,
                                         rows.Count > 0 ? (erroneos*100.0/rows.Count).ToString("0.00") : "0.00",
                                         TimeSpan.FromTicks(DateTime.Now.Ticks - start));
            panelProgress.Visible = true;
        }

        private TicketMantenimiento GetTicket(Empresa empresa, string codigo)
        {
            if (codigo != string.Empty)
            {
                var t = DAOFactory.TicketMantenimientoDAO.GetByCode(empresa.Id, codigo);
                if (t != null) return t;
            }

            return new TicketMantenimiento { Empresa = empresa, Codigo = codigo};
        }

        private void Log(string status, int index, string motivo)
        {
            lblResult.Text += string.Format(Result, status, " INDICE: " + index, " " + motivo);
        }

        #region SubClasses

        private static class Fields
        {
            public static readonly FieldValue NroCliente = new FieldValue("NroCliente");
            public static readonly FieldValue Descripcion = new FieldValue("Descripción");
            public static readonly FieldValue Solicitud = new FieldValue("Solicitud");
            public static readonly FieldValue Patente = new FieldValue("Patente");
            public static readonly FieldValue Empleado = new FieldValue("Empleado");
            public static readonly FieldValue NroPresupuesto = new FieldValue("Nro. Presupuesto");
            public static readonly FieldValue Monto = new FieldValue("Monto");
            public static readonly FieldValue CodComercio = new FieldValue("Cód. Comercio");
            public static readonly FieldValue NombreComercio = new FieldValue("Nombre Comercio");
            public static readonly FieldValue RazonSocial = new FieldValue("Razón Social Cliente");
            public static readonly FieldValue IngresoSolicitud = new FieldValue("Ingreso Solicitud");
            public static readonly FieldValue FechaTurno = new FieldValue("Fecha Turno");
            public static readonly FieldValue FechaRecepcion = new FieldValue("Fecha Recepción");
            public static readonly FieldValue PrimerPresupuesto = new FieldValue("1er. Presupuesto");
            public static readonly FieldValue FechaPrimerPresupuesto = new FieldValue("Fecha 1er. Presupuesto");
            public static readonly FieldValue FechaPresupuestada = new FieldValue("Fecha Presupuestada");
            public static readonly FieldValue FechaRecotizacion = new FieldValue("Fecha Recotización");
            public static readonly FieldValue FechaVerificacion = new FieldValue("Fecha Verificación");
            public static readonly FieldValue FechaAprobacion = new FieldValue("Fecha Aprobación");
            public static readonly FieldValue FechaTrabajoTerminado = new FieldValue("Fecha Trabajo Terminado");
            public static readonly FieldValue FechaEntrega = new FieldValue("Fecha Entrega");
            public static readonly FieldValue FechaTrabajoAceptado = new FieldValue("Fecha Trabajo Aceptado");
            public static readonly FieldValue EstadoSolicitud = new FieldValue("Estado Solicitud");
            public static readonly FieldValue EstadoPresupuesto = new FieldValue("Estado Presupuesto");
            public static readonly FieldValue NivelComplejidad = new FieldValue("Nivel Complejidad");

            public static List<FieldValue> List
            {
                get { return new List<FieldValue> { NroCliente,
                                                    Descripcion,
                                                    Solicitud,
                                                    Patente,
                                                    Empleado,
                                                    NroPresupuesto,
                                                    Monto,
                                                    CodComercio,
                                                    NombreComercio,
                                                    RazonSocial,
                                                    IngresoSolicitud,
                                                    FechaTurno,
                                                    FechaRecepcion,
                                                    PrimerPresupuesto,
                                                    FechaPrimerPresupuesto,
                                                    FechaPresupuestada,
                                                    FechaRecotizacion,
                                                    FechaVerificacion,
                                                    FechaAprobacion,
                                                    FechaTrabajoTerminado,
                                                    FechaEntrega,
                                                    FechaTrabajoAceptado,
                                                    EstadoSolicitud,
                                                    EstadoPresupuesto,
                                                    NivelComplejidad }; 
                }
            }
            public static List<FieldValue> Obligatorios
            {
                get
                {
                    return new List<FieldValue> { Descripcion,
                                                  IngresoSolicitud,
                                                  NroCliente,
                                                  RazonSocial,
                                                  Descripcion,
                                                  CodComercio,
                                                  NombreComercio,
                                                  EstadoSolicitud };
                }
            }
        }

        #endregion
    }
}