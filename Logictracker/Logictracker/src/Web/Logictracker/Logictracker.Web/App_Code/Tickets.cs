using System;
using System.Linq;
using System.Web.Services;
using Logictracker.Messages.Saver;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Process.CicloLogistico;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Web.BaseClasses.Handlers;

/// <summary>
/// Summary description for Tickets
/// </summary>
[WebService(Namespace = "http://web.logictracker.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Tickets : BaseWebService
{
    [WebMethod]
    public Respuesta<bool> AssignAndInit(string sessionId, string company, string branch, DateTime utcDate, string clientCode, string deliveryPointCode, int tripNo, string vehicle, string driver, string load)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceTickets);

            var empresa = GetEmpresaByCode(company);

            var linea = string.IsNullOrEmpty(branch) ? null : GetLineaByCode(branch, empresa);

            Ticket ticket;
            if (tripNo > 0)
            {
                ticket = DAOFactory.TicketDAO.FindByOrdenDiario(empresa != null ? empresa.Id : -1,
                                                                linea != null ? linea.Id : -1, utcDate, tripNo);

                if (ticket == null) throw new ApplicationException("No se encontro un ticket con un orden " + tripNo + " para el dia " + utcDate.ToString("dd/MM/yyyy"));

                if (ticket.Cliente.Codigo != clientCode) throw new ApplicationException("El ticket con orden " + tripNo + " para el dia " + utcDate.ToString("dd/MM/yyyy") + " no tiene el mismo cliente que el ingresado. Ingresado: " + clientCode + " - Ticket " + ticket.Codigo + ": " + ticket.Cliente.Codigo);
            }
            else
            {
                var respuesta = Respuesta<bool>.Create(true);
                respuesta.Mensaje = "No se recibió número de viaje. Ticket omitido.";
                return respuesta;

                //var cliente = GetClienteByCode(clientCode);

                //var puntoEntrega = GetPuntoEntregaByCode(deliveryPointCode, cliente);

                //var tickets = DAOFactory.TicketDAO.GetList(new[] { empresa != null ? empresa.Id : -1 },
                //                                           new[] { linea != null ? linea.Id : -1 }, 
                //                                           new[] { -1 }, 
                //                                           new[] { -1 },
                //                                           new[] { -1 },
                //                                           new[] { -1 }, 
                //                                           new int[] { Ticket.Estados.Pendiente },
                //                                           new[] { cliente.Id },
                //                                           new[] { puntoEntrega != null ? puntoEntrega.Id : -1 },
                //                                           new[] { -1 },
                //                                           utcDate.Date, utcDate.Date.AddDays(1));

                //ticket = tickets.Where(t => t.Estado == Ticket.Estados.Pendiente).FirstOrDefault();

                //if (ticket == null) throw new ApplicationException("No se encontro un ticket para el cliente " + clientCode);
            }

            var vehiculo = GetCocheByInterno(empresa, linea, vehicle);

            if (vehiculo.Dispositivo == null)
            {
                throw new ApplicationException("El vehiculo " + vehiculo.Interno + " no tiene un dispositivo asignado.");
            }

            var multiplesRemitos = ticket.Pedido != null && ticket.Pedido.EsMinimixer;
            var ticketVehiculo = DAOFactory.TicketDAO.FindEnCurso(vehiculo.Dispositivo);
            if (ticketVehiculo != null)
            {
                if (!multiplesRemitos || ticketVehiculo.Pedido == null || ticket.Pedido == null || ticketVehiculo.Pedido.Id != ticket.Pedido.Id)
                {
                    ticketVehiculo.UserField3 += "(auto cerrado ws)";
                    var cicloAnterior = new CicloLogisticoHormigon(ticketVehiculo, DAOFactory, new MessageSaver(DAOFactory));
                    var eventoCierre = EventFactory.GetCloseEvent();
                    cicloAnterior.ProcessEvent(eventoCierre);
                }
                else
                {
                    ticket.Anular("Multiples remitos: " + ticketVehiculo.Codigo + " (ws)", Usuario);
                    DAOFactory.TicketDAO.SaveOrUpdate(ticket);
                    ticket = ticketVehiculo;
                }              
            }

            ticket.Vehiculo = vehiculo;
            ticket.Dispositivo = vehiculo.Dispositivo;
            ticket.CantidadCargaReal += load;
            ticket.UserField3 = "(iniciado via " + (tripNo > 0 ? "nro viaje" : "ws") + ")";

            var ciclo = new CicloLogisticoHormigon(ticket, DAOFactory, new MessageSaver(DAOFactory));
            ciclo.ProcessEvent(EventFactory.GetInitEvent());

            STrace.Trace("WebService Tickets", vehiculo.Dispositivo.Id, String.Format("Ticket {0} iniciado correctamente. {1}", ticket.Codigo, tripNo > 0 ? "Por nro de viaje" : "Por datos"));
            return Respuesta<bool>.Create(true);
        }
        catch (Exception e)
        {
            STrace.Error("WebService Tickets", e.Message);
            return Respuesta<bool>.CreateError(e.Message);
        }
    }
    [WebMethod]
    public Respuesta<bool> Anular(string sessionId, string company, string branch, string code, string reason)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceTickets);

            var empresa = GetEmpresaByCode(company);
            var linea = string.IsNullOrEmpty(branch) ? null : GetLineaByCode(branch, empresa);
            var codigo = code.Trim();
            if(string.IsNullOrEmpty(codigo))
            {
                throw new ApplicationException("El codigo no puede estar vacío");
            }

            var ticket = DAOFactory.TicketDAO.FindByCode(new[] {empresa.Id}, new[] {linea != null ? linea.Id : -1}, codigo);

            if(ticket == null)
            {
                throw new ApplicationException("No se encontró un ticket con el código: " + code);
            }
        
            if(ticket.Estado == Ticket.Estados.EnCurso)
            {
                var ciclo = new CicloLogisticoHormigon(ticket, DAOFactory, new MessageSaver(DAOFactory));
                var eventoCierre = EventFactory.GetCloseEvent();
                ciclo.ProcessEvent(eventoCierre);
            }
            ticket.Anular(reason, Usuario);

            DAOFactory.TicketDAO.SaveOrUpdate(ticket);

            return Respuesta<bool>.Create(true);
        }
        catch (Exception e)
        {
            STrace.Error("WebService Tickets", e.Message);
            return Respuesta<bool>.CreateError(e.Message);
        }
    }

    [WebMethod]
    public Respuesta<ControlDeCiclo[]> GetControlDeCiclo(string sessionId, string company, string branch, DateTime utcDate)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceTickets);
            ValidateExpiration(string.Concat("GetControlDeCiclo[", company, ",", branch, "]"), 180);

            var empresa = GetEmpresaByCode(company);
            var linea = string.IsNullOrEmpty(branch) ? null : GetLineaByCode(branch, empresa);
            var desde = utcDate.Date;
            var hasta = desde.AddDays(1).AddMilliseconds(-1);

            var empty = new[] {-1};

            var results = DAOFactory.TicketDAO.GetList(new[] { empresa.Id }, 
                                                       new[] { linea != null ? linea.Id : -1 }, 
                                                       empty, 
                                                       empty,
                                                       empty,
                                                       empty, 
                                                       empty, 
                                                       empty, 
                                                       empty,
                                                       empty,
                                                       empty,
                                                       desde, 
                                                       hasta)
                .Select(t => new ControlDeCiclo(t))
                .ToArray();

            return Respuesta<ControlDeCiclo[]>.Create(results);
        }
        catch (Exception e)
        {
            STrace.Error("WebService Tickets", e.Message);
            return Respuesta<ControlDeCiclo[]>.CreateError(e.Message);
        }
    }

    [Serializable]
    public class ControlDeCiclo
    {
        public DateTime Fecha { get; set; }
        public string Codigo { get; set; } 
        public string Cliente { get; set; }
        public string PuntoEntrega { get; set; }
        public string Vehiculo { get; set; }
        public int MinutosCiclo { get; set; }
        public int MinutosIda { get; set; }
        public int MinutosEsperaEnObra { get; set; }
        public int MinutosDescarga { get; set; }
        public int MinutosEnObra { get; set; }
        public int MinutosVuelta { get; set; }
        public double KmIda { get; set; } 
        public double KmCiclo { get; set; }
        public double AguaAgregada { get; set; }
        public int Excesos { get; set; }
        public int DescargasIndebidas { get; set; }

        public ControlDeCiclo()
        {
        }

        public ControlDeCiclo(Ticket ticket)
        {
            Fecha = ticket.FechaTicket.Value;
            Codigo = ticket.Codigo;
            Cliente = ticket.Cliente.Codigo;
            PuntoEntrega = ticket.PuntoEntrega.Codigo;
            Vehiculo = ticket.Vehiculo != null ? ticket.Vehiculo.Interno : string.Empty;

            if (ticket.Vehiculo == null) return;

            var daoFactory = new DAOFactory();

            DetalleTicket estadoSalidaPlanta = null;
            DetalleTicket estadoLlegadaObra = null;
            DetalleTicket estadoDescarga = null;
            DetalleTicket estadoSalidaObra = null;
            DetalleTicket estadoLlegadaPlanta = null;
            DetalleTicket firstEstado = null;
            DetalleTicket lastEstado = null;

            foreach (DetalleTicket detalle in ticket.Detalles)
            {
                var triggerEvent = detalle.EstadoLogistico.EsPuntoDeControl;
                switch (triggerEvent)
                {
                    case Estado.Evento.SaleDePlanta: estadoSalidaPlanta = detalle; break;
                    case Estado.Evento.LlegaAObra: estadoLlegadaObra = detalle; break;
                    case Estado.Evento.GiroTrompoDerecha:
                    case Estado.Evento.GiroTrompoHorarioLento:
                    case Estado.Evento.GiroTrompoHorarioRapido: estadoDescarga = detalle; break;
                        //case Estado.Evento.GiroTrompoIzquierda:
                        //case Estado.Evento.GiroTrompoAntihorarioLento:
                        //case Estado.Evento.GiroTrompoAntihorarioRapido:break;
                    case Estado.Evento.SaleDeObra: estadoSalidaObra = detalle; break;
                    case Estado.Evento.LlegaAPlanta: estadoLlegadaPlanta = detalle; break;
                }
                if (HasValue(detalle))
                {
                    if (firstEstado == null) firstEstado = detalle;
                    lastEstado = detalle;
                }
            }

            if (HasValue(estadoSalidaPlanta) && HasValue(estadoLlegadaObra))
            {
                KmIda = daoFactory.CocheDAO.GetDistance(ticket.Vehiculo.Id, 
                                                        estadoSalidaPlanta.Automatico.Value,
                                                        estadoLlegadaObra.Automatico.Value);
            }
            else if(HasValue(estadoSalidaObra) && HasValue(estadoLlegadaPlanta))
            {
                KmIda = daoFactory.CocheDAO.GetDistance(ticket.Vehiculo.Id, 
                                                        estadoSalidaObra.Automatico.Value,
                                                        estadoLlegadaPlanta.Automatico.Value);
            }

            if (HasValue(firstEstado) && HasValue(lastEstado))
            {
                KmCiclo = daoFactory.CocheDAO.GetDistance(ticket.Vehiculo.Id, firstEstado.Automatico.Value, lastEstado.Automatico.Value);
                MinutosCiclo = Convert.ToInt32(lastEstado.Automatico.Value.Subtract(firstEstado.Automatico.Value).TotalMinutes);
            }

            if (HasValue(estadoSalidaPlanta) && HasValue(estadoLlegadaObra))
            {
                MinutosIda = Convert.ToInt32(estadoLlegadaObra.Automatico.Value.Subtract(estadoSalidaPlanta.Automatico.Value).TotalMinutes);
            }

            if (HasValue(estadoLlegadaObra) && HasValue(estadoSalidaObra))
            {
                MinutosEnObra = Convert.ToInt32(estadoSalidaObra.Automatico.Value.Subtract(estadoLlegadaObra.Automatico.Value).TotalMinutes);
            }

            if (HasValue(estadoLlegadaObra) && HasValue(estadoDescarga))
            {
                MinutosEsperaEnObra = Convert.ToInt32( estadoDescarga.Automatico.Value.Subtract(estadoLlegadaObra.Automatico.Value).TotalMinutes);
            }

            if (HasValue(estadoDescarga) && HasValue(estadoSalidaObra))
            {
                MinutosDescarga = Convert.ToInt32(estadoSalidaObra.Automatico.Value.Subtract(estadoDescarga.Automatico.Value).TotalMinutes);
            }

            if (HasValue(estadoSalidaObra) && HasValue(estadoLlegadaPlanta))
            {
                MinutosVuelta = Convert.ToInt32(estadoLlegadaPlanta.Automatico.Value.Subtract(estadoSalidaObra.Automatico.Value).TotalMinutes);
            }

            if(HasValue(firstEstado) && HasValue(lastEstado) && !firstEstado.Automatico.Value.Equals(lastEstado.Automatico.Value))
            {
                Excesos = daoFactory.InfraccionDAO.GetByVehiculo(ticket.Vehiculo.Id,
                                                                 Infraccion.Codigos.ExcesoVelocidad,
                                                                 firstEstado.Automatico.Value,
                                                                 lastEstado.Automatico.Value).Count;

                if (HasValue(estadoLlegadaObra) || HasValue(estadoSalidaObra))
                {
                    var maxMonths = ticket.Vehiculo.Empresa != null ? ticket.Vehiculo.Empresa.MesesConsultaPosiciones : 3;
                    var descargas = daoFactory.LogMensajeDAO.GetEventos(new[] {ticket.Vehiculo.Id},
                                                                        new[]
                                                                            {
                                                                                MessageCode.MixerClockwise.GetMessageCode(),
                                                                                MessageCode.MixerClockwiseFast.GetMessageCode(),
                                                                                MessageCode.MixerClockwiseSlow.GetMessageCode()
                                                                            },
                                                                        firstEstado.Automatico.Value,
                                                                        lastEstado.Automatico.Value,
                                                                        maxMonths);

                    DescargasIndebidas = descargas.Count(d => (HasValue(estadoLlegadaObra) && d.Fecha < estadoLlegadaObra.Automatico.Value) ||
                                                              (HasValue(estadoSalidaObra) && d.Fecha > estadoSalidaObra.Automatico.Value));
                }
            }
        }

        private static bool HasValue(DetalleTicket detalle)
        {
            return detalle != null && detalle.Automatico.HasValue;
        }
    }
}