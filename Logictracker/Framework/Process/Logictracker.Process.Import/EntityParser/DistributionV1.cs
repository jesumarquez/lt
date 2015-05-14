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
using Logictracker.Utils;

namespace Logictracker.Process.Import.EntityParser
{
    public class DistributionV1 : EntityParserBase
    {
        // QUILMES
        protected override string EntityName { get { return "Distribution"; } }

        public DistributionV1() {}
        public DistributionV1(DAOFactory daoFactory) : base(daoFactory) {}

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var oEmpresa = DaoFactory.EmpresaDAO.FindById(empresa);
            
            var patente = data.AsString(Properties.Distribution.Coche, 32).Trim();
            var vehiculo = patente != string.Empty ? DaoFactory.CocheDAO.FindByPatente(empresa, patente) : null;
            
            var oLinea = vehiculo != null && vehiculo.Linea != null ? vehiculo.Linea : DaoFactory.LineaDAO.FindById(linea);
            if (oLinea == null) ThrowProperty("LINEA");
            var oCliente = DaoFactory.ClienteDAO.GetList(new[] {empresa}, new[] {-1}).FirstOrDefault();
            if (oCliente == null) ThrowProperty("CLIENTE");
            const int vigencia = 12;

            var sFecha = data.AsString(Properties.Distribution.Fecha, 6).Trim();
            var codigo = sFecha + data.AsString(Properties.Distribution.Codigo, 8).Trim();
            var item = GetDistribucion(empresa, oLinea.Id, codigo);
            if (data.Operation == (int) Operation.Delete) return item;

            var sHora = data.AsString(Properties.Distribution.Horario, 4).Trim();
            if (sHora.Length == 3) sHora = "0" + sHora;
            else if (sHora.Length != 4) ThrowProperty("HORARIO");

            var latitud = data.AsString(Properties.Distribution.Latitud, 10).Trim();
            var longitud = data.AsString(Properties.Distribution.Longitud, 10).Trim();
            var esBase = latitud.Trim().Equals(string.Empty) && longitud.Trim().Equals(string.Empty);
            //var orden = data.AsString(Properties.Distribution.Orden, 2).Trim(); 
            // HACE FALTA AGREGAR EL CAMPO AL MAPEO

            var dia = Convert.ToInt32(sFecha.Substring(0, 2));
            var mes = Convert.ToInt32(sFecha.Substring(2, 2));
            var anio = Convert.ToInt32(sFecha.Substring(4, 2)) + 2000;
            var hora = Convert.ToInt32(sHora.Substring(0, 2));
            var min = Convert.ToInt32(sHora.Substring(2, 2));
            var gmt = new TimeSpan(-3, 0, 0);
            var fecha = new DateTime(anio, mes, dia, hora, min, 0).Subtract(gmt);

            if (item.Id == 0)
            {
                item.Empresa = oEmpresa;
                item.Linea = oLinea;
                item.Vehiculo = vehiculo;
                item.Inicio = fecha;
                item.Fin = fecha;
                item.Tipo = ViajeDistribucion.Tipos.Desordenado;
                item.RegresoABase = true;
                item.Estado = ViajeDistribucion.Estados.Pendiente;
                item.Alta = DateTime.UtcNow;
                item.ProgramacionDinamica = codigo.Contains("TR");

                var nroViaje = data.AsString(Properties.Distribution.NroViaje, 1).Trim();
                item.NumeroViaje = Convert.ToInt32(nroViaje);

                if (patente != string.Empty)
                {
                    if (vehiculo != null)
                    {   
                        item.Empleado = !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                        item.CentroDeCostos = vehiculo.CentroDeCostos;
                        item.SubCentroDeCostos = vehiculo.SubCentroDeCostos;
                    }
                    else
                    {
                        STrace.Error("Logiclink", string.Format("Patente {0} no encontrada para el viaje: {1}", patente, codigo));
                    }
                }
                else
                {
                    STrace.Error("Logiclink", "Patente vacía para el viaje: " + codigo);
                }
            }
            else
            {
                if (fecha < item.Inicio) item.Inicio = fecha;
            }

