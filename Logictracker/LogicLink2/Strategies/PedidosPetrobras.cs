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
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class PedidosPetrobras : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Linea Linea { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private readonly List<Transportista> _transportistasBuffer = new List<Transportista>();
        private readonly List<Insumo> _insumosBuffer = new List<Insumo>();
        private const int IdClienteDefault = 4738; // SIN ASIGNAR

        public static void Parse(LogicLinkFile file, out int pedidos, out string observaciones)
        {
            new PedidosPetrobras(file).Parse(out pedidos, out observaciones);            
        }

        public PedidosPetrobras(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Linea = file.Linea;
        }

        public void Parse(out int pedidos, out string observaciones)
        {
            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, false, 25);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));

            #region Set Indexes

            var indexCodigoTransportista = Properties.PedidosPetrobras.CodigoTransportista;
            var indexCodigoPlanta = Properties.PedidosPetrobras.CodigoPlanta;

            var indexCodigoCliente = Properties.PedidosPetrobras.CodigoCliente;
            var indexNombreCliente = Properties.PedidosPetrobras.NombreCliente;
            var indexFechaSolicitud = Properties.PedidosPetrobras.FechaSolicitud;
            var indexCondicion = Properties.PedidosPetrobras.Condicion;
            var indexCodigoPedido = Properties.PedidosPetrobras.CodigoPedido;

            var indexNaftaSuper = Properties.PedidosPetrobras.NaftaSuper;
            var indexNaftaPremium = Properties.PedidosPetrobras.NaftaPremium;
            var indexGasoilMenor500 = Properties.PedidosPetrobras.GasoilMenor500;
            var indexGasoilMayor500 = Properties.PedidosPetrobras.GasoilMayor500;
            var indexGasoilPremium = Properties.PedidosPetrobras.GasoilPremium;

            var indexCodigoEntrega = Properties.PedidosPetrobras.CodigoEntrega;
            var indexFechaLiberacion = Properties.PedidosPetrobras.FechaLiberacion;
            var indexObservaciones = Properties.PedidosPetrobras.Observaciones;            

            var cabecera = rows[6];
            for (int i = 0; i < 20; i++)
            {
                switch (cabecera[i].ToString().Trim())
                {
                    case "Cta.": indexCodigoCliente = i; break;
                    case "Razon Social": indexNombreCliente = i; break;
                    case "Entrega Solic.": indexFechaSolicitud = i; break;
                    case "Inco.": indexCondicion = i; break;
                    case "Pedido": indexCodigoPedido = i; break;
                    case "Entrega": indexCodigoEntrega = i; break;
                    case "Fecha Liberacion": indexFechaLiberacion = i; break;
                    case "Observaciones": indexObservaciones = i; break;
                    case "Nafta Súper": indexNaftaSuper = i; break;
                    case "Nafta Premium": indexNaftaPremium = i; break;
                    case "Gasoil Igual o Inferior 500 ppm Azufre": indexGasoilMenor500 = i; break;
                    case "Gasoil Superior 500 ppm Azufre": indexGasoilMayor500 = i; break;
                    case "Gasoil Premium": indexGasoilPremium = i; break;
                    case "Gasóleo Premium": indexGasoilPremium = i; break;
                }
            }

            #endregion

            te.Restart();
            PreBufferRows(rows, indexCodigoCliente, indexCodigoTransportista);
            STrace.Trace(Component, string.Format("PreBufferRows en {0} segundos", te.getTimeElapsed().TotalSeconds));
            PreBufferInsumos();

            var listOrdenes = new List<Order>(rows.Count);
            pedidos = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);

            // ARRANCO EN LA FILA 5
            for (var i = 4; i < rows.Count; i++)
            {
                var row = rows[i];
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));

                var codigoTransportista = row[indexCodigoTransportista].ToString().Trim();
                var transportista = codigoTransportista != string.Empty ? _transportistasBuffer.SingleOrDefault(t => t.Codigo.Equals(codigoTransportista)) : null;

                // BAJO UNA FILA
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                row = rows[i];
                var codigoPlanta = row[indexCodigoPlanta].ToString().Trim();
                var planta = DaoFactory.LineaDAO.FindByCodigo(Empresa.Id, codigoPlanta);

                // BAJO 2 FILAS
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                i++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                row = rows[i];

                while (row[indexCodigoCliente].ToString().Trim() != string.Empty)
                {
                    var condicion = row[indexCondicion].ToString().Trim();                   

                    if (condicion == "YIF")
                    {
                        var codigoCliente = row[indexCodigoCliente].ToString().Trim();
                        var nombreCliente = row[indexNombreCliente].ToString().Trim();
                        var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codigoCliente);
                        if (puntoEntrega == null) puntoEntrega = GetNuevoPuntoEntrega(codigoCliente, nombreCliente);

                        //var codigoEntrega = row[indexCodigoEntrega].ToString().Trim();
                        var codigoPedido = row[indexCodigoPedido].ToString().Trim();
                        
                        var fechaSolicitud = row[indexFechaSolicitud].ToString().Trim();
                        var arrayFechaSolicitud = fechaSolicitud.Split('.');
                        var diaSolicitud = Convert.ToInt32(arrayFechaSolicitud[0]);
                        var mesSolicitud = Convert.ToInt32(arrayFechaSolicitud[1]);
                        var anioSolicitud = Convert.ToInt32(arrayFechaSolicitud[2]);
                        var dtFechaSolicitud = new DateTime(anioSolicitud, mesSolicitud, diaSolicitud).AddHours(3);
                        //var fechaLiberacion = row[indexFechaLiberacion].ToString().Trim();

                        // ARMAR NUEVO PEDIDO
                        var orden = new Order();
                        orden.CodigoPedido = codigoPedido;
                        orden.Empresa = Empresa;
                        orden.FechaPedido = dtFechaSolicitud;
                        orden.FechaAlta = DateTime.UtcNow;
                        orden.Linea = planta;
                        orden.PuntoEntrega = puntoEntrega;
                        orden.Transportista = transportista;
                        orden.Programado = false;

                        var naftaSuper = 0.0;
                        var naftaPremium = 0.0;
                        var gasoilMenos500 = 0.0;
                        var gasoilMayor500 = 0.0;
                        var gasoilPremium = 0.0;
                        
                        double.TryParse(row[indexNaftaSuper].ToString().Trim(), out naftaSuper);
                        double.TryParse(row[indexNaftaPremium].ToString().Trim(), out naftaPremium);
                        double.TryParse(row[indexGasoilMenor500].ToString().Trim(), out gasoilMenos500);
                        double.TryParse(row[indexGasoilMayor500].ToString().Trim(), out gasoilMayor500);
                        double.TryParse(row[indexGasoilPremium].ToString().Trim(), out gasoilPremium);

                        naftaSuper = naftaSuper / 1000;
                        naftaPremium = naftaPremium / 1000;
                        gasoilMenos500 = gasoilMenos500 / 1000;
                        gasoilMayor500 = gasoilMayor500 / 1000;
                        gasoilPremium = gasoilPremium / 1000;

                        Insumo insumo = null;
                        if (naftaSuper > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)naftaSuper;
                            insumo = _insumosBuffer.FirstOrDefault(a => a.Codigo == "NS");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;
                            detalle.Estado = OrderDetail.Estados.Pendiente;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (naftaPremium > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)naftaPremium;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "NP");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;
                            detalle.Estado = OrderDetail.Estados.Pendiente;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (gasoilMenos500 > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)gasoilMenos500;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "GI500");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;
                            detalle.Estado = OrderDetail.Estados.Pendiente;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (gasoilMayor500 > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)gasoilMayor500;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "GS500");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;
                            detalle.Estado = OrderDetail.Estados.Pendiente;

                            orden.OrderDetails.Add(detalle);
                        }
                        if (gasoilPremium > 0)
                        {
                            var detalle = new OrderDetail();
                            detalle.Cantidad = (int)gasoilPremium;
                            insumo = _insumosBuffer.FirstOrDefault(b => b.Codigo == "GP");
                            detalle.Insumo = insumo;
                            detalle.PrecioUnitario = (decimal)insumo.ValorReferencia;
                            detalle.Order = orden;
                            detalle.Estado = OrderDetail.Estados.Pendiente;

                            orden.OrderDetails.Add(detalle);
                        }
                        
                        listOrdenes.Add(orden);
                        pedidos++;
                    }

                    i++;
                    if (i < rows.Count)
                    {
                        STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", i, rows.Count));
                        row = rows[i];
                    }
                    else
                    {
                        break;
                    }
                }
            }

            STrace.Trace(Component, "Guardando pedidos: " + listOrdenes.Count);
            te.Restart();
            foreach (var orden in listOrdenes)
            {
                DaoFactory.OrderDAO.SaveOrUpdate(orden);
            }
            STrace.Trace(Component, string.Format("Pedidos guardadas en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }

        private PuntoEntrega GetNuevoPuntoEntrega(string codigoCliente, string nombreCliente)
        {
            var cliente = DaoFactory.ClienteDAO.FindById(IdClienteDefault);
            
            var georef = new ReferenciaGeografica
            {
                Codigo = codigoCliente,
                Descripcion = nombreCliente,
                Empresa = Empresa,
                EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                Vigencia = new Vigencia
                {
                    Inicio = DateTime.UtcNow
                },
                Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
            };

            var latitud = cliente.ReferenciaGeografica.Latitude;
            var longitud = cliente.ReferenciaGeografica.Longitude;
            var posicion = GetNewDireccion(latitud, longitud);

            var poligono = new Poligono { Radio = 100, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
            poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

            georef.AddHistoria(posicion, poligono, DateTime.UtcNow);

            DaoFactory.ReferenciaGeograficaDAO.SaveOrUpdate(georef);
                            
            var puntoEntrega = new PuntoEntrega
            {
                Cliente = cliente,
                Codigo = codigoCliente,
                Descripcion = nombreCliente,
                Telefono = string.Empty,
                Baja = false,
                ReferenciaGeografica = georef,
                Nomenclado = true,
                DireccionNomenclada = posicion.Descripcion,
                Nombre = nombreCliente
            };

            DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);

            return puntoEntrega;
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

        private void PreBufferInsumos()
        {
            var insumos = DaoFactory.InsumoDAO.GetList(new[] { Empresa.Id }, new[] { -1 }, new[] { -1 });
            _insumosBuffer.AddRange(insumos);
        }

        private void PreBufferRows(IEnumerable<Row> rows, int indexCodigoCliente, int indexCodigoTransportista)
        {
            var lastCodPunto = string.Empty;
            var lastCodTransportista = string.Empty;

            var codPuntoStrList = new List<string>();
            var codTransportistaStrList = new List<string>();

            foreach (var row in rows)
            {
                #region Buffer PuntoEntrega

                try
                {
                    var codigoPuntoEntrega = row[indexCodigoCliente].ToString().Trim();

                    if (lastCodPunto != codigoPuntoEntrega && codigoPuntoEntrega != string.Empty)
                    {
                        if (!codPuntoStrList.Contains(codigoPuntoEntrega))
                            codPuntoStrList.Add(codigoPuntoEntrega);

                        lastCodPunto = codigoPuntoEntrega;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, string.Format("Error Buffering Punto de Entrega ({0})", row[indexCodigoCliente]));
                }

                #endregion

                #region Buffer Transportistas

                try
                {
                    var codigo = row[indexCodigoTransportista].ToString().Trim();
                    
                    if (lastCodTransportista != codigo)
                    {
                        if (!codTransportistaStrList.Contains(codigo))
                            codTransportistaStrList.Add(codigo);

                        lastCodTransportista = codigo;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, string.Format("Error Buffering Transportista ({0})", row[indexCodigoTransportista]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codPuntoStrList.Any())
            {
                foreach (var l in codPuntoStrList.InSetsOf(batchSize))
                {
                    var puntos = DaoFactory.PuntoEntregaDAO.FindByEmpresaAndCodes(Empresa.Id, l);
                    if (puntos != null && puntos.Any())
                    {
                        _puntosBuffer.AddRange(puntos);
                    }
                }
            }

            if (codTransportistaStrList.Any())
            {
                foreach (var l in codTransportistaStrList.InSetsOf(batchSize))
                {
                    var transportistas = DaoFactory.TransportistaDAO.FindByCodigos(Empresa.Id, -1, l);
                    if (transportistas != null && transportistas.Any())
                    {
                        _transportistasBuffer.AddRange(transportistas);
                    }
                }
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
