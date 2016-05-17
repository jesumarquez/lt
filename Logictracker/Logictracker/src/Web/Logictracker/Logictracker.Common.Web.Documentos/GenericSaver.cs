using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos.Helpers;
using Logictracker.Web.Documentos.Interfaces;

namespace Logictracker.Web.Documentos
{
    public class GenericSaver: ISaverStrategy
    {
        protected readonly TipoDocumento TipoDocumento;
        protected readonly IDocumentView view;
        protected DAOFactory DAOFactory;
        protected List<string> NewImages = new List<string>();
        protected List<string> OldImages = new List<string>();
        protected TipoDocumentoHelper TipoDocumentoHelper;
        private bool editMode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tipoDoc"></param>
        /// <param name="view"></param>
        /// <param name="daof"></param>
        public GenericSaver(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof)
        {
            TipoDocumento = tipoDoc;
            this.view = view;
            DAOFactory = daof;
        }

        #region ISaverStrategy Members

        /// <summary>
        /// Saves the Document.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="user"></param>
        public virtual void Save(Documento doc)
        {
            TipoDocumentoHelper = new TipoDocumentoHelper(TipoDocumento);
            editMode = doc.Id > 0;

            Validate();

            AfterValidate(doc);

            var cbEmpresa = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI01) as DropDownList;
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
            var txtCodigo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CODIGO) as TextBox;
            var txtDescripcion = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_DESCRIPCION) as TextBox;
            var txtFecha = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_FECHA) as TextBox;
            var txtVencimiento = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_VENCIMIENTO) as TextBox;
            var txtPresentacion = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PRESENTACION) as TextBox;
            var txtCierre = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CIERRE) as TextBox;
            var cbEstado = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_ESTADO) as DropDownList;
            var cbVehiculo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI03) as DropDownList;
            var cbEmpleado = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI09) as DropDownList;
            var cbTransportista = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI07) as DropDownList;
            var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;

            var idLinea = Convert.ToInt32(cbLinea.SelectedValue);
            doc.Empresa = DAOFactory.EmpresaDAO.FindById(Convert.ToInt32(cbEmpresa.SelectedValue));
            doc.Linea = idLinea > 0 ? DAOFactory.LineaDAO.FindById(idLinea) : null;
            doc.Codigo = GetRequiredNotEmpty("Codigo", txtCodigo.Text);

            if (TipoDocumento.AplicarAVehiculo)
            {
                var vehiculo = GetRequiredInt("Vehiculo", cbVehiculo.SelectedValue);
                if(vehiculo > 0) doc.Vehiculo = DAOFactory.CocheDAO.FindById(vehiculo);
            }
            if (TipoDocumento.AplicarAEmpleado)
            {
                var empleado = GetRequiredInt("Empleado", cbEmpleado.SelectedValue);
                if(empleado > 0) doc.Empleado = DAOFactory.EmpleadoDAO.FindById(empleado);
            }
            if(TipoDocumento.AplicarATransportista)
            {
                var transportista = GetRequiredInt("Transportista", cbTransportista.SelectedValue);
                if(transportista > 0) doc.Transportista = DAOFactory.TransportistaDAO.FindById(transportista);
            }
            if (TipoDocumento.AplicarAEquipo)
            {
                var equipo = GetRequiredInt("Equipo", cbEquipo.SelectedValue);
                if(equipo > 0) doc.Equipo = DAOFactory.EquipoDAO.FindById(equipo);
            }

            var docs = DAOFactory.DocumentoDAO.FindByTipoYCodigo(TipoDocumento.Id, doc.Codigo);
            if(docs.Count > 0 && (docs[0] as Documento).Id != doc.Id)
                throw new ApplicationException("Ya existe un documento con el mismo Codigo");

            doc.Descripcion = txtDescripcion != null ? txtDescripcion.Text.Trim() : doc.Codigo;
            doc.Fecha = GetRequiredValidDate("Fecha", txtFecha.Text);

            var presentacion =
                txtPresentacion != null
                    ? TipoDocumento.RequerirPresentacion
                          ? GetRequiredValidDate("Fecha de Presentacion", txtPresentacion.Text)
                          : GetValidDate(txtPresentacion.Text)
                    : DateTime.UtcNow;

            doc.Presentacion = presentacion == DateTime.MinValue ? (DateTime?) null : presentacion;

            var vencimiento = TipoDocumento.RequerirVencimiento ? GetRequiredValidDate("Fecha de Vencimiento", txtVencimiento.Text)
                : txtVencimiento != null ? GetValidDate(txtVencimiento.Text): DateTime.MinValue;

            var actualizaVencimiento = doc.Vencimiento != vencimiento;
            doc.Vencimiento = vencimiento == DateTime.MinValue ? (DateTime?) null : vencimiento;
            if (actualizaVencimiento)
            {
                doc.EnviadoAviso1 = false;
                doc.EnviadoAviso2 = false;
                doc.EnviadoAviso3 = false;

            }

            var cierre = txtCierre != null ? GetValidDate(txtCierre.Text) : DateTime.MinValue;
            doc.FechaCierre = cierre == DateTime.MinValue ? (DateTime?) null : cierre;
            
            doc.TipoDocumento = TipoDocumento;

            doc.Estado = cbEstado != null ? Convert.ToInt16(cbEstado.SelectedValue) : (short)0;

            if (doc.Id == 0)
            {
                doc.FechaAlta = DateTime.UtcNow;
                doc.Usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
            }
            else
            {
                doc.FechaModificacion = DateTime.UtcNow;
                doc.UsuarioModificacion = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
            }

            foreach (TipoDocumentoParametro parametro in doc.TipoDocumento.Parametros)
            {
                if(parametro.Repeticion == 1)
                {
                    var valor = GetParameterValue(parametro, 1);
                    if(valor != null)
                        SetValor(doc, parametro.Nombre, 1, valor);
                }
                else
                    for (short i = 0; i < parametro.Repeticion; i++)
                    {
                        var valor = GetParameterValue(parametro, i);
                        SetValor(doc, parametro.Nombre, i, valor);
                    }
            }

            BeforeSave(doc);

            DAOFactory.DocumentoDAO.SaveOrUpdate(doc);
        }

        #endregion

        protected virtual void AfterValidate(Documento doc){}
        protected virtual void BeforeSave(Documento doc) {}

        public static string GetCurrentValue(Documento doc, string campo, int repeticion)
        {
            var param = (from DocumentoValor v in doc.Parametros
                         where v.Parametro.Nombre == campo
                               && v.Repeticion == repeticion
                         select v).ToList();

            var val = param.Count > 0 ? param[0] : null;

            return (val == null) ? null : val.Valor;
        }

        public static void SetValor(Documento doc, string campo, int repeticion, string valor)
        {
            var param = (from DocumentoValor v in doc.Parametros
                         where v.Parametro.Nombre == campo
                               && v.Repeticion == repeticion
                         select v).ToList();

            var val = param.Count > 0 ? param[0] : null;

            if (val == null)
            {
                var par = from TipoDocumentoParametro p in doc.TipoDocumento.Parametros
                          where p.Nombre == campo
                          select p;
                val = new DocumentoValor
                          {
                              Documento = doc,
                              Parametro = par.ToList()[0],
                              Repeticion = ((short) repeticion)
                          };
                doc.Parametros.Add(val);
            }
            else if(valor == null)
                doc.Parametros.Remove(val);

            val.Valor = valor;
        }

        /// <summary>
        /// Validates the minimum requirements of the document.
        /// </summary>
        protected virtual void Validate()
        {
            var cbLinea = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI02) as DropDownList;
            var txtCodigo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CODIGO) as TextBox;
            var txtFecha = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_FECHA) as TextBox;

            if(cbLinea == null) throw new ApplicationException("No se encontro el campo Base");
            if(txtCodigo == null) throw new ApplicationException("No se encontro el campo Codigo");
            if(txtFecha == null) throw new ApplicationException("No se encontro el campo Fecha");
        }

        protected static string GetRequiredNotEmpty(string campo, string valor)
        {
            if(string.IsNullOrEmpty(valor.Trim()))
                throw new ApplicationException("El campo " + campo + " no puede estar vacío");

            return valor.Trim();
        }

        /// <summary>
        /// Gets a Valid DateTime value. Throws exception at error.
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected static DateTime GetRequiredValidDate(string campo, string valor)
        {
            DateTime date;
            if(!DateTime.TryParse(valor, new CultureInfo("es-AR"), DateTimeStyles.None, out date))
                throw new ApplicationException("El valor del campo " + campo + " no tiene el formato correcto (dd/MM/yyyy)");

            if(date == DateTime.MinValue)
                throw new ApplicationException("El valor del campo " + campo + " no es valido");

            return date;
        }

        /// <summary>
        /// Gets a Valid DateTime value. 
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected static DateTime GetValidDate(string valor)
        {
            DateTime date;
            if (!DateTime.TryParse(valor, new CultureInfo("es-AR"), DateTimeStyles.None, out date))
                return DateTime.MinValue;

            return date;
        }

        /// <summary>
        /// Gets an int value. Throws exception if fails.
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected static int GetRequiredInt(string campo, string valor)
        {
            int number;
            if (!int.TryParse(valor, out number))
                throw new ApplicationException("El valor del campo " + campo + " debe ser numerico");

            return number;
        }

        /// <summary>
        /// Gets an int value. 
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected static int GetInt(string valor)
        {
            int number;
            if (!int.TryParse(valor, out number))
                return -999;

            return number;
        }

        /// <summary>
        /// Gets a float value. Throws exception if fails.
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected static float GetRequiredFloat(string campo, string valor)
        {
            float number;
            if (!float.TryParse(valor,NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                throw new ApplicationException("El valor del campo " + campo + " debe ser numerico");

            return number;
        }

        /// <summary>
        /// Gets a float value.
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected static float GetFloat(string valor)
        {
            float number;
            if (!float.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                return -999;

            return number;
        }
        protected string GetRequiredFile(string campo, FileUpload control)
        {
            if(!control.HasFile)
                throw new ApplicationException("La imagen " + campo + " es obligatoria");


            return GetFile(campo, control);
        }
        protected string GetFile(string campo, FileUpload control)
        {
            if (!control.HasFile) return null;

            var uploadPath = HttpContext.Current.Server.MapPath(TipoDocumentoHelper.UploadDir);
            if (!Directory.Exists(uploadPath))
            {
                try
                {
                    Directory.CreateDirectory(uploadPath);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException("No se puede crear el directorio de Upload: " + ex.Message);
                }
            }
            if(!control.FileName.ToLower().EndsWith(".gif")
               && !control.FileName.ToLower().EndsWith(".jpg")
               && !control.FileName.ToLower().EndsWith(".png"))
                throw new ApplicationException("El archivo " + campo + " debe tener una de las siguientes extensiones: .gif, .jpg, .png");

            var filename = control.FileName;
            var name = filename.Substring(0, filename.LastIndexOf('.'));
            var ext = filename.Substring(filename.LastIndexOf('.') + 1);

            var cnt = 1;
            while (File.Exists(uploadPath + filename))
            {
                filename = string.Concat(name,"(", cnt,").", ext);
                cnt++;
            }
            control.SaveAs(uploadPath + filename);
            return filename;
        }

        protected Control GetControlFromView(string id)
        {
            return view.DocumentContainer.FindControl(id);
        }

        protected virtual string GetParameterValue(TipoDocumentoParametro parameter, short repeticion)
        {
            var id = (parameter.Repeticion != 1)
                            ? TipoDocumentoHelper.GetControlName(parameter, repeticion)
                            : TipoDocumentoHelper.GetControlName(parameter);

            var control = GetControlFromView(id);
            if (control == null) return null;

            string textValue;
            switch (parameter.TipoDato.ToLower())
            {
                case TipoParametroDocumento.Integer:
                    textValue = GetTextBoxValue(id);
                    if (parameter.Obligatorio)
                        return GetRequiredInt(parameter.Nombre, textValue).ToString();

                    var integer = GetInt(textValue);
                    if (integer == -999) return null;
                    return integer.ToString();
                case TipoParametroDocumento.Float:
                    textValue = GetTextBoxValue(id).Replace(',','.');
                    if (parameter.Obligatorio)
                        return GetRequiredFloat(parameter.Nombre, textValue).ToString();
                    var floatnum = GetFloat(textValue);
                    if (floatnum == -999) return null;
                    return floatnum.ToString();
                case TipoParametroDocumento.String:
                case TipoParametroDocumento.Barcode:
                    textValue = GetTextBoxValue(id);
                    return parameter.Obligatorio
                               ? GetRequiredNotEmpty(parameter.Nombre, textValue)
                               : textValue;
                case TipoParametroDocumento.DateTime:
                    textValue = GetTextBoxValue(id);
                    if(parameter.Obligatorio)
                        return GetRequiredValidDate(parameter.Nombre, textValue).ToString(CultureInfo.InvariantCulture);

                    var date = GetValidDate(textValue);
                    if(date == DateTime.MinValue) return null;
                    return date.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Boolean:
                    return GetCheckBoxValue(id) ? "true" : "false";
                case TipoParametroDocumento.Coche:
                case TipoParametroDocumento.Chofer:
                case TipoParametroDocumento.Aseguradora:
                case TipoParametroDocumento.Equipo:
                case TipoParametroDocumento.Cliente:
                case TipoParametroDocumento.Tanque:
                case TipoParametroDocumento.CentroCostos:
                    textValue = GetDropDownListValue(id);
                    if (parameter.Obligatorio)
                        return GetRequiredInt(parameter.Nombre, textValue).ToString();

                    var number = GetInt(textValue);
                    if (number == 0) return null;
                    return number.ToString();
                case TipoParametroDocumento.Image:
                    if (parameter.Obligatorio && !editMode)
                        return GetRequiredFile(parameter.Nombre, control as FileUpload);
                    return GetFile(parameter.Nombre, control as FileUpload);
            }

            return null;
        }

        protected string GetTextBoxValue(string id)
        {
            var txtTextBox = GetControlFromView(id) as TextBox;
            if (txtTextBox == null) return string.Empty;
            return txtTextBox.Text;
        }
        protected bool GetCheckBoxValue(string id)
        {
            var chkBoolean = GetControlFromView(id) as CheckBox;
            if (chkBoolean == null) return false;
            return chkBoolean.Checked;
        }
        protected string GetDropDownListValue(string id)
        {
            var cbDropDown = GetControlFromView(id) as DropDownList;
            if (cbDropDown == null) return string.Empty;
            return cbDropDown.SelectedValue;
        }
    }
}