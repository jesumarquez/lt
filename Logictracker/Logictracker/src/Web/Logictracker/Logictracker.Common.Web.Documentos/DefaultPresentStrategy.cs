using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.CustomWebControls.DropDownLists;
using Logictracker.Web.Documentos.Interfaces;

namespace Logictracker.Web.Documentos
{
    public class DefaultPresentStrategy: IPresentStrategy
    {
        #region Fields

        protected readonly Dictionary<string, TipoDocumentoParametro> parametros = new Dictionary<string, TipoDocumentoParametro>();
        protected readonly Dictionary<string, string> styles = new Dictionary<string, string>();
        protected readonly TipoDocumento TipoDocumento;
        protected readonly IDocumentView view;
        protected DAOFactory DAOFactory;
        private int LiteralCount;
        protected string template;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tipoDoc">El tipo de documento a procesar</param>
        /// <param name="view">La vista donde se va a presentar el documento</param>
        /// <param name="daof">DAOFactory</param>
        public DefaultPresentStrategy(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof)
        {
            TipoDocumento = tipoDoc;
            this.view = view;
            DAOFactory = daof;
        } 
        #endregion

        #region IPresentStrategy Members

        public void CrearForm()
        {
            CreateDictionary();

            if (string.IsNullOrEmpty(TipoDocumento.Template))
                CreateWithoutTemplate();
            else
                CreateWithTemplate();
        }

        
        public virtual void SetValores(Documento documento)
        {
            //var cbEmpresa = view.DocumentContainer.FindControl("parenti01") as LocacionDropDownList;
            //var cbLinea = view.DocumentContainer.FindControl("parenti02") as PlantaDropDownList;
            var txtCodigo = view.DocumentContainer.FindControl("codigo") as TextBox;
            var txtFecha = view.DocumentContainer.FindControl("fecha") as TextBox;

            linea = documento.Linea.Id;
            empresa = documento.Linea.Empresa.Id;
            if(txtCodigo != null) txtCodigo.Text = documento.Codigo;
            if(txtFecha != null) txtFecha.Text = documento.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy");

            foreach (DocumentoValor valor in documento.Parametros)
                SetDocValue(valor, valor.Repeticion);

            OnValuesSetted();
        }

        public void SetDefaults()
        {
        }

        public Control GetControlFromView(string id)
        {
            return view.DocumentContainer.FindControl(id);
        }

        #endregion

        #region Creation Methods
        private void CreateWithoutTemplate()
        {
            //Todavia no tenemos una generacion default si no
            //hay un template definido
            throw new NotImplementedException("No se puede generar el formulario sin un template");
        }

        private void CreateWithTemplate()
        {
            var template_file = HttpContext.Current.Server.MapPath("~/Documentos/templates/" + TipoDocumento.Template);

            if (!File.Exists(template_file))
                throw new FileNotFoundException("No se encontro el archivo de Template", TipoDocumento.Template);

            template = File.ReadAllText(template_file);

            CreateStylesDictionary();
            ParseTemplate();
            OnCreated();
        }
        #endregion

        protected virtual void OnCreated()
        {
            
        }
        protected virtual void OnValuesSetted()
        {
            
        }

        #region Dictionary Creation
        /// <summary>
        /// Crea el diccionario de parametros para el tipo de documento actual
        /// </summary>
        private void CreateDictionary()
        {
            foreach (TipoDocumentoParametro parametro in TipoDocumento.Parametros)
                parametros.Add(string.Concat("(((", parametro.Nombre, ")))"), parametro);
        }

        /// <summary>
        /// Crea el diccionario de estilos a partir del template actual y 
        /// elimina la seccion de estilos del template
        /// </summary>
        private void CreateStylesDictionary()
        {
            var parts = template.Split(new[] { CommonTags.BEGIN_STYLE, CommonTags.END_STYLE }, StringSplitOptions.None);

            if (parts.Length == 1) return;
            if (parts.Length > 3) throw new ApplicationException("Solo puede haber una seccion de estilos en el template");

            var styleIdx = 1;
            if (parts.Length == 2)
                styleIdx = (template.StartsWith(CommonTags.BEGIN_STYLE)) ? 0 : 1;

            var elems = parts[styleIdx].Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in elems)
            {
                if (s.IndexOf('=') > -1)
                {
                    var kv = s.Split('=');
                    styles.Add(kv[0].Trim(), kv[1].Trim());
                }
            }

