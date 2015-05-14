#region Usings

using System;
using System.Collections.Generic;
using System.Net;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Model
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Obtiene un diccionario poblado desde el componente Query de una Uri
        /// parseado segun el formato donde '&' separa registros y '=' separa 
        /// la clave del valor. 
        /// </summary>
        /// <remarks>
        /// En caso de parametros repetidos se sobreescribe al ultimo valor, si 
        /// falta el caracter '=' se saltea dicho parametro.
        /// </remarks>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryToDictionary(this Uri uri)
        {
            try
            {
                var result = new Dictionary<string, string>();
                var query = uri.GetComponents(UriComponents.Query, UriFormat.Unescaped);
                var nameValuePairs = query.TrimStart('?').Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in nameValuePairs)
                {
                    var vars = pair.Split('=');
                    if (vars.GetLength(0) != 2)
                    {
						SimpleTrace.Trace(typeof(ExtensionMethods).FullName,"QueryToDictionary, Invalid Query Format at pair '{0}' of '{1}'", pair, uri.Query);
                        continue;
                    }
                    if (!result.ContainsKey(vars[0]))
                    {
                        result.Add(vars[0], vars[1]);
                    } else
                    {
                        result[vars[0]] = vars[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
				SimpleTrace.Exception(typeof(ExtensionMethods).FullName,e, "Uri.QueryToDictionary");
                return null;
            }
        }

        /// <summary>
        /// Obtiene del componente Query de una Uri, el valor correspondienta al campo 
        /// especificado.
        /// </summary>
        /// <param name="uri">Objeto Uri</param>
        /// <param name="field">Campo a obtener</param>
        /// <param name="default">valor por defecto si el campo no esta presente.</param>
        /// <returns></returns>
        public static string GetQueryField(this Uri uri, string field, string @default)
        {
            try
            {
                var d = uri.QueryToDictionary();
                return d.ContainsKey(field) ? d[field] : @default;
            }
            catch (Exception e)
            {
                SimpleTrace.Exception(typeof(ExtensionMethods).FullName,e, "Uri.GetQueryField");
                return null;
            }
        }

        /// <summary>
        /// Obtiene del componente Path de una Uri.
        /// </summary>
        /// <param name="uri">Objeto Uri</param>
        /// <returns></returns>
        public static string GetPath(this Uri uri)
        {
            try
            {
                return uri.GetComponents(UriComponents.Path,UriFormat.Unescaped);
            }
            catch (Exception e)
            {
                SimpleTrace.Exception(typeof(ExtensionMethods).FullName,e, "Uri.GetPath");
                return null;
            }
        }

        /// <summary>
        /// Obtien el componente Host:Port de una Uri en forma de IPEndPoint.
        /// </summary>
        /// <param name="uri">Objeto Uri</param>
        /// <remarks>
        /// La clase esta especializada para los esquemas "utn.service" y "utn.device".
        /// Cualquier otro esquema retorna null.
        /// </remarks>
        /// <returns>si es valido, el IPEndPoint equivalente, sino null.</returns>
        public static IPEndPoint GetIPEndPoint(this Uri uri)
        {
            try
            {
                if (uri == null) return null;
                if (uri.Scheme == "utn.service")
                {
                    return new IPEndPoint(IPAddress.Any, uri.Port);
                }
                if (uri.Scheme == "utn.device")
                {
                    var addr = IPAddress.Parse(uri.Host) ?? Dns.GetHostAddresses(uri.Host)[0];
                    return addr == null ? null : new IPEndPoint(addr, uri.Port);
                }
                return null;
            }
            catch (Exception e) 
            {
                SimpleTrace.Exception(typeof(ExtensionMethods).FullName,e, "Uri.GetIPEndPoint");
                return null;
            }
        }
        
    }
}