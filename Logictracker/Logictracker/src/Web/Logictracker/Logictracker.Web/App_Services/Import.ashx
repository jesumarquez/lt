<%@ WebHandler Language="C#" Class="Import" %>

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Logictracker.Process.Import.Service;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.Handlers;

public class Import : BaseServiceHandler, IHttpHandler
{
    protected override string Securable
    {
        get { return Logictracker.Security.Securables.WebServiceImport; }
    }
    //Crea formularios para hacer el testing
//public new void ProcessRequest(HttpContext context)
//{
//    base.ProcessRequest(context);
//    if (context.Request.Form.Count == 0 && context.Request.QueryString["f"] != null)
//        {
//            context.Response.ContentType = "text/html";
//        context.Response.Clear();
//            WriteLine("<HTML>");
//            WriteLine("<HEAD><TITLE></TITLE></HEAD>");
//            WriteLine("<BODY>");
//            WriteLine(@"<FORM method=""POST"" target=""_blank"">
//<INPUT type=""text"" name=""method"" value=""login"" />            
//                <INPUT type=""text"" name=""username"" />
//                <INPUT type=""text"" name=""password"" />
//                <INPUT type=""submit""/>
//            </FORM>
//            <FORM method=""POST"" target=""_blank"">
//<INPUT type=""text"" name=""method"" value=""importdata"" />            
//<INPUT type=""text"" name=""sessionid"" />
//                <INPUT type=""text"" name=""company"" />
//                <INPUT type=""text"" name=""branch"" />
//                
//                <textarea name=""data"" cols=""40"" rows=""10"" style=""width:200px; height:100px;""  >
//</textarea>                
//                <INPUT type=""submit""/>
//            </FORM>");
//            WriteLine("</BODY>");
//            WriteLine("</HTML>");
//        }
//}
    public override void ProcessRequest (string method) {
        if (method == "importdata")
        {
            var company = Context.Request.Form["company"];
            var branch = Context.Request.Form["branch"];
            var data = Context.Request.Form["data"];
            var empresa = string.IsNullOrEmpty(company) ? null : BaseService.GetEmpresaByCode(company);
            var linea = string.IsNullOrEmpty(branch) || empresa == null ? null : BaseService.GetLineaByCode(branch, empresa);
            var idEmpresa = empresa != null ? empresa.Id : -1;
            var idLinea = linea != null ? linea.Id : -1;

            var sw = File.CreateText(Context.Server.MapPath("req.xml"));
            sw.Write(data);
            sw.Close();
            
            TextReader reader = new StringReader(data);
            var xml = new XmlSerializer(typeof(OPERACIONES));
            var ds = (OPERACIONES)xml.Deserialize(reader);
            reader.Close();

            var server = new Server();
            WriteLine("<Respuesta>");
            foreach (var op in ds.OPERACION)
            {
                try
                {
                    var oDispositivo = op.DISPOSITIVO;
                    var oVehiculo = op.VEHICULO;
                    var oBase = op.BASE;
                    var oDistrito = op.DISTRITO;

                    var operacion = Logictracker.Process.Import.Client.Types.Operation.None;

                    switch (op.movimiento)
                    {
                        case uOperacion.A:
                            operacion = Logictracker.Process.Import.Client.Types.Operation.Add;
                            break;
                        case uOperacion.B:
                            operacion = Logictracker.Process.Import.Client.Types.Operation.Delete;
                            break;
                        case uOperacion.M:
                            operacion = Logictracker.Process.Import.Client.Types.Operation.Modify;
                            break;
                    }
                    Logictracker.Types.BusinessObjects.Empresa uempresa;
                    Logictracker.Types.BusinessObjects.Linea ulinea;
                       
                    var done = false;
                    if (operacion == Logictracker.Process.Import.Client.Types.Operation.Add)
                    {
                        var dataDistrito = GetData(oDistrito, operacion);
                        uempresa = BaseService.DAOFactory.EmpresaDAO.FindByCodigo(oDistrito.codigo);
                        idEmpresa = uempresa != null ? uempresa.Id : -1;
                        done = uempresa == null;
                        server.Import(idEmpresa, idLinea, dataDistrito, 1);
                        
                        uempresa = BaseService.DAOFactory.EmpresaDAO.FindByCodigo(oDistrito.codigo);
                        idEmpresa = uempresa != null ? uempresa.Id : -1;

                        var dataBase = GetData(oBase, operacion);
                        if (dataBase != null)
                        {
                            ulinea = BaseService.DAOFactory.LineaDAO.FindByCodigo(idEmpresa, oBase.codigo);
                            idLinea = ulinea != null ? ulinea.Id : -1;
                            done = ulinea == null;
                            server.Import(idEmpresa, idLinea, dataBase, 1);

                            ulinea = BaseService.DAOFactory.LineaDAO.FindByCodigo(idEmpresa, oBase.codigo);
                            idLinea = ulinea != null ? ulinea.Id : -1;
                        }

                        var dataDispositivo = GetData(oDispositivo, operacion);
                        if (dataDispositivo != null)
                        {
                            var dataTel = GetDataTel(oDispositivo, operacion);
                            server.Import(idEmpresa, idLinea, dataTel, 1);

                            var dispo = BaseService.DAOFactory.DispositivoDAO.GetByCode(oDispositivo.codigo);
                            done = dispo == null || dispo.Estado == Dispositivo.Estados.Inactivo;

                            dataDispositivo.Operation = (int)(done ? Logictracker.Process.Import.Client.Types.Operation.AddOrModify :
                                                                                                                        Logictracker.Process.Import.Client.Types.Operation.Add);
                            server.Import(idEmpresa, idLinea, dataDispositivo, 1);
                        }

                        var dataVehiculo = GetData(oVehiculo, operacion);
                        if (dataVehiculo != null)
                        {
                            var dataMarca = GetDataMarca(oVehiculo, operacion);
                            server.Import(idEmpresa, idLinea, dataMarca, 1);
                            var dataModelo = GetDataModelo(oVehiculo, operacion);
                            server.Import(idEmpresa, idLinea, dataModelo, 1);
                            var dataTipoVeh = GetDataTipoVeh(oVehiculo, operacion);
                            server.Import(idEmpresa, idLinea, dataTipoVeh, 1);
                            if (oDispositivo != null)
                            {
                                dataVehiculo.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Dispositivo,
                                                 oDispositivo.codigo);
                            }
                            var vh = BaseService.DAOFactory.CocheDAO.FindByInterno(new[] { idEmpresa }, new[] { idLinea },
                                                                                  oVehiculo.interno);
                            done = vh == null || vh.Estado == Logictracker.Types.BusinessObjects.Vehiculos.Coche.Estados.Inactivo;

                            dataVehiculo.Operation = (int)(done ? Logictracker.Process.Import.Client.Types.Operation.AddOrModify :
                                                                                                                                Logictracker.Process.Import.Client.Types.Operation.Add);
                            if (oBase != null)
                                dataVehiculo.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Base, oBase.codigo);
                            server.Import(idEmpresa, -1, dataVehiculo, 1);
                        }
                    }
                    else if (operacion == Logictracker.Process.Import.Client.Types.Operation.Modify)
                    {
                        uempresa = BaseService.DAOFactory.EmpresaDAO.FindByCodigo(oDistrito.codigo);
                        idEmpresa = uempresa != null ? uempresa.Id : -1;
                        if (oBase != null)
                        {
                            ulinea = BaseService.DAOFactory.LineaDAO.FindByCodigo(idEmpresa, oBase.codigo);
                            idLinea = ulinea != null ? ulinea.Id : -1;
                        }
                    
                        if (oDispositivo != null || oVehiculo != null)
                        {
                            var dataMarca = GetDataMarca(oVehiculo, Logictracker.Process.Import.Client.Types.Operation.Add);
                            server.Import(idEmpresa, idLinea, dataMarca, 1);
                            var dataModelo = GetDataModelo(oVehiculo, Logictracker.Process.Import.Client.Types.Operation.Add);
                            server.Import(idEmpresa, idLinea, dataModelo, 1);
                            var dataTipoVeh = GetDataTipoVeh(oVehiculo, Logictracker.Process.Import.Client.Types.Operation.Add);
                            server.Import(idEmpresa, idLinea, dataTipoVeh, 1);
                            
                            var dataVehiculo = GetData(oVehiculo, operacion);
                            if (oDispositivo != null)
                            {
                                var dataTel = GetDataTel(oDispositivo, Logictracker.Process.Import.Client.Types.Operation.Add);
                                server.Import(idEmpresa, idLinea, dataTel, 1);
                                
                                var dataDispositivo = GetData(oDispositivo, operacion);

                                done = BaseService.DAOFactory.DispositivoDAO.GetByCode(oDispositivo.codigo) != null;
                                server.Import(idEmpresa, idLinea, dataDispositivo, 1);
                                dataVehiculo.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Dispositivo,
                                                 oDispositivo.codigo);
                            }
                            else
                            {
                                done = BaseService.DAOFactory.CocheDAO.FindByInterno(new[]{idEmpresa}, new[]{idLinea}, oVehiculo.interno) == null;
                            }
                            server.Import(idEmpresa, idLinea, dataVehiculo, 1);
                        }
                        else if (oBase != null)
                        {
                            var dataBase = GetData(oBase, operacion);
                            done = BaseService.DAOFactory.LineaDAO.FindByCodigo(idEmpresa, oBase.codigo) != null;
                            server.Import(idEmpresa, idLinea, dataBase, 1);
                        }
                        else
                        {
                            var dataDistrito = GetData(oDistrito, operacion);
                            done = BaseService.DAOFactory.EmpresaDAO.FindByCodigo(oDistrito.codigo) != null;
                            server.Import(idEmpresa, idLinea, dataDistrito, 1);
                        }
                    }
                    else if (operacion == Logictracker.Process.Import.Client.Types.Operation.Delete)
                    {
                        uempresa = BaseService.DAOFactory.EmpresaDAO.FindByCodigo(oDistrito.codigo);
                        idEmpresa = uempresa != null ? uempresa.Id : -1;
                        if (oBase != null)
                        {
                            ulinea = BaseService.DAOFactory.LineaDAO.FindByCodigo(idEmpresa, oBase.codigo);
                            idLinea = ulinea != null ? ulinea.Id : -1;
                        }
                        
                        if (oDispositivo != null)
                        {
                            var dataDispositivo = GetData(oDispositivo, operacion);
                            done = BaseService.DAOFactory.DispositivoDAO.GetByCode(oDispositivo.codigo) != null;
                            server.Import(idEmpresa, idLinea, dataDispositivo, 1);
                        }
                        else if (oVehiculo != null)
                        {
                            var dataVehiculo = GetData(oVehiculo, operacion);
                            done = BaseService.DAOFactory.CocheDAO.FindByInterno(new[] { idEmpresa }, new[] { idLinea }, oVehiculo.interno) == null;
                            server.Import(idEmpresa, idLinea, dataVehiculo, 1);
                        }
                        else if (oBase != null)
                        {
                            var dataBase = GetData(oBase, operacion);
                            done = BaseService.DAOFactory.LineaDAO.FindByCodigo(idEmpresa, oBase.codigo) != null;
                            server.Import(idEmpresa, idLinea, dataBase, 1);
                        }
                        else
                        {
                            var dataDistrito = GetData(oDistrito, operacion);
                            done = BaseService.DAOFactory.EmpresaDAO.FindByCodigo(oDistrito.codigo) != null;
                            server.Import(idEmpresa, idLinea, dataDistrito, 1);
                        }
                    }
                    WriteLine("<Import trans=\"{0}\">{1}</Import>", op.trans, done);
                }
                catch (System.Exception ex)
                {
                    Logictracker.DatabaseTracer.Core.STrace.Exception("Cusat Web Service", ex, 0, new Dictionary<string, string> { { "transaction", op.trans } }, ex.Message);
                    WriteLine("<Error trans=\"{0}\">{1}</Error>", op.trans, ex.Message);
                }
            }
            WriteLine("</Respuesta>");
            sw = File.CreateText(Context.Server.MapPath("res.xml"));
            sw.Write(res.ToString());
            sw.Close();
        }
    }

    private StringBuilder res = new StringBuilder();
    public new void WriteLine(string text, params object[] parameters)
    {
        res.AppendFormat(text + "\n", parameters);
        Context.Response.Write(string.Format(text + "\n", parameters));
    }
    private Logictracker.Process.Import.Client.Types.Data GetData(OPERACIONESOPERACIONDISTRITO oDistrito, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oDistrito == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
                       {
                           Entity = (int)Logictracker.Process.Import.Client.Types.Entities.Distrito,
                           Operation = (int) operation
                       };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Distrito.Codigo, oDistrito.codigo);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Distrito.Descripcion, oDistrito.razsoc);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Distrito.Gmt, "0");
        return data;
    }
    private Logictracker.Process.Import.Client.Types.Data GetData(OPERACIONESOPERACIONBASE oBase, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oBase == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
                       {
                           Entity = (int)Logictracker.Process.Import.Client.Types.Entities.Base, 
                           Operation = (int) operation
                       };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Base.Codigo, oBase.codigo);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Base.Descripcion, oBase.descripcion);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Base.Gmt, "0");
        return data;
    }
    private Logictracker.Process.Import.Client.Types.Data GetData(OPERACIONESOPERACIONDISPOSITIVO oDispositivo, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oDispositivo == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
                       {
                           Entity = (int)Logictracker.Process.Import.Client.Types.Entities.Dispositivo,
                           Operation = (int) operation
                       };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Dispositivo.TipoDispositivo, oDispositivo.tipo);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Dispositivo.Codigo, oDispositivo.codigo);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Dispositivo.Imei, oDispositivo.imei);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Dispositivo.Estado, oDispositivo.estado.ToString());
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Dispositivo.LineaTelefonica, oDispositivo.linea);
        return data;
    }
    private Logictracker.Process.Import.Client.Types.Data GetData(OPERACIONESOPERACIONVEHICULO oVehiculo, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oVehiculo == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
        {
            Entity = (int)Logictracker.Process.Import.Client.Types.Entities.Vehiculo,
            Operation = (int)operation
        };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.TipoVehiculo, oVehiculo.tipov);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Interno, oVehiculo.interno);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Patente, oVehiculo.dominio);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Marca, oVehiculo.marca);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Modelo, oVehiculo.modelo);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Estado, oVehiculo.estado.ToString());
        //data.Add(Logictracker.Process.Import.Client.Types.Properties.Vehiculo.Visible, oVehiculo.visible.ToString());
        return data;
    }

    private Logictracker.Process.Import.Client.Types.Data GetDataTel(OPERACIONESOPERACIONDISPOSITIVO oDispositivo, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oDispositivo == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
        {
            Entity = (int)Logictracker.Process.Import.Client.Types.Entities.LineaTelefonica,
            Operation = (int)operation
        };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.LineaTelefonica.Numero, oDispositivo.linea);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.LineaTelefonica.Sim, oDispositivo.linea);
        return data;
    }
    private Logictracker.Process.Import.Client.Types.Data GetDataTipoVeh(OPERACIONESOPERACIONVEHICULO oVehiculo, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oVehiculo == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
        {
            Entity = (int)Logictracker.Process.Import.Client.Types.Entities.TipoVehiculo,
            Operation = (int)operation
        };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.TipoVehiculo.Codigo, oVehiculo.tipov);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.TipoVehiculo.Descripcion, oVehiculo.tipov);
        return data;
    }
    private Logictracker.Process.Import.Client.Types.Data GetDataMarca(OPERACIONESOPERACIONVEHICULO oVehiculo, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oVehiculo == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
        {
            Entity = (int)Logictracker.Process.Import.Client.Types.Entities.Marca,
            Operation = (int)operation
        };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Marca.Nombre, oVehiculo.marca);
        return data;
    }
    private Logictracker.Process.Import.Client.Types.Data GetDataModelo(OPERACIONESOPERACIONVEHICULO oVehiculo, Logictracker.Process.Import.Client.Types.Operation operation)
    {
        if (oVehiculo == null) return null;

        var data = new Logictracker.Process.Import.Client.Types.Data
        {
            Entity = (int)Logictracker.Process.Import.Client.Types.Entities.Modelo,
            Operation = (int)operation
        };
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Modelo.Marca, oVehiculo.marca);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Modelo.Codigo, oVehiculo.modelo);
        data.Add(Logictracker.Process.Import.Client.Types.Properties.Modelo.Descripcion, oVehiculo.modelo);
        return data;
    }
}