            var tmp = string.Empty;
            for (var i = 0; i < parts.Length; i++)
                if (i != styleIdx) tmp += parts[i];
            template = tmp;
        } 
        #endregion

        #region Parse Template
        /// <summary>
        /// Parsea el template
        /// </summary>
        private void ParseTemplate()
        {
            ParseTemplate(template, -1);
        }

        /// <summary>
        /// Parsea una iteracion del template
        /// </summary>
        /// <param name="tmplt">El texto del template a procesar</param>
        /// <param name="repeat">Numero de iteracion</param>
        private void ParseTemplate(string tmplt, int repeat)
        {
            var startIndex = tmplt.IndexOf("(((");
            var endIndex = tmplt.IndexOf(")))");
            if (startIndex == -1 || endIndex == -1)
            {
                AddLiteral(tmplt);
                return;
            }
            var text = tmplt.Substring(0, startIndex);
            var tag = tmplt.Substring(startIndex, endIndex - startIndex + 3);
            tmplt = tmplt.Substring(endIndex + 3);
            AddLiteral(text);

            var style = styles.ContainsKey(tag) ? styles[tag] : null;

            switch (tag)
            {
                case CommonTags.EMPRESA:
                    AddFieldEmpresa("parenti01", style);
                    break;
                case CommonTags.LINEA:
                    AddFieldLinea("parenti02", style);
                    break;
                case CommonTags.CODIGO:
                    AddFieldCodigo("codigo", style);
                    break;
                case CommonTags.DATE:
                    AddFieldFecha("fecha", style);
                    break;
                case CommonTags.BEGIN_REPEAT:
                    var endRepeaterIndex = tmplt.IndexOf(CommonTags.END_REPEAT);
                    var repeated = tmplt.Substring(0, endRepeaterIndex);

                    var subStartIndex = tmplt.IndexOf("(((");
                    var subEndIndex = tmplt.IndexOf(")))");
                    var firstSubTag = tmplt.Substring(subStartIndex, subEndIndex - subStartIndex + 3);

                    if (!parametros.ContainsKey(firstSubTag)) break;
                    int times = parametros[firstSubTag].Repeticion;
                    if (times == 0) times = 1;

                    for (var i = 0; i < times; i++)
                        ParseTemplate(repeated, i);

                    tmplt = tmplt.Substring(endRepeaterIndex + CommonTags.END_REPEAT.Length);
                    break;
                default:
                    if (parametros.ContainsKey(tag))
                    {
                        var id = parametros[tag].Nombre.Replace(' ', '_');
                        if (repeat > -1) id += repeat;
                        AddParameter(parametros[tag], id, style);
                    }
                    break;
            }

            ParseTemplate(tmplt, repeat);
        } 
        #endregion

        #region Add HardCode Fields
        protected virtual void AddFieldEmpresa(string id, string style)
        {
            AddEmpresa(id, style);
        }

        protected virtual void AddFieldLinea(string id, string style)
        {
            AddLinea(id, style);
        }
        protected virtual void AddFieldCodigo(string id, string style)
        {
            AddTextBox(id, style);
        }
        protected virtual void AddFieldFecha(string id, string style)
        {
            AddDate(id, style);
        }
        
        #endregion

        #region Add Typed Parameters
        protected virtual void AddParameter(TipoDocumentoParametro par, string id, string style)
        {
            switch (par.TipoDato.ToLower())
            {
                case TipoParametroDocumento.Integer:
                    AddParameterInt(par, id, style);
                    break;
                case TipoParametroDocumento.Float:
                    AddParameterFloat(par, id, style);
                    break;
                case TipoParametroDocumento.String:
                    AddParameterString(par, id, style);
                    break;
                case TipoParametroDocumento.StringBarcode:
                    AddParameterString(par, id, style);
                    break;
                case TipoParametroDocumento.DateTime:
                    AddParameterDateTime(par, id, style);
                    break;
                case TipoParametroDocumento.Boolean:
                    AddParameterBoolean(par, id, style);
                    break;
                case TipoParametroDocumento.Coche:
                    AddParameterCoche(par, id, style);
                    break;
                case TipoParametroDocumento.Chofer:
                    AddParameterChofer(par, id, style);
                    break;
                case TipoParametroDocumento.Aseguradora:
                    AddParameterAseguradora(par, id, style);
                    break;
                case TipoParametroDocumento.Equipo:
                    AddParameterEquipo(par, id, style);
                    break;
                case TipoParametroDocumento.CentroCostos:
                    AddParameterCentroCostos(par, id, style);
                    break;
            }
        }

