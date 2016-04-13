using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.DAO.BusinessObjects.Positions;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObject.Positions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;

namespace LogicTracker.App.Web.Api
{
    /// <summary>
    /// Summary description for WebServiceLT
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceLT : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public XmlDocument ObtenerEstadoEntrega(String Distrito, String Base, String Entrega, String Cliente)
        {
            var address = HttpContext.Current.Request.UserHostAddress;

            try
            {
                using (StreamWriter sw = File.AppendText(Directory.GetCurrentDirectory() + @"\LOGIP.txt"))
                {
                    sw.WriteLine(address.ToString() + "\t" + DateTime.Now.ToString("ddMMyyyyHHmmssfff"));
                }
            }
            catch (Exception)
            {
            }

            if (!String.IsNullOrEmpty(Distrito) &&
                !String.IsNullOrEmpty(Base) &&
                (!String.IsNullOrEmpty(Entrega) ||
                !String.IsNullOrEmpty(Cliente)))
            {
                var EstadoActual = "";
                var TiempoLlegada = "";
                var DistanciakmtsLlegada = "";
                var linkGoogleMaps = "https://www.google.com.ar/maps/dir/-34.598198,-58.3841437/-34.5972089,-58.3780497";
                ViajeDistribucionDAO dao = new ViajeDistribucionDAO();
                LogPosicionDAO daopos = new LogPosicionDAO();
                var sinbase = dao.FindAll().Where(x => x.Inicio.Day.Equals(DateTime.Now.Day)  &&
                    x.Inicio.Month.Equals(DateTime.Now.Month) &&
                    x.Inicio.Year.Equals(DateTime.Now.Year) && x.Empresa.RazonSocial.ToString().Trim().ToUpper().Contains(Distrito.Trim().ToUpper()));
                if (!String.IsNullOrEmpty(Base))
                {
                    sinbase = sinbase.Where(x => x.Linea == null
                        || x.Linea != null
                        && x.Linea.Descripcion.ToString().Trim().ToUpper().Contains(Base.Trim().ToUpper()));
                }
                bool closeLoop = false;
                foreach (var item in sinbase)
                {
                    foreach (EntregaDistribucion detalle in item.Detalles.Where(x => x.PuntoEntrega != null))
                    {
                        //codigo = entrega
                        //descripcion cliente
                        if (detalle.PuntoEntrega.Codigo.Trim().ToUpper().Equals(Entrega.Trim().ToUpper()) ||
                            detalle.PuntoEntrega.Descripcion.Trim().ToUpper().Equals(Cliente.Trim().ToUpper()))
                        {
                            switch (detalle.Estado)
                            {
                                case EntregaDistribucion.Estados.EnSitio:
                                    {
                                        EstadoActual = "EnSitio";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.Completado:
                                    {
                                        EstadoActual = "Completado";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.Cancelado:
                                    {
                                        EstadoActual = "Cancelado";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.EnZona:
                                    {
                                        EstadoActual = "EnZona";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.NoCompletado:
                                    {
                                        EstadoActual = "NoCompletado";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.Pendiente:
                                    {
                                        EstadoActual = "Pendiente";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.Restaurado:
                                    {
                                        EstadoActual = "Restaurado";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.SinVisitar:
                                    {
                                        EstadoActual = "SinVisitar";
                                        break;
                                    }
                                case EntregaDistribucion.Estados.Visitado:
                                    {
                                        EstadoActual = "Visitado";
                                        break;
                                    }
                                default:
                                    break;
                            }

                            LogUltimaPosicionVo ult = daopos.GetLastVehiclePosition(item.Vehiculo);
                            if (ult == null)
                            {
                                ult = daopos.GetLastOnlineVehiclePosition(item.Vehiculo);
                            }
                            LatLon origen = new LatLon(ult.Latitud, ult.Longitud);
                            LatLon destino = new LatLon(detalle.PuntoEntrega.ReferenciaGeografica.Latitude, detalle.PuntoEntrega.ReferenciaGeografica.Longitude);

                            linkGoogleMaps = "https://www.google.com.ar/maps/dir/" + origen.Latitud.ToString().Replace(',', '.') + "," + origen.Longitud.ToString().Replace(',', '.') + "/" + destino.Latitud.ToString().Replace(',', '.') + "," + destino.Longitud.ToString().Replace(',', '.');

                            var waypoints = new List<LatLon>();
                            waypoints.Add(origen);
                            waypoints.Add(destino);
                            var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, waypoints.ToArray());
                            int reintentos = 0;
                            while (directions == null && reintentos < 4)
                            {
                                reintentos++;
                                Thread.Sleep(2000);
                                directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, waypoints.ToArray());
                            }
                            if (directions != null)
                            {
                                TiempoLlegada = new DateTime(directions.Duration.Ticks).ToString("HH:mm:ss");
                                DistanciakmtsLlegada = (directions.Distance / 1000).ToString();
                            }
                            closeLoop = true;
                            break;
                        }
                    }
                    if (closeLoop)
                    {
                        break;
                    }
                }
                XmlDocument doc = new XmlDocument();
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                //(2) string.Empty makes cleaner code
                XmlElement element1 = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(element1);

                XmlElement element2 = doc.CreateElement(string.Empty, "EstadoActual", string.Empty);
                XmlText text1 = doc.CreateTextNode(EstadoActual);
                element2.AppendChild(text1);
                element1.AppendChild(element2);

                XmlElement element3 = doc.CreateElement(string.Empty, "TiempoLlegadaHHMMSS", string.Empty);
                XmlText text2 = doc.CreateTextNode(TiempoLlegada);
                element3.AppendChild(text2);
                element1.AppendChild(element3);

                XmlElement element4 = doc.CreateElement(string.Empty, "DistanciakmtsLlegada", string.Empty);
                XmlText text3 = doc.CreateTextNode(DistanciakmtsLlegada);
                element4.AppendChild(text3);
                element1.AppendChild(element4);

                XmlElement element5 = doc.CreateElement(string.Empty, "linkGoogleMaps", string.Empty);
                XmlText text4 = doc.CreateTextNode(linkGoogleMaps);
                element5.AppendChild(text4);
                element1.AppendChild(element5);

                return doc;
            }
            else
            {
                return new XmlDocument();
            }
        }   
    }
}
