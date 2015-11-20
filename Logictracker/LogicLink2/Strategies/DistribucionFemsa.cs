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
    public class DistribucionFemsa : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Cliente Cliente { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private readonly List<ViajeDistribucion> _viajesBuffer = new List<ViajeDistribucion>();
        private readonly List<Linea> _lineasBuffer = new List<Linea>();
        private readonly List<Coche> _cochesBuffer = new List<Coche>();
        private readonly List<PreasignacionViajeVehiculo> _preasignoBuffer = new List<PreasignacionViajeVehiculo>();
        private readonly List<Empleado> _empleadosBuffer = new List<Empleado>();
        private readonly List<TipoServicioCiclo> _tiposServicioBuffer = new List<TipoServicioCiclo>();
        private readonly List <string> _rutasAExcluir = new List<string>(new []
        {   "RAM100,1", "RAM100,2", "RAM100,3", "RAM100,4", 
            "RAM200,1", "RAM200,2", "RAM200,3", "RAM200,4",
            "RAM300,1", "RAM300,2", "RAM300,3", "RAM300,4",
            "RAA400,1", "RAA400,2","RAA400,3","RAA400,4",
            "RAA454,1", "RAA454,2","RAA454,3","RAA454,4",
            "RAB198,1", "RAB198,2","RAB198,3","RAB198,4",
            "RAB199,1", "RAB199,2","RAB199,3","RAB199,4",
            "RAB920,1", "RAB920,2","RAB920,3","RAB920,4",
            "RAB899,1", "RAB899,2","RAB899,3","RAB899,4"});

        public static void Parse(LogicLinkFile file, out int rutas, out int entregas)
        {
            new DistribucionFemsa(file).Parse(out rutas, out entregas);
        }

        public DistribucionFemsa(LogicLinkFile file) 
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Cliente = DaoFactory.ClienteDAO.FindById(4186); // RESTO DE MERCADO            
        }

        public void Parse(out int rutas, out int entregas)
        {
            const char separator = ';';
            const int vigencia = 24;

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

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));

                var codLinea = row.Cells[Properties.DistribucionFemsa.Centro].ToString().Trim();

                var oLinea = _lineasBuffer.SingleOrDefault(l => l.DescripcionCorta == codLinea);

                if (oLinea == null) ThrowProperty("BASE", Llfile.Strategy);

                var ruta = row.Cells[Properties.DistribucionFemsa.Ruta].ToString().Trim();
                if (ruta == string.Empty) ThrowProperty("CODIGO_RUTA", Llfile.Strategy);

                if (_rutasAExcluir.Contains(ruta))
                {
                    STrace.Error("LogicLink2", "Ruta que pertenece al rango de rutas a excluir: " + ruta);
                    continue;                      
                }
                
                int nroViaje;
                if (ruta.Contains(","))
                {
                    var rutaSplitted = ruta.Split(',');
                    try
                    {
                        int.TryParse(rutaSplitted[1], out nroViaje);
                    }
                    catch (Exception ex)
                    {
                        nroViaje = 1;
                        try
                        {
                            ThrowProperty("NRO_VIAJE", Llfile.Strategy);
                        }
                        catch (Exception ex2)
                        {
                            STrace.Exception(Component, ex2, String.Format("Error en Fila #{0}: {1}", filas, ex.Message));
                            continue;
                        }
                    }
                }
                else
                {
                    nroViaje = 1;
                    try
                    {
                        ThrowProperty("NRO_VIAJE", Llfile.Strategy);
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(Component, ex2, String.Format("Error en Fila #{0}: {1}", filas, ex2.Message));
                    }
                }

                var codEntrega = row.Cells[Properties.DistribucionFemsa.Entrega].ToString().Trim();
                if (codEntrega == string.Empty) ThrowProperty("CODIGO_ENTREGA", Llfile.Strategy);

                var codCliente = row.Cells[Properties.DistribucionFemsa.CodigoCliente].ToString().Trim();
                if (codCliente == string.Empty) ThrowProperty("CODIGO_CLIENTE", Llfile.Strategy);

                var rowWithCoords = true;
                double latitud;
                double longitud;
                
                if (!double.TryParse(row.Cells[Properties.DistribucionFemsa.Latitud].ToString().Trim(), out latitud))
                    rowWithCoords = false;
                if (!double.TryParse(row.Cells[Properties.DistribucionFemsa.Longitud].ToString().Trim(), out longitud))
                    rowWithCoords = false;

                if (rowWithCoords)
                {
                    latitud = latitud/1000000.0;
                    longitud = longitud/1000000.0;
                }

                var fecha = row.Cells[Properties.DistribucionFemsa.Fecha].ToString().Trim();
                if (fecha == string.Empty) ThrowProperty("FECHA", Llfile.Strategy);

                var hora = row.Cells[Properties.DistribucionFemsa.Hora].ToString().Trim();
                if (hora == string.Empty) ThrowProperty("HORA", Llfile.Strategy);

                int dia, mes, anio, hr, min, seg;
                int.TryParse(fecha.Substring(0, 2), out dia);
                int.TryParse(fecha.Substring(3, 2), out mes);
                int.TryParse(fecha.Substring(6, 4), out anio);
                int.TryParse(hora.Substring(0, 2), out hr);
                if (hr == 0) hr = nroViaje <= 1 ? 6 : 12;
                int.TryParse(hora.Substring(3, 2), out min);
                int.TryParse(hora.Substring(6, 2), out seg);

                var gmt = new TimeSpan(-3, 0, 0);
                var date = new DateTime(anio, mes, dia, hr, min, seg).Subtract(gmt);

                int cajas;
                if (!int.TryParse(row.Cells[Properties.DistribucionFemsa.Cajas].ToString().Trim(), out cajas))
                {
                    try
                    {
                        ThrowProperty("CAJAS", Llfile.Strategy);
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
                    var patente = row.Cells[Properties.DistribucionFemsa.Patente].ToString().Trim();
                    if (!string.IsNullOrEmpty(patente)) vehiculo = _cochesBuffer.SingleOrDefault(c => c.Patente == patente);

                    if (vehiculo == null)
                    {
                        var asig = _preasignoBuffer.SingleOrDefault(p => p.Codigo == ruta);
                        if (asig != null) vehiculo = asig.Vehiculo;
                    }

                    Empleado empleado = null;
                    var legajo = row.Cells[Properties.DistribucionFemsa.Legajo].ToString().Trim();
                    if (!string.IsNullOrEmpty(legajo)) empleado = _empleadosBuffer.SingleOrDefault(e => e.Legajo == legajo);

                    item.Empresa = Empresa;
                    item.Linea = oLinea;
                    item.Vehiculo = vehiculo;
                    item.Empleado = empleado;
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
                        Linea = oLinea,
                        Descripcion = oLinea.Descripcion,
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Orden = 0,
                        Programado = date,
                        ProgramadoHasta = date,
                        Viaje = item
                    };
                    item.Detalles.Add(origen);

                    var llegada = new EntregaDistribucion
                    {
                        Linea = oLinea,
                        Descripcion = oLinea.Descripcion,
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
                    repetido.Bultos += cajas;
                    continue;
                }

                TipoServicioCiclo tipoServicio = null;
                var tipoServ = _tiposServicioBuffer.SingleOrDefault(ts => ts.Linea.Id == oLinea.Id || ts.Linea == null);
                if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

                var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codCliente);
                //var puntoEntrega = DaoFactory.PuntoEntregaDAO.GetByCode(new[] { Empresa.Id }, new[] { oLinea.Id }, new[] { -1 }, codCliente);
                if (puntoEntrega == null)
                {
                    var descCliente = row.Cells[Properties.DistribucionFemsa.DescripcionCliente].ToString().Trim();

                    var puntoDeInteres = new ReferenciaGeografica
                    {
                        Codigo = codCliente,
                        Descripcion = descCliente,
                        Empresa = Empresa,
                        Linea = oLinea,
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

                    puntoEntrega.ReferenciaGeografica.Linea = oLinea;
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
                    Bultos = cajas
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
            var lastCodPreAsigno = string.Empty;
            var lastCodLinea = string.Empty;
            var lastPatente = string.Empty;
            var lastLegajo = string.Empty;

            var codPuntoStrList = new List<string>();
            var codViajeStrList = new List<string>();
            var codPreAsignoStrList = new List<string>();
            var codLineaStrList = new List<string>();
            var patenteStrList = new List<string>();
            var legajoStrList = new List<string>();

            foreach (var row in rows)
            {
                #region Buffer PuntoEntrega

                try
                {
                    var codigoPuntoEntrega = row.Cells[Properties.DistribucionFemsa.CodigoCliente].ToString().Trim();

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
                        String.Format("Error Buffering Punto de Entrega ({0})", row.Cells[Properties.DistribucionFemsa.CodigoCliente]));
                }

                #endregion

                #region Buffer Viajes

                try
                {
                    var ruta = row.Cells[Properties.DistribucionFemsa.Ruta].ToString().Trim();
                    int nroViaje;
                    if (ruta.Contains(","))
                    {
                        var rutaSplitted = ruta.Split(',');
                        try
                        {
                            int.TryParse(rutaSplitted[1], out nroViaje);
                        }
                        catch (Exception)
                        {
                            nroViaje = 1;
                        }
                    }
                    else
                    {
                        nroViaje = 1;
                    }

                    var fecha = row.Cells[Properties.DistribucionFemsa.Fecha].ToString().Trim();
                    var hora = row.Cells[Properties.DistribucionFemsa.Hora].ToString().Trim();

                    int dia, mes, anio, hr, min, seg;
                    int.TryParse(fecha.Substring(0, 2), out dia);
                    int.TryParse(fecha.Substring(3, 2), out mes);
                    int.TryParse(fecha.Substring(6, 4), out anio);
                    int.TryParse(hora.Substring(0, 2), out hr);
                    if (hr == 0) hr = nroViaje <= 1 ? 6 : 12;
                    int.TryParse(hora.Substring(3, 2), out min);
                    int.TryParse(hora.Substring(6, 2), out seg);

                    var gmt = new TimeSpan(-3, 0, 0);
                    var date = new DateTime(anio, mes, dia, hr, min, seg).Subtract(gmt);

                    var codigoViaje = date.ToString("yyMMdd") + "|" + ruta;

                    if (lastCodViaje != codigoViaje)
                    {
                        if (!codViajeStrList.Contains(codigoViaje))
                            codViajeStrList.Add(codigoViaje);

                        lastCodViaje = codigoViaje;
                    }

                    if (lastCodPreAsigno != ruta)
                    {
                        if (!codPreAsignoStrList.Contains(ruta))
                            codPreAsignoStrList.Add(ruta);

                        lastCodPreAsigno = ruta;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex,
                        String.Format("Error Buffering Viaje ({0}/{1}/{2})", row.Cells[Properties.DistribucionFemsa.Ruta],
                            row.Cells[Properties.DistribucionFemsa.Fecha], row.Cells[Properties.DistribucionFemsa.Hora]));
                }


                #endregion

                #region Buffer Linea

                try
                {
                    var codigoLinea = row.Cells[Properties.DistribucionFemsa.Centro].ToString().Trim();

                    if (lastCodLinea != codigoLinea)
                    {
                        if (!codLineaStrList.Contains(codigoLinea))
                            codLineaStrList.Add(codigoLinea);

                        lastCodLinea = codigoLinea;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex,
                        String.Format("Error Linea ({0})", row.Cells[Properties.DistribucionFemsa.Centro]));
                }

                #endregion

                #region Buffer Vehiculo

                try
                {
                    var patente = row.Cells[Properties.DistribucionFemsa.Patente].ToString().Trim();

                    if (lastPatente != patente)
                    {
                        if (!patenteStrList.Contains(patente))
                            patenteStrList.Add(patente);

                        lastPatente = patente;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Vehiculo ({0})", row.Cells[Properties.DistribucionFemsa.Patente]));
                }

                #endregion

                #region Buffer Empleado

                try
                {
                    var legajo = row.Cells[Properties.DistribucionFemsa.Legajo].ToString().Trim();

                    if (lastLegajo != legajo)
                    {
                        if (!legajoStrList.Contains(legajo))
                            legajoStrList.Add(legajo);

                        lastLegajo = legajo;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Empleado ({0})", row.Cells[Properties.DistribucionFemsa.Legajo]));
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

            if (codLineaStrList.Any())
            {
                var i = 0;
                foreach (var l in codLineaStrList.InSetsOf(batchSize))
                {
                    i++;
                    var lineas = DaoFactory.LineaDAO.FindByCodigosByEmpresa(new[] { Empresa.Id }, l.ToArray());
                    
                    if (lineas != null && lineas.Any())
                    {
                        _lineasBuffer.AddRange(lineas);
                        foreach (Linea l2 in lineas)
                        {
                            STrace.Debug("LineaDebug2", String.Format("{0}:{1} ( {2}/#{3})", l2.Id, l2.DescripcionCorta, _lineasBuffer.Count, i));
                        }
                    }

                    var tiposServicios = DaoFactory.TipoServicioCicloDAO.FindDefaults(Empresa.Id , lineas.Select(li => li.Id));
                    if (tiposServicios != null && tiposServicios.Any())
                    {
                        _tiposServicioBuffer.AddRange(tiposServicios);
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

            if (codPreAsignoStrList.Any())
            {
                foreach (var l in codPreAsignoStrList.InSetsOf(batchSize))
                {
                    var preasignos = DaoFactory.PreasignacionViajeVehiculoDAO.FindByCodigos(Empresa.Id, -1, l);
                    if (preasignos != null && preasignos.Any())
                    {
                        _preasignoBuffer.AddRange(preasignos);
                    }
                }
            }

            if (legajoStrList.Any())
            {
                foreach (var l in legajoStrList.InSetsOf(batchSize))
                {
                    var empleados = DaoFactory.EmpleadoDAO.FindByLegajos(Empresa.Id, -1, l);
                    if (empleados != null && empleados.Any())
                    {
                        _empleadosBuffer.AddRange(empleados);
                    }
                }
            }
        }
    }
}
