#region Usings

using System;
using System.Xml.Linq;

#endregion

namespace Logictracker.Model.Utils
{
    public static class XHelper
    {
        /// <summary>
        /// Obtiene un atributo, o un valor no nulo.
        /// si el atributo es id y no existe, lo establece 
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static string Attr(this XElement xElement, string attr)
        {
            if (xElement == null) return "";
            var xAttr = xElement.Attribute(attr);
            if (xAttr != null) return xAttr.Value;
            if (attr == "id")
            {
                var val = GenerateId(null);
                xElement.SetAttributeValue("id", val);
                return val;
            }
            return "";
        }

        private static string GenerateId(object @object)
        {
            return "ID" + NextUInt32(@object == null ? 0 : (uint)@object.GetHashCode());
        }

        private static UInt32 NextUInt32(uint graph)
        {
            var buffer = new byte[sizeof(UInt32)];
            RandomUInt32.NextBytes(buffer);
            return graph + BitConverter.ToUInt32(buffer, 0);
        }

		private static readonly Random RandomUInt32 = new Random((int)(DateTime.Now.Ticks / 37 * DateTime.Now.Second));
    }
}
