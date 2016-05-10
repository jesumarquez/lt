using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.Utils;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using System.Text;
using System.Configuration;

namespace LogicTracker.App.Web.Api
{
    /// <summary>
    /// Summary description for WebServiceLTViajeDistribucion
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceLTViajeDistribucion : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public String CrearViajeDistribucion(
            String DistritoCodigo,
            String BaseLineaCodigo,
            String CodigoViajeUnico,
            String[] CodigoclienteOrigen,
            String[] NombreClienteOrigen,
            String[] CodigoSubclienteOrigen,
            String[] NombreSubclienteOrigen,
            String[] GeoposicionSubclienteOrigen,
            String FechaYHoraDePosicionamiento,
            String DominioVehiculo,
            String CodigoLegajoChofer,
            String CodigoFleteroTransportista,
            String TipodelDocumentoPtoVtaNroDocumento,
            String[] CodigoClienteDestino,
            String[] NombreClienteDestino,
            String[] CodigoSubClienteDestino,
            String[] NombreSubClienteDestino,
            String[] GeoposicionSubClienteDestino)
        {
            String Resultado = "0";
            bool error = false;
            Exception errorCause = null;
            var address = HttpContext.Current.Request.UserHostAddress;
            var vehiculoNoexiste = false;
            var empleadonoexiste = false;
            string IPPERMITIDAS = ConfigurationManager.AppSettings["WebServiceIPPERMITIDAS"].ToString();
            bool permitido = false;
            foreach (var item in IPPERMITIDAS.Split(';'))
            {
                if (address.ToString().Equals(item.ToString()))
                {
                    permitido = true;
                    break;
                }
            }
            if (!permitido)
            {
                Resultado = "La direccion de internet publica " + address + " no tiene permisos. Comuniquese con soporte tecnico ";
                return Resultado;
            }

            try
            {
                string filePath = HttpContext.Current.Server.MapPath("~");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=======================================================================");
                sb.AppendLine("REQUEST HORA " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                sb.AppendLine("IP PUBLICA: " + address.ToString());
                sb.AppendLine("=======================================================================");
                File.AppendAllText(filePath + "LOGIP.txt", sb.ToString());

            }
            catch (Exception ex)
            {
            }

            try
            {
                DAOFactory _DAOFactory = new DAOFactory();

                ViajeDistribucion EditObject = new ViajeDistribucion();
                var empreasa = -1;
                var linea = -1;
                EditObject.Empresa = !String.IsNullOrEmpty(DistritoCodigo) ? _DAOFactory.EmpresaDAO.FindByCodigo(DistritoCodigo) : null;
                EditObject.Linea = !String.IsNullOrEmpty(BaseLineaCodigo) ? _DAOFactory.LineaDAO.FindByCodigo(EditObject.Empresa.Id, BaseLineaCodigo) : null;
                if (EditObject.Linea != null)
                {
                    linea = EditObject.Linea.Id; 
                }
                EditObject.Transportista = !String.IsNullOrEmpty(CodigoFleteroTransportista) ? _DAOFactory.TransportistaDAO.FindByCodigo(EditObject.Empresa.Id, linea, CodigoFleteroTransportista) : null;

                /*EditObject.CentroDeCostos = cbCentroDeCosto.Selected > 0 ? DAOFactory.CentroDeCostosDAO.FindById(cbCentroDeCosto.Selected) : null;
                EditObject.SubCentroDeCostos = cbSubCentroDeCosto.Selected > 0
                    ? DAOFactory.SubCentroDeCostosDAO.FindById(cbSubCentroDeCosto.Selected)
                    : null;                    
                EditObject.TipoCoche = cbTipoVehiculo.Selected > 0 ? DAOFactory.TipoCocheDAO.FindById(cbTipoVehiculo.Selected) : null;*/



                EditObject.Vehiculo = !String.IsNullOrEmpty(DominioVehiculo) ? _DAOFactory.CocheDAO.FindByPatente(EditObject.Empresa.Id, DominioVehiculo.Trim()) : null;
                if (EditObject.Vehiculo == null)
                {
                    Resultado = Resultado + "\n El vehiculo " + DominioVehiculo + " no existe";
                    vehiculoNoexiste = true;
                }

                EditObject.Empleado = !String.IsNullOrEmpty(CodigoLegajoChofer) ? _DAOFactory.EmpleadoDAO.FindByLegajo(EditObject.Empresa.Id, linea, CodigoLegajoChofer) : null;
                if (EditObject.Empleado == null)
                {
                    Resultado = Resultado + "\nEl empleado con codigo de legajo " + CodigoLegajoChofer + " no existe";
                    empleadonoexiste = true;
                }

                /*  EditObject.TipoCicloLogistico = cbTipoCicloLogistico.Selected > 0 ? DAOFactory.TipoCicloLogisticoDAO.FindById(cbTipoCicloLogistico.Selected) : null;*/

                EditObject.Codigo = CodigoViajeUnico + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff");
                EditObject.NumeroViaje = 0;

                EditObject.Tipo = ViajeDistribucion.Tipos.RecorridoFijo;
                EditObject.Desvio = 100;
                EditObject.RegresoABase = false;
                EditObject.ProgramacionDinamica = false;
                EditObject.Inicio = DateTime.ParseExact(FechaYHoraDePosicionamiento, "ddMMyyyyHHmmssfff", null);

                EditObject.Comentario = TipodelDocumentoPtoVtaNroDocumento;
                EditObject.Umbral = 0;
                EditObject.Estado = ViajeDistribucion.Estados.Pendiente;
                EditObject.Alta = DateTime.UtcNow;
                EditObject.Fin = DateTime.MaxValue;

                _DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(EditObject);

                for (int a = 0; a < CodigoclienteOrigen.Count(); a++)
                {
                    //CodigoclienteOrigen
                    var cliente = _DAOFactory.ClienteDAO.FindByCode(new int[] { EditObject.Empresa.Id }, new int[] { linea }, CodigoclienteOrigen[a]);
                    if (cliente == null)
                    {
                        Resultado = Resultado + "\nEl cliente con codigo de origen " + CodigoclienteOrigen[a] + " no existe y se lo va a dar de alta";
                        cliente = new Cliente();
                        cliente.Empresa = EditObject.Empresa;
                        cliente.Linea = EditObject.Linea;
                        cliente.Codigo = CodigoclienteOrigen[a];
                        cliente.Descripcion = NombreClienteOrigen[a];
                        cliente.DescripcionCorta = NombreClienteOrigen[a];
                        _DAOFactory.ClienteDAO.SaveOrUpdate(cliente);
                    }
                    var puntoentrega = _DAOFactory.PuntoEntregaDAO.FindByCode(new int[] { EditObject.Empresa.Id }, new int[] { linea }, new int[] { cliente.Id }, CodigoSubclienteOrigen[a]);
                    if (puntoentrega == null)
                    {
                        Resultado = Resultado + "\nEl codigo de subcliente origen " + CodigoSubclienteOrigen[a] + " no existe y se lo va a dar de alta";
                        ReferenciaGeografica direccion = new ReferenciaGeografica();
                        direccion.Empresa = EditObject.Empresa;
                        direccion.Linea = EditObject.Linea;
                        direccion.TipoReferenciaGeografica = _DAOFactory.TipoReferenciaGeograficaDAO.FindByCodigo(new int[] { EditObject.Empresa.Id }, new int[] { linea }, "CL");
                        direccion.Codigo = CodigoclienteOrigen[a];
                        direccion.Descripcion = NombreSubclienteOrigen[a];

                        direccion.Vigencia = new Vigencia();
                        direccion.Vigencia.Inicio = DateTime.UtcNow;
                        var now = DateTime.UtcNow;

                        var newDireccion = new Direccion
                        {
                            Vigencia = new Vigencia { Inicio = now },
                            Altura = -1,
                            Calle = "",
                            Descripcion = NombreSubclienteOrigen[a],
                            IdCalle = -1,
                            IdEntrecalle = -1,
                            IdEsquina = -1,
                            IdMapa = -1,
                            Latitud = double.Parse(GeoposicionSubclienteOrigen[a].ToString().Split(';')[0]),
                            Longitud = double.Parse(GeoposicionSubclienteOrigen[a].ToString().Split(';')[1]),
                            Pais = "Argentina",
                            Partido = "",
                            Provincia = ""
                        };

                        Poligono newPoligono = new Poligono { Vigencia = new Vigencia { Inicio = now } };

                        PointF point = new PointF(float.Parse(GeoposicionSubclienteOrigen[a].ToString().Split(';')[0]),
                            float.Parse(GeoposicionSubclienteOrigen[a].ToString().Split(';')[0]));
                        List<PointF> points = new List<PointF>();
                        points.Add(point);
                        IEnumerable<PointF> pointenum = points as IEnumerable<PointF>;
                        newPoligono.AddPoints(pointenum);
                        direccion.AddHistoria(newDireccion, newPoligono, now);
                        _DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(direccion);

                        puntoentrega = new PuntoEntrega();
                        puntoentrega.Cliente = cliente;
                        puntoentrega.Codigo = CodigoSubclienteOrigen[a];
                        puntoentrega.Descripcion = NombreSubclienteOrigen[a];
                        puntoentrega.ReferenciaGeografica = direccion;

                        _DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoentrega);

                    }
                    var kms = 0;
                    var entrega = new EntregaDistribucion()
                    {
                        Cliente = cliente,
                        PuntoEntrega = puntoentrega,
                        Descripcion = CodigoSubclienteOrigen[a],
                        Orden = EditObject.Detalles.Count(),
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Programado = EditObject.Inicio.AddHours(a),
                        ProgramadoHasta = EditObject.Inicio.AddHours(a),
                        Viaje = EditObject,
                        KmCalculado = kms
                    };

                    _DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(entrega);
                    EditObject.Detalles.Add(entrega);


                    //CodigoClienteDestino
                    var clientedestino = _DAOFactory.ClienteDAO.FindByCode(new int[] { EditObject.Empresa.Id }, new int[] { linea }, CodigoClienteDestino[a]);
                    if (clientedestino == null)
                    {
                        Resultado = Resultado + "\nEl cliente con codigo de destino " + CodigoClienteDestino[a] + " no existe y se lo va a dar de alta";

                        clientedestino = new Cliente();
                        clientedestino.Empresa = EditObject.Empresa;
                        clientedestino.Linea = EditObject.Linea;
                        clientedestino.Codigo = CodigoClienteDestino[a];
                        clientedestino.Descripcion = NombreClienteDestino[a];
                        clientedestino.DescripcionCorta = NombreClienteDestino[a];
                        _DAOFactory.ClienteDAO.SaveOrUpdate(clientedestino);
                    }

                    puntoentrega = _DAOFactory.PuntoEntregaDAO.FindByCode(new int[] { EditObject.Empresa.Id }, new int[] { linea }, new int[] { clientedestino.Id }, CodigoSubClienteDestino[a]);

                    if (puntoentrega == null)
                    {
                        Resultado = Resultado + "\nEl codigo de subcliente destino " + CodigoSubClienteDestino[a] + " no existe y se lo va a dar de alta";
                        ReferenciaGeografica direccion = new ReferenciaGeografica();
                        direccion.Empresa = EditObject.Empresa;
                        direccion.Linea = EditObject.Linea;
                        direccion.TipoReferenciaGeografica = _DAOFactory.TipoReferenciaGeograficaDAO.FindByCodigo(new int[] { EditObject.Empresa.Id }, new int[] { linea }, "CL");
                        direccion.Codigo = CodigoClienteDestino[a];
                        direccion.Descripcion = NombreSubClienteDestino[a];
                        direccion.Vigencia = new Vigencia();
                        direccion.Vigencia.Inicio = DateTime.UtcNow;

                        var now = DateTime.UtcNow;
                        var newDireccion = new Direccion
                        {
                            Vigencia = new Vigencia { Inicio = now },
                            Altura = -1,
                            Calle = "",
                            Descripcion = NombreSubclienteOrigen[a],
                            IdCalle = -1,
                            IdEntrecalle = -1,
                            IdEsquina = -1,
                            IdMapa = -1,
                            Latitud = double.Parse(GeoposicionSubClienteDestino[a].ToString().Split(';')[0]),
                            Longitud = double.Parse(GeoposicionSubClienteDestino[a].ToString().Split(';')[1]),
                            Pais = "Argentina",
                            Partido = "",
                            Provincia = ""
                        };

                        Poligono newPoligono = new Poligono { Vigencia = new Vigencia { Inicio = now } };

                        PointF point = new System.Drawing.PointF(float.Parse(GeoposicionSubClienteDestino[a].ToString().Split(';')[0]), float.Parse(GeoposicionSubClienteDestino[a].ToString().Split(';')[1]));
                        List<PointF> points = new List<PointF>();
                        points.Add(point);
                        IEnumerable<PointF> pointenum = points as IEnumerable<PointF>;
                        newPoligono.AddPoints(pointenum);
                        direccion.AddHistoria(newDireccion, newPoligono, now);

                        _DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(direccion);
                        puntoentrega = new PuntoEntrega();
                        puntoentrega.Cliente = clientedestino;
                        puntoentrega.Codigo = CodigoSubClienteDestino[a];
                        puntoentrega.Descripcion = NombreSubClienteDestino[a];
                        puntoentrega.ReferenciaGeografica = direccion;
                        _DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoentrega);

                    }

                    entrega = new EntregaDistribucion()
                    {
                        Cliente = clientedestino,
                        PuntoEntrega = puntoentrega,
                        Descripcion = CodigoSubClienteDestino[a],
                        Orden = EditObject.Detalles.Count(),
                        Estado = EntregaDistribucion.Estados.Pendiente,
                        Programado = EditObject.Inicio.AddHours(a),
                        ProgramadoHasta = EditObject.Inicio.AddHours(a),
                        Viaje = EditObject
                    };
                    EditObject.Detalles.Add(entrega);
                    _DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(entrega);
                }


                EditObject.Recorrido.Clear();
               /* ReferenciaGeografica refgeo = null;
                PuntoEntrega refentrega = null;
                Recorrido trayecto = new Recorrido();
                trayecto.Desvio = 100;
                trayecto.Empresa = EditObject.Empresa;
                trayecto.Codigo = "";
                trayecto.Nombre = "";
                int orden = 0;
                trayecto.Linea = EditObject.Linea;
                var nombreRecorrido = "";
                foreach (var item in EditObject.Detalles)
                {
                    nombreRecorrido = nombreRecorrido.Replace(item.PuntoEntrega.Descripcion + "_", "") + item.PuntoEntrega.Descripcion + "_";
                }
                Recorrido buscarrecorrido = _DAOFactory.RecorridoDAO.FindAll().Where(x => x.Nombre.Equals(nombreRecorrido)).FirstOrDefault();
                if (buscarrecorrido == null)
                {
                    foreach (var item in EditObject.Detalles)
                    {
                        if (refgeo != null)
                        {
                            var origen = new LatLon(refgeo.Latitude, refgeo.Longitude);
                            var destino = new LatLon(item.ReferenciaGeografica.Latitude, item.ReferenciaGeografica.Longitude);
                            var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, null);
                            var posiciones = directions.Legs.SelectMany(l => l.Steps.SelectMany(s => s.Points)).ToList();
                            RecorridoDistribucion last = null;
                            var codigoentrega = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 3);
                            var codigopuntoentrega = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 3);
                            if (refentrega.Codigo.Length > 4)
                            {
                                codigoentrega = refentrega.Codigo.Substring(refentrega.Codigo.Length - 3, 3);
                            }
                            if (item.PuntoEntrega.Codigo.Length > 4)
                            {
                                codigopuntoentrega = item.PuntoEntrega.Codigo.Substring(item.PuntoEntrega.Codigo.Length - 3, 3);
                            }
                            trayecto.Codigo = trayecto.Codigo.Replace(codigoentrega + "_", "") + codigoentrega + "_" + codigopuntoentrega + "_";
                            trayecto.Nombre = trayecto.Nombre.Replace(refentrega.Descripcion + "_", "") + refentrega.Descripcion + "_" + item.PuntoEntrega.Descripcion + "_";

                            for (var i = 0; i < posiciones.Count; i++)
                            {
                                var det = new RecorridoDistribucion { Latitud = posiciones[i].Latitud, Longitud = posiciones[i].Longitud, Distribucion = EditObject, Orden = orden };
                                det.Distancia = last == null ? 0 : Logictracker.Utils.Distancias.Loxodromica(last.Latitud, last.Longitud, det.Latitud, det.Longitud) / 1000.0;
                                EditObject.Recorrido.Add(det);
                                last = det;
                                trayecto.Detalles.Add(
                                    new DetalleRecorrido()
                                    {
                                        Latitud = posiciones[i].Latitud,
                                        Longitud = posiciones[i].Longitud,
                                        Recorrido = trayecto,
                                        Distancia = det.Distancia,
                                        Orden = orden
                                    }
                                );
                                orden++;
                            }
                            refgeo = item.ReferenciaGeografica;
                            refentrega = item.PuntoEntrega;
                        }
                        else
                        {
                            refgeo = item.ReferenciaGeografica;
                            refentrega = item.PuntoEntrega;
                        }
                    }
                    _DAOFactory.RecorridoDAO.SaveOrUpdate(trayecto);
                }
                else
                {
                    foreach (var item in buscarrecorrido.Detalles)
                    {
                        var det = new RecorridoDistribucion { Latitud = item.Latitud, Longitud = item.Longitud, Distribucion = EditObject, Orden = item.Orden };
                        det.Distancia = item.Distancia;
                        EditObject.Recorrido.Add(det);
                    }

                }*/
                _DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(EditObject);
            }
            catch (Exception ex)
            {
                Resultado = Resultado + "\nError desconocido ";
                string filePath = HttpContext.Current.Server.MapPath("~");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=======================================================================");
                sb.AppendLine("IP PUBLICA: " + address.ToString());
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace.ToString());
                sb.AppendLine("=======================================================================");
                File.AppendAllText(filePath + "LOGIP.txt", sb.ToString());
                error = true;
            }

            /*
                Respuesta	Código de Respuesta - N(3) 
             * (en caso de que la recepción fuera correcta deberá devolvernos un 0, sino un código de error según lo determinen uds. 
             * qué datos son obligatorios para dar de alta el viaje en la plataforma. 
             * Habrá alertas por datos faltantes o Chofer/Fletero inexistentes)
                Descripcion	Descripción Respuesta - C(40) (según el atributo anterior. 
             * Si hubo errores y/o aletas deberá especificar en que atributos se produjo)
*/

            return Resultado;
        }
    }
}
