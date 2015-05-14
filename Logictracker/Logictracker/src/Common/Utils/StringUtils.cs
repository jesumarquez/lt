#region Usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Utils
{
    public static class StringUtils
    {

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        #region Funciones de BitStrings

        public static byte[] BitsStringToByteArray(String bitString)
        {
            return Enumerable.Range(0, bitString.Length/8)
                .Select(pos => Convert.ToByte(bitString.Substring(pos*8, 8), 2))
                .ToArray();
        }

        public static String MakeString(byte[] ba)
        {
            if (ba == null) return string.Empty;
            return ba.Any(c => (c > 0x7F) || (c < 0x20)) // && !(new byte[] {9, 10, 13}).Contains(c)
                       ? ByteArrayToHexString(ba, 0, ba.Length)
                       : Encoding.ASCII.GetString(ba);
        }

        #endregion

        #region Funciones de HexaStrings

        public static bool AreBitsSet(byte entrada, int mask)
        {
            return (entrada & mask) == mask;
        }

        public static bool AreBitsSet(String entrada, int mask)
        {
            return (Convert.ToInt16(entrada, 16) & mask) == mask;
        }

        public static bool AreBitsSet(char entrada, int mask)
        {
            return AreBitsSet(entrada.ToString(CultureInfo.InvariantCulture), mask);
        }

        public static String RfidHexaFromBase36(String entrada)
        {
            const String clist = "0123456789abcdefghijklmnopqrstuvwxyz";
            long result = 0;
            int pos = 0;
            for (int i = entrada.Length - 1; i > -1; i--)
            {
                char c = Char.ToLower(entrada[i]);
                result += clist.IndexOf(c)*(long) Math.Pow(36, pos);
                pos++;
            }
            return result.ToString("X10");
        }

        private static String ToHexString(UInt64 value, int bytes)
        {
            string result = string.Format("{0:x}", value).ToUpper();
            while (result.Length < (bytes*2))
            {
                result = "0" + result;
            }
            return result;
        }

        public static string UInt16ToHexString(UInt16 value)
        {
            return ToHexString(value, 2);
        }

        public static String ByteToHexString(byte _byte)
        {
            return ToHexString(_byte, 1);
        }

        public static String ByteArrayToHexString(byte[] bytes, int index, int count)
        {
            var result = new StringBuilder();

            foreach (byte b in bytes.Skip(index).Take(count).ToArray())
            {
                result.Append(ByteToHexString(b));
            }
            return result.ToString();
        }

        public static List<byte> HexStringToByteList(String hex, int index)
        {
            int count = hex.Length - index;
            var result = HexStringToByteList(hex, index, count);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex">string de hexadecimales</param>
        /// <param name="index">indice del primer caracter a convertir</param>
        /// <param name="count">cantidad de _caracteres_ a convertir a numeros</param>
        /// <returns></returns>
        public static List<byte> HexStringToByteList(String hex, int index, int count)
        {
            if (count%2 != 0) count -= 1;

            return Enumerable
                .Range(0, count)
                .Where(i => 0 == i%2)
                .Select(i => Convert.ToByte(hex.Substring(index + i, 2), 16))
                .ToList();
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static String ToBase36(UInt64 input)
        {
            const String CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            char[] clistarr = CharList.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(clistarr[input%36]);
                input /= 36;
            }
            return new String(result.ToArray());
        }

        #endregion

        #region Funciones de String

        private static readonly char[] Gap = {'<', '>', '\\'};

        public static String ReplaceNonPrintableCharactersAndGap(String s)
        {
            var result = new StringBuilder(); //s.Length); el tamaño puede ser mayor si hay chars de control
            foreach (char c in s)
            {
                if (char.IsControl(c) || (c == Gap[0]) || (c == Gap[1]) || (c == Gap[2]))
                    result.Append(String.Format(@"\{0:X2}", (byte) c));
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        public static String GetParam(String param, String container)
        {
            try
            {
                param += "=";
                int ini = container.IndexOf(param) + param.Length;
                if (ini < param.Length) return null;
                int len = container.IndexOf(";", ini, StringComparison.Ordinal) - ini;
                return container.Substring(ini, len);
            }
            catch
            {
                return String.Empty;
            }
        }

        public static Int32 GetParamAsInt(String param, String container)
        {
            try
            {
                return Convert.ToInt32(GetParam(param, container));
            }
            catch
            {
                return 0;
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
        public static String GetQueryField(this Uri uri, String field, String @default)
        {
            try
            {
                Dictionary<string, string> d = uri.QueryToDictionary();
                return d.ContainsKey(field) ? d[field] : @default;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof (StringUtils).FullName, e);
                return null;
            }
        }

        public static T GetSerialQueryField<T>(this Uri uri, String field, String @default)
        {
            return new JavaScriptSerializer().Deserialize<T>(GetQueryField(uri, field, @default));
        }

        private static Dictionary<String, String> QueryToDictionary(this Uri uri)
        {
            try
            {
                var result = new Dictionary<String, String>();
                string query = uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
//               STrace.Debug(typeof(StringUtils).FullName, "QUERY: " + query);
                NameValueCollection nameValuePairs = HttpUtility.ParseQueryString(query);                

                foreach (string key in nameValuePairs.AllKeys)
                {

                    if (key == null)
                    {
                        STrace.Debug(typeof (StringUtils).FullName,
                                     String.Format("QueryToDictionary, Invalid Query Format"));
                    }
                
                    var value = nameValuePairs.Get(key);                    
/*
                    STrace.Debug(typeof(StringUtils).FullName, "KEY: " + key);
                    STrace.Debug(typeof(StringUtils).FullName, "VALUE: " + (value ?? "null"));
*/
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                    else
                    {
                        result[key] = value;
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof (StringUtils).FullName, e);
                return null;
            }
        }

        public static String GetPath(this Uri uri)
        {
            try
            {
                return uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            }
            catch (Exception e)
            {
                STrace.Exception(typeof (StringUtils).FullName, e);
                return null;
            }
        }

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
                    IPAddress addr = IPAddress.Parse(uri.Host);
                    return new IPEndPoint(addr, uri.Port);
                }
                return null;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof (StringUtils).FullName, e);
                return null;
            }
        }

        /// <summary>
        /// Elimina los acentos de un String
        /// </summary>
        /// <param name="text">String a modificar</param>
        /// <returns>El String modificado</returns>
        public static String RemoverAcentos(String text)
        {
            text = text.Trim().ToLower();
            var sb = new StringBuilder(text.Length);
            foreach (char current in text)
            {
                switch (current)
                {
                        #region Variantes de 'a'

                    case 'á':
                    case 'ä':
                    case 'à':
                    case 'ã':
                        sb.Append('a');
                        break;

                        #endregion

                        #region Variantes de 'e'

                    case 'é':
                    case 'ë':
                    case 'è':
                        sb.Append('e');
                        break;

                        #endregion

                        #region Variantes de 'i'

                    case 'í':
                    case 'ï':
                    case 'ì':
                        sb.Append('i');
                        break;

                        #endregion

                        #region Variantes de 'o'

                    case 'ó':
                    case 'ö':
                    case 'ò':
                        sb.Append('o');
                        break;

                        #endregion

                        #region Variantes de 'u'

                    case 'ú':
                    case 'ù':
                    case 'ü':
                        sb.Append('u');
                        break;

                        #endregion

                        #region Variantes de 'y'

                    case 'ÿ':
                    case 'ý':
                        sb.Append('y');
                        break;

                        #endregion

                    default:
                        sb.Append(current);
                        break;
                }
            }
            return sb.ToString();
        }

        public static IEnumerable<String> SplitLines(this String s)
        {
            return s.Split(new[] {Environment.NewLine, "\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static String JoinLines(this IEnumerable<String> ss)
        {
            return String.Join(Environment.NewLine, ss.ToArray());
        }

        #endregion
    }
}