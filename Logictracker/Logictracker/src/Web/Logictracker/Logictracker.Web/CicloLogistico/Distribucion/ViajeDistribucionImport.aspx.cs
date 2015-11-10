using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Geocoder.Core.VO;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Process.Import.EntityParser;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.CicloLogistico.Distribucion
{
    public partial class ViajeDistribucionImport : SecuredImportPage
    {
        protected override string RedirectUrl { get { return "ViajeDistribucionLista.aspx"; } }
        protected override string VariableName { get { return "CLOG_DISTRIBUCION"; } }
        protected override string GetRefference() { return "DISTRIBUCION"; }

        private List<Coche> _cochesBuffer = new List<Coche>();
        private List<ViajeDistribucion> _viajesBuffer = new List<ViajeDistribucion>();
        private List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private Dictionary<int, List<int>> _empresasLineas = new Dictionary<int, List<int>>();

        private static class Modes
        {
            public const string Default = "Default";
            public const string Axiodis = "Axiodis";
            public const string AxiodisF = "AxiodisF";
            public const string Roadshow = "Roadshow";
            public const string RoadshowCsv = "RoadshowCsv";
            public const string RoadshowCsv2 = "RoadshowCsv2";
            public const string RoadshowCsv2ByInterno = "RoadshowCsv2ByInterno";
            public const string Roadnet = "Roadnet";
            public const string ExcelTemplate = "ExcelTemplate";
            public const string RL = "RL";
        }

        protected override string[] ImportModes
        {
            get { return new[] { Modes.Default, Modes.Axiodis, Modes.AxiodisF, Modes.Roadshow, Modes.RoadshowCsv, Modes.RoadshowCsv2, Modes.RoadshowCsv2ByInterno, Modes.Roadnet, Modes.ExcelTemplate, Modes.RL }; }
        }
        protected override List<FieldValue> GetMappingFields()
        {
            switch(CurrentImportMode)
            {
                case Modes.Axiodis: return Fields.Axiodis;
                case Modes.AxiodisF: return Fields.AxiodisF;
                case Modes.Roadshow: return Fields.Roadshow;
                case Modes.RoadshowCsv: return Fields.RoadshowCsv;
                case Modes.RoadshowCsv2: return Fields.RoadshowCsv;
                case Modes.RoadshowCsv2ByInterno: return Fields.RoadshowCsv;
                case Modes.Roadnet: return Fields.Roadnet;
                case Modes.ExcelTemplate: return Fields.ExcelTemplate;
                case Modes.RL: return Fields.ReginaldLee;
                default: return Fields.Default;
            }
        }
        private List<FieldValue> MappingFieldsOpcionales() 
        {
            switch (CurrentImportMode)
            {
                case Modes.Axiodis: return Fields.AxiodisOpcional;
                case Modes.Roadshow: return Fields.RoadshowOpcional;
                default: return Fields.DefaultOpcional;
            }
        }

        protected override void ValidateMapping()
        {
            var fields = GetMappingFields();
            for(var i = 1; i < fields.Count; i++)
            {
                var field = fields[i];

                if (!MappingFieldsOpcionales().Contains(field) && !IsMapped(field.Value))
                    throw new ApplicationException("Falta mapear " + field.Name);
            }

            ValidateEntity(cbEmpresa.Selected, "PARENTI01");

            switch (CurrentImportMode)
            {
                case Modes.Axiodis:
                case Modes.AxiodisF:
                    ValidateEntity(cbClienteEntregas.Selected, "CLIENT");
                    ValidateEntity(cbClienteSucursales.Selected, "CLIENT");
                    var vigencia = ValidateEmpty(txtVigencia.Text, "VIGENCIA_HORAS");
                    var horasVigencia = ValidateInt32(vigencia, "VIGENCIA_HORAS");
                    if (horasVigencia <= 0) ThrowInvalidValue("VIGENCIA_HORAS");
                    break;
                case Modes.Roadshow:
                case Modes.RoadshowCsv:
                case Modes.RoadshowCsv2:
                case Modes.RoadshowCsv2ByInterno:
                    ValidateEntity(cbLinea.Selected, "PARENTI02");
                    ValidateEntity(cbClienteRoadshow.Selected, "CLIENT");
                    var vigenciaRoadshow = ValidateEmpty(txtVigencia.Text, "VIGENCIA_HORAS");
                    var horasVigenciaRoadshow = ValidateInt32(vigenciaRoadshow, "VIGENCIA_HORAS");
                    if (horasVigenciaRoadshow <= 0) ThrowInvalidValue("VIGENCIA_HORAS");
                    break;
                case Modes.Roadnet:
                    ValidateEntity(cbLinea.Selected, "PARENTI02");
                    var vigenciaRoadnet = ValidateEmpty(txtVigenciaRoadnet.Text, "VIGENCIA_HORAS");
                    var horasVigenciaRoadnet = ValidateInt32(vigenciaRoadnet, "VIGENCIA_HORAS");
                    if (horasVigenciaRoadnet <= 0) ThrowInvalidValue("VIGENCIA_HORAS");
                    break;
                case Modes.ExcelTemplate:
                    ValidateEntity(cbEmpresa.Selected, "PARENTI01");
                    ValidateEntity(cbCliente.Selected, "CLIENT");
                    var horasVig = ValidateInt32(txtVigenciaExcelTemplate.Text.Trim(), "VIGENCIA_HORAS");
                    if (horasVig <= 0) ThrowInvalidValue("VIGENCIA_HORAS");
                    
                    var inicio = ValidateEmpty(txtInicio.Text.Trim(), "INICIO");
                    if (inicio.Length != 5 || !inicio.Contains(":") || inicio.IndexOf(':') != 2) 
                        ThrowInvalidValue("INICIO");
                    
                    var hora = ValidateInt32(inicio.Split(':')[0], "INICIO");
                    if (hora > 23) ThrowInvalidValue("INICIO");
                    var min = ValidateInt32(inicio.Split(':')[1], "INICIO");
                    if (min > 59) ThrowInvalidValue("INICIO");
                    break;
                default:
                    ValidateEntity(cbLinea.Selected, "PARENTI02");
                    break;
            }
        }

        protected override bool ValidateRow(ImportRow row)
        {
            switch (CurrentImportMode)
            {
                case Modes.Axiodis:
                        ValidateInt32(row[GetColumnByValue(Fields.Viaje.Value)], "VIAJE");
                        ValidateInt32(row[GetColumnByValue(Fields.Secuencia.Value)],"SECUENCIA");
                        ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "LATITUD");
                        ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "LONGITUD");

                        var fecha = row[GetColumnByValue(Fields.Fecha.Value)];
                        if (fecha.Length < 10) ThrowInvalidValue("FECHA");
                        ValidateInt32(fecha.Substring(0, 4), "FECHA");
                        ValidateInt32(fecha.Substring(4, 2), "FECHA");
                        ValidateInt32(fecha.Substring(6, 2), "FECHA");
                        ValidateInt32(fecha.Substring(8, 2), "FECHA");
                        return true;
                case Modes.AxiodisF:
                        ValidateEmpty(row[GetColumnByValue(Fields.Viaje.Value)], "VIAJE");
                        ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "LATITUD");
                        ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "LONGITUD");

                        var dt = row[GetColumnByValue(Fields.Fecha.Value)];
                        if (dt.Length < 8) ThrowInvalidValue("FECHA");
                        ValidateInt32(dt.Substring(0, 4), "FECHA");
                        ValidateInt32(dt.Substring(4, 2), "FECHA");
                        ValidateInt32(dt.Substring(6, 2), "FECHA");
                        return true;
                case Modes.Roadshow:
                        ValidateInt32(row[GetColumnByValue(Fields.Viaje.Value)], "VIAJE");
                        ValidateInt32(row[GetColumnByValue(Fields.Secuencia.Value)], "SECUENCIA");
                        ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "LATITUD");
                        ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "LONGITUD");

                        DateTime date;
                        var dateOk = DateTime.TryParse(row[GetColumnByValue(Fields.Fecha.Value)], out date);
                        if (!dateOk) ThrowInvalidValue("FECHA");
                        DateTime programado;
                        var programadoOk = DateTime.TryParse(row[GetColumnByValue(Fields.Programada.Value)], out programado);
                        if (!programadoOk) ThrowInvalidValue("PROGRAMADO");

                        return true;
                case Modes.ExcelTemplate:
                        ValidateEmpty(row.GetString(GetColumnByValue(Fields.CodigoPedido.Value)), "CODIGO_PEDIDO");
                        ValidateEmpty(row.GetString(GetColumnByValue(Fields.Cuadrilla.Value)), "CUADRILLA");
                        var interno = ValidateEmpty(row.GetString(GetColumnByValue(Fields.Vehiculo.Value)), "INTERNO");
                        var vehiculo = DAOFactory.CocheDAO.GetByInterno(new[] { cbEmpresa.Selected }, new[] { -1 }, interno);
                        if (vehiculo == null) ThrowInvalidValue("INTERNO");
                        if (vehiculo != null && vehiculo.Linea == null) ThrowMustEnter("Entities", "PARENTI02");
                        return true;
                default: return true;
            }
        }

        protected override void OnImportModeChange()
        {
            panelAxiodis.Visible = CurrentImportMode == Modes.Axiodis || CurrentImportMode == Modes.AxiodisF;
            if (CurrentImportMode == Modes.AxiodisF)
            {
                txtVigencia.Text = "20";
            }
            panelRoadshow.Visible = CurrentImportMode == Modes.Roadshow || CurrentImportMode == Modes.RoadshowCsv || CurrentImportMode == Modes.RoadshowCsv2 || CurrentImportMode == Modes.RoadshowCsv2ByInterno;
            panelExcel.Visible = CurrentImportMode == Modes.ExcelTemplate;
            panelRoadnet.Visible = CurrentImportMode == Modes.Roadnet;
            updPanelAxiodis.Update();
            updPanelRoadshow.Update();
            updPanelRoadnet.Update();
            updPanelExcel.Update();
        }

        protected override void Import(List<ImportRow> rows)
        {
            switch (CurrentImportMode)
            {
                case Modes.Axiodis: ImportAxiodis(rows); break;
                case Modes.AxiodisF: ImportAxiodisF(rows); break;
                case Modes.Roadshow: ImportRoadshow(rows); break;
                case Modes.RoadshowCsv: ImportRoadshowCsv(rows); break;
                case Modes.RoadshowCsv2: ImportRoadshowCsv2(rows); break;
                case Modes.RoadshowCsv2ByInterno: ImportRoadshowCsv2ByInterno(rows); break;
                case Modes.Roadnet: ImportRoadnet(rows); break;
                case Modes.ExcelTemplate: ImportExcelTemplate(rows); break;
                case Modes.RL: ImportRL(rows); break;
                default: ImportDefault(rows); break;
            }
        }
        
        protected void ImportDefault(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected;
            var linea = cbLinea.Selected;

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var conDireccion = IsMapped(Fields.Latitud.Value) || IsMapped(Fields.Direccion.Value) || IsMapped(Fields.Calle.Value);

                    var parserReferencia = new ReferenciaGeograficaV1(DAOFactory);
                    var parserCliente = new ClienteV1(DAOFactory);
                    var parserPuntoEntrega = new PuntoEntregaV1(DAOFactory);
                    var parserDistribucion = new DistribucionV1(DAOFactory);

                    var doneReferencias = new List<string>();
                    var doneClientes = new List<string>();
                    var donePuntosEntrega = new List<string>();
                    var doneDistribucion = new List<string>();

                    foreach (var row in rows)
                    {
                        Data data;
                        if (conDireccion)
                        {
                            var codigogeoref = row.GetString(GetColumnByValue(Fields.Cliente.Value)) + "-" +
                                               row.GetString(GetColumnByValue(Fields.PuntoEntrega.Value));
                            if (!doneReferencias.Contains(codigogeoref))
                            {
                                data = GetLogiclinkData(Entities.ReferenciaGeografica, row);
                                data.Add(Properties.ReferenciaGeografica.Codigo, codigogeoref);
                                var georef = parserReferencia.Parse(empresa, linea, data);
                                parserReferencia.SaveOrUpdate(georef, empresa, linea, data);
                                doneReferencias.Add(codigogeoref);
                            }

                            var codigocliente = row.GetString(GetColumnByValue(Fields.Cliente.Value));
                            if (!doneClientes.Contains(codigocliente))
                            {
                                data = GetLogiclinkData(Entities.Cliente, row);
                                data.Add(Properties.Cliente.Codigo, codigocliente);
                                data.Add(Properties.Cliente.Descripcion, row.GetString(GetColumnByValue(Fields.Descripcion.Value)));
                                data.Add(Properties.Cliente.DescripcionCorta, row.GetString(GetColumnByValue(Fields.Descripcion.Value)));
                                data.Add(Properties.Cliente.ReferenciaGeografica, row.GetString(GetColumnByValue(Fields.PuntoEntrega.Value)));
                                var cliente = parserCliente.Parse(empresa, linea, data) as Cliente;
                                if (cliente != null &&
                                    cliente.Id == 0)
                                {
                                    parserCliente.Save(cliente, empresa, linea, data);
                                }
                                else
                                {
                                    DAOFactory.RemoveFromSession(cliente);
                                }
                                doneClientes.Add(codigocliente);
                            }

                            var codigopunto = row.GetString(GetColumnByValue(Fields.PuntoEntrega.Value));
                            var keyPunto = codigocliente + "|" + codigopunto;
                            if (!donePuntosEntrega.Contains(keyPunto))
                            {
                                data = GetLogiclinkData(Entities.PuntoEntrega, row);
                                data.Add(Properties.PuntoEntrega.Cliente, codigocliente);
                                data.Add(Properties.PuntoEntrega.Descripcion, row.GetString(GetColumnByValue(Fields.Descripcion.Value)));
                                data.Add(Properties.PuntoEntrega.Codigo, codigopunto);
                                data.Add(Properties.PuntoEntrega.ReferenciaGeografica, codigogeoref);
                                var puntoEntrega = parserPuntoEntrega.Parse(empresa, linea, data);
                                parserPuntoEntrega.SaveOrUpdate(puntoEntrega, empresa, linea, data);
                                donePuntosEntrega.Add(keyPunto);
                            }
                        }

                        data = GetLogiclinkData(Entities.Distribucion, row);
                        if (data.Values.Any(v => !string.IsNullOrEmpty(v.Trim()))) continue;
                        data.Add(Properties.Distribucion.Gmt, Usuario.GmtModifier.ToString(CultureInfo.InvariantCulture));
                        var codigoDistri = row.GetString(GetColumnByValue(Fields.CodigoPedido.Value));
                        data.Add(Properties.Distribucion.Codigo, codigoDistri);
                        if (!doneDistribucion.Contains(codigoDistri))
                        {
                            data.Add(Properties.Distribucion.ModificaCabecera, "true");
                            doneDistribucion.Add(codigoDistri);
                        }
                        var distribucion = parserDistribucion.Parse(empresa, linea, data) as ViajeDistribucion;
                        if (distribucion != null &&
                            distribucion.Estado == ViajeDistribucion.Estados.Pendiente)
                        {
                            parserDistribucion.SaveOrUpdate(distribucion, empresa, linea, data);
                        }
                        else
                        {
                            DAOFactory.RemoveFromSession(distribucion);
                        }
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        protected void ImportAxiodis(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var clienteEntregas = cbClienteEntregas.Selected > 0
                                      ? DAOFactory.ClienteDAO.FindById(cbClienteEntregas.Selected)
                                      : null;
            var clienteSucursales = cbClienteSucursales.Selected > 0
                                        ? DAOFactory.ClienteDAO.FindById(cbClienteSucursales.Selected)
                                        : null;
            var vigencia = Convert.ToInt32(txtVigencia.Text.Trim());
            var tipoServicio = DAOFactory.TipoServicioCicloDAO.FindDefault(new[] {cbEmpresa.Selected},
                                                                           new[] {cbLinea.Selected});

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var stringFecha = row[GetColumnByValue(Fields.Fecha.Value)];

                        var fecha = new DateTime(Convert.ToInt32(stringFecha.Substring(0, 4)), Convert.ToInt32(stringFecha.Substring(4, 2)),
                            Convert.ToInt32(stringFecha.Substring(6, 2)), Convert.ToInt32(stringFecha.Substring(8, 2)), 0, 0);

                        var patente = row[GetColumnByValue(Fields.Vehiculo.Value)];
                        var codigoPedido = row[GetColumnByValue(Fields.CodigoPedido.Value)];
                        var numeroViaje = Convert.ToInt32(row[GetColumnByValue(Fields.Viaje.Value)]);
                        //var secuencia = Convert.ToInt32(row[GetColumnByValue(Fields.Secuencia.Value)]);
                        var codigoViaje = string.Format("{0}|{1}|{2}", stringFecha.Substring(0, 8), patente, numeroViaje);
                        var latitud = ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "");
                        var longitud = ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "");
                        var celular = row[GetColumnByValue(Fields.Celular.Value)].Replace("-", string.Empty);
                        var compania = row[GetColumnByValue(Fields.Compania.Value)];
                        var importe = row[GetColumnByValue(Fields.Importe.Value)] != "" ? Convert.ToDouble(row[GetColumnByValue(Fields.Importe.Value)]) : 0.0;
                        var nombre = row[GetColumnByValue(Fields.Nombre.Value)];
                        var mail = string.Empty;

                        if (celular.Length > 1 &&
                            celular[0] == '0')
                            celular = celular.Substring(1);

                        if (celular.Length > 2 &&
                            celular.Substring(0, 2) != "11")
                            celular = "11" + celular.Substring(2);

                        switch (compania.Trim().ToUpperInvariant())
                        {
                            case "MOVISTAR":
                                mail = celular + "@sms.movistar.net.ar";
                                break;
                            case "CLARO":
                                mail = celular + "@sms.ctimovil.com.ar" /*"@sms.cmail.com.ar"*/;
                                break;
                            case "PERSONAL":
                                mail = celular + "@alerta.personal.com.ar" /*"@personal-net.com.ar"*/;
                                break;
                            case "NEXTEL":
                                mail = "TwoWay." + celular + "@nextel.net.ar";
                                break;
                            default:
                                if (celular != string.Empty)
                                    mail = celular + "@sms.movistar.net.ar;" + celular + "@sms.cmail.com.ar;" + celular + "@personal-net.com.ar;" + "TwoWay." +
                                           celular + "@nextel.net.ar";
                                break;
                        }

                        var esSucursal = codigoPedido.IndexOf('|') < 0;
                        var cliente = esSucursal ? clienteSucursales : clienteEntregas;

                        #endregion

                        if (list.Count == 0 ||
                            codigoViaje != list.Last().Codigo)
                        {
                            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, codigoViaje);
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoViaje == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            var vehiculo = DAOFactory.CocheDAO.GetByPatente(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, patente);
                            var chofer = vehiculo != null && !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                            viaje = new ViajeDistribucion
                                    {
                                        Empresa = empresa,
                                        Linea = linea,
                                        Vehiculo = vehiculo,
                                        Empleado = chofer,
                                        Codigo = codigoViaje,
                                        Estado = ViajeDistribucion.Estados.Pendiente,
                                        Inicio = fecha.ToDataBaseDateTime(),
                                        Fin = fecha.ToDataBaseDateTime(),
                                        NumeroViaje = Convert.ToInt32(numeroViaje),
                                        Tipo = ViajeDistribucion.Tipos.Desordenado,
                                        Alta = DateTime.UtcNow,
                                        RegresoABase = chkRegresaABase.Checked
                                    };

                            #endregion

                            list.Add(viaje);
                        }
                        viaje.Fin = fecha.ToDataBaseDateTime();

                        if (viaje.Detalles.Count == 0)
                        {
                            //el primer elemento es la base
                            var origen = new EntregaDistribucion
                                         {
                                             Linea = linea,
                                             Descripcion = linea.Descripcion,
                                             Estado = EntregaDistribucion.Estados.Pendiente,
                                             Orden = 0,
                                             Programado = fecha.ToDataBaseDateTime(),
                                             ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                             Viaje = viaje
                                         };
                            viaje.Detalles.Add(origen);

                            var llegada = new EntregaDistribucion
                                          {
                                              Linea = linea,
                                              Descripcion = linea.Descripcion,
                                              Estado = EntregaDistribucion.Estados.Pendiente,
                                              Orden = viaje.Detalles.Count,
                                              Programado = fecha.ToDataBaseDateTime(),
                                              ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                              Viaje = viaje
                                        };
                            viaje.Detalles.Add(llegada);
                        }

                        var direccion = GeocoderHelper.GetEsquinaMasCercana(latitud, longitud);

                        #region var puntoEntrega = [Find By Empresa, Linea, Codigo, Cliente].FirstOrDefault()

                        var puntoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {cliente.Id},
                            codigoPedido);

                        #endregion

                        if (puntoEntrega == null)
                        {
                            #region var puntoDeInteres = new ReferenciaGeografica()

                            var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : cliente.Empresa == null ? null : empresa;
                            var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea != null 
                                                ? viaje.Vehiculo.Linea 
                                                : cliente.Linea ?? linea;

                            var puntoDeInteres = new ReferenciaGeografica
                                                 {
                                                     Codigo = codigoPedido,
                                                     Descripcion = codigoPedido,
                                                     Empresa = empresaGeoRef,
                                                     Linea = lineaGeoRef,
                                                     EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                                     EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                                     EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                                     InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                                     TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                                     Vigencia =
                                                         new Vigencia
                                                         {
                                                             Inicio = DateTime.UtcNow,
                                                             Fin = fecha.AddHours(vigencia).ToDataBaseDateTime()
                                                         },
                                                     Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                                 };

                            #endregion

                            #region var posicion = new Direccion()

                            var posicion = new Direccion
                                           {
                                               Altura = direccion != null ? direccion.Altura : -1,
                                               IdMapa = (short) (direccion != null ? direccion.IdMapaUrbano : -1),
                                               Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                               IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                               IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                               IdEntrecalle = -1,
                                               Latitud = latitud,
                                               Longitud = longitud,
                                               Partido = direccion != null ? direccion.Partido : string.Empty,
                                               Pais = string.Empty,
                                               Calle = direccion != null ? direccion.Calle : string.Empty,
                                               Descripcion =
                                                   direccion != null
                                                       ? direccion.Direccion
                                                       : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                           longitud.ToString(CultureInfo.InvariantCulture)),
                                               Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                           };

                            #endregion

                            #region var poligono = new Poligono()

                            var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                            poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                            #endregion

                            puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);
                            AddReferenciasGeograficas(puntoDeInteres);

                            #region puntoEntrega = new PuntoEntrega()

                            puntoEntrega = new PuntoEntrega
                                           {
                                               Cliente = cliente,
                                               Codigo = codigoPedido,
                                               Descripcion = codigoPedido,
                                               Telefono = string.Empty,
                                               Baja = false,
                                               ReferenciaGeografica = puntoDeInteres,
                                               Nomenclado = true,
                                               DireccionNomenclada = string.Empty,
                                               Mail = mail,
                                               Importe = importe,
                                               Nombre = nombre
                                           };

                            #endregion

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }
                        else
                        {
                            if (puntoEntrega.ReferenciaGeografica.Latitude != latitud ||
                                puntoEntrega.ReferenciaGeografica.Longitude != longitud)
                            {
                                puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                                puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                                #region var posicion = new Direccion()

                                var posicion = new Direccion
                                               {
                                                   Altura = direccion != null ? direccion.Altura : -1,
                                                   IdMapa = (short) (direccion != null ? direccion.IdMapaUrbano : -1),
                                                   Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                                   IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                                   IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                                   IdEntrecalle = -1,
                                                   Latitud = latitud,
                                                   Longitud = longitud,
                                                   Partido = direccion != null ? direccion.Partido : string.Empty,
                                                   Pais = string.Empty,
                                                   Calle = direccion != null ? direccion.Calle : string.Empty,
                                                   Descripcion =
                                                       direccion != null
                                                           ? direccion.Direccion
                                                           : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                               longitud.ToString(CultureInfo.InvariantCulture)),
                                                   Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                               };

                                #endregion

                                #region var poligono = new Poligono()

                                var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                                poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                                #endregion

                                puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                            }

                            #region puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end

                            var end = fecha.AddHours(vigencia).ToDataBaseDateTime();
                            if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                            #endregion


                            puntoEntrega.ReferenciaGeografica.Linea = linea;
                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                            AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                            puntoEntrega.Nombre = nombre;
                            puntoEntrega.Importe = importe;

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                                      {
                                          Cliente = cliente,
                                          PuntoEntrega = puntoEntrega,
                                          Descripcion = nombre.Truncate(128),
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          //Orden = secuencia,
                                          Orden = viaje.Detalles.Count - 1,
                                          Programado = fecha.ToDataBaseDateTime(),
                                          ProgramadoHasta = fecha.ToDataBaseDateTime().AddMinutes(empresa.MarginMinutes),
                                          TipoServicio = tipoServicio,
                                          Viaje = viaje
                                      };

                        #endregion

                        viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        if (chkCalcularHoras.Checked &&
                            viajeDistribucion.Detalles.Count > 0)
                        {
                            var dirBase = viajeDistribucion.Detalles.First().ReferenciaGeografica;

                            var coche = viajeDistribucion.Vehiculo;
                            var velocidadPromedio = coche != null && coche.VelocidadPromedio > 0
                                ? coche.VelocidadPromedio
                                : coche != null && coche.TipoCoche.VelocidadPromedio > 0 ? coche.TipoCoche.VelocidadPromedio : 20;

                            var hora = viajeDistribucion.Inicio;
                            foreach (var detalle in viajeDistribucion.Detalles)
                            {
                                var distancia = GeocoderHelper.CalcularDistacia(dirBase.Latitude, dirBase.Longitude, detalle.ReferenciaGeografica.Latitude,
                                    detalle.ReferenciaGeografica.Longitude);
                                var horas = distancia/velocidadPromedio;
                                var demora = detalle.TipoServicio != null ? detalle.TipoServicio.Demora : 0;
                                detalle.Programado = hora.AddHours(horas).AddMinutes(demora);
                                detalle.ProgramadoHasta = detalle.Programado.AddMinutes(empresa.MarginMinutes);
                                dirBase = detalle.ReferenciaGeografica;
                                hora = detalle.Programado;
                            }
                        }

                        var maxDate = viajeDistribucion.Detalles.Max(d => d.Programado);
                        viajeDistribucion.Fin = maxDate;

                        var ultimo = viajeDistribucion.Detalles.Last(e => e.Linea != null);
                        ultimo.Programado = maxDate;
                        ultimo.ProgramadoHasta = maxDate.AddMinutes(empresa.MarginMinutes);
                        ultimo.Orden = viajeDistribucion.Detalles.Count - 1;

                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    STrace.Exception("ViajeDistribucionImport", ex, "transaction.Rollback()");
                    throw ex;
                }
            }
        }
        protected void ImportAxiodisF(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var clienteEntregas = cbClienteEntregas.Selected > 0
                                      ? DAOFactory.ClienteDAO.FindById(cbClienteEntregas.Selected)
                                      : null;
            var clienteSucursales = cbClienteSucursales.Selected > 0
                                        ? DAOFactory.ClienteDAO.FindById(cbClienteSucursales.Selected)
                                        : null;
            var vigencia = Convert.ToInt32(txtVigencia.Text.Trim());
            var tipoServicio = DAOFactory.TipoServicioCicloDAO.FindDefault(new[] { cbEmpresa.Selected },
                                                                           new[] { cbLinea.Selected });

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var stringFecha = row[GetColumnByValue(Fields.Fecha.Value)];
                        var fecha = new DateTime(Convert.ToInt32(stringFecha.Substring(0, 4)), 
                                                 Convert.ToInt32(stringFecha.Substring(4, 2)), 
                                                 Convert.ToInt32(stringFecha.Substring(6, 2)), 
                                                 6, 0, 0);

                        var patente = row[GetColumnByValue(Fields.Vehiculo.Value)];
                        var codigoPedido = row[GetColumnByValue(Fields.CodigoPedido.Value)];
                        var ruta = row[GetColumnByValue(Fields.Viaje.Value)];
                        var codigoViaje = string.Format("{0}|{1}", stringFecha.Substring(0, 8), ruta);
                        var latitud = ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "");
                        var longitud = ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "");
                        var codPuntoEntrega = row[GetColumnByValue(Fields.PuntoEntrega.Value)];
                        var nombre = row[GetColumnByValue(Fields.Nombre.Value)];
                        var dir = row[GetColumnByValue(Fields.Direccion.Value)];
                        
                        var cliente = clienteEntregas ?? clienteSucursales;

                        #endregion

                        if (list.Count == 0 ||
                            codigoViaje != list.Last().Codigo)
                        {
                            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, codigoViaje);
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoViaje == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            var vehiculo = DAOFactory.CocheDAO.GetByPatente(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected }, patente);
                            var chofer = vehiculo != null && !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                            viaje = new ViajeDistribucion
                            {
                                Empresa = empresa,
                                Linea = linea,
                                Vehiculo = vehiculo,
                                Empleado = chofer,
                                Codigo = codigoViaje,
                                Estado = ViajeDistribucion.Estados.Pendiente,
                                Inicio = fecha.ToDataBaseDateTime(),
                                Fin = fecha.ToDataBaseDateTime(),
                                NumeroViaje = 1,
                                Tipo = ViajeDistribucion.Tipos.Desordenado,
                                Alta = DateTime.UtcNow
                            };

                            #endregion

                            list.Add(viaje);
                        }
                        viaje.Fin = fecha.ToDataBaseDateTime();

                        if (viaje.Detalles.Count == 0)
                        {
                            //el primer elemento es la base
                            var origen = new EntregaDistribucion
                            {
                                Linea = linea,
                                Descripcion = linea.Descripcion,
                                Estado = EntregaDistribucion.Estados.Pendiente,
                                Orden = 0,
                                Programado = fecha.ToDataBaseDateTime(),
                                ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                Viaje = viaje
                            };
                            viaje.Detalles.Add(origen);
                        }

                        var direccion = GeocoderHelper.GetEsquinaMasCercana(latitud, longitud);

                        #region var puntoEntrega = [Find By Empresa, Linea, Codigo, Cliente].FirstOrDefault()

                        var puntoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected }, new[] { cliente.Id }, codPuntoEntrega);

                        #endregion

                        if (puntoEntrega == null)
                        {
                            #region var puntoDeInteres = new ReferenciaGeografica()

                            var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : cliente.Empresa == null ? null : empresa;
                            var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea == null ? null : cliente.Linea == null ? null : linea;

                            var puntoDeInteres = new ReferenciaGeografica
                            {
                                Codigo = codPuntoEntrega,
                                Descripcion = dir,
                                Empresa = empresaGeoRef,
                                Linea = lineaGeoRef,
                                EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                Vigencia =
                                    new Vigencia
                                    {
                                        Inicio = DateTime.UtcNow,
                                        Fin = fecha.AddHours(vigencia).ToDataBaseDateTime()
                                    },
                                Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                            };

                            #endregion

                            #region var posicion = new Direccion()

                            var posicion = new Direccion
                            {
                                Altura = direccion != null ? direccion.Altura : -1,
                                IdMapa = (short)(direccion != null ? direccion.IdMapaUrbano : -1),
                                Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                IdEntrecalle = -1,
                                Latitud = latitud,
                                Longitud = longitud,
                                Partido = direccion != null ? direccion.Partido : string.Empty,
                                Pais = string.Empty,
                                Calle = direccion != null ? direccion.Calle : string.Empty,
                                Descripcion =
                                    direccion != null
                                        ? direccion.Direccion
                                        : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                            longitud.ToString(CultureInfo.InvariantCulture)),
                                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                            };

                            #endregion

                            #region var poligono = new Poligono()

                            var poligono = new Poligono { Radio = 100, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                            poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

                            #endregion

                            puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);
                            AddReferenciasGeograficas(puntoDeInteres);

                            #region puntoEntrega = new PuntoEntrega()

                            puntoEntrega = new PuntoEntrega
                            {
                                Cliente = cliente,
                                Codigo = codPuntoEntrega,
                                Descripcion = dir,
                                Telefono = string.Empty,
                                Baja = false,
                                ReferenciaGeografica = puntoDeInteres,
                                Nomenclado = true,
                                DireccionNomenclada = string.Empty,
                                Nombre = nombre
                            };

                            #endregion

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }
                        else
                        {
                            if (puntoEntrega.ReferenciaGeografica.Latitude != latitud ||
                                puntoEntrega.ReferenciaGeografica.Longitude != longitud)
                            {
                                puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                                puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                                #region var posicion = new Direccion()

                                var posicion = new Direccion
                                {
                                    Altura = direccion != null ? direccion.Altura : -1,
                                    IdMapa = (short)(direccion != null ? direccion.IdMapaUrbano : -1),
                                    Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                    IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                    IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                    IdEntrecalle = -1,
                                    Latitud = latitud,
                                    Longitud = longitud,
                                    Partido = direccion != null ? direccion.Partido : string.Empty,
                                    Pais = string.Empty,
                                    Calle = direccion != null ? direccion.Calle : string.Empty,
                                    Descripcion =
                                        direccion != null
                                            ? direccion.Direccion
                                            : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                longitud.ToString(CultureInfo.InvariantCulture)),
                                    Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                                };

                                #endregion

                                #region var poligono = new Poligono()

                                var poligono = new Poligono { Radio = 100, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                                poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

                                #endregion

                                puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                            }

                            #region puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end

                            var end = fecha.AddHours(vigencia).ToDataBaseDateTime();
                            if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                            #endregion

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                            AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                            puntoEntrega.Nombre = nombre;

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                        {
                            Cliente = cliente,
                            PuntoEntrega = puntoEntrega,
                            Descripcion = codigoPedido.Truncate(128),
                            Estado = EntregaDistribucion.Estados.Pendiente,
                            Orden = viaje.Detalles.Count,
                            Programado = fecha.ToDataBaseDateTime(),
                            ProgramadoHasta = fecha.ToDataBaseDateTime().AddMinutes(empresa.MarginMinutes),
                            TipoServicio = tipoServicio,
                            Viaje = viaje
                        };

                        #endregion

                        viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        if (chkCalcularHoras.Checked &&
                            viajeDistribucion.Detalles.Count > 0)
                        {
                            var dirBase = viajeDistribucion.Detalles.First().ReferenciaGeografica;

                            var coche = viajeDistribucion.Vehiculo;
                            var velocidadPromedio = coche != null && coche.VelocidadPromedio > 0
                                ? coche.VelocidadPromedio
                                : coche != null && coche.TipoCoche.VelocidadPromedio > 0 ? coche.TipoCoche.VelocidadPromedio : 20;

                            var hora = viajeDistribucion.Inicio;
                            foreach (var detalle in viajeDistribucion.Detalles)
                            {
                                var distancia = GeocoderHelper.CalcularDistacia(dirBase.Latitude, dirBase.Longitude, detalle.ReferenciaGeografica.Latitude,
                                    detalle.ReferenciaGeografica.Longitude);
                                var horas = distancia / velocidadPromedio;
                                var demora = detalle.TipoServicio != null ? detalle.TipoServicio.Demora : 0;
                                detalle.Programado = hora.AddHours(horas).AddMinutes(demora);
                                detalle.ProgramadoHasta = detalle.Programado.AddMinutes(empresa.MarginMinutes);
                                dirBase = detalle.ReferenciaGeografica;
                                hora = detalle.Programado;
                            }
                        }

                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    STrace.Exception("ViajeDistribucionImport", ex, "transaction.Rollback()");

                    throw ex;
                }
            }
        }
        protected void ImportRL(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var codigoBaseOrigen = row[GetColumnByValue(Fields.CodigoBaseOrigen.Value)];
                        var codigoBaseDestino = row[GetColumnByValue(Fields.CodigoBaseDestino.Value)];
                        var interno = row[GetColumnByValue(Fields.Vehiculo.Value)];
                        var ruta = row[GetColumnByValue(Fields.Viaje.Value)];
                        var stringFechaSalida = row[GetColumnByValue(Fields.FechaSalida.Value)];
                        var stringFechaLlegada = row[GetColumnByValue(Fields.FechaLlegada.Value)];
                        var stringHoraSalida = row[GetColumnByValue(Fields.HoraSalida.Value)];
                        var stringHoraLlegada = row[GetColumnByValue(Fields.HoraLlegada.Value)];
                        var bultos = row[GetColumnByValue(Fields.Bultos.Value)];


                        #endregion

                        #region viaje = new ViajeDistribucion()

                        var vehiculo = DAOFactory.CocheDAO.GetByInterno(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected }, interno);

                        var baseOrigen = DAOFactory.LineaDAO.FindByCodigo(empresa.Id, codigoBaseOrigen);
                        var cliente = DAOFactory.ClienteDAO.FindByCode(new[] { cbEmpresa.Selected }, new[] { -1 }, "PP");

                        var destino = DAOFactory.PuntoEntregaDAO.FindByCode(new[] { empresa.Id }, new[] { -1 }, new[] { cliente.Id }, codigoBaseDestino);

                        var anioSalida = 2000 + Convert.ToInt32(stringFechaSalida.Substring(1, 2));
                        var mesSalida = Convert.ToInt32(stringFechaSalida.Substring(3, 2));
                        var diaSalida = Convert.ToInt32(stringFechaSalida.Substring(5, 2));

                        var anioLlegada = 2000 + Convert.ToInt32(stringFechaLlegada.Substring(1, 2));
                        var mesLlegada = Convert.ToInt32(stringFechaLlegada.Substring(3, 2));
                        var diaLlegada = Convert.ToInt32(stringFechaLlegada.Substring(5, 2));

                        var horaSalida = 0;
                        var minutosSalida = 0;
                        var horaLlegada = 0;
                        var minutosLlegada = 0;

                        if (stringHoraSalida.Length == 3)
                        {
                            horaSalida = Convert.ToInt32(stringHoraSalida.Substring(0, 1));
                            minutosSalida = Convert.ToInt32(stringHoraSalida.Substring(1, 2));
                        }
                        if (stringHoraSalida.Length == 4)
                        {
                            horaSalida = Convert.ToInt32(stringHoraSalida.Substring(0, 2));
                            minutosSalida = Convert.ToInt32(stringHoraSalida.Substring(2, 2));
                        }

                        if (stringHoraLlegada.Length == 3)
                        {
                            horaLlegada = Convert.ToInt32(stringHoraLlegada.Substring(0, 1));
                            minutosLlegada = Convert.ToInt32(stringHoraLlegada.Substring(1, 2));
                        }
                        if (stringHoraLlegada.Length == 4)
                        {
                            horaLlegada = Convert.ToInt32(stringHoraLlegada.Substring(0, 2));
                            minutosLlegada = Convert.ToInt32(stringHoraLlegada.Substring(2, 2));
                        }

                        var bultosPorViaje = Convert.ToInt32(bultos);

                        var salida = new DateTime(anioSalida, mesSalida, diaSalida, horaSalida, minutosSalida, 0);
                        var llegada = new DateTime(anioLlegada, mesLlegada, diaLlegada, horaLlegada, minutosLlegada, 0);

                        var viaje = new ViajeDistribucion
                        {
                            Empresa = empresa,
                            Linea = baseOrigen,
                            Vehiculo = vehiculo,
                            Codigo = ruta,
                            Estado = ViajeDistribucion.Estados.Pendiente,
                            Inicio = salida.ToDataBaseDateTime(),
                            Fin = llegada.ToDataBaseDateTime(),
                            NumeroViaje = 1,
                            Tipo = ViajeDistribucion.Tipos.Desordenado,
                            Alta = DateTime.UtcNow,
                            RegresoABase = true
                        };

                        #endregion

                        //el primer elemento es la base
                        var origen = new EntregaDistribucion
                        {
                            Linea = baseOrigen,
                            Descripcion = baseOrigen.Descripcion,
                            Estado = EntregaDistribucion.Estados.Pendiente,
                            Orden = 0,
                            Programado = salida.ToDataBaseDateTime(),
                            ProgramadoHasta = salida.ToDataBaseDateTime(),
                            Viaje = viaje
                        };
                        viaje.Detalles.Add(origen);

                        var fin = new EntregaDistribucion
                        {
                            PuntoEntrega = destino,
                            Descripcion = destino.Descripcion,
                            Estado = EntregaDistribucion.Estados.Pendiente,
                            Orden = 1,
                            Programado = llegada.ToDataBaseDateTime(),
                            ProgramadoHasta = llegada.ToDataBaseDateTime(),
                            Viaje = viaje,
                            Bultos = bultosPorViaje
                        };
                        viaje.Detalles.Add(fin);

                        list.Add(viaje);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                    }

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    STrace.Exception("ViajeDistribucionImportReginaldLee", ex, "transaction.Rollback()");

                    throw ex;
                }
            }
        }
        protected void ImportRoadshow(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var cliente = cbClienteRoadshow.Selected > 0 ? DAOFactory.ClienteDAO.FindById(cbClienteRoadshow.Selected) : null;
            var vigencia = Convert.ToInt32(txtVigenciaRoadshow.Text.Trim());

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var codigoRuta = row[GetColumnByValue(Fields.CodigoRuta.Value)];
                        var numeroViaje = Convert.ToInt32(row[GetColumnByValue(Fields.Viaje.Value)]);
                        var interno = row[GetColumnByValue(Fields.Vehiculo.Value)];
                        var fecha = DateTime.Parse(row[GetColumnByValue(Fields.Fecha.Value)]);
                        var secuencia = Convert.ToInt32(row[GetColumnByValue(Fields.Secuencia.Value)]);
                        var codigoPedido = row[GetColumnByValue(Fields.CodigoPedido.Value)];
                        var latitud = ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "");
                        var longitud = ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "");
                        var programado = DateTime.Parse(row[GetColumnByValue(Fields.Programada.Value)]);
                        fecha = fecha.AddHours(programado.Hour).AddMinutes(programado.Minute);

                        var codigoPuntoEntrega = row[GetColumnByValue(Fields.PuntoEntrega.Value)];
                        var nombre = row[GetColumnByValue(Fields.Nombre.Value)];

                        #endregion

                        if (list.Count == 0 ||
                            codigoRuta != list.Last().Codigo)
                        {
                            var inicioBusquedaViaje = DateTime.UtcNow;
                            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, codigoRuta);
                            var duracionBusquedaViaje = DateTime.UtcNow.Subtract(inicioBusquedaViaje);
                            STrace.Trace("ViajeDistribucionImport",
                                "Búsqueda del viaje " + codigoRuta + " demoró " + duracionBusquedaViaje.TotalSeconds + " segundos");
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoRuta == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            var vehiculo = DAOFactory.CocheDAO.GetByInterno(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, interno);
                            var chofer = vehiculo != null && !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                            viaje = new ViajeDistribucion
                                    {
                                        Empresa = empresa,
                                        Linea = linea,
                                        Vehiculo = vehiculo,
                                        Empleado = chofer,
                                        Codigo = codigoRuta,
                                        Estado = ViajeDistribucion.Estados.Pendiente,
                                        Inicio = fecha.ToDataBaseDateTime(),
                                        Fin = fecha.ToDataBaseDateTime(),
                                        NumeroViaje = Convert.ToInt32(numeroViaje),
                                        Alta = DateTime.UtcNow
                                    };

                            #endregion

                            list.Add(viaje);
                        }
                        viaje.Fin = fecha.ToDataBaseDateTime();


                        if (viaje.Detalles.Count == 0)
                        {
                            //el primer elemento es la base
                            var origen = new EntregaDistribucion
                                         {
                                             Linea = linea,
                                             Descripcion = linea.Descripcion,
                                             Estado = EntregaDistribucion.Estados.Pendiente,
                                             Orden = 0,
                                             Programado = fecha.ToDataBaseDateTime(),
                                             ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                             Viaje = viaje
                                         };
                            viaje.Detalles.Add(origen);
                        }

                        var direccion = GeocoderHelper.GetEsquinaMasCercana(latitud, longitud);

                        #region var puntoEntrega = [Find By Empresa, Linea, Codigo, Cliente].FirstOrDefault()

                        var puntoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {cliente.Id},
                            codigoPuntoEntrega);

                        #endregion

                        if (puntoEntrega == null)
                        {
                            #region var puntoDeInteres = new ReferenciaGeografica()

                            var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : cliente.Empresa == null ? null : empresa;
                            var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea == null ? null : cliente.Linea == null ? null : linea;

                            var puntoDeInteres = new ReferenciaGeografica
                                                 {
                                                     Codigo = codigoPuntoEntrega,
                                                     Descripcion = nombre,
                                                     Empresa = empresaGeoRef,
                                                     Linea = lineaGeoRef,
                                                     EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                                     EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                                     EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                                     InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                                     TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                                     Vigencia =
                                                         new Vigencia
                                                         {
                                                             Inicio = DateTime.UtcNow,
                                                             Fin = fecha.AddHours(vigencia).ToDataBaseDateTime()
                                                         },
                                                     Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                                 };

                            #endregion

                            #region var posicion = new Direccion()

                            var posicion = new Direccion
                                           {
                                               Altura = direccion != null ? direccion.Altura : -1,
                                               IdMapa = (short) (direccion != null ? direccion.IdMapaUrbano : -1),
                                               Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                               IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                               IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                               IdEntrecalle = -1,
                                               Latitud = latitud,
                                               Longitud = longitud,
                                               Partido = direccion != null ? direccion.Partido : string.Empty,
                                               Pais = string.Empty,
                                               Calle = direccion != null ? direccion.Calle : string.Empty,
                                               Descripcion =
                                                   direccion != null
                                                       ? direccion.Direccion
                                                       : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                           longitud.ToString(CultureInfo.InvariantCulture)),
                                               Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                           };

                            #endregion

                            #region var poligono = new Poligono()

                            var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                            poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                            #endregion

                            puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);
                            AddReferenciasGeograficas(puntoDeInteres);

                            #region puntoEntrega = new PuntoEntrega()

                            puntoEntrega = new PuntoEntrega
                                           {
                                               Cliente = cliente,
                                               Codigo = codigoPuntoEntrega,
                                               Descripcion = nombre,
                                               Telefono = string.Empty,
                                               Baja = false,
                                               ReferenciaGeografica = puntoDeInteres,
                                               Nomenclado = true,
                                               DireccionNomenclada = string.Empty,
                                               Nombre = nombre
                                           };

                            #endregion

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }
                        else
                        {
                            if (chkSobreescribir.Checked &&
                                (puntoEntrega.ReferenciaGeografica.Latitude != latitud || puntoEntrega.ReferenciaGeografica.Longitude != longitud))
                            {
                                puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                                puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                                #region var posicion = new Direccion()

                                var posicion = new Direccion
                                               {
                                                   Altura = direccion != null ? direccion.Altura : -1,
                                                   IdMapa = (short) (direccion != null ? direccion.IdMapaUrbano : -1),
                                                   Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                                   IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                                   IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                                   IdEntrecalle = -1,
                                                   Latitud = latitud,
                                                   Longitud = longitud,
                                                   Partido = direccion != null ? direccion.Partido : string.Empty,
                                                   Pais = string.Empty,
                                                   Calle = direccion != null ? direccion.Calle : string.Empty,
                                                   Descripcion =
                                                       direccion != null
                                                           ? direccion.Direccion
                                                           : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                               longitud.ToString(CultureInfo.InvariantCulture)),
                                                   Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                               };

                                #endregion

                                #region var poligono = new Poligono()

                                var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                                poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                                #endregion

                                puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);

                                puntoEntrega.Nombre = nombre;
                            }

                            #region puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end

                            var end = fecha.AddHours(vigencia).ToDataBaseDateTime();
                            if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                            #endregion

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                            AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                                      {
                                          Cliente = cliente,
                                          PuntoEntrega = puntoEntrega,
                                          Descripcion = codigoPedido,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = secuencia,
                                          Programado = fecha.ToDataBaseDateTime(),
                                          ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                          Viaje = viaje
                                      };

                        #endregion

                        viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex ;
                }
            }
        }
        protected void ImportRoadshowCsv(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var cliente = cbClienteRoadshow.Selected > 0 ? DAOFactory.ClienteDAO.FindById(cbClienteRoadshow.Selected) : null;
            var vigencia = Convert.ToInt32(txtVigenciaRoadshow.Text.Trim());

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var campos = row.Values.First().Replace("\"", string.Empty).Split('|');

                        var codigoRuta = ValidateEmpty(campos[0], "CODIGO_RUTA").Trim();
                        var numeroViaje = ValidateInt32(campos[1], "VIAJE");
                        var patente = ValidateEmpty(campos[2], "PATENTE").Trim();
                        var dia = ValidateInt32(campos[3].Substring(0, 2), "DIA");
                        var mes = ValidateInt32(campos[3].Substring(2, 2), "MES");
                        var anio = ValidateInt32(campos[3].Substring(4, 2), "AÑO") + 2000;
                        var secuencia = ValidateInt32(campos[4], "SECUENCIA");
                        var codigoPedido = ValidateEmpty(campos[5], "CODIGO_PEDIDO").Trim();
                        var horario = ValidateEmpty(campos[6], "HORARIO").Trim();

                        if (horario.Length == 3) horario = "0" + horario;
                        else if (horario.Length != 4) ThrowInvalidValue("HORARIO");

                        var hora = ValidateInt32(horario.Substring(0, 2), "HORA");
                        var min = ValidateInt32(horario.Substring(2, 2), "MINUTOS");
                        var codigoPuntoEntrega = ValidateEmpty(campos[7], "CODIGO_PUNTO_ENTREGA").Trim();
                        var nombre = campos[8].Trim();
                        var latitud = ValidateDouble(campos[9], "LATITUD");
                        var longitud = ValidateDouble(campos[10], "LONGITUD");

                        var fecha = new DateTime(anio, mes, dia, hora, min, 0);

                        TipoServicioCiclo tipoServicio = null;
                        var tipoServ = DAOFactory.TipoServicioCicloDAO.FindDefault(new[] {linea.Empresa.Id}, new[] {linea.Id});
                        if (tipoServ != null &&
                            tipoServ.Id > 0)
                            tipoServicio = tipoServ;

                        #endregion

                        if (list.Count == 0 ||
                            codigoRuta != list.Last().Codigo)
                        {
                            var inicioBusquedaViaje = DateTime.UtcNow;
                            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, codigoRuta);
                            var duracionBusquedaViaje = DateTime.UtcNow.Subtract(inicioBusquedaViaje);
                            STrace.Trace("ViajeDistribucionImport",
                                "Búsqueda del viaje " + codigoRuta + " demoró " + duracionBusquedaViaje.TotalSeconds + " segundos");
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoRuta == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            var vehiculo = DAOFactory.CocheDAO.GetByPatente(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, patente);
                            var chofer = vehiculo != null && !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                            viaje = new ViajeDistribucion
                                    {
                                        Empresa = empresa,
                                        Linea = linea,
                                        Vehiculo = vehiculo,
                                        Empleado = chofer,
                                        Codigo = codigoRuta,
                                        Estado = ViajeDistribucion.Estados.Pendiente,
                                        Inicio = fecha.ToDataBaseDateTime(),
                                        Fin = fecha.ToDataBaseDateTime(),
                                        NumeroViaje = Convert.ToInt32(numeroViaje),
                                        Alta = DateTime.UtcNow
                                    };

                            #endregion

                            list.Add(viaje);
                        }
                        viaje.Fin = fecha.ToDataBaseDateTime();


                        if (viaje.Detalles.Count == 0)
                        {
                            //el primer elemento es la base
                            var origen = new EntregaDistribucion
                                         {
                                             Linea = linea,
                                             Descripcion = linea.Descripcion,
                                             Estado = EntregaDistribucion.Estados.Pendiente,
                                             Orden = 0,
                                             Programado = fecha.ToDataBaseDateTime(),
                                             ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                             Viaje = viaje
                                         };
                            viaje.Detalles.Add(origen);
                        }

                        var direccion = GeocoderHelper.GetEsquinaMasCercana(latitud, longitud);

                        #region var puntoEntrega = [Find By Empresa, Linea, Codigo, Cliente].FirstOrDefault()

                        var puntoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {cliente.Id},
                            codigoPuntoEntrega);

                        #endregion

                        if (puntoEntrega == null)
                        {
                            #region var puntoDeInteres = new ReferenciaGeografica()

                            var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : cliente.Empresa == null ? null : empresa;
                            var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea == null ? null : cliente.Linea == null ? null : linea;

                            var puntoDeInteres = new ReferenciaGeografica
                                                 {
                                                     Codigo = codigoPuntoEntrega,
                                                     Descripcion = nombre,
                                                     Empresa = empresaGeoRef,
                                                     Linea = lineaGeoRef,
                                                     EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                                     EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                                     EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                                     InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                                     TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                                     Vigencia =
                                                         new Vigencia
                                                         {
                                                             Inicio = DateTime.UtcNow,
                                                             Fin = fecha.AddHours(vigencia).ToDataBaseDateTime()
                                                         },
                                                     Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                                 };

                            #endregion

                            #region var posicion = new Direccion()

                            var posicion = new Direccion
                                           {
                                               Altura = direccion != null ? direccion.Altura : -1,
                                               IdMapa = (short) (direccion != null ? direccion.IdMapaUrbano : -1),
                                               Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                               IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                               IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                               IdEntrecalle = -1,
                                               Latitud = latitud,
                                               Longitud = longitud,
                                               Partido = direccion != null ? direccion.Partido : string.Empty,
                                               Pais = string.Empty,
                                               Calle = direccion != null ? direccion.Calle : string.Empty,
                                               Descripcion =
                                                   direccion != null
                                                       ? direccion.Direccion
                                                       : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                           longitud.ToString(CultureInfo.InvariantCulture)),
                                               Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                           };

                            #endregion

                            #region var poligono = new Poligono()

                            var poligono = new Poligono {Radio = 50, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                            poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                            #endregion

                            puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);
                            AddReferenciasGeograficas(puntoDeInteres);

                            #region puntoEntrega = new PuntoEntrega()

                            puntoEntrega = new PuntoEntrega
                                           {
                                               Cliente = cliente,
                                               Codigo = codigoPuntoEntrega,
                                               Descripcion = nombre,
                                               Telefono = string.Empty,
                                               Baja = false,
                                               ReferenciaGeografica = puntoDeInteres,
                                               Nomenclado = true,
                                               DireccionNomenclada = string.Empty,
                                               Nombre = nombre
                                           };

                            #endregion

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }
                        else
                        {
                            if (chkSobreescribir.Checked &&
                                (puntoEntrega.ReferenciaGeografica.Latitude != latitud || puntoEntrega.ReferenciaGeografica.Longitude != longitud))
                            {
                                puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                                puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                                #region var posicion = new Direccion()

                                var posicion = new Direccion
                                               {
                                                   Altura = direccion != null ? direccion.Altura : -1,
                                                   IdMapa = (short) (direccion != null ? direccion.IdMapaUrbano : -1),
                                                   Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                                   IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                                   IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                                   IdEntrecalle = -1,
                                                   Latitud = latitud,
                                                   Longitud = longitud,
                                                   Partido = direccion != null ? direccion.Partido : string.Empty,
                                                   Pais = string.Empty,
                                                   Calle = direccion != null ? direccion.Calle : string.Empty,
                                                   Descripcion =
                                                       direccion != null
                                                           ? direccion.Direccion
                                                           : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                               longitud.ToString(CultureInfo.InvariantCulture)),
                                                   Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                               };

                                #endregion

                                #region var poligono = new Poligono()

                                var poligono = new Poligono {Radio = 50, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                                poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                                #endregion

                                puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);

                                puntoEntrega.Nombre = nombre;
                            }

                            #region puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end

                            var end = fecha.AddHours(vigencia).ToDataBaseDateTime();
                            if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                            #endregion

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                            AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                                      {
                                          Cliente = cliente,
                                          PuntoEntrega = puntoEntrega,
                                          Descripcion = codigoPedido,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = secuencia,
                                          Programado = fecha.ToDataBaseDateTime(),
                                          ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                          TipoServicio = tipoServicio,
                                          Viaje = viaje
                                      };

                        #endregion

                        viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private string[] parseRoadshowCsv2Row(ImportRow row)
        {
            return row.Values.First().Replace("\"", string.Empty).Split('|');
        }
        private void PreBufferRows(IEnumerable<ImportRow> rows)
        {
            var lastPatente = string.Empty;
            var lastCodRuta = string.Empty;
            var lastCodPunto = string.Empty;

            var patentesStrList = new List<string>();
            var codrutaStrList = new List<string>();
            var codPuntoStrList = new List<string>();

            foreach(var row in rows)
            {
                var campos = parseRoadshowCsv2Row(row);                

                #region Buffer Coches

                var patente = ValidateEmpty(campos[2], "PATENTE").Trim();

                if (patente != lastPatente)
                {
                    if (!patentesStrList.Contains(patente))
                        patentesStrList.Add(patente);
                    lastPatente = patente;
                }                
                #endregion

                #region Buffer Viajes

                var codigoRuta = ValidateEmpty(campos[0], "CODIGO_RUTA").Trim();
                var dia = ValidateInt32(campos[3].Substring(0, 2), "DIA");
                var mes = ValidateInt32(campos[3].Substring(2, 2), "MES");
                var anio = ValidateInt32(campos[3].Substring(4, 2), "AÑO") + 2000;
                var horario = ValidateEmpty(campos[6], "HORARIO").Trim();
                if (horario.Length == 3) horario = "0" + horario;
                else if (horario.Length != 4) ThrowInvalidValue("HORARIO");
                var hora = ValidateInt32(horario.Substring(0, 2), "HORA");
                var min = ValidateInt32(horario.Substring(2, 2), "MINUTOS");
                var fecha = new DateTime(anio, mes, dia, hora, min, 0);
                codigoRuta = fecha.ToString("ddMMyy") + codigoRuta;

                if (lastCodRuta != codigoRuta)
                {
                    if (!codrutaStrList.Contains(codigoRuta))
                        codrutaStrList.Add(codigoRuta);

                    lastCodRuta = codigoRuta;
                }

                #endregion                

                #region Buffer PuntoEntrega

                var codigoPuntoEntrega = ValidateEmpty(campos[7], "CODIGO_PUNTO_ENTREGA").Trim();

                if (lastCodPunto != codigoPuntoEntrega)
                {
                    if (!codPuntoStrList.Contains(codigoPuntoEntrega)) 
                        codPuntoStrList.Add(codigoPuntoEntrega);

                    lastCodPunto = codigoPuntoEntrega;
                }

                #endregion
            }

            #region fetch

            const int batchSize = 1000;

            if (patentesStrList.Any())
            {
                foreach(var l in IEnumerableExtensions.InSetsOf(patentesStrList, batchSize))
                {
                    var coches = DAOFactory.CocheDAO.GetByPatentes(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, l);
                    if (coches != null && coches.Any())
                    {
                        _cochesBuffer.AddRange(coches);
                    }
                }

            }

            if (codPuntoStrList.Any())
            {
                foreach (var l in IEnumerableExtensions.InSetsOf(codPuntoStrList, batchSize))
                {
                    var puntos = DAOFactory.PuntoEntregaDAO.FindByCodes(new[] {cbEmpresa.Selected},
                                                                        new[] {cbLinea.Selected},
                                                                        new[] {cbClienteRoadshow.Selected},
                                                                        l);
                    if (puntos != null && puntos.Any())
                    {
                        _puntosBuffer.AddRange(puntos);
                    }
                }
            }

            if (codrutaStrList.Any())
            {
                foreach (var l in IEnumerableExtensions.InSetsOf(codrutaStrList, batchSize))
                {
                    var viajes = DAOFactory.ViajeDistribucionDAO.FindByCodigos(new[] {cbEmpresa.Selected},
                                                                               new[] {cbLinea.Selected},
                                                                               l);
                    if (viajes != null && viajes.Any())
                    {
                        _viajesBuffer.AddRange(viajes);
                    }
                }
            }

            #endregion
        }

        protected void ImportRoadshowCsv2(List<ImportRow> rows)
        {
            var te = new TimeElapsed();
            PreBufferRows(rows);
            STrace.Trace("ImportRoadshowCsv2", "preBufferRows demoró " + te.getTimeElapsed().TotalSeconds + " segundos.");
            
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var cliente = cbClienteRoadshow.Selected > 0 ? DAOFactory.ClienteDAO.FindById(cbClienteRoadshow.Selected) : null;
            var vigencia = Convert.ToInt32((string) txtVigenciaRoadshow.Text.Trim());            

            TipoServicioCiclo tipoServicio = null;
            var tipoServ = DAOFactory.TipoServicioCicloDAO.FindDefault(new[] { linea.Empresa.Id },
                                                                       new[] { linea.Id });
            if (tipoServ != null && tipoServ.Id > 0)
                tipoServicio = tipoServ;

            te.Restart();
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var campos = parseRoadshowCsv2Row(row);

                        var esBase = campos[1].Trim().Equals(string.Empty) && campos[9].Trim().Equals(string.Empty) && campos[10].Trim().Equals(string.Empty);

                        var codigoRuta = ValidateEmpty(campos[0], "CODIGO_RUTA").Trim();
                        var numeroViaje = esBase ? list.Last().NumeroViaje : ValidateInt32(campos[1], "VIAJE");
                        var patente = ValidateEmpty(campos[2], "PATENTE").Trim();
                        var dia = ValidateInt32(campos[3].Substring(0, 2), "DIA");
                        var mes = ValidateInt32(campos[3].Substring(2, 2), "MES");
                        var anio = ValidateInt32(campos[3].Substring(4, 2), "AÑO") + 2000;
                        //var secuencia = ValidateInt32(campos[4], "SECUENCIA");
                        //var codigoPedido = ValidateEmpty(campos[5], "CODIGO_PEDIDO").Trim();
                        var horario = ValidateEmpty(campos[6], "HORARIO").Trim();

                        if (horario.Length == 3) horario = "0" + horario;
                        else if (horario.Length != 4) ThrowInvalidValue("HORARIO");

                        var hora = ValidateInt32(horario.Substring(0, 2), "HORA");
                        var min = ValidateInt32(horario.Substring(2, 2), "MINUTOS");
                        var codigoPuntoEntrega = ValidateEmpty(campos[7], "CODIGO_PUNTO_ENTREGA").Trim();
                        var nombre = campos[8].Trim() != string.Empty ? campos[8].Trim() : "SIN NOMBRE";
                        var latitud = esBase ? 0.0 : ValidateDouble(campos[9], "LATITUD");
                        var longitud = esBase ? 0.0 : ValidateDouble(campos[10], "LONGITUD");
                        var km = campos.Length == 12 ? ValidateDouble(campos[11], "KM") : 0.0;

                        var fecha = new DateTime(anio, mes, dia, hora, min, 0);
                        codigoRuta = fecha.ToString("ddMMyy") + codigoRuta;

                        #endregion

                        if (list.Count == 0 ||
                            codigoRuta != list.Last().Codigo)
                        {
                            var byCode = _viajesBuffer.SingleOrDefault(v => v.Codigo == codigoRuta);
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoRuta == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            var vehiculo = _cochesBuffer.SingleOrDefault(c => c.Patente == patente);
                            var chofer = vehiculo != null && !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                            var cc = vehiculo != null ? vehiculo.CentroDeCostos : null;
                            var scc = vehiculo != null ? vehiculo.SubCentroDeCostos : null;
                            viaje = new ViajeDistribucion
                                    {
                                        Empresa = empresa,
                                        Linea = linea,
                                        Vehiculo = vehiculo,
                                        Empleado = chofer,
                                        CentroDeCostos = cc,
                                        SubCentroDeCostos = scc,
                                        Codigo = codigoRuta,
                                        Estado = ViajeDistribucion.Estados.Pendiente,
                                        Inicio = fecha.ToDataBaseDateTime(),
                                        Fin = fecha.ToDataBaseDateTime(),
                                        NumeroViaje = Convert.ToInt32(numeroViaje),
                                        Tipo = ViajeDistribucion.Tipos.Desordenado,
                                        RegresoABase = true,
                                        Alta = DateTime.UtcNow
                                    };

                            #endregion

                            list.Add(viaje);
                        }
                        viaje.Fin = fecha.ToDataBaseDateTime();

                        if (esBase)
                        {
                            var llegada = new EntregaDistribucion
                                          {
                                              Linea = linea,
                                              Descripcion = linea.Descripcion,
                                              Estado = EntregaDistribucion.Estados.Pendiente,
                                              Orden = viaje.Detalles.Count,
                                              Programado = fecha.ToDataBaseDateTime(),
                                              ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                              Viaje = viaje,
                                              KmCalculado = km
                                          };
                            viaje.Detalles.Add(llegada);

                            var isNew = !DAOFactory.Session.Contains(viaje);
                            DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
                            if (isNew) _viajesBuffer.Add(viaje);
                            continue;
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
                                             Programado = fecha.ToDataBaseDateTime(),
                                             ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                             Viaje = viaje
                                         };
                            viaje.Detalles.Add(origen);
                        }

                        //var direccion = GeocoderHelper.GetEsquinaMasCercana(latitud, longitud);
                        if (viaje.Detalles.Any(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codigoPuntoEntrega))
                            continue;

                        #region var puntoEntrega = [Find By Empresa, Linea, Codigo, Cliente].FirstOrDefault()

                        var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codigoPuntoEntrega);

                        #endregion

                        if (puntoEntrega == null)
                        {
                            #region var puntoDeInteres = new ReferenciaGeografica()

                            var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : cliente.Empresa == null ? null : empresa;
                            var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea == null ? null : cliente.Linea == null ? null : linea;

                            var puntoDeInteres = new ReferenciaGeografica
                                                 {
                                                     Codigo = codigoPuntoEntrega,
                                                     Descripcion = nombre,
                                                     Empresa = empresaGeoRef,
                                                     Linea = lineaGeoRef,
                                                     EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                                     EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                                     EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                                     InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                                     TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                                     Vigencia =
                                                         new Vigencia
                                                         {
                                                             Inicio = DateTime.UtcNow,
                                                             Fin = fecha.AddHours(vigencia).ToDataBaseDateTime()
                                                         },
                                                     Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                                 };

                            #endregion

                            #region var posicion = new Direccion()

                            var posicion = new Direccion
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
                                               Descripcion =
                                                   string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                       longitud.ToString(CultureInfo.InvariantCulture)),
                                               Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                           };

                            #endregion

                            #region var poligono = new Poligono()

                            var poligono = new Poligono {Radio = 50, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                            poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                            #endregion

                            puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);
                            AddReferenciasGeograficas(puntoDeInteres);

                            #region puntoEntrega = new PuntoEntrega()

                            puntoEntrega = new PuntoEntrega
                                           {
                                               Cliente = cliente,
                                               Codigo = codigoPuntoEntrega,
                                               Descripcion = nombre,
                                               Telefono = string.Empty,
                                               Baja = false,
                                               ReferenciaGeografica = puntoDeInteres,
                                               Nomenclado = true,
                                               DireccionNomenclada = string.Empty,
                                               Nombre = nombre
                                           };

                            #endregion

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }
                        else
                        {
                            if (!puntoEntrega.ReferenciaGeografica.IgnoraLogiclink &&
                                chkSobreescribir.Checked &&
                                (puntoEntrega.ReferenciaGeografica.Latitude != latitud || puntoEntrega.ReferenciaGeografica.Longitude != longitud))
                            {
                                puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                                puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                                #region var posicion = new Direccion()

                                var posicion = new Direccion
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
                                                   Descripcion =
                                                       string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                           longitud.ToString(CultureInfo.InvariantCulture)),
                                                   Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                               };

                                #endregion

                                #region var poligono = new Poligono()

                                var poligono = new Poligono {Radio = 50, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                                poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                                #endregion

                                puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);

                                puntoEntrega.Nombre = nombre;
                            }

                            #region puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end

                            var end = fecha.AddHours(vigencia).ToDataBaseDateTime();
                            if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                            #endregion

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                            AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                                      {
                                          Cliente = cliente,
                                          PuntoEntrega = puntoEntrega,
                                          Descripcion = codigoPuntoEntrega,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = viaje.Detalles.Count,
                                          Programado = fecha.ToDataBaseDateTime(),
                                          ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                          TipoServicio = tipoServicio,
                                          Viaje = viaje,
                                          KmCalculado = km
                                      };

                        #endregion

                        viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        if (viajeDistribucion.Detalles.Last().Linea != null) continue;
                        //el ultimo elemento es la base
                        var llegada = new EntregaDistribucion
                                      {
                                          Linea = linea,
                                          Descripcion = linea.Descripcion,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = viajeDistribucion.Detalles.Count,
                                          Programado = viajeDistribucion.Detalles.Last().Programado,
                                          ProgramadoHasta = viajeDistribucion.Detalles.Last().ProgramadoHasta,
                                          Viaje = viajeDistribucion
                                      };
                        viajeDistribucion.Detalles.Add(llegada);

                        var isNew = !DAOFactory.Session.Contains(viajeDistribucion);
                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                        if (isNew) _viajesBuffer.Add(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            STrace.Trace("ImportRoadshowCsv2", "la transacción duró " + te.getTimeElapsed().TotalSeconds + " segundos.");
        }

        private void PreBufferRowsByInterno(IEnumerable<ImportRow> rows)
        {
            var lastInterno = string.Empty;
            var lastCodRuta = string.Empty;
            var lastCodPunto = string.Empty;

            var internosStrList = new List<string>();
            var codrutaStrList = new List<string>();
            var codPuntoStrList = new List<string>();

            foreach (var row in rows)
            {
                var campos = parseRoadshowCsv2Row(row);

                #region Buffer Coches

                var interno = ValidateEmpty(campos[2], "INTERNO").Trim();

                if (interno != lastInterno)
                {
                    if (!internosStrList.Contains(interno))
                        internosStrList.Add(interno);
                    lastInterno = interno;
                }
                #endregion

                #region Buffer Viajes

                var codigoRuta = ValidateEmpty(campos[0], "CODIGO_RUTA").Trim();
                var dia = ValidateInt32(campos[3].Substring(0, 2), "DIA");
                var mes = ValidateInt32(campos[3].Substring(2, 2), "MES");
                var anio = ValidateInt32(campos[3].Substring(4, 2), "AÑO") + 2000;
                var horario = ValidateEmpty(campos[6], "HORARIO").Trim();
                if (horario.Length == 3) horario = "0" + horario;
                else if (horario.Length != 4) ThrowInvalidValue("HORARIO");
                var hora = ValidateInt32(horario.Substring(0, 2), "HORA");
                var min = ValidateInt32(horario.Substring(2, 2), "MINUTOS");
                var fecha = new DateTime(anio, mes, dia, hora, min, 0);
                codigoRuta = fecha.ToString("ddMMyy") + codigoRuta;

                if (lastCodRuta != codigoRuta)
                {
                    if (!codrutaStrList.Contains(codigoRuta))
                        codrutaStrList.Add(codigoRuta);

                    lastCodRuta = codigoRuta;
                }

                #endregion

                #region Buffer PuntoEntrega

                var codigoPuntoEntrega = ValidateEmpty(campos[7], "CODIGO_PUNTO_ENTREGA").Trim();

                if (lastCodPunto != codigoPuntoEntrega)
                {
                    if (!codPuntoStrList.Contains(codigoPuntoEntrega))
                        codPuntoStrList.Add(codigoPuntoEntrega);

                    lastCodPunto = codigoPuntoEntrega;
                }

                #endregion
            }

            #region fetch

            const int batchSize = 1000;

            if (internosStrList.Any())
            {
                foreach (var l in IEnumerableExtensions.InSetsOf(internosStrList, batchSize))
                {
                    var coches = DAOFactory.CocheDAO.GetByInternos(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected }, l);
                    if (coches != null && coches.Any())
                    {
                        _cochesBuffer.AddRange(coches);
                    }
                }
            }

            if (codPuntoStrList.Any())
            {
                foreach (var l in IEnumerableExtensions.InSetsOf(codPuntoStrList, batchSize))
                {
                    var puntos = DAOFactory.PuntoEntregaDAO.FindByCodes(new[] { cbEmpresa.Selected },
                                                                        new[] { cbLinea.Selected },
                                                                        new[] { cbClienteRoadshow.Selected },
                                                                        l);
                    if (puntos != null && puntos.Any())
                    {
                        _puntosBuffer.AddRange(puntos);
                    }
                }
            }

            if (codrutaStrList.Any())
            {
                foreach (var l in IEnumerableExtensions.InSetsOf(codrutaStrList, batchSize))
                {
                    var viajes = DAOFactory.ViajeDistribucionDAO.FindByCodigos(new[] { cbEmpresa.Selected },
                                                                               new[] { cbLinea.Selected },
                                                                               l);
                    if (viajes != null && viajes.Any())
                    {
                        _viajesBuffer.AddRange(viajes);
                    }
                }
            }

            #endregion
        }
        protected void ImportRoadshowCsv2ByInterno(List<ImportRow> rows)
        {
            var te = new TimeElapsed();
            PreBufferRowsByInterno(rows);
            STrace.Trace("ImportRoadshowCsv2", "preBufferRows demoró " + te.getTimeElapsed().TotalSeconds + " segundos.");

            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var cliente = cbClienteRoadshow.Selected > 0 ? DAOFactory.ClienteDAO.FindById(cbClienteRoadshow.Selected) : null;
            var vigencia = Convert.ToInt32((string)txtVigenciaRoadshow.Text.Trim());

            TipoServicioCiclo tipoServicio = null;
            var tipoServ = DAOFactory.TipoServicioCicloDAO.FindDefault(new[] { linea.Empresa.Id },
                                                                       new[] { linea.Id });
            if (tipoServ != null && tipoServ.Id > 0)
                tipoServicio = tipoServ;

            te.Restart();
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var campos = parseRoadshowCsv2Row(row);

                        var esBase = campos[1].Trim().Equals(string.Empty) && campos[9].Trim().Equals(string.Empty) && campos[10].Trim().Equals(string.Empty);

                        var codigoRuta = ValidateEmpty(campos[0], "CODIGO_RUTA").Trim();
                        var numeroViaje = esBase ? list.Last().NumeroViaje : ValidateInt32(campos[1], "VIAJE");
                        var interno = ValidateEmpty(campos[2], "INTERNO").Trim();
                        var dia = ValidateInt32(campos[3].Substring(0, 2), "DIA");
                        var mes = ValidateInt32(campos[3].Substring(2, 2), "MES");
                        var anio = ValidateInt32(campos[3].Substring(4, 2), "AÑO") + 2000;
                        //var secuencia = ValidateInt32(campos[4], "SECUENCIA");
                        //var codigoPedido = ValidateEmpty(campos[5], "CODIGO_PEDIDO").Trim();
                        var horario = ValidateEmpty(campos[6], "HORARIO").Trim();

                        if (horario.Length == 3) horario = "0" + horario;
                        else if (horario.Length != 4) ThrowInvalidValue("HORARIO");

                        var hora = ValidateInt32(horario.Substring(0, 2), "HORA");
                        var min = ValidateInt32(horario.Substring(2, 2), "MINUTOS");
                        var codigoPuntoEntrega = ValidateEmpty(campos[7], "CODIGO_PUNTO_ENTREGA").Trim();
                        var nombre = campos[8].Trim() != string.Empty ? campos[8].Trim() : "SIN NOMBRE";
                        var latitud = esBase ? 0.0 : ValidateDouble(campos[9], "LATITUD");
                        var longitud = esBase ? 0.0 : ValidateDouble(campos[10], "LONGITUD");
                        var km = campos.Length == 12 ? ValidateDouble(campos[11], "KM") : 0.0;

                        var fecha = new DateTime(anio, mes, dia, hora, min, 0);
                        codigoRuta = fecha.ToString("ddMMyy") + codigoRuta;

                        #endregion

                        if (list.Count == 0 ||
                            codigoRuta != list.Last().Codigo)
                        {
                            var byCode = _viajesBuffer.SingleOrDefault(v => v.Codigo == codigoRuta);
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoRuta == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            var vehiculo = _cochesBuffer.SingleOrDefault(c => c.Interno == interno);
                            var chofer = vehiculo != null && !vehiculo.IdentificaChoferes ? vehiculo.Chofer : null;
                            var cc = vehiculo != null ? vehiculo.CentroDeCostos : null;
                            var scc = vehiculo != null ? vehiculo.SubCentroDeCostos : null;
                            viaje = new ViajeDistribucion
                            {
                                Empresa = empresa,
                                Linea = linea,
                                Vehiculo = vehiculo,
                                Empleado = chofer,
                                CentroDeCostos = cc,
                                SubCentroDeCostos = scc,
                                Codigo = codigoRuta,
                                Estado = ViajeDistribucion.Estados.Pendiente,
                                Inicio = fecha.ToDataBaseDateTime(),
                                Fin = fecha.ToDataBaseDateTime(),
                                NumeroViaje = Convert.ToInt32(numeroViaje),
                                Tipo = ViajeDistribucion.Tipos.Desordenado,
                                RegresoABase = true,
                                Alta = DateTime.UtcNow
                            };

                            #endregion

                            list.Add(viaje);
                        }
                        viaje.Fin = fecha.ToDataBaseDateTime();

                        if (esBase)
                        {
                            var llegada = new EntregaDistribucion
                            {
                                Linea = linea,
                                Descripcion = linea.Descripcion,
                                Estado = EntregaDistribucion.Estados.Pendiente,
                                Orden = viaje.Detalles.Count,
                                Programado = fecha.ToDataBaseDateTime(),
                                ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                Viaje = viaje,
                                KmCalculado = km
                            };
                            viaje.Detalles.Add(llegada);

                            var isNew = !DAOFactory.Session.Contains(viaje);
                            DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
                            if (isNew) _viajesBuffer.Add(viaje);
                            continue;
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
                                Programado = fecha.ToDataBaseDateTime(),
                                ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                Viaje = viaje
                            };
                            viaje.Detalles.Add(origen);
                        }

                        //var direccion = GeocoderHelper.GetEsquinaMasCercana(latitud, longitud);
                        if (viaje.Detalles.Any(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codigoPuntoEntrega))
                            continue;

                        #region var puntoEntrega = [Find By Empresa, Linea, Codigo, Cliente].FirstOrDefault()

                        var puntoEntrega = _puntosBuffer.SingleOrDefault(p => p.Codigo == codigoPuntoEntrega);

                        #endregion

                        if (puntoEntrega == null)
                        {
                            #region var puntoDeInteres = new ReferenciaGeografica()

                            var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : cliente.Empresa == null ? null : empresa;
                            var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea == null ? null : cliente.Linea == null ? null : linea;

                            var puntoDeInteres = new ReferenciaGeografica
                            {
                                Codigo = codigoPuntoEntrega,
                                Descripcion = nombre,
                                Empresa = empresaGeoRef,
                                Linea = lineaGeoRef,
                                EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                Vigencia =
                                    new Vigencia
                                    {
                                        Inicio = DateTime.UtcNow,
                                        Fin = fecha.AddHours(vigencia).ToDataBaseDateTime()
                                    },
                                Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                            };

                            #endregion

                            #region var posicion = new Direccion()

                            var posicion = new Direccion
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
                                Descripcion =
                                    string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                        longitud.ToString(CultureInfo.InvariantCulture)),
                                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                            };

                            #endregion

                            #region var poligono = new Poligono()

                            var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                            poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

                            #endregion

                            puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);
                            AddReferenciasGeograficas(puntoDeInteres);

                            #region puntoEntrega = new PuntoEntrega()

                            puntoEntrega = new PuntoEntrega
                            {
                                Cliente = cliente,
                                Codigo = codigoPuntoEntrega,
                                Descripcion = nombre,
                                Telefono = string.Empty,
                                Baja = false,
                                ReferenciaGeografica = puntoDeInteres,
                                Nomenclado = true,
                                DireccionNomenclada = string.Empty,
                                Nombre = nombre
                            };

                            #endregion

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }
                        else
                        {
                            if (!puntoEntrega.ReferenciaGeografica.IgnoraLogiclink &&
                                chkSobreescribir.Checked &&
                                (puntoEntrega.ReferenciaGeografica.Latitude != latitud || puntoEntrega.ReferenciaGeografica.Longitude != longitud))
                            {
                                puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                                puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                                #region var posicion = new Direccion()

                                var posicion = new Direccion
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
                                    Descripcion =
                                        string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                            longitud.ToString(CultureInfo.InvariantCulture)),
                                    Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                                };

                                #endregion

                                #region var poligono = new Poligono()

                                var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                                poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

                                #endregion

                                puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);

                                puntoEntrega.Nombre = nombre;
                            }

                            #region puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end

                            var end = fecha.AddHours(vigencia).ToDataBaseDateTime();
                            if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                            #endregion

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                            AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                        {
                            Cliente = cliente,
                            PuntoEntrega = puntoEntrega,
                            Descripcion = codigoPuntoEntrega,
                            Estado = EntregaDistribucion.Estados.Pendiente,
                            Orden = viaje.Detalles.Count,
                            Programado = fecha.ToDataBaseDateTime(),
                            ProgramadoHasta = fecha.ToDataBaseDateTime(),
                            TipoServicio = tipoServicio,
                            Viaje = viaje,
                            KmCalculado = km
                        };

                        #endregion

                        viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        if (viajeDistribucion.Detalles.Last().Linea != null) continue;
                        //el ultimo elemento es la base
                        var llegada = new EntregaDistribucion
                        {
                            Linea = linea,
                            Descripcion = linea.Descripcion,
                            Estado = EntregaDistribucion.Estados.Pendiente,
                            Orden = viajeDistribucion.Detalles.Count,
                            Programado = viajeDistribucion.Detalles.Last().Programado,
                            ProgramadoHasta = viajeDistribucion.Detalles.Last().ProgramadoHasta,
                            Viaje = viajeDistribucion
                        };
                        viajeDistribucion.Detalles.Add(llegada);

                        var isNew = !DAOFactory.Session.Contains(viajeDistribucion);
                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                        if (isNew) _viajesBuffer.Add(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            STrace.Trace("ImportRoadshowCsv2", "la transacción duró " + te.getTimeElapsed().TotalSeconds + " segundos.");
        }
        protected void ImportRoadnet(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var vigencia = Convert.ToInt32(txtVigenciaRoadnet.Text.Trim());

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var campos = row.Values.First();

                        var orden = campos.Substring(0, 10).Trim();
                        var codigoPuntoEntrega = campos.Substring(10, 10).Trim();
                        var fecha = campos.Substring(24, 8).Trim();
                        var ruta = campos.Substring(34, 10).Trim().Replace(',', '.');

                        var dia = Convert.ToInt32(fecha.Substring(2, 2));
                        var mes = Convert.ToInt32(fecha.Substring(4, 2));
                        var anio = Convert.ToInt32(fecha.Substring(6, 2)) + 2000;
                        var date = new DateTime(anio, mes, dia, 8, 0, 0);

                        var codigoRuta = fecha + "|" + ruta;
                        var nroViaje = ruta.Split('.')[1];

                        #endregion

                        if (list.Count == 0 ||
                            codigoRuta != list.Last().Codigo)
                        {
                            var inicioBusquedaViaje = DateTime.UtcNow;
                            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, codigoRuta);
                            var duracionBusquedaViaje = DateTime.UtcNow.Subtract(inicioBusquedaViaje);
                            STrace.Trace("ViajeDistribucionImport",
                                "Búsqueda del viaje " + codigoRuta + " demoró " + duracionBusquedaViaje.TotalSeconds + " segundos");
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoRuta == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            viaje = new ViajeDistribucion
                                    {
                                        Empresa = empresa,
                                        Linea = linea,
                                        Codigo = codigoRuta,
                                        Estado = ViajeDistribucion.Estados.Pendiente,
                                        Inicio = date.ToDataBaseDateTime(),
                                        Fin = date.AddHours(10).ToDataBaseDateTime(),
                                        NumeroViaje = Convert.ToInt32(nroViaje),
                                        Tipo = ViajeDistribucion.Tipos.Desordenado,
                                        RegresoABase = true,
                                        Alta = DateTime.UtcNow
                                    };

                            #endregion

                            list.Add(viaje);
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
                                             Programado = date.ToDataBaseDateTime(),
                                             ProgramadoHasta = date.ToDataBaseDateTime(),
                                             Viaje = viaje
                                         };
                            viaje.Detalles.Add(origen);
                        }

                        if (orden.Substring(0, 3) == "012") continue;

                        if (viaje.Detalles.Any(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codigoPuntoEntrega))
                            continue;

                        var inicioBusquedaPunto = DateTime.UtcNow;
                        var puntoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {-1},
                            codigoPuntoEntrega);
                        var duracionBusquedaPunto = DateTime.UtcNow.Subtract(inicioBusquedaPunto);
                        STrace.Trace("ViajeDistribucionImport",
                            "Búsqueda del punto " + codigoPuntoEntrega + " demoró " + duracionBusquedaPunto.TotalSeconds + " segundos");
                        if (puntoEntrega == null) continue;

                        var end = date.AddHours(vigencia).ToDataBaseDateTime();
                        if (puntoEntrega.ReferenciaGeografica.Vigencia == null)
                            puntoEntrega.ReferenciaGeografica.Vigencia = new Vigencia {Fin = end};

                        if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                            puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                        DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                        AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                                      {
                                          Cliente = puntoEntrega.Cliente,
                                          PuntoEntrega = puntoEntrega,
                                          Descripcion = orden,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = viaje.Detalles.Count,
                                          Programado = date.ToDataBaseDateTime(),
                                          ProgramadoHasta = date.ToDataBaseDateTime(),
                                          Viaje = viaje
                                      };

                        #endregion

                        viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        if (viajeDistribucion.Detalles.Last().Linea != null) continue;
                        //el ultimo elemento es la base
                        var llegada = new EntregaDistribucion
                                      {
                                          Linea = linea,
                                          Descripcion = linea.Descripcion,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = viajeDistribucion.Detalles.Count,
                                          Programado = viajeDistribucion.Detalles.Last().Programado.AddHours(10),
                                          ProgramadoHasta = viajeDistribucion.Detalles.Last().ProgramadoHasta.AddHours(10),
                                          Viaje = viajeDistribucion
                                      };
                        viajeDistribucion.Detalles.Add(llegada);

                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        protected void ImportExcelTemplate(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var cliente = cbCliente.Selected > 0 ? DAOFactory.ClienteDAO.FindById(cbCliente.Selected) : null;
            var vigencia = Convert.ToInt32(txtVigenciaExcelTemplate.Text.Trim());

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var list = new List<ViajeDistribucion>(rows.Count);

                    foreach (var row in rows)
                    {
                        #region Properties

                        var interno = row.GetString(GetColumnByValue(Fields.Vehiculo.Value)).Trim();
                        var codigoEntrega = row.GetString(GetColumnByValue(Fields.CodigoPedido.Value)).Trim();
                        var cuadrilla = row.GetString(GetColumnByValue(Fields.Cuadrilla.Value)).Trim();
                        var inicio = txtInicio.Text.Trim().Split(':');
                        var hora = Convert.ToInt32(inicio[0]);
                        var min = Convert.ToInt32(inicio[1]);

                        var fecha = DateTime.Today.AddHours(hora).AddMinutes(min);
                        var codigoRuta = fecha.ToString("yyyyMMdd") + "-" + cuadrilla + "-" + interno;
                        var vehiculo = DAOFactory.CocheDAO.GetByInterno(new[] {cbEmpresa.Selected}, new[] {-1}, interno);
                        var responsable = vehiculo.Chofer;
                        var linea = vehiculo.Linea;
                        var cc = vehiculo.CentroDeCostos;
                        var scc = vehiculo.SubCentroDeCostos;
                        var calle = ValidateEmpty(row.GetString(GetColumnByValue(Fields.Calle.Value)), "CALLE");
                        var altura = ValidateInt32(row.GetString(GetColumnByValue(Fields.Altura.Value)), "ALTURA");
                        var esquina = string.Empty;
                        var localidad = ValidateEmpty(row.GetString(GetColumnByValue(Fields.Localidad.Value)), "LOCALIDAD");
                        const int provincia = -1;

                        TipoServicioCiclo tipoServicio = null;
                        var tipoServ = DAOFactory.TipoServicioCicloDAO.FindDefault(new[] {linea.Empresa.Id}, new[] {linea.Id});
                        if (tipoServ != null &&
                            tipoServ.Id > 0)
                            tipoServicio = tipoServ;

                        #endregion

                        if (list.Count == 0 ||
                            codigoRuta != list.Last().Codigo)
                        {
                            var inicioBusquedaViaje = DateTime.UtcNow;
                            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, codigoRuta);
                            var duracionBusquedaViaje = DateTime.UtcNow.Subtract(inicioBusquedaViaje);
                            STrace.Trace("ViajeDistribucionImport",
                                "Búsqueda del viaje " + codigoRuta + " demoró " + duracionBusquedaViaje.TotalSeconds + " segundos");
                            if (byCode != null) continue;
                        }

                        ViajeDistribucion viaje;

                        if (list.Count > 0 &&
                            codigoRuta == list.Last().Codigo)
                        {
                            viaje = list.Last();
                        }
                        else
                        {
                            #region viaje = new ViajeDistribucion()

                            viaje = new ViajeDistribucion
                                    {
                                        Empresa = empresa,
                                        Linea = linea,
                                        Vehiculo = vehiculo,
                                        Empleado = responsable,
                                        CentroDeCostos = cc,
                                        SubCentroDeCostos = scc,
                                        Codigo = codigoRuta,
                                        Estado = ViajeDistribucion.Estados.Pendiente,
                                        Inicio = fecha.ToDataBaseDateTime(),
                                        Fin = fecha.ToDataBaseDateTime(),
                                        NumeroViaje = 1,
                                        Tipo = ViajeDistribucion.Tipos.Desordenado,
                                        RegresoABase = true,
                                        Umbral = 30,
                                        Alta = DateTime.UtcNow
                                    };

                            #endregion

                            list.Add(viaje);
                        }
                        viaje.Fin = fecha.ToDataBaseDateTime();

                        if (viaje.Detalles.Count == 0)
                        {
                            //el primer elemento es la base
                            var origen = new EntregaDistribucion
                                         {
                                             Linea = linea,
                                             Descripcion = linea.Descripcion,
                                             Estado = EntregaDistribucion.Estados.Pendiente,
                                             Orden = 0,
                                             Programado = fecha.ToDataBaseDateTime(),
                                             ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                             Viaje = viaje
                                         };
                            viaje.Detalles.Add(origen);
                        }

                        #region var puntoEntrega = [Find By Empresa, Linea, Codigo, Cliente].FirstOrDefault()

                        var puntoEntrega = DAOFactory.PuntoEntregaDAO.GetByCode(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {cliente.Id},
                            codigoEntrega);

                        #endregion

                        if (puntoEntrega == null)
                        {
                            var direcciones = GeocoderHelper.GetDireccion(calle, altura, esquina, localidad, provincia);
                            DireccionVO direccion = null;
                            if (direcciones.Count > 0) direccion = direcciones.First();
                            var latitud = direccion != null ? direccion.Latitud : 0.0;
                            var longitud = direccion != null ? direccion.Longitud : 0.0;

                            #region var puntoDeInteres = new ReferenciaGeografica()

                            var empresaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Empresa == null ? null : cliente.Empresa == null ? null : empresa;
                            var lineaGeoRef = viaje.Vehiculo != null && viaje.Vehiculo.Linea == null ? null : cliente.Linea == null ? null : linea;

                            var puntoDeInteres = new ReferenciaGeografica
                                                 {
                                                     Codigo = codigoEntrega,
                                                     Descripcion = codigoEntrega,
                                                     Empresa = empresaGeoRef,
                                                     Linea = lineaGeoRef,
                                                     EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                                     EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                                     EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                                     InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                                     TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                                     Vigencia =
                                                         new Vigencia
                                                         {
                                                             Inicio = DateTime.UtcNow,
                                                             Fin = fecha.AddHours(vigencia).ToDataBaseDateTime()
                                                         },
                                                     Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                                 };

                            #endregion

                            #region var posicion = new Direccion()

                            var posicion = new Direccion
                                           {
                                               Altura = direccion != null ? direccion.Altura : -1,
                                               IdMapa = (short) (direccion != null ? direccion.IdMapaUrbano : -1),
                                               Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                               IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                               IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                               IdEntrecalle = -1,
                                               Latitud = latitud,
                                               Longitud = longitud,
                                               Partido = direccion != null ? direccion.Partido : string.Empty,
                                               Pais = string.Empty,
                                               Calle = direccion != null ? direccion.Calle : string.Empty,
                                               Descripcion =
                                                   direccion != null
                                                       ? direccion.Direccion
                                                       : string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                           longitud.ToString(CultureInfo.InvariantCulture)),
                                               Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                           };

                            #endregion

                            #region var poligono = new Poligono()

                            var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                            poligono.AddPoints(new[] {new PointF((float) longitud, (float) latitud)});

                            #endregion

                            #region puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow)

                            if (direccion != null)
                                puntoDeInteres.AddHistoria(posicion, poligono, DateTime.UtcNow);

                            #endregion

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);
                            AddReferenciasGeograficas(puntoDeInteres);

                            #region puntoEntrega = new PuntoEntrega()

                            puntoEntrega = new PuntoEntrega
                                           {
                                               Cliente = cliente,
                                               Codigo = codigoEntrega,
                                               Descripcion = calle + " " + altura + " " + localidad,
                                               Telefono = string.Empty,
                                               Baja = false,
                                               ReferenciaGeografica = puntoDeInteres,
                                               Nomenclado = direccion != null,
                                               DireccionNomenclada = string.Empty,
                                               Nombre = codigoEntrega
                                           };

                            #endregion

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }
                        else
                        {
                            var end = fecha.AddHours(vigencia).ToDataBaseDateTime();
                            if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                            DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                            AddReferenciasGeograficas(puntoEntrega.ReferenciaGeografica);

                            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
                        }

                        #region var entrega = new EntregaDistribucion()

                        var entrega = new EntregaDistribucion
                                      {
                                          Cliente = cliente,
                                          PuntoEntrega = puntoEntrega,
                                          Descripcion = codigoEntrega,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = viaje.Detalles.Count,
                                          Programado = fecha.ToDataBaseDateTime(),
                                          ProgramadoHasta = fecha.ToDataBaseDateTime(),
                                          TipoServicio = tipoServicio,
                                          Viaje = viaje
                                      };

                        #endregion

                        if (viaje.Detalles.All(d => d.PuntoEntrega == null || d.PuntoEntrega.Codigo != codigoEntrega))
                            viaje.Detalles.Add(entrega);
                    }

                    foreach (var viajeDistribucion in list)
                    {
                        //el ultimo elemento es la base
                        var llegada = new EntregaDistribucion
                                      {
                                          Linea = viajeDistribucion.Linea,
                                          Descripcion = viajeDistribucion.Linea.Descripcion,
                                          Estado = EntregaDistribucion.Estados.Pendiente,
                                          Orden = viajeDistribucion.Detalles.Count,
                                          Programado = viajeDistribucion.Detalles.Last().Programado,
                                          ProgramadoHasta = viajeDistribucion.Detalles.Last().ProgramadoHasta,
                                          Viaje = viajeDistribucion
                                      };
                        viajeDistribucion.Detalles.Add(llegada);

                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viajeDistribucion);
                    }

                    transaction.Commit();
                    DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                    _empresasLineas.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private void AddReferenciasGeograficas(ReferenciaGeografica rg)
        {
            if (rg == null)
                STrace.Error(GetType().FullName, "AddReferenciasGeograficas: rg is null");
            else if (rg.Empresa == null)
                STrace.Error(GetType().FullName, "AddReferenciasGeograficas: rg.Empresa is null");
            else
            {
                if (!_empresasLineas.ContainsKey(rg.Empresa.Id))
                    _empresasLineas.Add(rg.Empresa.Id, new List<int> { -1 });

                if (rg.Linea != null)
                {
                    if (!_empresasLineas[rg.Empresa.Id].Contains(rg.Linea.Id))
                        _empresasLineas[rg.Empresa.Id].Add(rg.Linea.Id);
                }
                else
                {
                    var todaslaslineas = DAOFactory.LineaDAO.GetList(new[] { rg.Empresa.Id });
                    foreach (var linea in todaslaslineas)
                    {
                        if (!_empresasLineas.ContainsKey(linea.Id))
                            _empresasLineas[rg.Empresa.Id].Add(linea.Id);
                    }
                }
            }
        }

        #region SubClasses

        private static class Fields
        {
            public static readonly FieldValue CodigoRuta = new FieldValue("Código de Ruta", Properties.Distribucion.Codigo, Entities.Distribucion);
            public static readonly FieldValue CodigoPedido = new FieldValue("Código Pedido");
            public static readonly FieldValue CodigoBaseOrigen = new FieldValue("Código Base Origen");
            public static readonly FieldValue NombreBaseOrigen = new FieldValue("Nombre Base Origen");
            public static readonly FieldValue NombreBaseDestino = new FieldValue("Nombre Base Destino");
            public static readonly FieldValue Orden = new FieldValue("Orden");
            public static readonly FieldValue Transporte = new FieldValue("Transporte");
            public static readonly FieldValue CodigoBaseDestino = new FieldValue("Código Base Destino");
            public static readonly FieldValue Bultos = new FieldValue("Bultos");
            public static readonly FieldValue EstadoEntrega = new FieldValue("Estado de entrega");
            public static readonly FieldValue Vehiculo = new FieldValue("Vehículo", Properties.Distribucion.Vehiculo, Entities.Distribucion);
            public static readonly FieldValue Viaje = new FieldValue("Viaje");
            public static readonly FieldValue Secuencia = new FieldValue("Secuencia");
            public static readonly FieldValue Fecha = new FieldValue("Fecha", Properties.Distribucion.Fecha, Entities.Distribucion);
            public static readonly FieldValue FechaSalida = new FieldValue("Fecha de salida");
            public static readonly FieldValue FechaLlegada = new FieldValue("Fecha de llegada");
            public static readonly FieldValue HoraSalida = new FieldValue("Hora de salida");
            public static readonly FieldValue HoraLlegada = new FieldValue("Hora de llegada");
            public static readonly FieldValue Km = new FieldValue("Km");
            public static readonly FieldValue Longitud = new FieldValue("Longitud", Properties.ReferenciaGeografica.Longitud, Entities.ReferenciaGeografica);
            public static readonly FieldValue Latitud = new FieldValue("Latitud", Properties.ReferenciaGeografica.Latitud, Entities.ReferenciaGeografica);
            public static readonly FieldValue Celular = new FieldValue("Celular");
            public static readonly FieldValue Compania = new FieldValue("Compañia");
            public static readonly FieldValue Importe = new FieldValue("Importe");
            public static readonly FieldValue Nombre = new FieldValue("Nombre");
            public static readonly FieldValue Base = new FieldValue("Base", Properties.Distribucion.Linea, Entities.Distribucion);
            public static readonly FieldValue Empleado = new FieldValue("Empleado", Properties.Distribucion.Empleado, Entities.Distribucion);
            public static readonly FieldValue TipoDistribucion = new FieldValue("Tipo", Properties.Distribucion.TipoCiclo, Entities.Distribucion);
            public static readonly FieldValue Cliente = new FieldValue("Cliente", Properties.Distribucion.Cliente, Entities.Distribucion);
            public static readonly FieldValue PuntoEntrega = new FieldValue("Punto de Entrega", Properties.Distribucion.PuntoEntrega, Entities.Distribucion);
            public static readonly FieldValue RegresaABase = new FieldValue("Regresa a Base", Properties.Distribucion.RegresaABase, Entities.Distribucion);
            public static readonly FieldValue TipoServicio = new FieldValue("Tipo de Servicio", Properties.Distribucion.TipoServicio, Entities.Distribucion);
            public static readonly FieldValue Programada = new FieldValue("Hora Programada", Properties.Distribucion.Programado, Entities.Distribucion);
            public static readonly FieldValue Direccion = new FieldValue("1. Direccion", Properties.ReferenciaGeografica.Direccion, Entities.ReferenciaGeografica);
            public static readonly FieldValue Calle = new FieldValue("2. Calle", Properties.ReferenciaGeografica.Calle, Entities.ReferenciaGeografica);
            public static readonly FieldValue Altura = new FieldValue("2. Altura", Properties.ReferenciaGeografica.Altura, Entities.ReferenciaGeografica);
            public static readonly FieldValue Esquina = new FieldValue("2. Esquina", Properties.ReferenciaGeografica.Esquina, Entities.ReferenciaGeografica);
            public static readonly FieldValue Localidad = new FieldValue("2. Localidad", Properties.ReferenciaGeografica.Localidad, Entities.ReferenciaGeografica);
            public static readonly FieldValue Partido = new FieldValue("2. Partido", Properties.ReferenciaGeografica.Partido, Entities.ReferenciaGeografica);
            public static readonly FieldValue Provincia = new FieldValue("2. Provincia", Properties.ReferenciaGeografica.Provincia, Entities.ReferenciaGeografica);
            public static readonly FieldValue Radio = new FieldValue("Radio", Properties.ReferenciaGeografica.Radio, Entities.ReferenciaGeografica);
            public static readonly FieldValue Descripcion = new FieldValue("Descripcion", Properties.ReferenciaGeografica.Descripcion, Entities.ReferenciaGeografica);
            public static readonly FieldValue TipoReferenciaGeografica = new FieldValue("Tipo de Referencia Geografica", Properties.ReferenciaGeografica.TipoReferenciaGeografica, Entities.ReferenciaGeografica);
            public static readonly FieldValue Cuadrilla = new FieldValue("Cuadrilla");

            public static List<FieldValue> Axiodis
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   CodigoPedido,
                                   Vehiculo,
                                   Viaje,
                                   Secuencia,
                                   Fecha,
                                   Km,
                                   Longitud,
                                   Latitud,
                                   Celular,
                                   Compania,
                                   Importe,
                                   Nombre
                               };
                }
            }
            public static List<FieldValue> AxiodisF
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Viaje,
                                   Vehiculo,
                                   CodigoPedido,
                                   Fecha,
                                   Latitud,
                                   Longitud,
                                   PuntoEntrega,
                                   Nombre,
                                   Direccion
                               };
                }
            }
            public static List<FieldValue> AxiodisOpcional
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Nombre,
                                   Importe,
                                   Compania,
                                   Celular
                               };
                }
            }
            public static List<FieldValue> Roadshow
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   CodigoRuta,
                                   Viaje,
                                   Vehiculo,
                                   Fecha,
                                   Secuencia,
                                   CodigoPedido,
                                   Programada,
                                   PuntoEntrega,
                                   Nombre,
                                   Latitud,
                                   Longitud
                               };
                }
            }
            public static List<FieldValue> RoadshowCsv
            {
                get
                {
                    return new List<FieldValue>();
                }
            }
            public static List<FieldValue> RoadshowOpcional
            {
                get
                {
                    return new List<FieldValue>();
                }
            }
            public static List<FieldValue> Roadnet
            {
                get
                {
                    return new List<FieldValue>();
                }
            }
            public static List<FieldValue> Default
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Base,
                                   Vehiculo,
                                   Empleado,
                                   CodigoPedido,
                                   Fecha,
                                   TipoDistribucion,
                                   Cliente,
                                   PuntoEntrega,
                                   RegresaABase,
                                   TipoServicio,
                                   Programada,
                                   Latitud,
                                   Longitud,
                                   Direccion,
                                   Calle,
                                   Altura,
                                   Esquina,
                                   Localidad,
                                   Partido,
                                   Provincia,
                                   Radio,
                                   Descripcion,
                                   TipoReferenciaGeografica
                               };
                }
            }
            public static List<FieldValue> DefaultOpcional
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Base,
                                   Vehiculo,
                                   Empleado,
                                   TipoDistribucion,
                                   RegresaABase,
                                   TipoServicio,
                                   Programada,
                                   Latitud,
                                   Longitud,
                                   Direccion,
                                   Calle,
                                   Altura,
                                   Esquina,
                                   Localidad,
                                   Partido,
                                   Provincia,
                                   Radio,
                                   Descripcion,
                                   TipoReferenciaGeografica
                               };
                }
            }
            public static List<FieldValue> ExcelTemplate
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Vehiculo,
                                   Cuadrilla,
                                   CodigoPedido,
                                   Calle,
                                   Altura,
                                   Localidad
                               };
                }
            }
            public static List<FieldValue> ReginaldLee
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Viaje,
                                   CodigoBaseOrigen,
                                   NombreBaseOrigen,
                                   CodigoBaseDestino,
                                   Orden,
                                   NombreBaseDestino,
                                   Vehiculo,
                                   Transporte,
                                   EstadoEntrega,
                                   FechaSalida,
                                   HoraSalida,
                                   FechaLlegada,
                                   HoraLlegada,
                                   Bultos
                               };
                }
            }
        } 

        #endregion
    }
}