        protected virtual void AddParameterInt(TipoDocumentoParametro par, string id, string style)
        {
            AddTextBox(id, style);
        }

        protected virtual void AddParameterFloat(TipoDocumentoParametro par, string id, string style)
        {
            AddTextBox(id, style);
        }

        protected virtual void AddParameterString(TipoDocumentoParametro par, string id, string style)
        {
            AddTextBox(id, style);
        }

        protected virtual void AddParameterDateTime(TipoDocumentoParametro par, string id, string style)
        {
            AddDate(id, style);
        }

        protected virtual void AddParameterBoolean(TipoDocumentoParametro par, string id, string style)
        {
            AddCheckBox(id, style);
        }

        protected virtual void AddParameterCoche(TipoDocumentoParametro par, string id, string style)
        {
            AddCoche(id, style);
        }

        protected virtual void AddParameterChofer(TipoDocumentoParametro par, string id, string style)
        {
            AddTextBox(id, style);
        }
        protected virtual void AddParameterAseguradora(TipoDocumentoParametro par, string id, string style)
        {
            AddAseguradora(id, style);
        }
        protected virtual void AddParameterEquipo(TipoDocumentoParametro par, string id, string style)
        {
            AddEquipo(id, style);
        }
        protected virtual void AddParameterCentroCostos(TipoDocumentoParametro par, string id, string style)
        {
            AddCentroCostos(id, style);
        } 
        
        #endregion

        #region Add Controls
        protected virtual void AddLiteral(string text)
        {
            var literal = new Literal {ID = ("litContent" + LiteralCount++), Text = text};
            view.DocumentContainer.Controls.Add(literal);
        }
        protected virtual void AddTextBox(string id, string style)
        {
            var textbox = new TextBox {ID = id};
            textbox.Style.Value = style;
            view.DocumentContainer.Controls.Add(textbox);
        }
        protected virtual void AddCheckBox(string id, string style)
        {
            var checkbox = new CheckBox {ID = id};
            checkbox.Style.Value = style;
            view.DocumentContainer.Controls.Add(checkbox);
        }
        protected virtual void AddDate(string id, string style)
        {
            var date = new TextBox {ID = id};
            date.Style.Value = style;
            var calendar = new CalendarExtender {ID = (id + "_calendar"), TargetControlID = date.ClientID};

            view.DocumentContainer.Controls.Add(date);
            view.DocumentContainer.Controls.Add(calendar);
        }
        protected virtual void AddEmpresa(string id, string style)
        {
            var loc = new LocacionDropDownList {ID = id};
            loc.Style.Value = style;
            loc.InitialBinding += cbEmpresa_PreBind;
            view.DocumentContainer.Controls.Add(loc);
        }
        protected virtual void AddLinea(string id, string style)
        {
            var pla = new PlantaDropDownList {ID = id};
            pla.Style.Value = style;
            pla.ParentControls = "parenti01";
            pla.InitialBinding += cbLinea_PreBind;
            view.DocumentContainer.Controls.Add(pla);
        }
        protected virtual void AddCoche(string id, string style)
        {
            var pla = new MovilDropDownList {ID = id};
            pla.Style.Value = style;
            pla.ParentControls = "parenti01,parenti02";
            pla.InitialBinding += cbCoche_PreBind;
            view.DocumentContainer.Controls.Add(pla);
        } 
        protected virtual void AddAseguradora(string id, string style)
        {
            var ase = new TransportistaDropDownList {ID = id};
            ase.Style.Value = style;
            ase.AutoPostBack = true;
            ase.ParentControls = "parenti01,parenti02";
            ase.InitialBinding += cbAseguradora_PreBind;
            view.DocumentContainer.Controls.Add(ase);
        }
        protected virtual void AddEquipo(string id, string style)
        {
            var ddl = new EquipoDropDownList {ID = id};
            ddl.Style.Value = style;
            ddl.ParentControls = "parenti01,parenti02";
            ddl.InitialBinding += cbEquipo_PreBind;
            view.DocumentContainer.Controls.Add(ddl);
        }
        protected virtual void AddCentroCostos(string id, string style)
        {
            var ddl = new CentroDeCostosDropDownList { ID = id };
            ddl.AddNoneItem = true;
            ddl.Style.Value = style;
            ddl.ParentControls = "parenti01,parenti02";
            ddl.InitialBinding += cbCentroCostos_PreBind;
            view.DocumentContainer.Controls.Add(ddl);
        }
        #endregion

