using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.CustomWebControls.DropDownLists;
using Logictracker.Web.Documentos.Interfaces;

namespace Logictracker.Web.Documentos
{
    public class DefaultSaveStrategy: ISaveStrategy
    {
        protected readonly Dictionary<string, TipoDocumentoParametro> parametros = new Dictionary<string, TipoDocumentoParametro>();
        protected readonly Dictionary<string, string> styles = new Dictionary<string, string>();
        protected readonly TipoDocumento TipoDocumento;
        protected readonly IDocumentView view;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tipoDoc">El tipo de documento a procesar</param>
        /// <param name="view">La vista donde se va a presentar el documento</param>
        public DefaultSaveStrategy(TipoDocumento tipoDoc, IDocumentView view)
        {
            TipoDocumento = tipoDoc;
            this.view = view;
        }

        #region ISaveStrategy Members

        public virtual void Save(Documento doc, int idUsuario, DAOFactory daoF)
        {
            FillValues(doc, idUsuario, daoF);
            Validate(doc, idUsuario, daoF);
            SaveInDB(doc, daoF);
        }

        #endregion

        protected virtual Documento FillValues(Documento doc, int idUsuario, DAOFactory daoF)
        {
            doc.Empresa = Empresa > 0 ? daoF.EmpresaDAO.FindById(Empresa) : null;
            doc.Linea = Linea > 0 ? daoF.LineaDAO.FindById(Linea) : null;
            doc.Codigo = Codigo;
            doc.Fecha = Fecha;
            doc.FechaAlta = DateTime.UtcNow;
            doc.TipoDocumento = TipoDocumento;
            doc.Usuario = daoF.UsuarioDAO.FindById(idUsuario);
            doc.Descripcion = Descripcion;

            var valoresDic = new Dictionary<string, DocumentoValor>();
            foreach (DocumentoValor valor in doc.Parametros)
                valoresDic.Add(valor.Parametro.Nombre+"_"+valor.Repeticion, valor);

            foreach (var value in Valores)
            {
                if (!value.Parametro.Obligatorio && string.IsNullOrEmpty(value.Valor)) continue;

                value.Documento = doc;
                var key = value.Parametro.Nombre + "_" + value.Repeticion;
                if (valoresDic.ContainsKey(key))
                    valoresDic[key].Valor = value.Valor;
                else
                    doc.Parametros.Add(value);
            }
            return doc;
        }
        protected virtual void SaveInDB(Documento doc, DAOFactory daoF) { daoF.DocumentoDAO.SaveOrUpdate(doc); }

        protected virtual void Validate(Documento doc, int idUsuario, DAOFactory daoF)
        {
            if (string.IsNullOrEmpty(Codigo))
                throw new ApplicationException("El codigo del documento no puede estar vacio");

            if (Fecha == DateTime.MinValue)
                throw new ApplicationException("Ingrese una fecha valida");
        }