            var km = data.AsString(Properties.Distribution.Km, 32).Trim();
            var distance = Convert.ToDouble(km, CultureInfo.InvariantCulture);

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
                        distance = directions.Distance / 1000.0;
                        var duracion = directions.Duration;
                        fecha = item.Detalles.Last().Programado.Add(duracion);
                    }
                }

                var llegada = new EntregaDistribucion
                                  {
                                      Linea = oLinea,
                                      Descripcion = oLinea.Descripcion,
                                      Estado = EntregaDistribucion.Estados.Pendiente,
                                      Orden = item.Detalles.Count,
                                      Programado = fecha,
                                      ProgramadoHasta = fecha,
                                      Viaje = item,
                                      KmCalculado = distance
                                  };
                item.Detalles.Add(llegada);

                return item;
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

            var codigoPuntoEntrega = data.AsString(Properties.Distribution.PuntoEntrega, 16).Trim();
            var nombre = data.AsString(Properties.Distribution.Nombre, 32).Trim();
            if (string.IsNullOrEmpty(codigoPuntoEntrega)) ThrowProperty("PuntoEntrega");

            if (item.Detalles.Any(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codigoPuntoEntrega))
                return item;

            TipoServicioCiclo tipoServicio = null;
            var tipoServ = DaoFactory.TipoServicioCicloDAO.FindDefault(new[] {empresa}, new[] {oLinea.Id});
            if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

            latitud = latitud.Replace(',', '.');
            longitud = longitud.Replace(',', '.');
            var lat = Convert.ToDouble(latitud, CultureInfo.InvariantCulture);
            var lon = Convert.ToDouble(longitud, CultureInfo.InvariantCulture);
            ValidateGpsPoint(codigo, codigoPuntoEntrega, (float)lat, (float)lon);

            var puntoEntrega = DaoFactory.PuntoEntregaDAO.GetByCode(new[] {empresa}, new[] {oLinea.Id}, new[] {oCliente.Id}, codigoPuntoEntrega);

            if (puntoEntrega == null)
            {
                var empresaGeoRef = item.Vehiculo != null && item.Vehiculo.Empresa == null ? null : oCliente.Empresa == null ? null : oEmpresa;
                var lineaGeoRef = item.Vehiculo != null && item.Vehiculo.Linea == null ? null : oCliente.Linea == null ? null : oLinea;

                var puntoDeInteres = new ReferenciaGeografica
                                         {
                                             Codigo = codigoPuntoEntrega,
                                             Descripcion = nombre,
                                             Empresa = empresaGeoRef,
                                             Linea = lineaGeoRef,
                                             EsFin = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                             EsInicio = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                             EsIntermedio = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                             InhibeAlarma = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                             TipoReferenciaGeografica = oCliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                             Vigencia = new Vigencia {Inicio = DateTime.UtcNow, Fin = fecha.AddHours(vigencia)},
                                             Icono = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
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

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoDeInteres);

                puntoEntrega = new PuntoEntrega
                                   {
                                       Cliente = oCliente,
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

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoEntrega.ReferenciaGeografica);
            }

            DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);

            if (codigo.Contains("TR"))
            {
                var ultimo = item.Detalles.Last().ReferenciaGeografica;
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

            if (item.Detalles.Last().TipoServicio != null)
                fecha = fecha.AddMinutes(item.Detalles.Last().TipoServicio.Demora);

            var entrega = new EntregaDistribucion
                              {
                                  Cliente = oCliente,
                                  PuntoEntrega = puntoEntrega,
                                  Descripcion = codigoPuntoEntrega,
                                  Estado = EntregaDistribucion.Estados.Pendiente,
                                  Orden = item.Detalles.Count,
                                  //Orden = Convert.ToInt32(orden, CultureInfo.InvariantCulture),
                                  Programado = fecha,
                                  ProgramadoHasta = fecha,
                                  TipoServicio = tipoServicio,
                                  Viaje = item,
                                  KmCalculado = distance
                              };

            item.Detalles.Add(entrega);

            var lastDetail = item.Detalles.LastOrDefault();
            item.Fin = lastDetail == null ? item.Inicio : lastDetail.Programado;
            return item;
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
            var destinatarios = new List<string> {"soporte@logictracker.com", "metzler.lucas@gmail.com"};

            if (WebSecurity.AuthenticatedUser.Name != string.Empty)
            {
                var usuario = DaoFactory.UsuarioDAO.GetByNombreUsuario(WebSecurity.AuthenticatedUser.Name);
                if (usuario != null && usuario.Email.Trim() != string.Empty)
                    destinatarios.Add(usuario.Email.Trim());
            }

            sender.Config.Subject = "Logiclink: Error de importación";
            foreach (var destinatario in destinatarios)
            {
                sender.Config.ToAddress = destinatario;
                sender.SendMail(parametros);
                STrace.Trace("Logiclink", "Email sent to: " + destinatario);
            }
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateSaveOrUpdate(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateDelete(tipo)) DaoFactory.ViajeDistribucionDAO.Delete(tipo);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateSave(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateUpdate(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        #endregion

        protected virtual ViajeDistribucion GetDistribucion(int empresa, int linea, string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) ThrowCodigo();

            var sameCode = DaoFactory.ViajeDistribucionDAO.FindByCodigo(empresa, linea, codigo);
            return sameCode ?? new ViajeDistribucion {Codigo = codigo};
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
                           Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                       }
                ;
        }
    }
}