        #region SetData Methods

        private int aseguradora;
        private int coche;
        private int empresa;
        private int equipo;
        private int linea;
        private int centrocostos;

        private void SetDocValue(DocumentoValor val, short repeticion)
        {
            var par = val.Parametro;

            var id = par.Nombre.Replace(' ', '_');
            if (par.Repeticion != 1) id += repeticion;

            var ctl = view.DocumentContainer.FindControl(id);
            if (ctl == null) return;
            switch (par.TipoDato.ToLower())
            {
                case TipoParametroDocumento.Integer:
                    SetInt32Value(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.Float:
                    SetFloatValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.String:
                    SetStringValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.StringBarcode:
                    SetStringValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.DateTime:
                    SetDateTimeValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.Boolean:
                    SetBooleanValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.Coche:
                    SetCocheValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.Chofer:
                    SetChoferValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.Aseguradora:
                    SetAseguradoraValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.Equipo:
                    SetEquipoValue(ctl, val, repeticion);
                    break;
                case TipoParametroDocumento.CentroCostos:
                    SetCentroCostosValue(ctl, val, repeticion);
                    break;
            }
        }

        protected virtual void SetInt32Value(Control ctl, DocumentoValor valor, int repeticion)
        {
            var textbox = (ctl as TextBox);
            if (textbox == null) return;
            textbox.Text = valor.Valor;
        }
        protected virtual void SetFloatValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            var textbox = (ctl as TextBox);
            if (textbox == null) return;
            textbox.Text = valor.Valor;
        }
        protected virtual void SetStringValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            var textbox = (ctl as TextBox);
            if (textbox == null) return;
            textbox.Text = valor.Valor;
        }
        protected virtual void SetBooleanValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            var checkbox = (ctl as CheckBox);
            if (checkbox == null) return;
            checkbox.Checked = valor.Valor == "true";
        }

        protected virtual void SetCocheValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            coche = Convert.ToInt32(valor.Valor);
        }
        
        protected virtual void SetDateTimeValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            var d = Convert.ToDateTime(valor.Valor, CultureInfo.InvariantCulture);
            var textbox = (ctl as TextBox);
            if(textbox == null) return;
            textbox.Text = d.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
        }
        protected virtual void SetChoferValue(Control ctl, DocumentoValor valor, int repeticion)
        {

        }
        protected virtual void SetAseguradoraValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            aseguradora = Convert.ToInt32(valor.Valor);
        }
        protected virtual void SetEquipoValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            equipo = Convert.ToInt32(valor.Valor);
        }

        protected virtual void SetCentroCostosValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            centrocostos = Convert.ToInt32(valor.Valor);
        }

        void cbLinea_PreBind(object sender, EventArgs e)
        {
            var cb = (sender as PlantaDropDownList);
            if(cb != null && linea > 0) cb.EditValue = linea;
        }

        void cbEmpresa_PreBind(object sender, EventArgs e)
        {
            var cb = (sender as LocacionDropDownList);
            if(cb != null && empresa > 0) cb.EditValue = empresa;
        }
        void cbCoche_PreBind(object sender, EventArgs e)
        {
            var cb = (sender as MovilDropDownList);
            if(cb != null && coche > 0) cb.EditValue = coche;
        }
        void cbAseguradora_PreBind(object sender, EventArgs e)
        {
            var cb = (sender as TransportistaDropDownList);
            if(cb != null && aseguradora > 0) cb.EditValue = aseguradora;
        }
        void cbEquipo_PreBind(object sender, EventArgs e)
        {
            var cb = (sender as EquipoDropDownList);
            if(cb != null && equipo > 0) cb.EditValue = equipo;
        }
        void cbCentroCostos_PreBind(object sender, EventArgs e)
        {
            var cb = (sender as CentroDeCostosDropDownList);
            if (cb != null && equipo > 0) cb.EditValue = centrocostos;
        }
        
        #endregion
    }
}