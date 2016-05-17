#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.CustomWebControls.Binding;
using Logictracker.Web.CustomWebControls.DropDownLists;
using Logictracker.Web.CustomWebControls.Wrappers.DropDownLists;
using Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects;
using Logictracker.Web.Documentos.Helpers;
using Logictracker.Web.Documentos.Interfaces;
using BaseParser = Logictracker.Web.Documentos.Helpers.BaseParser;
using TemplateParser = Logictracker.Web.Documentos.Helpers.TemplateParser;

#endregion

namespace Logictracker.Web.Documentos
{
    public class GenericPresenter: IPresentStrategy
    {
        protected const string BINDCOCHE = "C:";
        protected const string BINDEMPLEADO = "M:";
        protected const string BINDEMPRESA = "E:";
        protected const string BINDEQUIPO = "Q:";
        protected const string BINDLINEA = "L:";
        protected const string BINDTRANSPORTISTA = "T:";
        protected const string BINDCLIENTE = "I:";
        protected const string BINDTANQUE = "U:";
        protected const string BINDCENTRODECOSTO = "N:";

        private BindingManager _bindingManager;

        protected BindingManager BindingManager { get { return _bindingManager ?? (_bindingManager = new BindingManager()); } }

        #region Fields

        protected readonly Dictionary<string, TipoDocumentoParametro> parametros = new Dictionary<string, TipoDocumentoParametro>();
        protected readonly Dictionary<string, string> styles = new Dictionary<string, string>();
        protected readonly TipoDocumento TipoDocumento;
        protected readonly IDocumentView view;
        protected DAOFactory DAOFactory;

        private int LiteralCount;
        protected string template;
        protected TipoDocumentoHelper TipoDocumentoHelper;
        protected BaseParser parser;


        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tipoDoc">El tipo de documento a procesar</param>
        /// <param name="view">La vista donde se va a presentar el documento</param>
        /// <param name="daof">DAOFactory</param>
        public GenericPresenter(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof)
        {
            TipoDocumento = tipoDoc;
            TipoDocumentoHelper = new TipoDocumentoHelper(TipoDocumento);
            this.view = view;
            DAOFactory = daof;
        } 
        #endregion

        #region IPresentStrategy Members

        public void CrearForm()
        {
            if (parser == null)
            {
                parser = string.IsNullOrEmpty(TipoDocumento.Template)
                             ? new NormalParser(TipoDocumentoHelper) as BaseParser
                             : new TemplateParser(TipoDocumento.Template, TipoDocumentoHelper);
            }
            parser.Base += parser_Base;
            parser.Planta += parser_Planta;
            parser.Codigo += parser_Codigo;
            parser.Fecha += parser_Fecha;
            parser.Presentacion += parser_Presentacion;
            parser.Vencimiento += parser_Vencimiento;
            parser.Cierre += parser_Cierre;
            parser.Literal += parser_Literal;
            parser.Parametro += parser_Parametro;
            parser.Descripcion += parser_Descripcion;
            parser.Estado += parser_Estado;
            parser.Vehiculo += parser_Vehiculo;
            parser.Empleado += parser_Empleado;
            parser.Transportista += parser_Transportista;
            parser.Equipo += parser_Equipo;
            parser.Parse();

            OnCreated();
        }

