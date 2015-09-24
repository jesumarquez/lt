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
        private readonly List<ViajeDistribucion> _rutasBuffer = new List<ViajeDistribucion>();

        public static Dictionary<int, List<int>> ParseRutas(LogicLinkFile file, out int rutas, out int entregas, out string observaciones)
        {
            new DistribucionCCU(file).ParseRutas(out rutas, out entregas, out observaciones);
            return EmpresasLineas;
        }

        public static void ParseClientes(LogicLinkFile file, out int clientes)
        {
            new DistribucionCCU(file).ParseClientes(out clientes);            
        }

        public DistribucionCCU(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Cliente = DaoFactory.ClienteDAO.GetList(new[] { Empresa.Id }, new[] { -1 }).FirstOrDefault();
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

            var listRutas = new List<ViajeDistribucion>();

            rutas = 0;
            entregas = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));


            }
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

            for (int i = 1; i < rows.Count; i++)
            {
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count-1));

                var fila = rows[i];
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
            var clientesTotales = listReferencias.Count();
            foreach (var puntoEntrega in listClientes)
            {
                clientes++;
                STrace.Trace(Component, string.Format("Guardando clientes: {0}/{1}", clientes, clientesTotales));
                DaoFactory.PuntoEntregaDAO.SaveOrUpdateWithoutTransaction(puntoEntrega);                
            }
            STrace.Trace(Component, string.Format("Clientes guardados en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }

        public static void ParseAsignaciones(LogicLinkFile file, out int asignaciones)
        {
            var te = new TimeElapsed();
            var rows = ParseFile(file.FilePath).Rows;
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));

            asignaciones = 0;
        }

        private void PreBufferRutas(IEnumerable<Row> rows)
        {
            var lastCodRuta = string.Empty;
            var lastCodCliente = string.Empty;
            var codRutaStrList = new List<string>();
            var codClienteStrList = new List<string>();

            for (int i = 1; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);

                STrace.Trace(Component, string.Format("Prebuffering: {0}/{1}", i, rows.Count()));

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
                        _rutasBuffer.AddRange(rutas);
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

            for (int i = 1; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);

                STrace.Trace(Component, string.Format("Prebuffering: {0}/{1}", i, rows.Count()));

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
    }
}
