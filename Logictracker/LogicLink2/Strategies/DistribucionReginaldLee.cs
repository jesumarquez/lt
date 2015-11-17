using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class DistribucionReginaldLee : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Cliente Cliente { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private readonly List<ViajeDistribucion> _viajesBuffer = new List<ViajeDistribucion>();
        private readonly List<Coche> _cochesBuffer = new List<Coche>();
        private readonly List<TipoServicioCiclo> _tiposServicioBuffer = new List<TipoServicioCiclo>();

        public static void Parse(LogicLinkFile file, out int rutas, out int entregas)
        {
            new DistribucionReginaldLee(file).Parse(out rutas, out entregas);
        }

        public DistribucionReginaldLee(LogicLinkFile file) 
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Cliente = DaoFactory.ClienteDAO.GetList(new[] { Empresa.Id }, new[] { -1 }).FirstOrDefault();
        }

        public void Parse(out int rutas, out int entregas)
        {
            const int vigencia = 24;

            var te = new TimeElapsed();
            var rows = ParseFile(Llfile.FilePath, Properties.DistribucionReginaldLee.Anchos).Rows;
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferRows(rows);
            STrace.Trace(Component, string.Format("PreBufferRows en {0} segundos", te.getTimeElapsed().TotalSeconds));

            var listViajes = new List<ViajeDistribucion>(rows.Count);
            var listPuntos = new List<PuntoEntrega>();
            var listReferencias = new List<ReferenciaGeografica>();

            rutas = 0;
            entregas = 0;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));

                var ruta = row.Cells[Properties.DistribucionReginaldLee.CodigoRuta].ToString().Trim();
                if (ruta == string.Empty) ThrowProperty("CODIGO_RUTA", Llfile.Strategy);

                var nroViaje = 1;
                if (ruta.Contains(","))
                {
                    var rutaSplitted = ruta.Split(',');
                    int.TryParse(rutaSplitted[1], out nroViaje);
                }

                var codEntrega = row.Cells[Properties.DistribucionReginaldLee.CodigoPedido].ToString().Trim();
                if (codEntrega == string.Empty) ThrowProperty("CODIGO_ENTREGA", Llfile.Strategy);

                var codCliente = row.Cells[Properties.DistribucionReginaldLee.CodigoCliente].ToString().Trim();
                if (codCliente == string.Empty) ThrowProperty("CODIGO_CLIENTE", Llfile.Strategy);

                var rowWithCoords = true;
                double latitud;
                double longitud;

                if (!double.TryParse(row.Cells[Properties.DistribucionReginaldLee.Latitud].ToString().Trim(), out latitud))
                    rowWithCoords = false;
                if (!double.TryParse(row.Cells[Properties.DistribucionReginaldLee.Longitud].ToString().Trim(), out longitud))
                    rowWithCoords = false;
                
                var orientacionSouthNorth = row.Cells[Properties.DistribucionReginaldLee.OrientacionNorthSouth].ToString().Trim();
                var orientacionEastWest = row.Cells[Properties.DistribucionReginaldLee.OrientacionEastWest].ToString().Trim();

                if (orientacionSouthNorth.Equals("S"))
                    latitud = latitud*(-1);
                if (orientacionEastWest.Equals("W"))
                    longitud = longitud * (-1);

                var fecha = row.Cells[Properties.DistribucionReginaldLee.Fecha].ToString().Trim();
                if (fecha == string.Empty) ThrowProperty("FECHA", Llfile.Strategy);

                int dia, mes, anio, hr, min;
                int.TryParse(fecha.Substring(0, 4), out anio);
                int.TryParse(fecha.Substring(5, 2), out mes);
                int.TryParse(fecha.Substring(8, 2), out dia);
                int.TryParse(fecha.Substring(11, 2), out hr);
                int.TryParse(fecha.Substring(14, 2), out min);

                var gmt = new TimeSpan(-3, 0, 0);
                var date = new DateTime(anio, mes, dia, hr, min, 0).Subtract(gmt);

                int packs;
                if (!int.TryParse(row.Cells[Properties.DistribucionReginaldLee.Packs].ToString().Trim(), out packs))
                {
                    try
                    {
                        ThrowProperty("PACKS", Llfile.Strategy);
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(Component, ex2, String.Format("Error en Fila #{0}: {1}", filas, ex2.Message));
                        continue;
                    }
                }

                var codigo = date.ToString("yyMMdd") + "|" + ruta;

                if (listViajes.Count == 0 || codigo != listViajes.Last().Codigo)
                {
                    var byCode = _viajesBuffer.SingleOrDefault(v => v.Codigo == codigo);
                    if (byCode != null) continue;
                }

                ViajeDistribucion item;

                if (listViajes.Count > 0 && listViajes.Any(v => v.Codigo == codigo))
                {
                    item = listViajes.SingleOrDefault(v => v.Codigo == codigo);
                }
                else
                {
                    item = new ViajeDistribucion { Codigo = codigo };
                    rutas++;

                    Coche vehiculo = null;
                    var interno = row.Cells[Properties.DistribucionReginaldLee.Interno].ToString().Trim();
                    if (interno.Length == 3) interno = "0" + interno;
                    if (!string.IsNullOrEmpty(interno)) vehiculo = _cochesBuffer.SingleOrDefault(c => c.Interno == interno);

                    if (vehiculo == null) ThrowProperty("VEHICULO", Llfile.Strategy);
                    else if (vehiculo.Linea == null) ThrowProperty("BASE", Llfile.Strategy);
                    
                    item.Empresa = Empresa;
                    item.Linea = vehiculo.Linea;
                    item.Vehiculo = vehiculo;
                    item.Inicio = date;
                    item.Fin = date;
                    item.Tipo = ViajeDistribucion.Tipos.Desordenado;
                    item.RegresoABase = true;
                    item.Estado = ViajeDistribucion.Estados.Pendiente;
                    item.Alta = DateTime.UtcNow;
                    item.NumeroViaje = Convert.ToInt32(nroViaje);

                    listViajes.Add(item);
                }

                // Entregas
                if (item.Detalles.Count == 0)
                {   // Si no existe, agrego la salida de base
                    var origen = new EntregaDistribucion
                    {
                        Linea = item.Linea,
                        Descripcion = item.Linea.Descripcion,
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Orden = 0,
                        Programado = date,
                        ProgramadoHasta = date,
                        Viaje = item
                    };
                    item.Detalles.Add(origen);

                    var llegada = new EntregaDistribucion
                    {
                        Linea = item.Linea,
                        Descripcion = item.Linea.Descripcion,
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Orden = item.Detalles.Count,
                        Programado = date,
                        ProgramadoHasta = date,
                        Viaje = item
                    };
                    item.Detalles.Add(llegada);
                }

                if (item.Detalles.Any(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codCliente))
                {
                    var repetido = item.Detalles.FirstOrDefault(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codCliente);
                    repetido.Bultos += packs;
                    continue;
                }

                TipoServicioCiclo tipoServicio = null;
                var tipoServ = _tiposServicioBuffer.SingleOrDefault(ts => ts.Linea.Id == item.Linea.Id || ts.Linea == null);
                if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

                var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codCliente);
                if (puntoEntrega == null)
                {
                    var descCliente = row.Cells[Properties.DistribucionReginaldLee.DescripcionCliente].ToString().Trim();

                    var puntoDeInteres = new ReferenciaGeografica
                    {
                        Codigo = codCliente,
                        Descripcion = descCliente,
                        Empresa = Empresa,
                        Linea = item.Linea,
                        EsFin = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                        EsInicio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                        EsIntermedio = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                        InhibeAlarma = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                        TipoReferenciaGeografica = Cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                        Vigencia = new Vigencia { Inicio = date.Date, Fin = date.AddHours(vigencia) },
                        Icono = Cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                    };

                    if (!rowWithCoords)
                    {
                        try
                        {
                            ThrowProperty("LATITUD_LONGITUD", Llfile.Strategy);
                        }
                        catch (Exception ex)
                        {
                            STrace.Exception(Component, ex, String.Format("Error en Fila #{0}: {1}", filas,  ex.Message));
                            continue;
                        }
                    }

                    var posicion = GetNewDireccion(latitud, longitud);

                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = date.Date } };
                    poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

                    puntoDeInteres.Historia.Add(new HistoriaGeoRef
                    {
                        ReferenciaGeografica = puntoDeInteres,
                        Direccion = posicion,
                        Poligono = poligono,
                        Vigencia = new Vigencia { Inicio = date.Date }
                    });

                    listReferencias.Add(puntoDeInteres);

                    puntoEntrega = new PuntoEntrega
                    {
                        Cliente = Cliente,
                        Codigo = codCliente,
                        Descripcion = descCliente,
                        Telefono = string.Empty,
                        Baja = false,
                        ReferenciaGeografica = puntoDeInteres,
                        Nomenclado = true,
                        DireccionNomenclada = string.Empty,
                        Nombre = descCliente
                    };
                }
                else
                {
                    if (!rowWithCoords)
                        try
                        {
                            ThrowProperty("LATITUD_LONGITUD", Llfile.Strategy);
                        }
                        catch (Exception ex)
                        {
                            STrace.Exception(Component, ex, String.Format("Error en Fila #{0}: {1}", filas, ex.Message));
                            continue;
                        }

                    if (!puntoEntrega.ReferenciaGeografica.IgnoraLogiclink && (puntoEntrega.ReferenciaGeografica.Latitude != latitud || puntoEntrega.ReferenciaGeografica.Longitude != longitud))
                    {
                        puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = date.Date;
                        puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = date.Date;

                        var posicion = GetNewDireccion(latitud, longitud);
                        var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = date.Date } };
                        poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

                        puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, date.Date);
                    }

                    if (puntoEntrega.ReferenciaGeografica.Vigencia.Inicio > date.Date)
                        puntoEntrega.ReferenciaGeografica.Vigencia.Inicio = date.Date;

                    var end = date.AddHours(vigencia);
                    if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                        puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                    puntoEntrega.ReferenciaGeografica.Linea = item.Linea;
                    listReferencias.Add(puntoEntrega.ReferenciaGeografica);
                }

                listPuntos.Add(puntoEntrega);

                var entrega = new EntregaDistribucion
                {
                    Cliente = puntoEntrega.Cliente,
                    PuntoEntrega = puntoEntrega,
                    Descripcion = codEntrega,
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Orden = item.Detalles.Count - 1,
                    Programado = date,
                    ProgramadoHasta = date,
                    TipoServicio = tipoServicio,
                    Viaje = item,
                    Bultos = packs
                };

                item.Detalles.Add(entrega);
                entregas++;

                var maxDate = item.Detalles.Max(d => d.Programado);
                item.Fin = maxDate;

                var ultimo = item.Detalles.Last(e => e.Linea != null);
                ultimo.Programado = maxDate;
                ultimo.ProgramadoHasta = maxDate;
                ultimo.Orden = item.Detalles.Count - 1;
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
            var lastInterno = string.Empty;

            var codPuntoStrList = new List<string>();
            var codViajeStrList = new List<string>();
            var internoStrList = new List<string>();

            foreach (var row in rows)
            {
                #region Buffer PuntoEntrega

                try
                {
                    var codigoPuntoEntrega = row.Cells[Properties.DistribucionReginaldLee.CodigoCliente].ToString().Trim();

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
                        String.Format("Error Buffering Punto de Entrega ({0})", row.Cells[Properties.DistribucionReginaldLee.CodigoCliente]));
                }

                #endregion

                #region Buffer Viajes

                try
                {
                    var ruta = row.Cells[Properties.DistribucionReginaldLee.CodigoRuta].ToString().Trim();
                    var fecha = row.Cells[Properties.DistribucionReginaldLee.Fecha].ToString().Trim();
                    
                    int dia, mes, anio, hr, min;
                    int.TryParse(fecha.Substring(0, 4), out anio);
                    int.TryParse(fecha.Substring(5, 2), out mes);
                    int.TryParse(fecha.Substring(8, 2), out dia);
                    int.TryParse(fecha.Substring(11, 2), out hr);
                    int.TryParse(fecha.Substring(14, 2), out min);

                    var gmt = new TimeSpan(-3, 0, 0);
                    var date = new DateTime(anio, mes, dia, hr, min, 0).Subtract(gmt);

                    var codigoViaje = date.ToString("yyMMdd") + "|" + ruta;

                    if (lastCodViaje != codigoViaje)
                    {
                        if (!codViajeStrList.Contains(codigoViaje))
                            codViajeStrList.Add(codigoViaje);

                        lastCodViaje = codigoViaje;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Viaje ({0}/{1})", row.Cells[Properties.DistribucionReginaldLee.CodigoRuta], row.Cells[Properties.DistribucionReginaldLee.Fecha]));
                }

                #endregion

                #region Buffer Vehiculo

                try
                {
                    var interno = row.Cells[Properties.DistribucionReginaldLee.Interno].ToString().Trim();
                    if (interno.Length == 3) interno = "0" + interno;

                    if (lastInterno != interno)
                    {
                        if (!internoStrList.Contains(interno))
                            internoStrList.Add(interno);

                        lastInterno = interno;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Vehiculo ({0})", row.Cells[Properties.DistribucionReginaldLee.Interno]));
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
                                                                        new[] { -1 },
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

            if (internoStrList.Any())
            {
                foreach (var l in internoStrList.InSetsOf(batchSize))
                {
                    var coches = DaoFactory.CocheDAO.GetByInternos(new[] { Empresa.Id }, new[] {-1}, l);
                    if (coches != null && coches.Any())
                    {
                        _cochesBuffer.AddRange(coches);
                    }
                }
            }
        }
    }
}