        public void SetValores(Documento documento)
        {
            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
            var txtCodigo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CODIGO) as TextBox;
            var txtDescripcion = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_DESCRIPCION) as TextBox;
            var txtFecha = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_FECHA) as TextBox;
            var txtPresentacion = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PRESENTACION) as TextBox;
            var txtVencimiento = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_VENCIMIENTO) as TextBox;
            var txtCierre = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CIERRE) as TextBox;
            var cbEstado = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_ESTADO) as DropDownList;
            var cbVehiculo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI03) as DropDownList;
            var cbEmpleado = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI09) as DropDownList;
            var cbTransportista = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI07) as DropDownList;
            var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;

            if (cbEmpresa != null && documento.Linea != null)
            {
                cbEmpresa.SelectedValue = documento.Linea.Empresa.Id.ToString();
                cbEmpresa_SelectedIndexChanged(cbEmpresa, EventArgs.Empty);
            }
            if (cbLinea != null && documento.Linea != null)
            {
                cbLinea.SelectedValue = documento.Linea.Id.ToString();
                cbLinea_SelectedIndexChanged(cbLinea, EventArgs.Empty);
            }
            if (cbTransportista != null && documento.Transportista != null)
            {
                cbTransportista.SelectedValue = documento.Transportista.Id.ToString();
                cbTransportista_SelectedIndexChanged(cbTransportista, EventArgs.Empty);
            }
            if (cbVehiculo != null && documento.Vehiculo != null)
            {
                cbVehiculo.SelectedValue = documento.Vehiculo.Id.ToString();
            }

            if (cbEmpleado != null && documento.Empleado != null)
            {
                cbEmpleado.SelectedValue = documento.Empleado.Id.ToString();
            }
            if (cbEquipo != null && documento.Equipo != null)
            {
                cbEquipo.SelectedValue = documento.Equipo.Id.ToString();
                cbEquipo_SelectedIndexChanged(cbEquipo, EventArgs.Empty);
            }
            if(txtCodigo != null) 
                txtCodigo.Text = documento.Codigo;
            if (txtDescripcion != null)
                txtDescripcion.Text = documento.Descripcion;
            if(txtFecha != null) 
                txtFecha.Text = documento.Fecha.ToString("dd/MM/yyyy");
            if(txtVencimiento != null && documento.Vencimiento.HasValue)
                txtVencimiento.Text = documento.Vencimiento.Value.ToString("dd/MM/yyyy");
            if (txtPresentacion != null && documento.Presentacion.HasValue)
                txtPresentacion.Text = documento.Presentacion.Value.ToString("dd/MM/yyyy");
            if (txtCierre != null && documento.FechaCierre.HasValue)
                txtCierre.Text = documento.FechaCierre.Value.ToString("dd/MM/yyyy");
            if (cbEstado != null)
                cbEstado.SelectedValue = documento.Estado.ToString();

            foreach (DocumentoValor valor in documento.Parametros)
                SetParameterValue(valor);

            OnValuesSetted();
        }

        public void SetDefaults()
        {
            foreach (TipoDocumentoParametro param in TipoDocumento.Parametros)
            {
                if(!string.IsNullOrEmpty(param.Default)) continue;
                if(param.Repeticion == 1)
                {
                    var id = TipoDocumentoHelper.GetControlName(param);
                    SetParameterValue(param.TipoDato, id, param.Default);
                }
                else
                {
                    for(var i = 0; i < param.Repeticion; i++)
                    {
                        var id = TipoDocumentoHelper.GetControlName(param, 0);
                        SetParameterValue(param.TipoDato, id, param.Default);
                    }
                }
            }
        }

        #endregion

        #region Parameter Handlers
        void parser_Literal(object sender, DocumentoLiteralEventArgs e)
        {
            AddLiteral(e.Text);
        }

        void parser_Base(object sender, DocumentoParametroEventArgs e)
        {
            AddEmpresa(e.FieldName, e.Style);
        }

        void parser_Planta(object sender, DocumentoParametroEventArgs e)
        {
            AddLinea(e.FieldName, e.Style);
        }

        void parser_Codigo(object sender, DocumentoParametroEventArgs e)
        {
            AddTextBox(e.FieldName, e.Style);
        }

        void parser_Descripcion(object sender, DocumentoParametroEventArgs e)
        {
            AddTextBox(e.FieldName, e.Style);
        }

        void parser_Fecha(object sender, DocumentoParametroEventArgs e)
        {
            AddDate(e.FieldName, e.Style);
        }

        void parser_Presentacion(object sender, DocumentoParametroEventArgs e)
        {
            AddDate(e.FieldName, e.Style);
        }

        void parser_Vencimiento(object sender, DocumentoParametroEventArgs e)
        {
            AddDate(e.FieldName, e.Style);
        }

        void parser_Cierre(object sender, DocumentoParametroEventArgs e)
        {
            AddDate(e.FieldName, e.Style);
        }

        void parser_Estado(object sender, DocumentoParametroEventArgs e)
        {
            AddEstado(e.FieldName, e.Style);
        }

        void parser_Parametro(object sender, DocumentoParametroEventArgs e)
        {
            AddParameter(e.Parametro, e.FieldName, e.Style);
        }

        void parser_CentroDeCosto(object sender, DocumentoParametroEventArgs e)
        {
            AddCentroDeCostos(e.FieldName, e.Style, true);
        }
        void parser_Equipo(object sender, DocumentoParametroEventArgs e)
        {
            AddEquipo(e.FieldName, e.Style, true);
        }

        void parser_Transportista(object sender, DocumentoParametroEventArgs e)
        {
            AddAseguradora(e.FieldName, e.Style, true);
        }

        void parser_Empleado(object sender, DocumentoParametroEventArgs e)
        {
            AddEmpleado(e.FieldName, e.Style, true);
        }

        void parser_Vehiculo(object sender, DocumentoParametroEventArgs e)
        {
            AddCoche(e.FieldName, e.Style, true);
        }
        
        #endregion

        protected virtual void AddParameter(TipoDocumentoParametro par, string id, string style)
        {
            switch (par.TipoDato.ToLower())
            {
                case TipoParametroDocumento.Integer:
                case TipoParametroDocumento.Float:
                case TipoParametroDocumento.String:
                case TipoParametroDocumento.Barcode:
                    AddTextBox(id, style);
                    break;
                case TipoParametroDocumento.DateTime:
                    AddDate(id, style);
                    break;
                case TipoParametroDocumento.Boolean:
                    AddCheckBox(id, style);
                    break;
                case TipoParametroDocumento.Coche:
                    AddCoche(id, style, par.Obligatorio);
                    break;
                case TipoParametroDocumento.Chofer:
                    AddEmpleado(id, style, par.Obligatorio);
                    break;
                case TipoParametroDocumento.Aseguradora:
                    AddAseguradora(id, style, par.Obligatorio);
                    break;
                case TipoParametroDocumento.Equipo:
                    AddEquipo(id, style, par.Obligatorio);
                    break;
                case TipoParametroDocumento.Image:
                    AddImagen(id, style, par.Obligatorio);
                    break;
                case TipoParametroDocumento.Tanque:
                    AddTanque(id, style, par.Obligatorio);
                    break;
                case TipoParametroDocumento.Cliente:
                    AddCliente(id, style, par.Obligatorio);
                    break;
                case TipoParametroDocumento.CentroCostos:
                    AddCentroDeCostos(id, style, par.Obligatorio);
                    break;
            }
            SetParameterValue(par.TipoDato, id, par.Default);
        }

        protected virtual void OnCreated() { }
        protected virtual void OnValuesSetted() { }

        #region Add Controls
        protected virtual void AddLiteral(string text)
        {
            var literal = new Literal { ID = ("litContent" + LiteralCount++), Text = text };
            AddControlToView(literal);
        }

        protected virtual DropDownList AddDropDownList(string id, string style)
        {
            var combo = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            combo.Style.Value = style;

            AddControlToView(combo);
            return combo;
        }

        protected virtual void AddEstado(string id, string style)
        {
            var cbEstado = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            cbEstado.Style.Value = style;
            cbEstado.Items.Add(new ListItem("Abierto", "0"));
            cbEstado.Items.Add(new ListItem("Prestado", "1"));
            cbEstado.Items.Add(new ListItem("Cerrado", "9"));

            AddControlToView(cbEstado);
        }

        protected virtual void AddEmpresa(string id, string style)
        {
            var cbEmpresa = new LocacionDropDownList { ID = id, Width = Unit.Pixel(200) };
            cbEmpresa.Style.Value = style;

            if(TipoDocumento.Empresa != null) BindingManager.BindLocacion(cbEmpresa, TipoDocumento.Empresa.Id);
            else BindingManager.BindLocacion(cbEmpresa);

            AddControlToView(cbEmpresa);
        }
        protected virtual void AddLinea(string id, string style)
        {
            var cbLinea = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            cbLinea.Style.Value = style;
            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;

            var ddlLinea = new DropDownListBaseWrapper<Linea>(cbLinea, TipoDocumento.Linea == null);
            ddlLinea.AddParent<Empresa>(cbEmpresa);

            if (TipoDocumento.Linea != null) BindingManager.BindPlanta(ddlLinea, TipoDocumento.Linea.Id);
            else BindingManager.BindPlanta(ddlLinea);

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;
                AddChildrenToControl(cbEmpresa.ID, id, BINDLINEA);
            }

            AddControlToView(cbLinea);
        }
        
        protected virtual void AddTextBox(string id, string style)
        {
            var textbox = new TextBox { ID = id, Width = Unit.Pixel(200) };
            textbox.Style.Value = style;
            AddControlToView(textbox);
        }
        protected virtual void AddLabel(string id, string style)
        {
            var label = new Label { ID = id };
            label.Style.Value = style;
            AddControlToView(label);
        }
        protected virtual void AddCheckBox(string id, string style)
        {
            var checkbox = new CheckBox {ID = id};
            checkbox.Style.Value = style;
            AddControlToView(checkbox);
        }
        protected virtual void AddDate(string id, string style)
        {
            var date = new TextBox { ID = id, Width = Unit.Pixel(80) };
            date.Style.Value = style;
            var calendar = new CalendarExtender {ID = (id + "_calendar"), TargetControlID = date.ClientID};

            AddControlToView(date);
            AddControlToView(calendar);
        }
        
        protected virtual void AddCoche(string id, string style)
        {
            AddCoche(id, style, false);
        }
        protected virtual void AddCoche(string id, string style, bool obligatorio)
        {
            var cbVehiculo = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            cbVehiculo.Style.Value = style;

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
            DropDownList cbTransportista = null;
            if (TipoDocumento.AplicarATransportista)
                cbTransportista = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI07) as DropDownList;

            var ddlVehiculo = new MovilDropDownListWrapper(cbVehiculo, false);
            ddlVehiculo.AddParent<Empresa>(cbEmpresa);
            ddlVehiculo.AddParent<Linea>(cbLinea);
            if(cbTransportista != null) ddlVehiculo.AddParent<Transportista>(cbTransportista);

            ddlVehiculo.AddAllItem = !obligatorio;

            BindingManager.BindMovil(ddlVehiculo);
            
            cbVehiculo.Attributes.Add("AddAll", !obligatorio ? "1" : "0");

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;

                AddChildrenToControl(cbEmpresa.ID, id, BINDCOCHE);
            }
            if (cbLinea != null)
            {
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
                cbLinea.AutoPostBack = true;

                AddChildrenToControl(cbLinea.ID, id, BINDCOCHE);
            }
            if (cbTransportista != null)
            {
                cbTransportista.SelectedIndexChanged += cbTransportista_SelectedIndexChanged;
                cbTransportista.AutoPostBack = true;

                AddChildrenToControl(cbTransportista.ID, id, BINDCOCHE);
            }

            AddControlToView(cbVehiculo);
        }

        protected virtual void AddAseguradora(string id, string style)
        {
            AddAseguradora(id, style, false);
        }
        protected virtual void AddAseguradora(string id, string style, bool obligatorio)
        {
            var cbTransportista = new DropDownList {ID = id, Width = Unit.Pixel(200)};
            cbTransportista.Style.Value = style;

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;

            var ddlTransportista = new TransportistaDropDownListWrapper(cbTransportista, !obligatorio);
            ddlTransportista.AddParent<Empresa>(cbEmpresa);
            ddlTransportista.AddParent<Linea>(cbLinea);

            BindingManager.BindTransportista(ddlTransportista);

            cbTransportista.Attributes.Add("AddAllItem", !obligatorio?"1":"0");

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;

                AddChildrenToControl(cbEmpresa.ID, id, BINDTRANSPORTISTA);
            }
            if (cbLinea != null)
            {
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
                cbLinea.AutoPostBack = true;

                AddChildrenToControl(cbLinea.ID, id, BINDTRANSPORTISTA);
            }

            AddControlToView(cbTransportista);
        }
        protected virtual void AddEquipo(string id, string style)
        {
            AddEquipo(id, style, false);
        }
        protected virtual void AddEquipo(string id, string style, bool obligatorio)
        {
            var cbEquipo = new DropDownList {ID = id, Width = Unit.Pixel(200)};
            cbEquipo.Style.Value = style;

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;

            var ddlEquipo = new EquipoDropDownListWrapper(cbEquipo);
            ddlEquipo.AddParent<Empresa>(cbEmpresa);
            ddlEquipo.AddParent<Linea>(cbLinea);

            BindingManager.BindEquipo(ddlEquipo);

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;

                AddChildrenToControl(cbEmpresa.ID, id, BINDEQUIPO);
            }
            if (cbLinea != null)
            {
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
                cbLinea.AutoPostBack = true;

                AddChildrenToControl(cbLinea.ID, id, BINDEQUIPO);
            }

            AddControlToView(cbEquipo);
        }
        protected virtual void AddEmpleado(string id, string style)
        {
            AddEmpleado(id, style, false);
        }
        protected virtual void AddEmpleado(string id, string style, bool obligatorio)
        {
            var cbEmpleado = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            cbEmpleado.Style.Value = style;

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;

            var cbEmpleadoWrapper = new EmpleadoDropDownListWrapper(cbEmpleado, false, !obligatorio);
            cbEmpleadoWrapper.AddParent<Empresa>(cbEmpresa);
            cbEmpleadoWrapper.AddParent<Linea>(cbLinea);
            
            BindingManager.BindEmpleados(cbEmpleadoWrapper);

            cbEmpleado.Attributes.Add("AddSinEmpleado", !obligatorio ? "1" : "0");

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;

                AddChildrenToControl(cbEmpresa.ID, id, BINDEMPLEADO);
            }
            if (cbLinea != null)
            {
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
                cbLinea.AutoPostBack = true;

                AddChildrenToControl(cbLinea.ID, id, BINDEMPLEADO);
            }
            AddControlToView(cbEmpleado);
        }
        protected virtual void AddImagen(string id, string style, bool obligatorio)
        {
            var filImagen = new FileUpload {ID = id, Width = Unit.Pixel(200)};
            filImagen.Style.Value = style;
            var imgImagen = new Image {ID = id + "_image", Width = Unit.Pixel(40), Height = Unit.Pixel(40), Visible = false };
            var lit1 = new Literal { ID = ("litContent" + LiteralCount++), Text = "<table border='0'><tr><td style='padding-right: 50px;'>" };
            var lit2 = new Literal { ID = ("litContent" + LiteralCount++), Text = "</td><td>" };
            var lit3 = new Literal { ID = ("litContent" + LiteralCount++), Text = "</td></tr></table>" };
            AddControlToView(lit1);
            AddControlToView(filImagen);
            AddControlToView(lit2);
            AddControlToView(imgImagen);
            AddControlToView(lit3);
        }
        protected virtual void AddCliente(string id, string style)
        {
            AddCliente(id, style, false);
        }
        protected virtual void AddCliente(string id, string style, bool obligatorio)
        {
            var cbCliente = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            cbCliente.Style.Value = style;

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;

            var cbClienteWrapper = new ClienteDropDownListWrapper(cbCliente, !obligatorio);
            cbClienteWrapper.AddParent<Empresa>(cbEmpresa);
            cbClienteWrapper.AddParent<Linea>(cbLinea);

            BindingManager.BindCliente(cbClienteWrapper);

            cbCliente.Attributes.Add("AddAllItem", !obligatorio ? "1" : "0");

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;

                AddChildrenToControl(cbEmpresa.ID, id, BINDCLIENTE);
            }

            if (cbLinea != null)
            {
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
                cbLinea.AutoPostBack = true;

                AddChildrenToControl(cbLinea.ID, id, BINDCLIENTE);
            }
            AddControlToView(cbCliente);
        }

        protected virtual void AddCentroDeCostos(string id, string style)
        {
            AddCentroDeCostos(id, style, false);
        }
        protected virtual void AddCentroDeCostos(string id, string style, bool obligatorio)
        {
            var cbCentroDeCostos = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            cbCentroDeCostos.Style.Value = style;

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;

            var cbCentroDeCostosWrapper = new CentroDeCostoDropDownListWrapper(cbCentroDeCostos, !obligatorio);
            cbCentroDeCostosWrapper.AddParent<Empresa>(cbEmpresa);
            cbCentroDeCostosWrapper.AddParent<Linea>(cbLinea);

            BindingManager.BindCentroDeCostos(cbCentroDeCostosWrapper);

            cbCentroDeCostos.Attributes.Add("AddAllItem", !obligatorio ? "1" : "0");

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;

                AddChildrenToControl(cbEmpresa.ID, id, BINDCENTRODECOSTO);
            }

            if (cbLinea != null)
            {
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
                cbLinea.AutoPostBack = true;

                AddChildrenToControl(cbLinea.ID, id, BINDCENTRODECOSTO);
            }
            AddControlToView(cbCentroDeCostos);
        }
        protected virtual void AddTanque(string id, string style)
        {
            AddTanque(id, style, false);
        }
        protected virtual void AddTanque(string id, string style, bool obligatorio)
        {
            var cbTanque = new DropDownList { ID = id, Width = Unit.Pixel(200) };
            cbTanque.Style.Value = style;

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
            var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;

            var cbTanqueWrapper = new TanqueDropDownListWrapper(cbTanque, !obligatorio);
            cbTanqueWrapper.AddParent<Empresa>(cbEmpresa);
            cbTanqueWrapper.AddParent<Linea>(cbLinea);
            cbTanqueWrapper.AddParent<Equipo>(cbEquipo);

            BindingManager.BindTanque(cbTanqueWrapper);

            cbTanque.Attributes.Add("AddAllItem", !obligatorio ? "1" : "0");

            if (cbEmpresa != null)
            {
                cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
                cbEmpresa.AutoPostBack = true;

                AddChildrenToControl(cbEmpresa.ID, id, BINDTANQUE);
            }

            if (cbLinea != null)
            {
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
                cbLinea.AutoPostBack = true;

                AddChildrenToControl(cbLinea.ID, id, BINDTANQUE);
            }

            if (cbEquipo != null)
            {
                cbEquipo.SelectedIndexChanged += cbEquipo_SelectedIndexChanged;
                cbEquipo.AutoPostBack = true;

                AddChildrenToControl(cbEquipo.ID, id, BINDTANQUE);
            }

            AddControlToView(cbTanque);
        }
        #endregion

        #region Helper Methods
        protected void AddControlToView(Control control)
        {
            view.DocumentContainer.Controls.Add(control);
        }
        public Control GetControlFromView(string id)
        {
            return view.DocumentContainer.FindControl(id);
        }

        protected string[] GetChildrenForControl(string id)
        {
            var children = GetControlFromView(id + "_children") as HiddenField;
            if (children == null) return new string[0];
            return children.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
        protected void AddChildrenToControl(string parent, string child, string bindType)
        {
            var children = GetControlFromView(parent + "_children") as HiddenField;

            if (children == null)
            {
                children = new HiddenField { ID = parent + "_children" };
                AddControlToView(children);
            }
            children.Value += "," + bindType + child;
        } 
        #endregion

        #region EventHandlers
        protected virtual void cbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbEmpresa = sender as DropDownList;
            if (cbEmpresa == null) return;

            var childs = GetChildrenForControl(cbEmpresa.ID);
            foreach (var child in childs)
            {
                if (child.StartsWith(BINDCOCHE))
                {
                    var id = child.Substring(BINDCOCHE.Length);
                    var cbVehiculo = GetControlFromView(id) as DropDownList;
                    var all = cbVehiculo.Attributes["AddAll"] == "1";

                    var cbLinea = GetControlFromView(id) as DropDownList;
                    var cbTransportista = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI07) as DropDownList;

                    var cbVehiculoWrapper = new MovilDropDownListWrapper(cbVehiculo, false);
                    cbVehiculoWrapper.AddParent<Empresa>(cbEmpresa);
                    cbVehiculoWrapper.AddParent<Linea>(cbLinea);
                    if (cbTransportista != null) cbVehiculoWrapper.AddParent<Transportista>(cbTransportista);
                    cbVehiculoWrapper.AddAllItem = all;

                    BindingManager.BindMovil(cbVehiculoWrapper);
                }
                else if (child.StartsWith(BINDLINEA))
                {
                    var id = child.Substring(BINDLINEA.Length);
                    var cbLinea = GetControlFromView(id) as DropDownList;

                    var ddlLinea = new DropDownListBaseWrapper<Linea>(cbLinea, TipoDocumento.Linea == null);
                    ddlLinea.AddParent<Empresa>(cbEmpresa);

                    if (TipoDocumento.Linea != null) BindingManager.BindPlanta(ddlLinea, TipoDocumento.Linea.Id);
                    else
                    {
                        
                        BindingManager.BindPlanta(ddlLinea);
                    }
                }
                else if(child.StartsWith(BINDTRANSPORTISTA))
                {
                    var id = child.Substring(BINDTRANSPORTISTA.Length);
                    var cbTrans = GetControlFromView(id) as DropDownList;
                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
                    var all = cbTrans.Attributes["AddAllItem"] == "1";

                    var ddlTransportista = new TransportistaDropDownListWrapper(cbTrans, all);
                    ddlTransportista.AddParent<Linea>(cbLinea);

                    BindingManager.BindTransportista(ddlTransportista);
                }
                else if (child.StartsWith(BINDEQUIPO))
                {
                    var id = child.Substring(BINDEQUIPO.Length);

                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
                    var cbEquipo = GetControlFromView(id) as DropDownList;

                    var ddlEquipo = new EquipoDropDownListWrapper(cbEquipo);
                    ddlEquipo.AddParent<Empresa>(cbEmpresa);
                    ddlEquipo.AddParent<Linea>(cbLinea);

                    BindingManager.BindEquipo(ddlEquipo);
                }
                else if (child.StartsWith(BINDEMPLEADO))
                {
                    var id = child.Substring(BINDEMPLEADO.Length);

                    var cbEmpleado = GetControlFromView(id) as DropDownList;
                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;

                    var all = cbEmpleado.Attributes["AddSinEmpleado"] == "1";

                    var cbEmpleadoWrapper = new EmpleadoDropDownListWrapper(cbEmpleado, false, all);
                    cbEmpleadoWrapper.AddParent<Empresa>(cbEmpresa);
                    cbEmpleadoWrapper.AddParent<Linea>(cbLinea);

                    BindingManager.BindEmpleados(cbEmpleadoWrapper);
                }
                else if(child.StartsWith(BINDCLIENTE))
                {
                    var id = child.Substring(BINDCLIENTE.Length);
                    var cbCliente = GetControlFromView(id) as DropDownList;
                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
                    var all = cbCliente.Attributes["AddAllItem"] == "1";
                    var cbClienteWrapper = new ClienteDropDownListWrapper(cbCliente, all);
                    cbClienteWrapper.AddParent<Empresa>(cbEmpresa);
                    cbClienteWrapper.AddParent<Linea>(cbLinea);
                    BindingManager.BindCliente(cbClienteWrapper);
                }
                else if (child.StartsWith(BINDCENTRODECOSTO))
                {
                    var id = child.Substring(BINDCENTRODECOSTO.Length);
                    var cbCentroDeCosto = GetControlFromView(id) as DropDownList;
                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
                    var all = cbCentroDeCosto.Attributes["AddAllItem"] == "1";
                    var cbCentroDeCostoWrapper = new CentroDeCostoDropDownListWrapper(cbCentroDeCosto, all);
                    cbCentroDeCostoWrapper.AddParent<Empresa>(cbEmpresa);
                    cbCentroDeCostoWrapper.AddParent<Linea>(cbLinea);
                    BindingManager.BindCentroDeCostos(cbCentroDeCostoWrapper);
                }
                else if (child.StartsWith(BINDTANQUE))
                {
                    var id = child.Substring(BINDTANQUE.Length);
                    var cbTanque = GetControlFromView(id) as DropDownList;
                    var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;
                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
                    var all = cbTanque.Attributes["AddAllItem"] == "1";
                    var cbTanqueWrapper = new TanqueDropDownListWrapper(cbTanque, all);
                    cbTanqueWrapper.AddParent<Empresa>(cbEmpresa);
                    cbTanqueWrapper.AddParent<Linea>(cbLinea);
                    cbTanqueWrapper.AddParent<Equipo>(cbEquipo);
                    BindingManager.BindTanque(cbTanqueWrapper);
                }
            }
        }

        protected virtual void cbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbLinea = sender as DropDownList;
            if (cbLinea == null) return;

            var childs = GetChildrenForControl(cbLinea.ID);
            foreach (var child in childs)
            {
                if (child.StartsWith(BINDCOCHE))
                {
                    var id = child.Substring(BINDCOCHE.Length);
                    var cbVehiculo = GetControlFromView(id) as DropDownList;
                    var all = cbVehiculo.Attributes["AddAll"] == "1";

                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var cbTransportista = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI07) as DropDownList;

                    var cbVehiculoWrapper = new MovilDropDownListWrapper(cbVehiculo, false);
                    cbVehiculoWrapper.AddParent<Empresa>(cbEmpresa);
                    cbVehiculoWrapper.AddParent<Linea>(cbLinea);
                    if(cbTransportista != null) cbVehiculoWrapper.AddParent<Transportista>(cbTransportista);
                    cbVehiculoWrapper.AddAllItem = all;

                    BindingManager.BindMovil(cbVehiculoWrapper);
                }
                else if (child.StartsWith(BINDEQUIPO))
                {
                    var id = child.Substring(BINDEQUIPO.Length);

                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var cbEquipo = GetControlFromView(id) as DropDownList;

                    var ddlEquipo = new EquipoDropDownListWrapper(cbEquipo);
                    ddlEquipo.AddParent<Empresa>(cbEmpresa);
                    ddlEquipo.AddParent<Linea>(cbLinea);

                    BindingManager.BindEquipo(ddlEquipo);
                }
                else if (child.StartsWith(BINDEMPLEADO))
                {
                    var id = child.Substring(BINDEMPLEADO.Length);

                    var cbEmpleado = GetControlFromView(id) as DropDownList;
                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;

                    var all = cbEmpleado.Attributes["AddSinEmpleado"] == "1";

                    var cbEmpleadoWrapper = new EmpleadoDropDownListWrapper(cbEmpleado, false, all);
                    cbEmpleadoWrapper.AddParent<Empresa>(cbEmpresa);
                    cbEmpleadoWrapper.AddParent<Linea>(cbLinea);

                    BindingManager.BindEmpleados(cbEmpleadoWrapper);
                }
                else if (child.StartsWith(BINDTRANSPORTISTA))
                {
                    var id = child.Substring(BINDTRANSPORTISTA.Length);
                    var cbTrans = GetControlFromView(id) as DropDownList;
                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var all = cbTrans.Attributes["AddAllItem"] == "1";

                    var ddlTransportista = new TransportistaDropDownListWrapper(cbTrans, all);
                    ddlTransportista.AddParent<Empresa>(cbEmpresa);

                    BindingManager.BindTransportista(ddlTransportista);
                }
                else if (child.StartsWith(BINDCLIENTE))
                {
                    var id = child.Substring(BINDCLIENTE.Length);
                    var cbCliente = GetControlFromView(id) as DropDownList;
                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var all = cbCliente.Attributes["AddAllItem"] == "1";
                    var cbClienteWrapper = new ClienteDropDownListWrapper(cbCliente, all);
                    cbClienteWrapper.AddParent<Empresa>(cbEmpresa);
                    cbClienteWrapper.AddParent<Linea>(cbLinea);
                    BindingManager.BindCliente(cbClienteWrapper);
                }
                else if (child.StartsWith(BINDCENTRODECOSTO))
                {
                    var id = child.Substring(BINDCENTRODECOSTO.Length);
                    var cbCentroDeCosto = GetControlFromView(id) as DropDownList;
                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var all = cbCentroDeCosto.Attributes["AddAllItem"] == "1";
                    var cbCentroDeCostoWrapper = new CentroDeCostoDropDownListWrapper(cbCentroDeCosto, all);
                    cbCentroDeCostoWrapper.AddParent<Empresa>(cbEmpresa);
                    cbCentroDeCostoWrapper.AddParent<Linea>(cbLinea);
                    BindingManager.BindCentroDeCostos(cbCentroDeCostoWrapper);
                }
                else if (child.StartsWith(BINDTANQUE))
                {
                    var id = child.Substring(BINDTANQUE.Length);
                    var cbTanque = GetControlFromView(id) as DropDownList;
                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;
                    var all = cbTanque.Attributes["AddAllItem"] == "1";
                    var cbTanqueWrapper = new TanqueDropDownListWrapper(cbTanque, all);
                    cbTanqueWrapper.AddParent<Empresa>(cbEmpresa);
                    cbTanqueWrapper.AddParent<Linea>(cbLinea);
                    cbTanqueWrapper.AddParent<Equipo>(cbEquipo);
                    BindingManager.BindTanque(cbTanqueWrapper);
                }
            }
        }

        protected virtual void cbEquipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbEquipo = sender as DropDownList;
            if (cbEquipo == null) return;

            var childs = GetChildrenForControl(cbEquipo.ID);
            foreach (var child in childs)
            {
                if (child.StartsWith(BINDTANQUE))
                {
                    var id = child.Substring(BINDTANQUE.Length);
                    var cbTanque = GetControlFromView(id) as DropDownList;
                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
                    var all = cbTanque.Attributes["AddAllItem"] == "1";
                    var cbTanqueWrapper = new TanqueDropDownListWrapper(cbTanque, all);
                    cbTanqueWrapper.AddParent<Empresa>(cbEmpresa);
                    cbTanqueWrapper.AddParent<Linea>(cbLinea);
                    cbTanqueWrapper.AddParent<Equipo>(cbEquipo);
                    BindingManager.BindTanque(cbTanqueWrapper);
                }
            }
        }

        protected virtual void cbTransportista_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbTransportista = sender as DropDownList;
            if (cbTransportista == null) return;

            var childs = GetChildrenForControl(cbTransportista.ID);
            foreach (var child in childs)
            {
                if (child.StartsWith(BINDCOCHE))
                {
                    var id = child.Substring(BINDCOCHE.Length);
                    var cbVehiculo = GetControlFromView(id) as DropDownList;
                    var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
                    var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
                    var all = cbVehiculo.Attributes["AddAll"] == "1";

                    var cbVehiculoWrapper = new MovilDropDownListWrapper(cbVehiculo, false);
                    cbVehiculoWrapper.AddParent<Empresa>(cbEmpresa);
                    cbVehiculoWrapper.AddParent<Linea>(cbLinea);
                    cbVehiculoWrapper.AddParent<Transportista>(cbTransportista);
                    cbVehiculoWrapper.AddAllItem = all;

                    BindingManager.BindMovil(cbVehiculoWrapper);
                }
            }
        }

        #endregion

        #region SetData Methods

        protected virtual void SetParameterValue(DocumentoValor val)
        {
            var id = (val.Parametro.Repeticion != 1)
                         ? TipoDocumentoHelper.GetControlName(val.Parametro, val.Repeticion)
                         : TipoDocumentoHelper.GetControlName(val.Parametro);

            SetParameterValue(val.Parametro.TipoDato, id, val.Valor);
        }

        protected virtual void SetParameterValue(string tipo, string id, string valor)
        {
            var control = GetControlFromView(id);
            if (control == null) return;

            switch (tipo.ToLower())
            {
                case TipoParametroDocumento.Integer:
                case TipoParametroDocumento.Float:
                case TipoParametroDocumento.String:
                case TipoParametroDocumento.Barcode:
                    SetTextValue(control, valor);
                    break;
                case TipoParametroDocumento.DateTime:
                    SetDateTimeValue(control, valor);
                    break;
                case TipoParametroDocumento.Boolean:
                    SetBooleanValue(control, valor);
                    break;
                case TipoParametroDocumento.Coche:
                case TipoParametroDocumento.Chofer:
                case TipoParametroDocumento.Aseguradora:
                case TipoParametroDocumento.Equipo:
                case TipoParametroDocumento.Cliente:
                case TipoParametroDocumento.Tanque:
                case TipoParametroDocumento.CentroCostos:
                    SetDropDownValue(control, valor);
                    break;
                case TipoParametroDocumento.Image:
                    SetImageValue(control, valor);
                    break;
            }
        }
        

        protected virtual void SetTextValue(Control ctl, string valor)
        {
            var txtText = ctl as TextBox;
            if (txtText == null) return;
            txtText.Text = valor;
        }

        protected virtual void SetBooleanValue(Control ctl, string valor)
        {
            var chkBoolean = ctl as CheckBox;
            if (chkBoolean == null) return;
            chkBoolean.Checked = valor == "true";
        }
        protected virtual void SetDropDownValue(Control ctl, string valor)
        {
            var cbDropDown = ctl as DropDownList;
            if (cbDropDown == null) return;
            var li = cbDropDown.Items.FindByValue(valor);
            if (li != null) li.Selected = true;
        }

        protected virtual void SetDateTimeValue(Control ctl, string valor)
        {
            DateTime date;
            if (!DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return;
            var txtDateTime = ctl as TextBox;
            if (txtDateTime == null) return;
            txtDateTime.Text = date.ToString("dd/MM/yyyy");
        }       
        protected virtual void SetImageValue(Control ctl, string valor)
        {
            var img = GetControlFromView(ctl.ID + "_image") as Image;
            if (img == null) return;
            img.Visible = true;
            img.ImageUrl = TipoDocumentoHelper.UploadDir + valor;
            img.Attributes.Add("onclick", "window.open('" + img.ResolveUrl(img.ImageUrl) + "');");
        }
        #endregion
    }
}