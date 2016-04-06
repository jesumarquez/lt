using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Utils;
using Logictracker.Messaging;

namespace Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico
{
    [Serializable]
    public class ReporteDistribucionVo
    {
        public const int IndexRuta = 0;
        public const int IndexTipoVehiculo = 1;
        public const int IndexVehiculo = 2;
        public const int IndexEmpleado = 3;
        public const int IndexFecha = 4;
        public const int IndexOrden = 5;
        public const int IndexOrdenReal = 6;
        public const int IndexPuntoEntrega = 7;
        public const int IndexDescripcion = 8;
        public const int IndexManual = 9;
        public const int IndexEntrada = 10;
        public const int IndexSalida = 11;
        public const int IndexDuracion = 12;
        public const int IndexKm = 13;
        public const int IndexEstado = 14;
        public const int IndexDistancia = 15;
        public const int IndexTieneFoto = 16;        
        public const int IndexConfirmacion = 17;
        public const int IndexHorario = 18;        
        public const int IndexUnreadInactive = 19;
        public const int IndexReadInactive = 20;

        public int Id { get; set; }
        public int IdDispositivo { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateManual { get; set; }
        public DateTime? DateEntrada { get; set; }
        public DateTime? DateSalida { get; set; }
        public double TiempoEntrega { get; set; }
        public DateTime? DateGarminUnreadInactive { get; set; }
        public DateTime? DateGarminReadInactive { get; set; }
        public DateTime? DateConfirmacion { get; set; }

        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }

        [GridMapping(Index = IndexRuta, ResourceName = "Labels", VariableName = "RUTA", AllowGroup = true)]
        public string Ruta { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexEmpleado, ResourceName = "Entities", VariableName = "PARENTI09", AllowGroup = true)]
        public string Empleado { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", AllowGroup = true)]
        public string Fecha { get; set; }

        [GridMapping(Index = IndexOrden, ResourceName = "Labels", VariableName = "ORDEN", AllowGroup = false)]
        public int Orden { get; set; }

        [GridMapping(Index = IndexOrdenReal, ResourceName = "Labels", VariableName = "ORDEN_REAL", AllowGroup = false)]
        public int OrdenReal { get; set; }

        [GridMapping(Index = IndexPuntoEntrega, ResourceName = "Entities", VariableName = "PARENTI44", AllowGroup = true)]
        public string PuntoEntrega { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexManual, ResourceName = "Labels", VariableName = "MANUAL", AllowGroup = false)]
        public string Manual { get; set; }

        [GridMapping(Index = IndexEntrada, ResourceName = "Labels", VariableName = "ENTRADA", AllowGroup = false)]
        public string Entrada { get; set; }

        [GridMapping(Index = IndexSalida, ResourceName = "Labels", VariableName = "SALIDA", AllowGroup = false)]
        public string Salida { get; set; }

        [GridMapping(Index = IndexDuracion, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public string Duracion { get; set; }

        [GridMapping(Index = IndexKm, ResourceName = "Labels", VariableName = "KM", AllowGroup = false, DataFormatString = "{0: 0.00}")]
        public double Km { get; set; }

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "STATE", AllowGroup = true)]
        public string Estado { get; set; }

        [GridMapping(Index = IndexDistancia, ResourceName = "Labels", VariableName = "DISTANCIA", AllowGroup = false, DataFormatString = "{0: 0.00}")]
        public double? Distancia { get; set; }

        [GridMapping(Index = IndexTieneFoto, IsTemplate = true, HeaderText = "", AllowGroup = false)]
        public bool TieneFoto { get; set; }

        [GridMapping(Index = IndexConfirmacion, ResourceName = "Labels", VariableName = "CONFIRMACION", AllowGroup = true)]
        public string Confirmacion { get; set; }

        [GridMapping(Index = IndexHorario, ResourceName = "Labels", VariableName = "HORARIO")]
        public DateTime? Horario { get; set; }
        
