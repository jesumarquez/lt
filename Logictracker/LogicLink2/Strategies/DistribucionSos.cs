using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Cache;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Messages.Saver;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class DistribucionSos : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Cliente Cliente { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        
        public static ViajeDistribucion Parse(LogicLinkFile file, out int rutas, out int entregas)
        {
            var distribucionSos = new DistribucionSos(file);
            return distribucionSos.Parse(out rutas, out entregas);
        }

        public DistribucionSos(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Cliente = DaoFactory.ClienteDAO.GetList(new[] { Empresa.Id }, new[] { -1 }).FirstOrDefault();
        }

        public ViajeDistribucion Parse(out int rutas, out int entregas)
        {
            const int vigencia = 12;

            var te = new TimeElapsed();
            var rows = ParseFile(Llfile.FilePath).Rows;
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            
            var listPuntos = new List<PuntoEntrega>();
            var listReferencias = new List<ReferenciaGeografica>();

            rutas = 0;
            entregas = 0;

            var sLinea = rows[Properties.DistribucionSos.Base][0].ToString().Replace("\t", string.Empty).Split(':')[1];
            var sFecha = rows[Properties.DistribucionSos.Fecha][0].ToString().Replace("\t", string.Empty).Split(':');
            var sHora = rows[Properties.DistribucionSos.HoraProgramada][0].ToString().Replace("\t", string.Empty).Split(':');
            var sEntrega = rows[Properties.DistribucionSos.Entrega][0].ToString().Replace("\t", string.Empty).Split(':')[1];
            var sPatente = rows[Properties.DistribucionSos.Patente][0].ToString().Replace("\t", string.Empty).Split(':')[1];
            var sLatO = rows[Properties.DistribucionSos.LatitudOrigen][0].ToString().Replace("\t", string.Empty).Split(':')[1];
            var sLonO = rows[Properties.DistribucionSos.LongitudOrigen][0].ToString().Replace("\t", string.Empty).Split(':')[1];
            var sLatD = rows[Properties.DistribucionSos.LatitudDestino][0].ToString().Replace("\t", string.Empty).Split(':')[1];
            var sLonD = rows[Properties.DistribucionSos.LongitudDestino][0].ToString().Replace("\t", string.Empty).Split(':')[1];

            if (string.IsNullOrEmpty(sEntrega)) ThrowProperty("CODIGO ENTREGA", Llfile.Strategy);
            if (string.IsNullOrEmpty(sPatente)) ThrowProperty("PATENTE", Llfile.Strategy);

            var linea = DaoFactory.LineaDAO.FindByCodigo(Empresa.Id, sLinea);
            if (linea == null) ThrowProperty("BASE", Llfile.Strategy);

            var viaje = DaoFactory.ViajeDistribucionDAO.FindByCodigo(Empresa.Id, linea.Id, sEntrega);
            if (viaje != null) return viaje;

            var dia = Convert.ToInt32(sFecha[1].Substring(0, 2));
            var mes = Convert.ToInt32(sFecha[1].Substring(3, 2));
            var anio = Convert.ToInt32(sFecha[1].Substring(6, 4));
            var hora = Convert.ToInt32(sHora[1]);
            var min = Convert.ToInt32(sHora[2]);
            var seg = Convert.ToInt32(sHora[3]);
            var gmt = new TimeSpan(-3, 0, 0);
            var fecha = new DateTime(anio, mes, dia, hora, min, seg).Subtract(gmt);

            var vehiculo = DaoFactory.CocheDAO.FindByPatente(Empresa.Id, sPatente);
            if (vehiculo == null) ThrowProperty("VEHICULO", Llfile.Strategy);

            viaje = new ViajeDistribucion
            {
                Inicio = fecha,
                Codigo = sEntrega,
                Empresa = Empresa,
                Linea = linea,
                Vehiculo = vehiculo,
                Tipo = ViajeDistribucion.Tipos.Desordenado,
                Alta = DateTime.UtcNow,
                RegresoABase = false,
                Estado = ViajeDistribucion.Estados.Pendiente
            };

            rutas++;

            var origen = new EntregaDistribucion
                             {
                                 Linea = linea,
                                 Descripcion = linea.Descripcion,
                                 Estado = EntregaDistribucion.Estados.Pendiente,
                                 Orden = 0,
                                 Programado = fecha,
                                 ProgramadoHasta = fecha,
                                 Viaje = viaje
                             };
            viaje.Detalles.Add(origen);

            var nombreOrigen = sEntrega + " - O";
            var nombreDestino = sEntrega + " - D";

            TipoServicioCiclo tipoServicio = null;
            var tipoServ = DaoFactory.TipoServicioCicloDAO.FindDefault(new[] {Empresa.Id}, new[] {linea.Id});
            if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

            sLatO = sLatO.Replace(',', '.');
            sLonO = sLonO.Replace(',', '.');
            var latO = Convert.ToDouble(sLatO, CultureInfo.InvariantCulture);
            var lonO = Convert.ToDouble(sLonO, CultureInfo.InvariantCulture);
            ValidateGpsPoint(nombreOrigen, nombreOrigen, (float)latO, (float)lonO);
            
            sLatD = sLatD.Replace(',', '.');
            sLonD = sLonD.Replace(',', '.');
            var latD = Convert.ToDouble(sLatD, CultureInfo.InvariantCulture);
            var lonD = Convert.ToDouble(sLonD, CultureInfo.InvariantCulture);
            ValidateGpsPoint(nombreOrigen, nombreOrigen, (float)latD, (float)lonD);

            #region Origen

            var puntoEntregaO = DaoFactory.PuntoEntregaDAO.FindByCode(new[] {Empresa.Id}, 
                                                                      new[] {linea.Id},
                                                                      new[] {Cliente.Id}, 
                                                                      nombreOrigen);

            if (puntoEntregaO == null)
            {
                var empresaGeoRef = Empresa;
                var lineaGeoRef = linea;

                var puntoDeInteres = new ReferenciaGeografica
                {
                    Codigo = nombreOrigen,
                    Descripcion = nombreOrigen,
                    Empresa = empresaGeoRef,
                    Linea = lineaGeoRef,
                    EsFin = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                    EsInicio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                    EsIntermedio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                    InhibeAlarma = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                    TipoReferenciaGeografica = Cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                    Vigencia = new Vigencia { Inicio = DateTime.UtcNow, Fin = fecha.AddHours(vigencia) },
                    Icono = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                };

                var posicion = GetNewDireccion(latO, lonO);

                var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                poligono.AddPoints(new[] { new PointF((float)lonO, (float)latO) });

                puntoDeInteres.Historia.Add(new HistoriaGeoRef
                {
                    ReferenciaGeografica = puntoDeInteres,
                    Direccion = posicion,
                    Poligono = poligono,
                    Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                });

                listReferencias.Add(puntoDeInteres);

                puntoEntregaO = new PuntoEntrega
                {
                    Cliente = Cliente,
                    Codigo = nombreOrigen,
                    Descripcion = nombreOrigen,
                    Telefono = string.Empty,
                    Baja = false,
                    ReferenciaGeografica = puntoDeInteres,
                    Nomenclado = false,
                    DireccionNomenclada = string.Empty,
                    Nombre = nombreOrigen
                };
            }
            else
            {
                if (!puntoEntregaO.ReferenciaGeografica.IgnoraLogiclink && (puntoEntregaO.ReferenciaGeografica.Latitude != latO || puntoEntregaO.ReferenciaGeografica.Longitude != lonO))
                {
                    puntoEntregaO.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                    puntoEntregaO.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                    var posicion = GetNewDireccion(latO, lonO);
                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    poligono.AddPoints(new[] { new PointF((float)lonO, (float)latO) });

                    puntoEntregaO.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                }

                var end = fecha.AddHours(vigencia);
                if (puntoEntregaO.ReferenciaGeografica.Vigencia.Fin < end)
                    puntoEntregaO.ReferenciaGeografica.Vigencia.Fin = end;

                listReferencias.Add(puntoEntregaO.ReferenciaGeografica);
            }

            listPuntos.Add(puntoEntregaO);

            var entregaO = new EntregaDistribucion
            {
                Cliente = Cliente,
                PuntoEntrega = puntoEntregaO,
                Descripcion = nombreOrigen,
                Estado = EntregaDistribucion.Estados.Pendiente,
                Orden = viaje.Detalles.Count,
                Programado = fecha,
                ProgramadoHasta = fecha,
                TipoServicio = tipoServicio,
                Viaje = viaje
            };

            viaje.Detalles.Add(entregaO);
            entregas++;

            #endregion

            #region Destino

            var puntoEntregaD = DaoFactory.PuntoEntregaDAO.FindByCode(new[] { Empresa.Id },
                                                                      new[] { linea.Id },
                                                                      new[] { Cliente.Id },
                                                                      nombreDestino);

            if (puntoEntregaD == null)
            {
                var empresaGeoRef = Empresa;
                var lineaGeoRef = linea;

                var puntoDeInteres = new ReferenciaGeografica
                {
                    Codigo = nombreDestino,
                    Descripcion = nombreDestino,
                    Empresa = empresaGeoRef,
                    Linea = lineaGeoRef,
                    EsFin = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                    EsInicio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                    EsIntermedio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                    InhibeAlarma = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                    TipoReferenciaGeografica = Cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                    Vigencia = new Vigencia { Inicio = DateTime.UtcNow, Fin = fecha.AddHours(vigencia) },
                    Icono = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                };

                var posicion = GetNewDireccion(latD, lonD);

                var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                poligono.AddPoints(new[] { new PointF((float)lonD, (float)latD) });

                puntoDeInteres.Historia.Add(new HistoriaGeoRef
                {
                    ReferenciaGeografica = puntoDeInteres,
                    Direccion = posicion,
                    Poligono = poligono,
                    Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                });

                listReferencias.Add(puntoDeInteres);

                puntoEntregaD = new PuntoEntrega
                {
                    Cliente = Cliente,
                    Codigo = nombreDestino,
                    Descripcion = nombreDestino,
                    Telefono = string.Empty,
                    Baja = false,
                    ReferenciaGeografica = puntoDeInteres,
                    Nomenclado = false,
                    DireccionNomenclada = string.Empty,
                    Nombre = nombreDestino
                };
            }
            else
            {
                if (!puntoEntregaD.ReferenciaGeografica.IgnoraLogiclink && (puntoEntregaD.ReferenciaGeografica.Latitude != latD || puntoEntregaD.ReferenciaGeografica.Longitude != lonD))
                {
                    puntoEntregaD.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                    puntoEntregaD.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                    var posicion = GetNewDireccion(latD, lonD);
                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    poligono.AddPoints(new[] { new PointF((float)lonD, (float)latD) });

                    puntoEntregaD.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                }

                var end = fecha.AddHours(vigencia);
                if (puntoEntregaD.ReferenciaGeografica.Vigencia.Fin < end)
                    puntoEntregaD.ReferenciaGeografica.Vigencia.Fin = end;

                listReferencias.Add(puntoEntregaD.ReferenciaGeografica);
            }

            listPuntos.Add(puntoEntregaD);

            var anterior = puntoEntregaO.ReferenciaGeografica;
            var siguiente = puntoEntregaD.ReferenciaGeografica;
            var o = new LatLon(anterior.Latitude, anterior.Longitude);
            var d = new LatLon(siguiente.Latitude, siguiente.Longitude);
            var directions = GoogleDirections.GetDirections(o, d, GoogleDirections.Modes.Driving, string.Empty, null);

            if (directions != null)
            {
                var duracion = directions.Duration;
                fecha = entregaO.Programado.Add(duracion);
            }

            var entregaD = new EntregaDistribucion
            {
                Cliente = Cliente,
                PuntoEntrega = puntoEntregaD,
                Descripcion = nombreDestino,
                Estado = EntregaDistribucion.Estados.Pendiente,
                Orden = viaje.Detalles.Count,
                Programado = fecha,
                ProgramadoHasta = fecha,
                TipoServicio = tipoServicio,
                Viaje = viaje
            };

            viaje.Detalles.Add(entregaD);
            entregas++;
            
            viaje.Fin = fecha;

            #endregion

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

            STrace.Trace(Component, "Guardando Viaje");
            te.Restart();
            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
            STrace.Trace(Component, string.Format("Viaje guardado en {0} segundos", te.getTimeElapsed().TotalSeconds));

            return viaje;
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
