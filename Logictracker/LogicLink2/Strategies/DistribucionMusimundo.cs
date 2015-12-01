using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using LinqToExcel;
using Geocoder.Core.VO;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class DistribucionMusimundo : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Linea Linea { get; set; }
        private Cliente Cliente { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private readonly List<ViajeDistribucion> _viajesBuffer = new List<ViajeDistribucion>();
        private readonly List<Coche> _cochesBuffer = new List<Coche>();
        private readonly List<TipoServicioCiclo> _tiposServicioBuffer = new List<TipoServicioCiclo>();

        public static void Parse(LogicLinkFile file, out int rutas, out int entregas, out string observaciones)
        {
            new DistribucionMusimundo(file).Parse(out rutas, out entregas, out observaciones);            
        }

        public DistribucionMusimundo(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Linea = file.Linea;
            Cliente = DaoFactory.ClienteDAO.GetList(new[] { Empresa.Id }, new[] { -1 }).FirstOrDefault();
        }

        public void Parse(out int rutas, out int entregas, out string observaciones)
        {
            const int vigencia = 12;

            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, true);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferRows(rows);
            STrace.Trace(Component, string.Format("PreBufferRows en {0} segundos", te.getTimeElapsed().TotalSeconds));

            var listViajes = new List<ViajeDistribucion>(rows.Count);
            var listPuntos = new List<PuntoEntrega>();
            var listReferencias = new List<ReferenciaGeografica>();

            rutas = 0;
            entregas = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));

                var stringFecha = row[Properties.DistribucionMusimundo.Fecha].ToString().Trim();

                var fecha = new DateTime(Convert.ToInt32(stringFecha.Substring(0, 4)), 
                                         Convert.ToInt32(stringFecha.Substring(4, 2)),
                                         Convert.ToInt32(stringFecha.Substring(6, 2)), 
                                         Convert.ToInt32(stringFecha.Substring(8, 2)), 
                                         0, 
                                         0).AddHours(-3);
                

                var patente = row[Properties.DistribucionMusimundo.Patente].ToString().Trim();
                var codigoPedido = row[Properties.DistribucionMusimundo.Factura].ToString().Trim();
                var numeroViaje = Convert.ToInt32(row[Properties.DistribucionMusimundo.TM].ToString().Trim());
                //var secuencia = Convert.ToInt32(row[GetColumnByValue(Fields.Secuencia.Value)]);
                var codigoViaje = string.Format("{0}|{1}|{2}", stringFecha.Substring(0, 8), patente, numeroViaje);
                var strLat = row[Properties.DistribucionMusimundo.Coord1].ToString().Trim().Replace('.', ',');
                var strLon = row[Properties.DistribucionMusimundo.Coord2].ToString().Trim().Replace('.', ',');
                var latitud =  Convert.ToDouble(strLat);
                var longitud = Convert.ToDouble(strLon);
                var importe = row[Properties.DistribucionMusimundo.Importe].ToString().Trim() != "" ? Convert.ToDouble(row[Properties.DistribucionMusimundo.Importe].ToString().Trim()) : 0.0;
                var nombre = row[Properties.DistribucionMusimundo.Direccion].ToString().Trim();
                
                var vehiculo = _cochesBuffer.SingleOrDefault(v => v.Patente.Contains(patente));
                var chofer = vehiculo != null && !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                var oLinea = vehiculo != null && vehiculo.Linea != null ? vehiculo.Linea : Linea;                
                if (oLinea == null)
                {
                    observaciones = "Valor inválido para el campo LINEA";
                    continue;
                }

                TipoServicioCiclo tipoServicio = null;
                var tipoServ = _tiposServicioBuffer.SingleOrDefault(ts => ts.Linea == null || ts.Linea.Id == oLinea.Id);
                if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;
                
                if (listViajes.Count == 0 || codigoViaje != listViajes.Last().Codigo)
                {
                    var byCode = _viajesBuffer.SingleOrDefault(v => v.Codigo == codigoViaje);
                    if (byCode != null) continue;
                }

                ViajeDistribucion viaje;

                if (listViajes.Count > 0 && codigoViaje == listViajes.Last().Codigo)
                {
                    viaje = listViajes.Last();
                }
                else
                {
                    #region viaje = new ViajeDistribucion()

                    viaje = new ViajeDistribucion
                            {
                                Empresa = vehiculo.Empresa,
                                Linea = oLinea,
                                Vehiculo = vehiculo,
                                Empleado = chofer,
                                Codigo = codigoViaje,
                                Estado = ViajeDistribucion.Estados.Pendiente,
                                Inicio = fecha,
                                Fin = fecha,
                                NumeroViaje = Convert.ToInt32(numeroViaje),
                                Tipo = ViajeDistribucion.Tipos.Desordenado,
                                Alta = DateTime.UtcNow,
                                RegresoABase = true
                            };

                    #endregion

                    listViajes.Add(viaje);
                    rutas++;
                }
                
                viaje.Fin = fecha;

                if (viaje.Detalles.Count == 0)
                {
                    //el primer elemento es la base
                    var origen = new EntregaDistribucion
                                     {
                                        Linea = oLinea,
                                        Descripcion = oLinea.Descripcion,
                                        Estado = EntregaDistribucion.Estados.Pendiente,
                                        Orden = 0,
                                        Programado = fecha,
                                        ProgramadoHasta = fecha,
                                        Viaje = viaje
                                     };
                    viaje.Detalles.Add(origen);

                    var llegada = new EntregaDistribucion
                    {
                        Linea = oLinea,
                        Descripcion = oLinea.Descripcion,
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Orden = viaje.Detalles.Count,
                        Programado = fecha,
                        ProgramadoHasta = fecha,
                        Viaje = viaje
                    };
                    viaje.Detalles.Add(llegada);
                }

                var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codigoPedido);

                if (puntoEntrega == null)
                {
                    #region var puntoDeInteres = new ReferenciaGeografica()

                    var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : Cliente.Empresa == null ? null : Empresa;
                    var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea != null 
                                            ? viaje.Vehiculo.Linea 
                                            : Cliente.Linea ?? oLinea;

                    var puntoDeInteres = new ReferenciaGeografica
                                             {
                                                Codigo = codigoPedido,
                                                Descripcion = codigoPedido,
                                                Empresa = empresaGeoRef,
                                                Linea = lineaGeoRef,
                                                EsFin = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                                EsInicio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                                EsIntermedio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                                InhibeAlarma = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                                TipoReferenciaGeografica = Cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                                Vigencia = new Vigencia
                                                                {
                                                                    Inicio = DateTime.UtcNow,
                                                                    Fin = fecha.AddHours(vigencia)
                                                                },
                                                Icono = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                             };

                    #endregion

                    #region var posicion = new Direccion()

                    var posicion = GetNewDireccion(latitud, longitud);

                    #endregion

                    #region var poligono = new Poligono()

                    var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                    poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                    #endregion

                    puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                    listReferencias.Add(puntoDeInteres);

                    #region puntoEntrega = new PuntoEntrega()

                    puntoEntrega = new PuntoEntrega
                                   {
                                        Cliente = Cliente,
                                        Codigo = codigoPedido,
                                        Descripcion = codigoPedido,
                                        Telefono = string.Empty,
                                        Baja = false,
                                        ReferenciaGeografica = puntoDeInteres,
                                        Nomenclado = true,
                                        DireccionNomenclada = string.Empty,
                                        Importe = importe,
                                        Nombre = nombre
                                   };

                    #endregion

                    listPuntos.Add(puntoEntrega);
                }
                else
                {
                    if (!puntoEntrega.ReferenciaGeografica.IgnoraLogiclink && (puntoEntrega.ReferenciaGeografica.Latitude != latitud || puntoEntrega.ReferenciaGeografica.Longitude != longitud))
                    {
                        puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                        puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                        #region var posicion = new Direccion()

                        var posicion = GetNewDireccion(latitud, longitud);

                        #endregion

                        #region var poligono = new Poligono()

                        var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                        poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                        #endregion

                        puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                    }

                    #region puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end

                    var end = fecha.AddHours(vigencia);
                    if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                        puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                    #endregion


                    puntoEntrega.ReferenciaGeografica.Linea = oLinea;
                    listReferencias.Add(puntoEntrega.ReferenciaGeografica);

                    puntoEntrega.Nombre = nombre;
                    puntoEntrega.Importe = importe;

                    listPuntos.Add(puntoEntrega);
                }

                #region var entrega = new EntregaDistribucion()                

                var entrega = new EntregaDistribucion
                                {
                                    Cliente = Cliente,
                                    PuntoEntrega = puntoEntrega,
                                    Descripcion = nombre.Length > 128 ? nombre.Substring(0, 128) : nombre,
                                    Estado = EntregaDistribucion.Estados.Pendiente,
                                    //Orden = secuencia,
                                    Orden = viaje.Detalles.Count - 1,
                                    Programado = fecha,
                                    ProgramadoHasta = fecha.AddMinutes(Empresa.MarginMinutes),
                                    TipoServicio = tipoServicio,
                                    Viaje = viaje
                                };

                #endregion

                viaje.Detalles.Add(entrega);
                entregas++;
            }

            foreach (var viajeDistribucion in listViajes)
            {
                if (viajeDistribucion.Detalles.Count > 0)
                {
                    var dirBase = viajeDistribucion.Detalles.First().ReferenciaGeografica;

                    var coche = viajeDistribucion.Vehiculo;
                    var velocidadPromedio = coche != null && coche.VelocidadPromedio > 0
                        ? coche.VelocidadPromedio
                        : coche != null && coche.TipoCoche.VelocidadPromedio > 0 ? coche.TipoCoche.VelocidadPromedio : 20;

                    var hora = viajeDistribucion.Inicio;
                    foreach (var detalle in viajeDistribucion.Detalles)
                    {
                        var distancia = GeocoderHelper.CalcularDistacia(dirBase.Latitude, dirBase.Longitude, detalle.ReferenciaGeografica.Latitude, detalle.ReferenciaGeografica.Longitude);
                        var horas = distancia/velocidadPromedio;
                        var demora = detalle.TipoServicio != null ? detalle.TipoServicio.Demora : 0;
                        detalle.Programado = hora.AddHours(horas).AddMinutes(demora);
                        detalle.ProgramadoHasta = detalle.Programado.AddMinutes(Empresa.MarginMinutes);
                        dirBase = detalle.ReferenciaGeografica;
                        hora = detalle.Programado;
                    }
                }

                var maxDate = viajeDistribucion.Detalles.Max(d => d.Programado);
                viajeDistribucion.Fin = maxDate;

                var ultimo = viajeDistribucion.Detalles.Last(e => e.Linea != null);
                ultimo.Programado = maxDate;
                ultimo.ProgramadoHasta = maxDate.AddMinutes(Empresa.MarginMinutes);
                ultimo.Orden = viajeDistribucion.Detalles.Count - 1;                
            }

            STrace.Trace(Component, "Guardando referencias geográficas: " + listReferencias.Count);
            te.Restart();
            foreach (var referenciaGeografica in listReferencias)
            {
                DaoFactory.ReferenciaGeograficaDAO.Guardar(referenciaGeografica);
            }
            STrace.Trace(Component, string.Format("Referencias guardadas en {0} segundos", te.getTimeElapsed().TotalSeconds));

            STrace.Trace(Component, "Guardando puntos de entrega: " + listPuntos.Count);
            te.Restart();
            foreach (var puntoEntrega in listPuntos)
            {
                DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
            }
            STrace.Trace(Component, string.Format("Puntos guardados en {0} segundos", te.getTimeElapsed().TotalSeconds));

            STrace.Trace(Component, "Guardando Viajes: " + listViajes.Count);
            te.Restart();
            foreach (var viajeDistribucion in listViajes)
            {
                DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
            }
            STrace.Trace(Component, string.Format("Viajes guardados en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }

        private static Direccion GetNewDireccion(double latitud, double longitud)
        {
            return new Direccion
            {
                Altura = -1,
                IdMapa = -1,
                Provincia = string.Empty,
                IdCalle = -1,
                IdEsquina = -1,
                IdEntrecalle = -1,
                Latitud = latitud,
                Longitud = longitud,
                Partido = string.Empty,
                Pais = string.Empty,
                Calle = string.Empty,
                Descripcion = string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture), longitud.ToString(CultureInfo.InvariantCulture)),
                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
            };            
        }

        private void PreBufferRows(IEnumerable<Row> rows)
        {
            var lastCodPunto = string.Empty;
            var lastCodViaje = string.Empty;
            var lastPatente = string.Empty;

            var codPuntoStrList = new List<string>();
            var codViajeStrList = new List<string>();
            var patenteStrList = new List<string>();

            foreach (var row in rows)
            {
                #region Buffer PuntoEntrega

                try
                {
                    var codigoPuntoEntrega = row[Properties.DistribucionMusimundo.Factura].ToString().Trim();

                    if (lastCodPunto != codigoPuntoEntrega)
                    {
                        if (!codPuntoStrList.Contains(codigoPuntoEntrega))
                            codPuntoStrList.Add(codigoPuntoEntrega);

                        lastCodPunto = codigoPuntoEntrega;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex,
                        String.Format("Error Buffering Punto de Entrega ({0})", row[Properties.DistribucionMusimundo.Factura]));
                }

                #endregion

                #region Buffer Viajes

                try
                {
                    var stringFecha = row[Properties.DistribucionMusimundo.Fecha].ToString().Trim();
                    var patente = row[Properties.DistribucionMusimundo.Patente].ToString().Trim();
                    var numeroViaje = Convert.ToInt32(row[Properties.DistribucionMusimundo.TM].ToString().Trim());
                    var codigoViaje = string.Format("{0}|{1}|{2}", stringFecha.Substring(0, 8), patente, numeroViaje);

                    if (lastCodViaje != codigoViaje)
                    {
                        if (!codViajeStrList.Contains(codigoViaje))
                            codViajeStrList.Add(codigoViaje);

                        lastCodViaje = codigoViaje;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex,
                        String.Format("Error Buffering Viaje ({0}|{1}|{2})",
                                      row[Properties.DistribucionMusimundo.Fecha],
                                      row[Properties.DistribucionMusimundo.Patente],
                                      row[Properties.DistribucionMusimundo.TM]));
                }

                #endregion

                #region Buffer Vehiculo

                try
                {
                    var patente = row[Properties.DistribucionMusimundo.Patente].ToString().Trim();

                    if (lastPatente != patente)
                    {
                        if (!patenteStrList.Contains(patente))
                            patenteStrList.Add(patente);

                        lastPatente = patente;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Vehiculo ({0})", row[Properties.DistribucionMusimundo.Patente]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codPuntoStrList.Any())
            {
                foreach (var l in codPuntoStrList.InSetsOf(batchSize))
                {
                    var puntos = DaoFactory.PuntoEntregaDAO.FindByCodes(new[] { Empresa.Id },
                                                                        new[] { -1 },
                                                                        new[] { Cliente.Id },
                                                                        l);
                    if (puntos != null && puntos.Any())
                    {
                        _puntosBuffer.AddRange(puntos);
                    }
                }
            }

            if (codViajeStrList.Any())
            {
                foreach (var l in codViajeStrList.InSetsOf(batchSize))
                {
                    var viajes = DaoFactory.ViajeDistribucionDAO.FindByCodigos(new[] { Empresa.Id },
                                                                               new[] { -1 },
                                                                               l);
                    if (viajes != null && viajes.Any())
                    {
                        _viajesBuffer.AddRange(viajes);
                    }
                }
            }

            if (patenteStrList.Any())
            {
                foreach (var l in patenteStrList.InSetsOf(batchSize))
                {
                    var coches = DaoFactory.CocheDAO.GetByPatentes(new[] { Empresa.Id }, new[] { -1 }, l);
                    if (coches != null && coches.Any())
                    {
                        _cochesBuffer.AddRange(coches);
                    }
                }
            }

            var tiposServicios = DaoFactory.TipoServicioCicloDAO.FindDefaults(Empresa.Id, new[] { -1 });
            if (tiposServicios != null && tiposServicios.Any())
            {
                _tiposServicioBuffer.AddRange(tiposServicios);
            }

        }

        private void ValidateGpsPoint(string codigoViaje, string codigoPuntoEntrega, float lat, float lon)
        {
            if (lat == lon)
            {
                var parametros = new[] { codigoViaje, codigoPuntoEntrega, lat.ToString("#0.00"), lon.ToString("#0.00") };
                SendMail(parametros);
            }

            try { var point = new GPSPoint(DateTime.UtcNow, lat, lon); }
            catch
            {
                var parametros = new[] { codigoViaje, codigoPuntoEntrega, lat.ToString("#0.00"), lon.ToString("#0.00") };
                SendMail(parametros);
            }
        }

        private void SendMail(string[] parametros)
        {
            var configFile = Config.Mailing.LogiclinkErrorMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            var destinatarios = new List<string> { "soporte@logictracker.com", "metzler.lucas@gmail.com" };

            if (WebSecurity.AuthenticatedUser.Name != string.Empty)
            {
                var usuario = DaoFactory.UsuarioDAO.GetByNombreUsuario(WebSecurity.AuthenticatedUser.Name);
                if (usuario != null && usuario.Email.Trim() != string.Empty)
                    destinatarios.Add(usuario.Email.Trim());
            }

            sender.Config.Subject = "Logiclink2: Error de importación";
            foreach (var destinatario in destinatarios)
            {
                sender.Config.ToAddress = destinatario;
                sender.SendMail(parametros);
                STrace.Trace(Component, "Email sent to: " + destinatario);
            }
        }
    }
}
