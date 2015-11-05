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

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class DistribucionQuilmes : Strategy
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
        private const double _latitudDefault = -34.5411981040848;
        private const double _longitudDefault = -57.9051147460951;

        public static void Parse(LogicLinkFile file, out int rutas, out int entregas, out string observaciones)
        {
            new DistribucionQuilmes(file).Parse(out rutas, out entregas, out observaciones);
        }

        public DistribucionQuilmes(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Linea = file.Linea;
            Cliente = DaoFactory.ClienteDAO.GetList(new[] { Empresa.Id }, new[] { -1 }).FirstOrDefault();
        }

        public void Parse(out int rutas, out int entregas, out string observaciones)
        {
            const char separator = '|';
            const int vigencia = 12;

            var te = new TimeElapsed();
            var rows = ParseFile(Llfile.FilePath, separator).Rows;
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

                var patente = row.Cells[Properties.DistribucionQuilmes.Patente].ToString().Trim();
                var vehiculo = _cochesBuffer.SingleOrDefault(v => v.Patente == patente);
                
                var oLinea = vehiculo != null && vehiculo.Linea != null ? vehiculo.Linea : Linea;
                if (oLinea == null)
                {
                    observaciones = "Valor inválido para el campo LINEA";
                    continue;
                }
                
                var sFecha = row.Cells[Properties.DistribucionQuilmes.Fecha].ToString().Trim();
                var codigo = sFecha + row.Cells[Properties.DistribucionQuilmes.Ruta].ToString().Trim();
            
                var sHora = row.Cells[Properties.DistribucionQuilmes.Hora].ToString().Trim();
                if (sHora.Length == 3) sHora = "0" + sHora;
                else if (sHora.Length != 4)
                {
                    observaciones = "Valor inválido para el campo HORARIO";
                    continue;
                }

                var sOrden = row.Cells[Properties.DistribucionQuilmes.Orden].ToString().Trim();
                var orden = Convert.ToInt32(sOrden);

                var nroViaje = row.Cells[Properties.DistribucionQuilmes.Viaje].ToString().Trim();
                var latitud = row.Cells[Properties.DistribucionQuilmes.Latitud].ToString().Trim();
                var longitud = row.Cells[Properties.DistribucionQuilmes.Longitud].ToString().Trim();
                var esBase = latitud.Equals(string.Empty) && longitud.Equals(string.Empty) && nroViaje.Equals(string.Empty);
                var incompleto = !esBase && (latitud.Trim().Equals(string.Empty) || longitud.Trim().Equals(string.Empty));
                
                var dia = Convert.ToInt32(sFecha.Substring(0, 2));
                var mes = Convert.ToInt32(sFecha.Substring(2, 2));
                var anio = Convert.ToInt32(sFecha.Substring(4, 2)) + 2000;
                var hora = Convert.ToInt32(sHora.Substring(0, 2));
                var min = Convert.ToInt32(sHora.Substring(2, 2));
                var gmt = new TimeSpan(-3, 0, 0);
                var fecha = new DateTime(anio, mes, dia, hora, min, 0).Subtract(gmt);

                if (listViajes.Count == 0 || codigo != listViajes.Last().Codigo)
                {
                    var byCode = _viajesBuffer.SingleOrDefault(v => v.Codigo == codigo);
                    if (byCode != null) continue;
                }

                ViajeDistribucion item;

                if (listViajes.Count > 0 && listViajes.Any(v => v.Codigo == codigo))
                {
                    item = listViajes.SingleOrDefault(v => v.Codigo == codigo);
                    if (fecha < item.Inicio) item.Inicio = fecha;
                }
                else
                {
                    item = new ViajeDistribucion { Codigo = codigo };
                    rutas++;
                    
                    item.Empresa = Empresa;
                    item.Linea = oLinea;
                    item.Vehiculo = vehiculo;
                    item.Inicio = fecha;
                    item.Fin = fecha;
                    item.Tipo = ViajeDistribucion.Tipos.Desordenado;
                    item.RegresoABase = true;
                    item.Estado = ViajeDistribucion.Estados.Pendiente;
                    item.Alta = DateTime.UtcNow;
                    item.ProgramacionDinamica = codigo.Contains("TR");

                    
                    item.NumeroViaje = Convert.ToInt32(nroViaje);

                    if (vehiculo != null)
                    {
                        item.Empleado = !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                        item.CentroDeCostos = vehiculo.CentroDeCostos;
                        item.SubCentroDeCostos = vehiculo.SubCentroDeCostos;
                    }
                    else
                    {
                        STrace.Error(Component, string.Format("Patente {0} no encontrada para el viaje: {1}", patente, codigo));
                    }

                    listViajes.Add(item);
                }

                var kms = row.Cells[Properties.DistribucionQuilmes.Kms].ToString().Trim();
                var distance = Convert.ToDouble(kms, CultureInfo.InvariantCulture);

                if (esBase)
                {
                    if (codigo.Contains("TR"))
                    {
                        var ultimo = item.Detalles.Last().ReferenciaGeografica;
                        var origen = new LatLon(ultimo.Latitude, ultimo.Longitude);
                        var destino = new LatLon(oLinea.ReferenciaGeografica.Latitude, oLinea.ReferenciaGeografica.Longitude);
                        var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, null);

                        if (directions != null)
                        {
                            distance = directions.Distance/1000.0;
                            var duracion = directions.Duration;
                            fecha = item.Detalles.Last().Programado.Add(duracion);
                            if (item.Detalles.Last().TipoServicio != null)
                                fecha = fecha.AddMinutes(item.Detalles.Last().TipoServicio.Demora);
                        }
                    }

                    var llegada = new EntregaDistribucion
                    {
                        Linea = oLinea,
                        Descripcion = oLinea.Descripcion,
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Orden = orden,
                        Programado = fecha,
                        ProgramadoHasta = fecha,
                        Viaje = item,
                        KmCalculado = distance
                    };
                      
                    item.Detalles.Add(llegada);
                    continue;
                }

                // Entregas
                if (item.Detalles.Count == 0)
                {
                    //Si no existe, agrego la salida de base
                    var origen = new EntregaDistribucion
                                     {
                                         Linea = oLinea,
                                         Descripcion = oLinea.Descripcion,
                                         Estado = EntregaDistribucion.Estados.Pendiente,
                                         Orden = 0,
                                         Programado = fecha,
                                         ProgramadoHasta = fecha,
                                         Viaje = item
                                     };
                    item.Detalles.Add(origen);
                }

                var codigoPuntoEntrega = row.Cells[Properties.DistribucionQuilmes.CodigoCliente].ToString().Trim();
                var nombre = row.Cells[Properties.DistribucionQuilmes.DescripcionCliente].ToString().Trim();
                if (string.IsNullOrEmpty(codigoPuntoEntrega))
                {
                    observaciones = "Valor inválido para el campo PUNTO ENTREGA";
                    continue;
                }

                if (item.Detalles.Any(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codigoPuntoEntrega))
                    continue;

                if (item.Detalles.Any(d => d.Orden == orden)) distance = 0;

                TipoServicioCiclo tipoServicio = null;
                var tipoServ = _tiposServicioBuffer.SingleOrDefault(ts => ts.Linea == null || ts.Linea.Id == oLinea.Id);
                if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

                if (codigo.Contains("TR"))
                {
                    tipoServ = DaoFactory.TipoServicioCicloDAO.FindByCode(new[] { item.Empresa.Id }, new[] { -1 }, "TR");
                    if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;
                }

                double lat, lon;

                if (incompleto)
                {
                    lat = _latitudDefault;
                    lon = _longitudDefault;
                }
                else
                {
                    latitud = latitud.Replace(',', '.');
                    longitud = longitud.Replace(',', '.');
                    lat = Convert.ToDouble(latitud, CultureInfo.InvariantCulture);
                    lon = Convert.ToDouble(longitud, CultureInfo.InvariantCulture);
                    ValidateGpsPoint(codigo, codigoPuntoEntrega, (float)lat, (float)lon);
                }                

                var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codigoPuntoEntrega);

                if (puntoEntrega == null)
                {
                    var empresaGeoRef = Empresa;
                    var lineaGeoRef = oLinea;

                    var puntoDeInteres = new ReferenciaGeografica
                                             {
                                                 Codigo = codigoPuntoEntrega,
                                                 Descripcion = nombre,
                                                 Empresa = empresaGeoRef,
                                                 Linea = lineaGeoRef,
                                                 EsFin = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                                 EsInicio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                                 EsIntermedio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                                 InhibeAlarma = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                                 TipoReferenciaGeografica = Cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                                 Vigencia = new Vigencia {Inicio = DateTime.UtcNow, Fin = fecha.AddHours(vigencia)},
                                                 Icono = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                             };

                    var posicion = GetNewDireccion(lat, lon);

                    var poligono = new Poligono {Radio = 50, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                    poligono.AddPoints(new[] {new PointF((float) lon, (float) lat)});

                    puntoDeInteres.Historia.Add(new HistoriaGeoRef
                                                    {
                                                        ReferenciaGeografica = puntoDeInteres,
                                                        Direccion = posicion,
                                                        Poligono = poligono,
                                                        Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                                    });

                    listReferencias.Add(puntoDeInteres);

                    puntoEntrega = new PuntoEntrega
                                       {
                                           Cliente = Cliente,
                                           Codigo = codigoPuntoEntrega,
                                           Descripcion = nombre,
                                           Telefono = string.Empty,
                                           Baja = false,
                                           ReferenciaGeografica = puntoDeInteres,
                                           Nomenclado = true,
                                           DireccionNomenclada = string.Empty,
                                           Nombre = nombre
                                       };
                }
                else
                {
                    if (!puntoEntrega.ReferenciaGeografica.IgnoraLogiclink && (puntoEntrega.ReferenciaGeografica.Latitude != lat || puntoEntrega.ReferenciaGeografica.Longitude != lon))
                    {
                        puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                        puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                        var posicion = GetNewDireccion(lat, lon);
                        var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                        poligono.AddPoints(new[] { new PointF((float)lon, (float)lat) });

                        puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                    }

                    var end = fecha.AddHours(vigencia);
                    if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                        puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                    listReferencias.Add(puntoEntrega.ReferenciaGeografica);
                }

                listPuntos.Add(puntoEntrega);

                if (codigo.Contains("TR"))                 
                {
                    if (puntoEntrega.ReferenciaGeografica.Latitude == _latitudDefault && puntoEntrega.ReferenciaGeografica.Longitude == _longitudDefault)
                    {
                        distance = 0.0;
                    }
                    else
                    {
                        var ultimo = item.Detalles.Last(d => d.ReferenciaGeografica.Latitude != _latitudDefault && d.ReferenciaGeografica.Longitude != _longitudDefault).ReferenciaGeografica;
                        var origen = new LatLon(ultimo.Latitude, ultimo.Longitude);
                        var destino = new LatLon(puntoEntrega.ReferenciaGeografica.Latitude, puntoEntrega.ReferenciaGeografica.Longitude);
                        var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, null);

                        if (directions != null)
                        {
                            distance = directions.Distance / 1000.0;
                            var duracion = directions.Duration;
                            fecha = item.Detalles.Last().Programado.Add(duracion);
                        }
                    }
                }

                if (item.Detalles.Last().TipoServicio != null)
                    fecha = fecha.AddMinutes(item.Detalles.Last().TipoServicio.Demora);

                var entrega = new EntregaDistribucion
                                  {
                                      Cliente = Cliente,
                                      PuntoEntrega = puntoEntrega,
                                      Descripcion = codigoPuntoEntrega,
                                      Estado = EntregaDistribucion.Estados.Pendiente,
                                      Orden = orden,
                                      Programado = fecha,
                                      ProgramadoHasta = fecha,
                                      TipoServicio = tipoServicio,
                                      Viaje = item,
                                      KmCalculado = distance
                                  };

                item.Detalles.Add(entrega);
                entregas++;

                var lastDetail = item.Detalles.LastOrDefault();
                item.Fin = lastDetail == null ? item.Inicio : lastDetail.Programado;
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

        private void PreBufferRows(IEnumerable<Record> rows)
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
                    var codigoPuntoEntrega = row.Cells[Properties.DistribucionQuilmes.CodigoCliente].ToString().Trim();

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
                        String.Format("Error Buffering Punto de Entrega ({0})", row.Cells[Properties.DistribucionQuilmes.CodigoCliente]));
                }

                #endregion

                #region Buffer Viajes

                try
                {
                    var sFecha = row.Cells[Properties.DistribucionQuilmes.Fecha].ToString().Trim();
                    var codigoViaje = sFecha + row.Cells[Properties.DistribucionQuilmes.Ruta].ToString().Trim();

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
                        String.Format("Error Buffering Viaje ({0}/{1})", 
                                      row.Cells[Properties.DistribucionQuilmes.Fecha],
                                      row.Cells[Properties.DistribucionQuilmes.Ruta]));
                }

                #endregion

                #region Buffer Vehiculo

                try
                {
                    var patente = row.Cells[Properties.DistribucionQuilmes.Patente].ToString().Trim();

                    if (lastPatente != patente)
                    {
                        if (!patenteStrList.Contains(patente))
                            patenteStrList.Add(patente);

                        lastPatente = patente;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Vehiculo ({0})", row.Cells[Properties.DistribucionQuilmes.Patente]));
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
                    var coches = DaoFactory.CocheDAO.GetByPatentes(new[] { Empresa.Id }, new[] {-1}, l);
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
