#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

#endregion

namespace Logictracker.Web.Documentos.Helpers
{
    internal class TemplateParser: BaseParser
    {
        protected readonly Dictionary<string, string> styles = new Dictionary<string, string>();
        private readonly string template;
        private string templateData;

        public TemplateParser(string template, TipoDocumentoHelper tipoDocHelper)
            : base(tipoDocHelper)
        {
            this.template = HttpContext.Current.Server.MapPath("~/Documentos/templates/" +template);
            
            if (!File.Exists(this.template))
                throw new FileNotFoundException("No se encontro el archivo de Template", template);

            LoadTemplate();
        }

        private void LoadTemplate()
        {
            templateData = File.ReadAllText(template);
            CreateStylesDictionary();
        }

        /// <summary>
        /// Parsea el template
        /// </summary>
        public override void Parse()
        {
            ParseTemplate(templateData, -1);
        }

        #region Parse Template
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
                OnLiteral(tmplt);
                return;
            }
            var text = tmplt.Substring(0, startIndex);
            var tag = tmplt.Substring(startIndex, endIndex - startIndex + 3);
            tmplt = tmplt.Substring(endIndex + 3);
            OnLiteral(text);

            var style = styles.ContainsKey(tag) ? styles[tag] : null;

            switch (tag)
            {
                case CommonTags.EMPRESA:
                    OnBase(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI01, style));
                    break;
                case CommonTags.LINEA:
                    OnPlanta(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI02, style));
                    break;
                case CommonTags.COCHE:
                    OnVehiculo(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI03, style));
                    break;
                case CommonTags.TRANSPORTISTA:
                    OnTransportista(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI07, style));
                    break;
                case CommonTags.CHOFER:
                    OnEmpleado(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI09, style));
                    break;
                case CommonTags.EQUIPO:
                    OnEquipo(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI19, style));
                    break;
                case CommonTags.CODIGO:
                    OnCodigo(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_CODIGO, style));
                    break;
                case CommonTags.DESCRIPCION:
                    OnDescripcion(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_DESCRIPCION, style));
                    break;
                case CommonTags.DATE:
                    OnFecha(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_FECHA, style));
                    break;
                case CommonTags.PRESENTACION:
                    OnPresentacion(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PRESENTACION, style));
                    break;
                case CommonTags.VENCIMIENTO:
                    OnVencimiento(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_VENCIMIENTO, style));
                    break;
                case CommonTags.CIERRE:
                    OnCierre(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_CIERRE, style));
                    break;
                case CommonTags.ESTADO:
                    OnEstado(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_ESTADO, style));
                    break;
                case CommonTags.BEGIN_REPEAT:
                    var endRepeaterIndex = tmplt.IndexOf(CommonTags.END_REPEAT);
                    var repeated = tmplt.Substring(0, endRepeaterIndex);

                    var subStartIndex = tmplt.IndexOf("(((");
                    var subEndIndex = tmplt.IndexOf(")))");
                    var firstSubTag = tmplt.Substring(subStartIndex, subEndIndex - subStartIndex + 3);

                    var parametro = TipoDocumentoHelper.GetByName(firstSubTag);
                    if (parametro == null) break;
                    int times = parametro.Repeticion;
                    if (times == 0) times = 1;

                    for (var i = 0; i < times; i++)
                        ParseTemplate(repeated, i);

                    tmplt = tmplt.Substring(endRepeaterIndex + CommonTags.END_REPEAT.Length);
                    break;
                default:
                    var parametroD = TipoDocumentoHelper.GetByName(tag);
                    if (parametroD != null)
                    {
                        var id = TipoDocumentoHelper.GetControlName(parametroD);
                        if (repeat > -1) id += repeat;
                        OnParametro(new DocumentoParametroEventArgs(id, style, parametroD));
                    }
                    break;
            }

            ParseTemplate(tmplt, repeat);
        }
        #endregion

        #region CreateStylesDictionary
        /// <summary>
        /// Crea el diccionario de estilos a partir del template actual y 
        /// elimina la seccion de estilos del template
        /// </summary>
        private void CreateStylesDictionary()
        {
            var parts = templateData.Split(new[] { CommonTags.BEGIN_STYLE, CommonTags.END_STYLE }, StringSplitOptions.None);

            if (parts.Length == 1) return;
            if (parts.Length > 3) throw new ApplicationException("Solo puede haber una seccion de estilos en el template");

            var styleIdx = 1;
            if (parts.Length == 2)
                styleIdx = (templateData.StartsWith(CommonTags.BEGIN_STYLE)) ? 0 : 1;

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

            templateData = tmp;
        } 
        #endregion
    }
}