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
            try
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
                var linea = -1;
                if ( employee.Linea != null)
                {
                    linea = employee.Linea.Id;
                }
                var lineas = new int[] { linea };
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
                                        var coches = cocheDAO.GetList(empresas, new int[] { });
                                        foreach (var itemcoche in coches)
                                        {

                                            if (itemcoche.Linea != null && itemcoche.Linea.Id.Equals(lineas[0]))
                                            {
                                                spinnerMobile.Add(new FormulariosSpinnerList()
                                                    {
                                                        id = Convert.ToString(itemcoche.Id),
                                                        descripcion = itemcoche.CompleteDescripcion()
                                                    });
                                            }
                                        }
                                        spinnerlist = spinnerMobile.ToArray();
                                    }
                                    break;
                                case "chofer":
                                    {
                                        var empleados = empleado.GetList(empresas, lineas, new int[] { }, new int[] { });
                                        foreach (var itemempleado in empleados)
                                        {
                                            spinnerMobile.Add(new FormulariosSpinnerList()
                                            {
                                                id = Convert.ToString(itemempleado.Id),
                                                descripcion = itemempleado.Entidad.Descripcion
                                            });
                                        }
                                        spinnerlist = spinnerMobile.ToArray();
                                    }
                                    break;
                                case "aseguradora":
                                    {

                                        List<Transportista> transportistas = new TransportistaDAO().GetList(empresas, new int[] { });
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
                            nombre = item.Nombre,
                            descripcion = item.Descripcion,
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
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }

        [Route("api/Formularios/formulario")]
        [HttpPost]
        [CompressContent]
        public IHttpActionResult cargarRegistrosFormularios([FromBody]FormularioRow[] records)
        {
            try
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
                List<int> empresas = new List<int>();
                empresas.Add(employee.Empresa.Id);
                var linea = -1;
                if (employee.Linea != null)
                {
                    linea = employee.Linea.Id;
                }
                var lineas = new int[] { linea };

                foreach (var item in records)
                {
                    Documento doc = new Documento();
                    doc.TipoDocumento = tipodocumentoDAO.FindById(int.Parse(item.idFormulario));
                    doc.Empresa = employee.Empresa;
                    doc.FechaAlta = DateTime.Now;
                    doc.Linea = employee.Linea;
                    bool firstfechafield = false;
                    foreach (var itemParametro in item.campoFormulario)
                    {
                        switch (itemParametro.datatypecampo)
                        {
                            case "stringbarcode":
                            case "string":
                                {
                                    switch (itemParametro.nombrecampo)
                                    {
                                        case "Codigo":
                                            {
                                                doc.Codigo = System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo);
                                                var finddoc = documentoDAO.FindByCodigo(doc.TipoDocumento.Id, doc.Codigo);
                                                if (finddoc != null)
                                                    doc = finddoc;
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

                                    }
                                    break;
                                }
                            case "datetime":
                                {
                                    switch (itemParametro.nombrecampo.ToUpper())
                                    {
                                        case "FECHA":
                                            {
                                                if (!firstfechafield)
                                                {
                                                    firstfechafield = true;
                                                    doc.Fecha = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                                }
                                                else
                                                {
                                                    var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                                              where p.Nombre == itemParametro.nombrecampo
                                                              select p;
                                                    var date = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                                                    var val = new DocumentoValor
                                                    {
                                                        Documento = doc,
                                                        Parametro = par.ToList()[0],
                                                        Repeticion = ((short)1),
                                                        Valor = date
                                                    };
                                                    doc.Parametros.Add(val);
                                                }
                                                break;
                                            }
                                        case "PRESENTACIÓN":
                                            {
                                                doc.Presentacion = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                                break;
                                            }
                                        case "VENCIMIENTO":
                                            {
                                                doc.Vencimiento = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                                break;
                                            }
                                        case "CIERRE":
                                            {
                                                doc.FechaCierre = DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                                break;
                                            }
                                        default:
                                            {
                                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                                          where p.Nombre == itemParametro.nombrecampo
                                                          select p;
                                               var date =  DateTime.ParseExact(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                                                var val = new DocumentoValor
                                                {
                                                    Documento = doc,
                                                    Parametro = par.ToList()[0],
                                                    Repeticion = ((short)1),
                                                    Valor = date
                                                };
                                                doc.Parametros.Add(val);
                                                break;
                                            }
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
                                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                                          where p.Nombre == itemParametro.nombrecampo
                                                          select p;
                                                var val = new DocumentoValor
                                                {
                                                    Documento = doc,
                                                    Parametro = par.ToList()[0],
                                                    Repeticion = ((short)1),
                                                    Valor = doc.Vehiculo.Id.ToString()
                                                };
                                                doc.Parametros.Add(val);
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
                                            string descr = itemempleado.Entidad.Descripcion;
                                            if (descr.Equals(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)))
                                            {
                                                doc.Empleado = itemempleado;
                                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                                          where p.Nombre == itemParametro.nombrecampo
                                                          select p;
                                                var val = new DocumentoValor
                                                {
                                                    Documento = doc,
                                                    Parametro = par.ToList()[0],
                                                    Repeticion = ((short)1),
                                                    Valor = doc.Empleado.Id.ToString()
                                                };
                                                doc.Parametros.Add(val);
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
                                                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                                          where p.Nombre == itemParametro.nombrecampo
                                                          select p;
                                                var val = new DocumentoValor
                                                {
                                                    Documento = doc,
                                                    Parametro = par.ToList()[0],
                                                    Repeticion = ((short)1),
                                                    Valor = doc.Transportista.Id.ToString()
                                                };
                                                doc.Parametros.Add(val);
                                                break;
                                            }
                                        }
                                      
                                    }
                                    break;
                                }
                            case "cliente":
                                {

                                    Cliente cliente = new ClienteDAO().GetList(empresas, lineas)
                                        .Where(X=> X.Descripcion.Equals(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo))).FirstOrDefault();
                                    
                                    var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                              where p.Nombre == itemParametro.nombrecampo
                                              select p;
                                    var val = new DocumentoValor
                                              {
                                                  Documento = doc,
                                                  Parametro = par.ToList()[0],
                                                  Repeticion = ((short)1),
                                                  Valor = cliente.Id.ToString()
                                              };
                                    doc.Parametros.Add(val);
                                    break;
                                }
                            case "equipo":
                                {
                                    var equipos = new EquipoDAO().FindAll().Cast<Equipo>();                                    
                                    Equipo equip = equipos.Where(x=> x.Descripcion.Equals(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo))).FirstOrDefault();
                                    var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                              where p.Nombre == itemParametro.nombrecampo
                                              select p;
                                    var val = new DocumentoValor
                                    {
                                        Documento = doc,
                                        Parametro = par.ToList()[0],
                                        Repeticion = ((short)1),
                                        Valor = equip.Id.ToString()
                                    };
                                    doc.Parametros.Add(val);
                                    break;
                                }
                            case "centrocostos":
                                {
                                    var departamentos = new DepartamentoDAO().GetList(empresas, lineas);
                                        List<int> dep = new List<int>();
                                        foreach (var itemDeparta in departamentos)
                                        {
                                            dep.Add(itemDeparta.Id);
                                        }
                                        CentroDeCostos centro = new CentroDeCostosDAO()
                                            .GetList(empresas, lineas, dep)
                                            .Where(x=> x.Descripcion.Equals(System.Text.Encoding.UTF8.GetString(itemParametro.objectocampo)))
                                            .FirstOrDefault();

                                    var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                                              where p.Nombre == itemParametro.nombrecampo
                                              select p;
                                    var val = new DocumentoValor
                                    {
                                        Documento = doc,
                                        Parametro = par.ToList()[0],
                                        Repeticion = ((short)1),
                                        Valor = centro.Id.ToString()
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
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }
    }
}