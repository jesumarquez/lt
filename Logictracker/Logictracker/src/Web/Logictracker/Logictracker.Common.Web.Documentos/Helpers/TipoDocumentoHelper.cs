#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Web.Documentos.Helpers
{
    public class TipoDocumentoHelper
    {
        public const string CONTROL_NAME_CODIGO = "__codigo";
        public const string CONTROL_NAME_DESCRIPCION = "__descripcion";
        public const string CONTROL_NAME_ESTADO = "__estado";
        public const string CONTROL_NAME_FECHA = "__fecha";
        public const string CONTROL_NAME_PARENTI01 = "__parenti01";
        public const string CONTROL_NAME_PARENTI02 = "__parenti02";
        public const string CONTROL_NAME_PARENTI03 = "__parenti03";
        public const string CONTROL_NAME_PARENTI07 = "__parenti07";
        public const string CONTROL_NAME_PARENTI09 = "__parenti09";
        public const string CONTROL_NAME_PARENTI19 = "__parenti19";
        public const string CONTROL_NAME_PRESENTACION = "__presentacion";
        public const string CONTROL_NAME_VENCIMIENTO = "__vencimiento";
        public const string CONTROL_NAME_CIERRE = "__cierre";
        public const string UploadDir = "~/Documentos/upload/";


        protected Dictionary<string, TipoDocumentoParametro> parametros = new Dictionary<string, TipoDocumentoParametro>();

        public TipoDocumentoHelper(TipoDocumento tipo)
        {
            TipoDocumento = tipo;
            foreach (TipoDocumentoParametro parametro in tipo.Parametros)
                parametros.Add(string.Concat("(((", parametro.Nombre,")))"), parametro);
        }

        public TipoDocumento TipoDocumento{ get; set;}

        public string GetControlName(TipoDocumentoParametro parametro)
        {
            return parametro.Nombre.Replace(' ', '_');
        }
        public string GetControlName(TipoDocumentoParametro parametro, short repeticion)
        {
            return parametro.Nombre.Replace(' ', '_') + repeticion;
        }
        public TipoDocumentoParametro GetByName(string name)
        {
            var n = !name.StartsWith("(((") ? string.Concat("(((", name, ")))") : name;
            if (!parametros.ContainsKey(n)) return null;
            return parametros[n];
        }
        public List<TipoDocumentoParametro> GetParametros()
        {
            return (from TipoDocumentoParametro p in TipoDocumento.Parametros orderby p.Orden select p).ToList();
        }
    }
}