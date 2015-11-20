using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.DAL.DAO.BusinessObjects.Documentos;
using Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.DAL.Factories;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Documentos;
using LogicTracker.App.Web.Api.Controllers;
using LogicTracker.App.Web.Api.Models;
using LogicTracker.App.Web.Api.Providers;
using Newtonsoft.Json.Linq;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class FormulariosController : BaseController
    {
        public IRouteService RouteService { get; set; }


        public IHttpActionResult Get()
        {
            return Ok();
        }

 

        // GET: api/Routes/1234567890A
      //   [DeflateCompression]
        [CompressContent]
        public IHttpActionResult Get(int id)
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return BadRequest();
            var items = new List<FormulariosList>();
            TipoDocumentoDAO documentos = new TipoDocumentoDAO();
            EmpleadoDAO empleado = new EmpleadoDAO();// emple //fecha
            DispositivoDAO dispositivo = new DispositivoDAO();
            var device = dispositivo.FindByImei(deviceId);
            var employee = empleado.FindEmpleadoByDevice(device);
            List<int> empresas = new List<int>();
            empresas.Add(employee.Empresa.Id);
            var lineas = new int[] { };
            List<TipoDocumento> lista = documentos.GetList(empresas, lineas).Where(x => x.Id > id).ToList();

            foreach (var item in lista)
            {
                if (item.Id > id)
                {
                    List<FormulariosParametros> parametros = new List<FormulariosParametros>();
                    foreach (var itemParametros in item.Parametros)
                    {
                        List<FormulariosSpinnerList> spinnerMobile = new List<FormulariosSpinnerList>();
                        FormulariosSpinnerList[] spinnerlist = null;
                        switch (Convert.ToString(itemParametros.TipoDato))
                        {
                            case "coche":
                                {
                                    CocheDAO cocheDAO = new CocheDAO();
                                    var coches = cocheDAO.GetList(empresas, lineas);
                                    foreach (var itemcoche in coches)
                                    {
                                        spinnerMobile.Add(new FormulariosSpinnerList()
                                            {
                                                id = Convert.ToString(itemcoche.Id),
                                                descripcion = itemcoche.CompleteDescripcion()
                                            });
                                    }
                                    spinnerlist = spinnerMobile.ToArray();
                                }
                                break;
                            case "chofer":
                                {
                                    List<TipoEmpleado> tipo = new TipoEmpleadoDAO().GetList(empresas, lineas);
                                    List<int> tipoempleados = new List<int>();
                                    foreach (var itemTipo in tipo)
                                    {
                                        tipoempleados.Add(itemTipo.Id);
                                    }
                                    List<Transportista> transpo = new TransportistaDAO().GetList(empresas, lineas);
                                    List<int> transportistas = new List<int>();
                                    foreach (var itemTranspo in transpo)
                                    {
                                        transportistas.Add(itemTranspo.Id);
                                    }
                                    var empleados = empleado.GetList(empresas, lineas, tipoempleados, transportistas);
                                    foreach (var itemempleado in empleados)
                                    {
                                        spinnerMobile.Add(new FormulariosSpinnerList()
                                        {
                                            id = Convert.ToString(itemempleado.Id),
                                            descripcion = itemempleado.ToString()
                                        });
                                    }
                                    spinnerlist = spinnerMobile.ToArray();
                                }
                                break;
                            case "aseguradora":
                                {

                                    List<Transportista> transportistas = new TransportistaDAO().GetList(empresas, lineas);
                                    foreach (var itemtransportista in transportistas)
                                    {
                                        spinnerMobile.Add(new FormulariosSpinnerList()
                                        {
                                            id = Convert.ToString(itemtransportista.Id),
                                            descripcion = itemtransportista.Descripcion
                                        });
                                    }
                                    spinnerlist = spinnerMobile.ToArray();
                                }
                                break;
                            case "cliente":
                                {
                                    List<Cliente> clientes = new ClienteDAO().GetList(empresas, lineas);
                                    foreach (var itemcliente in clientes)
                                    {
                                        spinnerMobile.Add(new FormulariosSpinnerList()
                                        {
                                            id = Convert.ToString(itemcliente.Id),
                                            descripcion = itemcliente.Descripcion
                                        });
                                    }
                                    spinnerlist = spinnerMobile.ToArray();
                                }
                                break;
                            case "equipo":
                                {
                                    IList equipos = new EquipoDAO().FindAll();
                                    foreach (var itemequipo in equipos.Cast<Equipo>().ToList())
                                    {
                                        spinnerMobile.Add(new FormulariosSpinnerList()
                                        {
                                            id = Convert.ToString(itemequipo.Id),
                                            descripcion = itemequipo.Descripcion
                                        });
                                    }
                                    spinnerlist = spinnerMobile.ToArray();
                                }
                                break;
                            case "centrocostos":
                                {
                                    var departamentos = new DepartamentoDAO().GetList(empresas, lineas);
                                    List<int> dep = new List<int>();
                                    foreach (var itemDeparta in departamentos)
                                    {
                                        dep.Add(itemDeparta.Id);
                                    }
                                    List<CentroDeCostos> centros = new CentroDeCostosDAO().GetList(empresas, lineas, dep);
                                    foreach (var itemcentros in centros)
                                    {
                                        spinnerMobile.Add(new FormulariosSpinnerList()
                                        {
                                            id = Convert.ToString(itemcentros.Id),
                                            descripcion = itemcentros.Descripcion
                                        });
                                    }
                                    spinnerlist = spinnerMobile.ToArray();
                                }
                                break;
                            default:
                                break;
                        }

                        parametros.Add(new FormulariosParametros()
                            {
                                id = Convert.ToString(itemParametros.Id),
                                pordefault = itemParametros.Default,
                                largo = Convert.ToString(itemParametros.Largo),
                                nombre = Convert.ToString(itemParametros.Nombre),
                                obligatorio = Convert.ToString(itemParametros.Obligatorio),
                                orden = Convert.ToString(itemParametros.Orden),
                                precision = Convert.ToString(itemParametros.Precision),
                                repeticion = Convert.ToString(itemParametros.Repeticion),
                                tipodato = Convert.ToString(itemParametros.TipoDato),
                                tipodocumento = itemParametros.TipoDocumento.ToString(),
                                spinnerlist = spinnerlist
                            });
                    }
                    items.Add(new FormulariosList()
                    {
                        id = Convert.ToString(item.Id),
                        nombre = item.Descripcion,
                        primeraviso = Convert.ToString(item.PrimerAviso),
                        requerirpresentacion = Convert.ToString(item.RequerirPresentacion),
                        requerirvencimiento = Convert.ToString(item.RequerirVencimiento),
                        segundoaviso = Convert.ToString(item.SegundoAviso),
                        strategy = Convert.ToString(item.Strategy),
                        template = Convert.ToString(item.Template),
                        parametros = parametros.ToArray()
                    });
                }
            }
            return Ok(items.ToArray().OrderBy(item => item.id).ToArray());
        }

        [Route("api/Formularios/formulario")]
        [HttpPost]
        [DeflateCompression]
        public IHttpActionResult cargarRegistrosFormularios([FromBody]FormularioRow[] records)
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null || records == null) return BadRequest();

            DocumentoDAO documentoDAO = new DocumentoDAO();
            TipoDocumentoDAO tipodocumentoDAO = new TipoDocumentoDAO();
            var items = new List<FormulariosList>();
            EmpleadoDAO empleado = new EmpleadoDAO();// emple //fecha
            DispositivoDAO dispositivo = new DispositivoDAO();
            CocheDAO cocheDao = new CocheDAO();
            var device = dispositivo.FindByImei(deviceId);
            var employee = empleado.FindEmpleadoByDevice(device);            

            foreach (var item in records)
            {
                Documento doc = new Documento();
                doc.TipoDocumento = tipodocumentoDAO.FindById(int.Parse(item.idFormulario));
                doc.Empresa = employee.Empresa;
                doc.FechaAlta = DateTime.Now;
                doc.Linea = employee.Linea;   
                foreach (var itemParametro in item.campoFormulario)
                {
                    switch (itemParametro.datatypecampo)
                    {
                        case "string":
                            {
                                switch (itemParametro.nombrecampo)
                                {
                                    case "Codigo":
                                        {
                                            doc.Codigo = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo); 
                                            break;
                                        }
                                    case "Descripcion":
                                        {
                                            doc.Descripcion = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo);
                                            break;
                                        }                                        
                                    case "Estado":
                                        {
                                            switch (System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo))
                                            {
                                                case "Abierto":
                                                    {
                                                        doc.Estado = Documento.Estados.Abierto;
                                                    }
                                                    break;
                                                case "Cerrado":
                                                    {
                                                        doc.Estado = Documento.Estados.Cerrado;
                                                    }
                                                    break;
                                                case "Prestado":
                                                    {
                                                        doc.Estado = Documento.Estados.Prestado;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }                                            
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                break;
                            }
                        case "datetime":
                            {
                                switch (itemParametro.nombrecampo)
                                {
                                    case "Fecha":
                                        {
                                            doc.Fecha = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            break;
                                        }
                                    case "Presentacion":
                                        {
                                            doc.Presentacion = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            break;
                                        }
                                    case "Vencimiento":
                                        {
                                            doc.Vencimiento = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            break;
                                        }
                                    case "Cierre":
                                        {
                                            doc.FechaCierre = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            break;
                                        }                                        
                                    default:
                                        break;
                                }
                                break;
                            }
                        case "float":
                            {
                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                          where p.Nombre == itemParametro.nombrecampo
                                          select p;
                                var val = new DocumentoValor
                                {
                                    Documento = doc,
                                    Parametro = par.ToList()[0],
                                    Repeticion = ((short)1),
                                    Valor = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)
                                };
                                doc.Parametros.Add(val);
                                break;
                            }
                        case "int":
                            {
                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                          where p.Nombre == itemParametro.nombrecampo
                                          select p;
                                var val = new DocumentoValor
                                {
                                    Documento = doc,
                                    Parametro = par.ToList()[0],
                                    Repeticion = ((short)1),
                                    Valor = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)
                                };
                                doc.Parametros.Add(val);
                                break;
                            }
                        case "bool":
                            {
                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                          where p.Nombre == itemParametro.nombrecampo
                                          select p;
                                var val = new DocumentoValor
                                {
                                    Documento = doc,
                                    Parametro = par.ToList()[0],
                                    Repeticion = ((short)1),
                                    Valor = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)
                                };
                                doc.Parametros.Add(val);
                                break;
                            }
                        case "image":
                            {
                                if (itemParametro.objectocampo != null)
                                {
                                    string filename = "";
                                    var uploadPath = ConfigurationManager.AppSettings["pathImages"];
                                    if (!Directory.Exists(uploadPath))
                                    {
                                        try
                                        {
                                            Directory.CreateDirectory(uploadPath);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    string filetimestamp = DateTime.Now.ToString("MMddyyyyHHmmssfff", CultureInfo.InvariantCulture);
                                    File.WriteAllBytes(uploadPath + filetimestamp + ".png", itemParametro.objectocampo);
                                    if (File.Exists(uploadPath + filetimestamp + ".png"))
                                    {
                                        filename = filetimestamp + ".png";
                                    }
                                    var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                              where p.Nombre == itemParametro.nombrecampo
                                              select p;
                                    var val = new DocumentoValor
                                    {
                                        Documento = doc,
                                        Parametro = par.ToList()[0],
                                        Repeticion = ((short)1),
                                        Valor = filename
                                    };
                                    doc.Parametros.Add(val);
                                }
                                break;
                            }
                        case "coche":
                            {
                                if (itemParametro.objectocampo != null)
                                {
                                    var vehiculos = cocheDao.FindAll().ToList();
                                    foreach (Logictracker.Types.BusinessObjects.Vehiculos.Coche itemVehiculos in vehiculos)
                                    {
                                        string descr = itemVehiculos.CompleteDescripcion();
                                        if (descr.Equals(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)))
                                        {
                                            doc.Vehiculo = itemVehiculos;
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        case "chofer":
                            {
                                if (itemParametro.objectocampo != null)
                                {
                                    var emple = empleado.FindAll().ToList();
                                    foreach (Empleado itemempleado in emple)
                                    {
                                        string descr = itemempleado.ToString();
                                        if (descr.Equals(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)))
                                        {
                                            doc.Empleado = itemempleado;
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        case "aseguradora":
                            {
                                if (itemParametro.objectocampo != null)
                                {
                                    var transportistas = new TransportistaDAO().FindAll().ToList();
                                    foreach (Transportista itemtranspor in transportistas)
                                    {
                                        string descr = itemtranspor.Descripcion;
                                        if (descr.Equals(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)))
                                        {
                                            doc.Transportista = itemtranspor;
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        case "cliente":
                            {
                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                          where p.Nombre == itemParametro.nombrecampo
                                          select p;
                                var val = new DocumentoValor
                                          {
                                              Documento = doc,
                                              Parametro = par.ToList()[0],
                                              Repeticion = ((short)1),
                                              Valor =  System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)
                                          };
                                doc.Parametros.Add(val);
                                break;
                            }
                        case "equipo":
                            {
                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                          where p.Nombre == itemParametro.nombrecampo
                                          select p;
                                var val = new DocumentoValor
                                {
                                    Documento = doc,
                                    Parametro = par.ToList()[0],
                                    Repeticion = ((short)1),
                                    Valor = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)
                                };
                                doc.Parametros.Add(val);
                                break;
                            }
                        case "centrocostos":
                            {
                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                          where p.Nombre == itemParametro.nombrecampo
                                          select p;
                                var val = new DocumentoValor
                                {
                                    Documento = doc,
                                    Parametro = par.ToList()[0],
                                    Repeticion = ((short)1),
                                    Valor = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)
                                };
                                doc.Parametros.Add(val);
                                break;
                            }
                        default:
                            break;
                    }                   
                }
                documentoDAO.SaveOrUpdate(doc);
            }
            return Ok("finalized");
        }
    }
}