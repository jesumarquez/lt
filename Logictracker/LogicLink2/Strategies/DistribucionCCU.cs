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
using Logictracker.Process.Import.Client.Types;
using LinqToExcel;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class DistribucionCCU : Strategy
    {
        private static Dictionary<int, List<int>> EmpresasLineas = new Dictionary<int, List<int>>();
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Cliente Cliente { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }

        private readonly List<PuntoEntrega> _clientesBuffer = new List<PuntoEntrega>();
        private readonly List<ViajeDistribucion> _viajesBuffer = new List<ViajeDistribucion>();
        private readonly List<Linea> _lineasBuffer = new List<Linea>();
        private readonly List<Coche> _cochesBuffer = new List<Coche>();

        public static Dictionary<int, List<int>> ParseRutas(LogicLinkFile file, out int rutas, out int entregas, out string observaciones)
        {
            new DistribucionCCU(file).ParseRutas(out rutas, out entregas, out observaciones);
            return EmpresasLineas;
        }
        public static void ParseClientes(LogicLinkFile file, out int clientes)
        {
            new DistribucionCCU(file).ParseClientes(out clientes);            
        }
        public static void ParseAsignaciones(LogicLinkFile file, out int rutas, out string observaciones)
        {
            new DistribucionCCU(file).ParseAsignaciones(out rutas, out observaciones);
        }

        public DistribucionCCU(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Cliente = DaoFactory.ClienteDAO.GetList(new[] { Empresa.Id }, new[] { -1 }).FirstOrDefault();
        }

        public void ParseAsignaciones(out int rutas, out string observaciones)
        {
            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, true);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferAsignaciones(rows);
            STrace.Trace(Component, string.Format("PreBufferAsignaciones en {0} segundos", te.getTimeElapsed().TotalSeconds));

            var listViajes = new List<ViajeDistribucion>();
            
            rutas = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));

                var planilla = row[Properties.AsignacionCCU.Planilla].ToString().Trim();
                var viaje = _viajesBuffer.SingleOrDefault(l => l.Codigo == planilla);
                if (viaje == null)
                {
                    observaciones = observaciones + "PLANILLA: " + planilla + " no encontrada. ";
                    continue;
                }

                var patente = row[Properties.AsignacionCCU.Patente].ToString().Trim();
                var vehiculo = _cochesBuffer.SingleOrDefault(v => v.Patente == patente);
                if (vehiculo == null)
                {
                    observaciones = observaciones + "PATENTE: " + patente + " no encontrada. ";
                    continue;
                }

                viaje.Vehiculo = vehiculo;

                listViajes.Add(viaje);
                rutas++;
            }
            
            STrace.Trace(Component, "Guardando Viajes: " + listViajes.Count);
            te.Restart();
            foreach (var viajeDistribucion in listViajes)
            {
                DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
            }
            STrace.Trace(Component, string.Format("Viajes guardados en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }
        public void ParseRutas(out int rutas, out int entregas, out string observaciones)
        {
            const int vigencia = 12;

            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, true);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferRutas(rows);
            STrace.Trace(Component, string.Format("PreBufferRutas en {0} segundos", te.getTimeElapsed().TotalSeconds));

            var listViajes = new List<ViajeDistribucion>();
            var listReferencias = new List<ReferenciaGeografica>();
            var listPuntosEntrega = new List<PuntoEntrega>();

            rutas = 0;
            entregas = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));

                var codigoLinea = "4"; // POR DEFAULT ES MUNRO
                var linea = _lineasBuffer.SingleOrDefault(l => l.DescripcionCorta == codigoLinea);
                if (linea == null)
                {
                    observaciones = "Valor inválido para el campo LINEA";
                    continue;
                }

                var sFecha = row[Properties.DistribucionCCU.Fecha].ToString().Trim();
                var planilla = row[Properties.DistribucionCCU.Planilla].ToString().Trim();
                var codCliente = row[Properties.DistribucionCCU.CodigoCliente].ToString().Trim();
                var factura = row[Properties.DistribucionCCU.Factura].ToString().Trim();
                var sHectolitros = row[Properties.DistribucionCCU.Hectolitros].ToString().Trim().Replace('.', ',');
                var sImporteTotal = row[Properties.DistribucionCCU.ImporteTotal].ToString().Trim().Replace('.', ',');
                var hectolitros = Convert.ToDouble(sHectolitros);
                var importeTotal = Convert.ToDouble(sImporteTotal);

                var dia = Convert.ToInt32(sFecha.Substring(0, 2));
                var mes = Convert.ToInt32(sFecha.Substring(3, 2));
                var anio = Convert.ToInt32(sFecha.Substring(6, 4));
                var gmt = new TimeSpan(-3, 0, 0);
                var fecha = new DateTime(anio, mes, dia, 4, 0, 0).Subtract(gmt);

                if (listViajes.Count == 0 || planilla != listViajes.Last().Codigo)
                {
                    var byCode = _viajesBuffer.SingleOrDefault(v => v.Codigo == planilla);
                    if (byCode != null) continue;
                }

                ViajeDistribucion viaje;

                if (listViajes.Count > 0 && listViajes.Any(v => v.Codigo == planilla))
                {
                    viaje = listViajes.SingleOrDefault(v => v.Codigo == planilla);                    
                }
                else
                {
                    viaje = new ViajeDistribucion
                    {
                        Empresa = this.Empresa,
                        Linea = linea,
                        Codigo = planilla,
                        Inicio = fecha,
                        Fin = fecha,
                        Tipo = ViajeDistribucion.Tipos.Desordenado,
                        RegresoABase = true,
                        Estado = ViajeDistribucion.Estados.Pendiente,
                        Alta = DateTime.UtcNow,
                        NumeroViaje = 1
                    };

                    listViajes.Add(viaje);
                    rutas++;
                }

                if (viaje.Detalles.Count == 0)
                {
                    //el primer elemento es la base
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

                    var llegada = new EntregaDistribucion
                    {
                        Linea = linea,
                        Descripcion = linea.Descripcion,
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Orden = viaje.Detalles.Count,
                        Programado = fecha,
                        ProgramadoHasta = fecha,
                        Viaje = viaje
                    };
                    viaje.Detalles.Add(llegada);
                }

                if (viaje.Detalles.Where(d => d.PuntoEntrega != null).Any(d => d.PuntoEntrega.Codigo == codCliente))
                {
                    var detalle = viaje.Detalles.Where(d => d.PuntoEntrega != null).SingleOrDefault(d => d.PuntoEntrega.Codigo == codCliente);
                    detalle.Volumen += hectolitros;
                    detalle.Valor = importeTotal;
                    continue;
                }                   

                var puntoEntrega = _clientesBuffer.SingleOrDefault(p => p.Codigo == codCliente);

                if (puntoEntrega == null)
                {
                    observaciones = "Valor inválido para el campo PUNTO ENTREGA";
                    continue;
                }
                else
                {
                    var end = fecha.AddHours(vigencia);
                    if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                        puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                    listReferencias.Add(puntoEntrega.ReferenciaGeografica);
                    listPuntosEntrega.Add(puntoEntrega);
                }

                var entrega = new EntregaDistribucion
                                {
                                    Cliente = puntoEntrega.Cliente,
                                    PuntoEntrega = puntoEntrega,
                                    Descripcion = factura,
                                    Estado = EntregaDistribucion.Estados.Pendiente,
                                    Orden = viaje.Detalles.Count - 1,
                                    Programado = fecha,
                                    ProgramadoHasta = fecha.AddMinutes(Empresa.MarginMinutes),                                    
                                    Viaje = viaje,
                                    Volumen = hectolitros,
                                    Valor = importeTotal
                                };

                viaje.Detalles.Add(entrega);
                entregas++;
            }

            var nroViaje = 0;            
            foreach (var viajeDistribucion in listViajes)
            {
                nroViaje++;                
                if (viajeDistribucion.Detalles.Count > 0)
                {
                    var dirBase = viajeDistribucion.Detalles.First().ReferenciaGeografica;
                    var velocidadPromedio = 20;

                    var hora = viajeDistribucion.Inicio;
                    var nroEntrega = 0;
                    foreach (var detalle in viajeDistribucion.Detalles)
                    {
                        nroEntrega++;
                        STrace.Trace(Component, string.Format("Calculando horarios, viaje {0}/{1}, entrega {2}/{3}", nroViaje, listViajes.Count, nroEntrega, viajeDistribucion.Detalles.Count()));
                        var distancia = GeocoderHelper.CalcularDistacia(dirBase.Latitude, dirBase.Longitude, detalle.ReferenciaGeografica.Latitude, detalle.ReferenciaGeografica.Longitude);
                        var horas = distancia / velocidadPromedio;
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
                AddReferenciasGeograficas(referenciaGeografica);
            }
            STrace.Trace(Component, string.Format("Referencias guardadas en {0} segundos", te.getTimeElapsed().TotalSeconds));

            STrace.Trace(Component, "Guardando puntos de entrega: " + listPuntosEntrega.Count);
            te.Restart();
            foreach (var puntoEntrega in listPuntosEntrega)
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
        public void ParseClientes(out int clientes)
        {
            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, true);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferClientes(rows);
            STrace.Trace(Component, string.Format("PreBufferClientes en {0} segundos", te.getTimeElapsed().TotalSeconds));

            var listClientes = new List<PuntoEntrega>();
            var listReferencias = new List<ReferenciaGeografica>();
            clientes = 0;
            var filas = 0;

            foreach(var fila in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count-1));

                var codigoCliente = fila[Properties.ClienteCCU.CodigoCliente].ToString().Trim();
                var nombre = fila[Properties.ClienteCCU.Nombre].ToString().Trim();
                var direccion = fila[Properties.ClienteCCU.Direccion].ToString().Trim();
                var latitud = fila[Properties.ClienteCCU.Latitud].ToString().Trim();
                var longitud = fila[Properties.ClienteCCU.Longitud].ToString().Trim();
                //var localidad = fila[Properties.ClienteCCU.Localidad].ToString().Trim();
                //var barrio = fila[Properties.ClienteCCU.Barrio].ToString().Trim();

                if (string.IsNullOrEmpty(codigoCliente)) continue;
                
                double lat, lon = 0.0;
                if (!double.TryParse(latitud, out lat) || !double.TryParse(longitud, out lon)) continue;                
                
                var puntoEntrega = _clientesBuffer.SingleOrDefault(p => p.Codigo == codigoCliente);

                if (puntoEntrega == null)
                {
                    var empresaGeoRef = Empresa;
                    //var lineaGeoRef = oLinea;

                    var puntoDeInteres = new ReferenciaGeografica
                    {
                        Codigo = codigoCliente,
                        Descripcion = nombre,
                        Empresa = empresaGeoRef,
                        //Linea = lineaGeoRef,
                        EsFin = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                        EsInicio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                        EsIntermedio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                        InhibeAlarma = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                        TipoReferenciaGeografica = Cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                        Vigencia = new Vigencia { Inicio = DateTime.UtcNow, Fin = DateTime.UtcNow },
                        Icono = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                    };

                    var posicion = GetNewDireccion(lat, lon);

                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    poligono.AddPoints(new[] { new PointF((float)lon, (float)lat) });

                    puntoDeInteres.Historia.Add(new HistoriaGeoRef
                    {
                        ReferenciaGeografica = puntoDeInteres,
                        Direccion = posicion,
                        Poligono = poligono,
                        Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                    });

                    listReferencias.Add(puntoDeInteres);

                    puntoEntrega = new PuntoEntrega
                    {
                        Cliente = Cliente,
                        Codigo = codigoCliente,
                        Descripcion = direccion,
                        Telefono = string.Empty,
                        Baja = false,
                        ReferenciaGeografica = puntoDeInteres,
                        Nomenclado = true,
                        DireccionNomenclada = string.Empty,
                        Nombre = nombre
                    };

                    listClientes.Add(puntoEntrega);
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

                        listReferencias.Add(puntoEntrega.ReferenciaGeografica);

                        puntoEntrega.Descripcion = direccion;
                        puntoEntrega.Nombre = nombre;

                        listClientes.Add(puntoEntrega);
                    }                    
                }                
            }

            STrace.Trace(Component, "Guardando referencias geográficas: " + listReferencias.Count);
            te.Restart();
            var referencia = 0;
            var referenciasTotales = listReferencias.Count();
            foreach (var referenciaGeografica in listReferencias)
            {
                referencia++;
                STrace.Trace(Component, string.Format("Guardando referencia: {0}/{1}", referencia, referenciasTotales));
                DaoFactory.ReferenciaGeograficaDAO.Guardar(referenciaGeografica);
            }
            STrace.Trace(Component, string.Format("Referencias guardadas en {0} segundos", te.getTimeElapsed().TotalSeconds));

            STrace.Trace(Component, "Guardando clientes: " + listClientes.Count);
            te.Restart();
            var clientesTotales = listClientes.Count();
            foreach (var puntoEntrega in listClientes)
            {
                clientes++;
                STrace.Trace(Component, string.Format("Guardando clientes: {0}/{1}", clientes, clientesTotales));
                DaoFactory.PuntoEntregaDAO.SaveOrUpdateWithoutTransaction(puntoEntrega);                
            }
            STrace.Trace(Component, string.Format("Clientes guardados en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }        

        private void PreBufferAsignaciones(IEnumerable<Row> rows)
        {
            var lastCodRuta = string.Empty;
            var lastPatente = string.Empty;

            var codRutaStrList = new List<string>();
            var patenteStrList = new List<string>();

            for (int i = 0; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);

                STrace.Trace(Component, string.Format("Prebuffering: {0}/{1}", i+1, rows.Count()));

                #region Buffer Ruta

                try
                {
                    var codigoRuta = row[Properties.AsignacionCCU.Planilla].ToString().Trim();

                    if (lastCodRuta != codigoRuta)
                    {
                        if (!codRutaStrList.Contains(codigoRuta))
                            codRutaStrList.Add(codigoRuta);

                        lastCodRuta = codigoRuta;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Ruta ({0})", row[Properties.AsignacionCCU.Planilla]));
                }

                #endregion

                #region Buffer Vehiculo

                try
                {
                    var patente = row[Properties.AsignacionCCU.Patente].ToString().Trim();

                    if (lastPatente != patente)
                    {
                        if (!patenteStrList.Contains(patente))
                            patenteStrList.Add(patente);

                        lastPatente = patente;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Patente ({0})", row[Properties.AsignacionCCU.Patente]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codRutaStrList.Any())
            {
                var cant = 0;
                foreach (var l in codRutaStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering: {0}/{1}", cant, codRutaStrList.Count()));

                    var rutas = DaoFactory.ViajeDistribucionDAO.FindByCodigos(new[] { Empresa.Id },
                                                                              new[] { -1 },
                                                                              l);
                    if (rutas != null && rutas.Any())
                    {
                        _viajesBuffer.AddRange(rutas);
                    }
                }
            }

            if (patenteStrList.Any())
            {
                var cant = 0;
                foreach (var l in patenteStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering: {0}/{1}", cant, patenteStrList.Count()));

                    var coches = DaoFactory.CocheDAO.FindByPatentes(Empresa.Id, l);
                    if (coches != null && coches.Any())
                    {
                        _cochesBuffer.AddRange(coches);
                    }
                }
            }
        }
        private void PreBufferRutas(IEnumerable<Row> rows)
        {
            var lastCodLinea = string.Empty;
            var lastCodRuta = string.Empty;
            var lastCodCliente = string.Empty;

            var codLineaStrList = new List<string>();
            var codRutaStrList = new List<string>();
            var codClienteStrList = new List<string>();

            for (int i = 0; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);

                STrace.Trace(Component, string.Format("Prebuffering: {0}/{1}", i+1, rows.Count()));

                #region Buffer Base

                try
                {
                    var codigoLinea = "4";

                    if (lastCodLinea != codigoLinea)
                    {
                        if (!codLineaStrList.Contains(codigoLinea))
                            codLineaStrList.Add(codigoLinea);

                        lastCodLinea = codigoLinea;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Linea ({0})", "4"));
                }

                #endregion

                #region Buffer Ruta

                try
                {
                    var codigoRuta = row[Properties.DistribucionCCU.Planilla].ToString().Trim();

                    if (lastCodRuta != codigoRuta)
                    {
                        if (!codRutaStrList.Contains(codigoRuta))
                            codRutaStrList.Add(codigoRuta);

                        lastCodRuta = codigoRuta;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Ruta ({0})", row[Properties.DistribucionCCU.Planilla]));
                }

                #endregion

                #region Buffer Cliente

                try
                {
                    var codigoCliente = row[Properties.DistribucionCCU.CodigoCliente].ToString().Trim();

                    if (lastCodCliente != codigoCliente)
                    {
                        if (!codClienteStrList.Contains(codigoCliente))
                            codClienteStrList.Add(codigoCliente);

                        lastCodCliente = codigoCliente;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Cliente ({0})", row[Properties.DistribucionCCU.CodigoCliente]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codLineaStrList.Any())
            {
                var cant = 0;
                foreach (var l in codLineaStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering: {0}/{1}", cant, codLineaStrList.Count()));

                    var lineas = DaoFactory.LineaDAO.FindByCodigos(new[] { Empresa.Id }, l);
                    if (lineas != null && lineas.Any())
                    {
                        _lineasBuffer.AddRange(lineas);
                    }
                }
            }

            if (codRutaStrList.Any())
            {
                var cant = 0;
                foreach (var l in codRutaStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering: {0}/{1}", cant, codRutaStrList.Count()));

                    var rutas = DaoFactory.ViajeDistribucionDAO.FindByCodigos(new[] { Empresa.Id },
                                                                              new[] { -1 },
                                                                              l);
                    if (rutas != null && rutas.Any())
                    {
                        _viajesBuffer.AddRange(rutas);
                    }
                }
            }

            if (codClienteStrList.Any())
            {
                var cant = 0;
                foreach (var l in codClienteStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering: {0}/{1}", cant, codClienteStrList.Count()));

                    var puntos = DaoFactory.PuntoEntregaDAO.FindByCodes(new[] { Empresa.Id },
                                                                        new[] { -1 },
                                                                        new[] { Cliente.Id },
                                                                        l);
                    if (puntos != null && puntos.Any())
                    {
                        _clientesBuffer.AddRange(puntos);
                    }
                }
            }
        }
        private void PreBufferClientes(IEnumerable<Row> rows)
        {
            var lastCodCliente = string.Empty;
            var codClienteStrList = new List<string>();

            for (int i = 0; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);

                STrace.Trace(Component, string.Format("Prebuffering: {0}/{1}", i+1, rows.Count()));

                #region Buffer Cliente

                try
                {
                    var codigoCliente = row[Properties.ClienteCCU.CodigoCliente].ToString().Trim();

                    if (lastCodCliente != codigoCliente)
                    {
                        if (!codClienteStrList.Contains(codigoCliente))
                            codClienteStrList.Add(codigoCliente);

                        lastCodCliente = codigoCliente;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Cliente ({0})", row[Properties.ClienteCCU.CodigoCliente]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codClienteStrList.Any())
            {
                var cant = 0;
                foreach (var l in codClienteStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering: {0}/{1}", cant, codClienteStrList.Count()));

                    var puntos = DaoFactory.PuntoEntregaDAO.FindByCodes(new[] { Empresa.Id },
                                                                        new[] { -1 },
                                                                        new[] { Cliente.Id },
                                                                        l);
                    if (puntos != null && puntos.Any())
                    {
                        _clientesBuffer.AddRange(puntos);
                    }
                }
            }
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

        private static void AddReferenciasGeograficas(ReferenciaGeografica rg)
        {
            if (rg == null)
                STrace.Error(Component, "AddReferenciasGeograficas: rg is null");
            else if (rg.Empresa == null)
                STrace.Error(Component, "AddReferenciasGeograficas: rg.Empresa is null");
            else
            {
                if (!EmpresasLineas.ContainsKey(rg.Empresa.Id))
                    EmpresasLineas.Add(rg.Empresa.Id, new List<int> { -1 });

                if (rg.Linea != null)
                {
                    if (!EmpresasLineas[rg.Empresa.Id].Contains(rg.Linea.Id))
                        EmpresasLineas[rg.Empresa.Id].Add(rg.Linea.Id);
                }
                else
                {
                    var todaslaslineas = new DAOFactory().LineaDAO.GetList(new[] { rg.Empresa.Id });
                    foreach (var linea in todaslaslineas)
                    {
                        if (!EmpresasLineas.ContainsKey(linea.Id))
                            EmpresasLineas[rg.Empresa.Id].Add(linea.Id);
                    }
                }
            }
        }
    }
}
