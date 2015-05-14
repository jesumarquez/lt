#region Usings
using System;
using Logictracker.Utils;
using NHibernate.SqlTypes;
using System.Data;
using NHibernate.Type;
#endregion

namespace Logictracker.Types.BusinessObjects.Auditoria
{
        [Serializable]
        public class XmlType<T> : MutableType
        {
            public XmlType() : base(new XmlSqlType())
            {
            }

            public XmlType(SqlType sqlType) : base(sqlType)
            {
            }

            public override string Name
            {
                get { return "XmlOfT"; }
            }

            public override Type ReturnedClass
            {
                get { return typeof(T); }
            }

            public override void Set(IDbCommand cmd, object value, int index)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = XmlUtil.ConvertToXml(value);
            }

            public override object Get(IDataReader rs, int index)
            {
                // according to documentation, GetValue should return a string, at list for MsSQL
                // hopefully all DataProvider has the same behaviour
                string xmlString = Convert.ToString(rs.GetValue(index));
                return FromStringValue(xmlString);
            }

            public override object Get(IDataReader rs, string name)
            {
                return Get(rs, rs.GetOrdinal(name));
            }

            public override string ToString(object val)
            {
                return val == null ? null : XmlUtil.ConvertToXml(val);
            }

            public override object FromStringValue(string xml)
            {
                if (xml != null)
                {
                    return XmlUtil.FromXml<T>(xml);
                }
                return null;
            }

            public override object DeepCopyNotNull(object value)
            {
                var original = (T)value;
                var copy = XmlUtil.FromXml<T>(XmlUtil.ConvertToXml(original));
                return copy;
            }

            public override bool IsEqual(object x, object y)
            {
                if (x == null && y == null)
                {
                    return true;
                }
                if (x == null || y == null)
                {
                    return false;
                }
                return XmlUtil.ConvertToXml(x) == XmlUtil.ConvertToXml(y);
            }
        }
}