        protected virtual DocumentoValor GetDocValue(TipoDocumentoParametro par, short repeticion)
        {
            var val = new DocumentoValor();
            val.Parametro = par;
            val.Repeticion = repeticion;

            var id = par.Nombre.Replace(' ', '_');
            if (par.Repeticion != 1) id += repeticion;

            var ctl = view.DocumentContainer.FindControl(id);
            if (ctl == null) return null;

            switch (par.TipoDato.ToLower())
            {
                case TipoParametroDocumento.Integer:
                    var ival = (ctl as TextBox).Text;
                    if (!par.Obligatorio && string.IsNullOrEmpty(ival)) return null;

                    int i;
                    if (!int.TryParse(ival, out i))
                        throw new ApplicationException("El campo " + par.Nombre + " debe ser un numerico entero");
                    val.Valor = i.ToString();
                    break;
                case TipoParametroDocumento.Float:
                    var fval = (ctl as TextBox).Text;
                    if (!par.Obligatorio && string.IsNullOrEmpty(fval))
                        break;
                    fval = fval.Replace(',', '.');
                    double f;
                    if (!double.TryParse(fval, NumberStyles.Any, CultureInfo.InvariantCulture, out f))
                        throw new ApplicationException("El campo " + par.Nombre + " debe ser un numerico decimal");
                    val.Valor = f.ToString(CultureInfo.InvariantCulture);
                    break;
                case TipoParametroDocumento.String:
                    var sval = (ctl as TextBox).Text;

                    if (string.IsNullOrEmpty(sval))
                        if (par.Obligatorio)
                            throw new ApplicationException("El campo " + par.Nombre + " no puede estar vacio");
                        else
                            break;
                    val.Valor = sval;
                    break;
                case TipoParametroDocumento.Barcode:
                    var svalcode = (ctl as TextBox).Text;

                    if (string.IsNullOrEmpty(svalcode))
                        if (par.Obligatorio)
                            throw new ApplicationException("El campo " + par.Nombre + " no puede estar vacio");
                        else
                            break;
                    val.Valor = svalcode;
                    break;
                case TipoParametroDocumento.DateTime:
                    var dval = (ctl as TextBox).Text;
                    if (!par.Obligatorio && string.IsNullOrEmpty(dval))
                        break;
                    DateTime d;
                    if (!DateTime.TryParse(dval, new CultureInfo("es-AR"), DateTimeStyles.None, out d))
                        throw new ApplicationException("El campo " + par.Nombre + " debe ser una fecha valida");
                    val.Valor = d.ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture);
                    break;
                case TipoParametroDocumento.Boolean:
                    val.Valor = (ctl as CheckBox).Checked ? "true" : "false";
                    break;
                case TipoParametroDocumento.Coche:
                    var md = (ctl as MovilDropDownList);
                    if (md.SelectedIndex < 0)
                        if (par.Obligatorio)
                            throw new ApplicationException("Debe seleccionar un valor para el campo " + par.Nombre);
                        else
                            break;
                    val.Valor = md.SelectedValue;
                    break;
                case TipoParametroDocumento.Chofer:
                    /*MovilDropDownList md = (ctl as MovilDropDownList);
                    if (md.SelectedIndex < 0)
                        if (par.Obligatorio)
                            throw new ApplicationException("Debe seleccionar un valor para el campo " + par.Nombre);
                        else
                            break;
                    val.Valor = md.SelectedValue;*/
                    break;
                case TipoParametroDocumento.Aseguradora:
                    var ase = (ctl as TransportistaDropDownList);
                    if (ase.SelectedIndex < 0)
                        if (par.Obligatorio)
                            throw new ApplicationException("Debe seleccionar un valor para el campo " + par.Nombre);
                        else
                            break;
                    val.Valor = ase.SelectedValue;
                    break;
                case TipoParametroDocumento.Equipo:
                    var eq = (ctl as EquipoDropDownList);
                    if (eq.SelectedIndex < 0)
                        if (par.Obligatorio)
                            throw new ApplicationException("Debe seleccionar un valor para el campo " + par.Nombre);
                        else
                            break;
                    val.Valor = eq.SelectedValue;
                    break;
                case TipoParametroDocumento.CentroCostos:
                    var cc = (ctl as CentroDeCostosDropDownList);
                    if (cc.SelectedIndex < 0)
                        if (par.Obligatorio)
                            throw new ApplicationException("Debe seleccionar un valor para el campo " + par.Nombre);
                        else
                            break;
                    val.Valor = cc.SelectedValue;
                    break;
            }

            return val;
        }

        #region Properties
        protected virtual int Empresa
        {
            get
            {
                var cbEmpresa = view.DocumentContainer.FindControl("parenti01") as LocacionDropDownList;
                return cbEmpresa != null ? Convert.ToInt32(cbEmpresa.SelectedValue) : -1;
            }
        }

        protected virtual int Linea
        {
            get
            {
                var cbLinea = view.DocumentContainer.FindControl("parenti02") as PlantaDropDownList;
                return cbLinea != null ? Convert.ToInt32(cbLinea.SelectedValue) : -1;
            }
        }

        protected virtual string Codigo
        {
            get
            {
                var txtCodigo = view.DocumentContainer.FindControl("codigo") as TextBox;
                return txtCodigo.Text.Trim();
            }
        }

        protected virtual string Descripcion
        {
            get { return string.Concat(TipoDocumento.Descripcion, " ", Codigo, " - ", Fecha.ToString("dd/MM/yyyy")); }
        }

        protected virtual DateTime Fecha
        {
            get
            {
                var txtFecha = view.DocumentContainer.FindControl("fecha") as TextBox;
                DateTime dt;
                if (DateTime.TryParse(txtFecha.Text, new CultureInfo("es-AR"), DateTimeStyles.None, out dt))
                    return dt;

                throw new ApplicationException("Debe ingresar una fecha");
            }
        }

        protected virtual List<DocumentoValor> Valores
        {
            get
            {
                var valores = new List<DocumentoValor>();
 
                var unicos = from TipoDocumentoParametro p in TipoDocumento.Parametros 
                             where p.Repeticion == 1
                             select p;

                var repetidos = from TipoDocumentoParametro p in TipoDocumento.Parametros
                                where p.Repeticion > 1
                                select p;

                foreach (var unico in unicos)
                {
                    var val = GetDocValue(unico, 1);
                    if (val != null) valores.Add(val);
                }

                for (short i = 0; i < repetidos.ToList()[0].Repeticion; i++)
                {
                    var rowWithData = false;
                    Exception lastException = null;
                    foreach (var repetido in repetidos)
                    {
                        try
                        {
                            //string parName = repetido.Nombre.Replace(' ', '_');
                            var val = GetDocValue(repetido, i);
                            if (val != null)
                            {
                                valores.Add(val);
                                rowWithData = true;
                            }
                        }
                        catch(Exception ex)
                        {
                            lastException = ex;
                        }
                    }

                    if(rowWithData && lastException != null)
                        throw lastException;
                    if (i == 0 && !rowWithData)
                        throw new ApplicationException("Ingrese datos");
                }
                return valores;
            }
        }
        #endregion
    }
}