        [GridMapping(Index = IndexUnreadInactive, ResourceName = "Labels", VariableName = "UNREAD_INACTIVE")]
        public string UnreadInactive { get; set; }

        [GridMapping(Index = IndexReadInactive, ResourceName = "Labels", VariableName = "READ_INACTIVE")]
        public string ReadInactive { get; set; }

        public int Radio { get; set; }

        public ReporteDistribucionVo(EntregaDistribucion entrega, EntregaDistribucion anterior, int orden, double km, bool verConfirmacion)
        {
            var dao = new DAOFactory();
            
            Id = entrega.Viaje.Id;
            Descripcion = entrega.Descripcion;
            IdDispositivo = entrega.Viaje.Vehiculo != null && entrega.Viaje.Vehiculo.Dispositivo != null
                                ? entrega.Viaje.Vehiculo.Dispositivo.Id
                                : 0;
            Ruta = entrega.Viaje.Codigo;
            TipoVehiculo = entrega.Viaje.Vehiculo != null && entrega.Viaje.Vehiculo.TipoCoche != null
                               ? entrega.Viaje.Vehiculo.TipoCoche.Descripcion
                               : string.Empty;
            Vehiculo = entrega.Viaje.Vehiculo != null ? entrega.Viaje.Vehiculo.Interno : string.Empty;
            Empleado = entrega.Viaje.Empleado != null && entrega.Viaje.Empleado.Entidad != null
                           ? entrega.Viaje.Empleado.Entidad.Descripcion
                           : string.Empty;
            Fecha = entrega.Viaje.Inicio.ToDisplayDateTime().ToString("dd/MM/yyyy");
            Orden = entrega.Orden;
            OrdenReal = orden;
            PuntoEntrega = entrega.PuntoEntrega != null
                               ? entrega.PuntoEntrega.Descripcion
                               : entrega.ReferenciaGeografica != null
                                     ? entrega.ReferenciaGeografica.Descripcion
                                     : string.Empty;
            Manual = entrega.Manual.HasValue
                         ? entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm")
                         : string.Empty;
            Entrada = entrega.Entrada.HasValue
                          ? entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm")
                          : string.Empty;
            Salida = entrega.Salida.HasValue
                         ? entrega.Salida.Value.ToDisplayDateTime().ToString("HH:mm")
                         : string.Empty;
            var duracion = entrega.Entrada.HasValue && entrega.Salida.HasValue
                               ? entrega.Salida.Value.Subtract(entrega.Entrada.Value)
                               : new TimeSpan(0);
            Duracion = duracion.TotalSeconds >= 0
                           ? duracion.Hours.ToString("00") + ":" + duracion.Minutes.ToString("00") + ":" + duracion.Seconds.ToString("00")
                           : string.Empty;
            Estado = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado));
            Km = km;
            UnreadInactive = entrega.GarminUnreadInactiveAt.HasValue
                                ? entrega.GarminUnreadInactiveAt.Value.ToDisplayDateTime().ToString("HH:mm")
                                : string.Empty;
            ReadInactive = entrega.GarminReadInactiveAt.HasValue
                                ? entrega.GarminReadInactiveAt.Value.ToDisplayDateTime().ToString("HH:mm")
                                : string.Empty;
            
            var reportDao = new ReportFactory(dao);
            
            if (entrega.Manual.HasValue && entrega.Viaje.Vehiculo != null)
            {
                Hasta = entrega.Manual.Value;
                if (anterior != null)
                {
                    if (anterior.Manual.HasValue)
                        Desde = anterior.Manual.Value;
                    else if (anterior.Salida.HasValue)
                        Desde = anterior.Salida.Value;
                    else
                        Desde = anterior.Programado;

                    var maxMonths = entrega.Viaje.Vehiculo.Empresa != null ? entrega.Viaje.Vehiculo.Empresa.MesesConsultaPosiciones : 3;
                    var eventos = reportDao.MobileEventDAO.GetMobilesEvents(new List<int>{entrega.Viaje.Vehiculo.Id},
                                                                            new[] {514},
                                                                            new List<int>{0},
                                                                            Desde,
                                                                            Hasta,
                                                                            maxMonths);

                    TieneFoto = eventos.Any();
                }
            }

            if (verConfirmacion && entrega.Viaje.Vehiculo != null)
            {
                var mensajes = new List<string> { MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode(),
                                                  MessageCode.EstadoLogisticoCumplidoManualNoRealizado.GetMessageCode(),
                                                  MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode()};
                var confirmaciones = dao.MensajeDAO.GetMensajesDeConfirmacion(new[] { entrega.Viaje.Empresa.Id }, new int[]{}).Select(m => m.Codigo);
                var rechazos = dao.MensajeDAO.GetMensajesDeRechazo(new[] { entrega.Viaje.Empresa.Id }, new int[]{}).Select(m => m.Codigo);
                mensajes.AddRange(confirmaciones);
                mensajes.AddRange(rechazos);

                Confirmacion = entrega.MensajeConfirmacion != null
                                    ? entrega.MensajeConfirmacion.Mensaje.Descripcion
                                    : string.Empty;
                Horario = entrega.RecepcionConfirmacion.HasValue
                              ? entrega.RecepcionConfirmacion.Value.ToDisplayDateTime()
                              : (DateTime?) null;
                var eventos = entrega.EventosDistri.Where(e => mensajes.Contains(e.LogMensaje.Mensaje.Codigo));
                var evento = eventos.Any() 
                                ? eventos.OrderBy(e => e.Fecha).FirstOrDefault() 
                                : null;

                Distancia = evento != null ? Distancias.Loxodromica(evento.LogMensaje.Latitud, evento.LogMensaje.Longitud, entrega.ReferenciaGeografica.Latitude, entrega.ReferenciaGeografica.Longitude) : (double?)null;
                if (entrega.PuntoEntrega != null && entrega.PuntoEntrega.ReferenciaGeografica != null
                 && entrega.PuntoEntrega.ReferenciaGeografica.Poligono != null)
                    Radio = entrega.PuntoEntrega.ReferenciaGeografica.Poligono.Radio;
            }
        }

        public ReporteDistribucionVo(DatamartDistribucion dm, bool verConfirmacion)
        {
            Id = dm.Viaje.Id;
            Descripcion = dm.Entrega;
            IdDispositivo = dm.Viaje.Vehiculo != null && dm.Viaje.Vehiculo.Dispositivo != null
                                ? dm.Viaje.Vehiculo.Dispositivo.Id
                                : 0;
            Ruta = dm.Ruta;
            TipoVehiculo = dm.Viaje.Vehiculo != null && dm.Viaje.Vehiculo.TipoCoche != null
                               ? dm.Viaje.Vehiculo.TipoCoche.Descripcion
                               : string.Empty;
            Vehiculo = dm.Vehiculo != null ? dm.Vehiculo.Interno : string.Empty;
            Empleado = dm.Viaje.Empleado != null && dm.Viaje.Empleado.Entidad != null
                           ? dm.Viaje.Empleado.Entidad.Descripcion
                           : string.Empty;
            Fecha = dm.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy");
            Orden = dm.Detalle.Orden;
            OrdenReal = dm.Orden;
            PuntoEntrega = dm.PuntoEntrega != null
                               ? dm.PuntoEntrega.Descripcion
                               : dm.Detalle.ReferenciaGeografica != null
                                     ? dm.Detalle.ReferenciaGeografica.Descripcion
                                     : string.Empty;
            Manual = dm.Manual.HasValue
                         ? dm.Manual.Value.ToDisplayDateTime().ToString("HH:mm")
                         : string.Empty;
            Entrada = dm.Entrada.HasValue
                          ? dm.Entrada.Value.ToDisplayDateTime().ToString("HH:mm")
                          : string.Empty;
            Salida = dm.Salida.HasValue
                         ? dm.Salida.Value.ToDisplayDateTime().ToString("HH:mm")
                         : string.Empty;
            var duracion = new TimeSpan(0, 0, (int)dm.TiempoEntrega*60);
            Duracion = dm.TiempoEntrega >= 0
                           ? duracion.Hours.ToString("00") + ":" + duracion.Minutes.ToString("00") + ":" + duracion.Seconds.ToString("00")
                           : string.Empty;
            Estado = dm.Estado;
            Km = dm.Km;
            UnreadInactive = dm.Detalle.GarminUnreadInactiveAt.HasValue
                                ? dm.Detalle.GarminUnreadInactiveAt.Value.ToDisplayDateTime().ToString("HH:mm")
                                : string.Empty;
            ReadInactive = dm.Detalle.GarminReadInactiveAt.HasValue
                                ? dm.Detalle.GarminReadInactiveAt.Value.ToDisplayDateTime().ToString("HH:mm")
                                : string.Empty;

            if (verConfirmacion && dm.Vehiculo != null)
            {
                Confirmacion = dm.Confirmacion;
                Horario = dm.Detalle.RecepcionConfirmacion.HasValue
                              ? dm.Detalle.RecepcionConfirmacion.Value.ToDisplayDateTime()
                              : (DateTime?)null;
                Distancia = dm.Distancia;
                if (dm.PuntoEntrega != null && dm.PuntoEntrega.ReferenciaGeografica != null
                 && dm.PuntoEntrega.ReferenciaGeografica.Poligono != null)
                    Radio = dm.PuntoEntrega.ReferenciaGeografica.Poligono.Radio;
            }
        }

        public ReporteDistribucionVo(ReporteDistribucionVo dm)
        {
            Id = dm.Id;
            IdDispositivo = dm.IdDispositivo;
            Ruta = dm.Ruta;
            TipoVehiculo = dm.TipoVehiculo;
            Vehiculo = dm.Vehiculo;
            Empleado = dm.Empleado;
            Orden = dm.Orden;
            OrdenReal = dm.OrdenReal;
            PuntoEntrega = dm.PuntoEntrega;
            Descripcion = dm.Descripcion;
            Km = dm.Km;
            Estado = dm.Estado;

            Fecha = dm.Date.ToDisplayDateTime().ToString("dd/MM/yyyy");
            Manual = dm.DateManual.HasValue
                         ? dm.DateManual.Value.ToDisplayDateTime().ToString("HH:mm")
                         : string.Empty;
            Entrada = dm.DateEntrada.HasValue
                          ? dm.DateEntrada.Value.ToDisplayDateTime().ToString("HH:mm")
                          : string.Empty;
            Salida = dm.DateSalida.HasValue
                         ? dm.DateSalida.Value.ToDisplayDateTime().ToString("HH:mm")
                         : string.Empty;
            var duracion = new TimeSpan(0, 0, (int)dm.TiempoEntrega * 60);
            Duracion = duracion.TotalSeconds >= 0
                           ? duracion.Hours.ToString("00") + ":" + duracion.Minutes.ToString("00") + ":" + duracion.Seconds.ToString("00")
                           : string.Empty;
            UnreadInactive = dm.DateGarminUnreadInactive.HasValue
                                ? dm.DateGarminUnreadInactive.Value.ToDisplayDateTime().ToString("HH:mm")
                                : string.Empty;
            ReadInactive = dm.DateGarminReadInactive.HasValue
                                ? dm.DateGarminReadInactive.Value.ToDisplayDateTime().ToString("HH:mm")
                                : string.Empty;
            Confirmacion = dm.Confirmacion;
            Horario = dm.DateConfirmacion.HasValue 
                            ? dm.DateConfirmacion.Value.ToDisplayDateTime()
                            : (DateTime?) null;
            Distancia = dm.Distancia;
            Radio = dm.Radio;
        }

        public ReporteDistribucionVo() { }
    }
}
