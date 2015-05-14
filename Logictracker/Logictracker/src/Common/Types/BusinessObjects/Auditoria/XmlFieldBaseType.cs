using System;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace Urbetrack.Types.BusinessObjects.Auditoria
{
    public class XmlFieldBaseType<T> : IUserType where T : ICloneable
    {
        #region Equals member

        bool IUserType.Equals(object x, object y) { return (x == y) || ((x != null) && x.Equals(y)); }

        #endregion

        #region IUserType Members

        public object Assemble(object cached, object owner) { return cached; }

        public object DeepCopy(object value) { if (value == null) return null;  return ((T)value).Clone(); }

        public object Disassemble(object value) { return value; }

        public int GetHashCode(object x) { return x.GetHashCode(); }

        public bool IsMutable { get { return true; } }

        public object NullSafeGet(System.Data.IDataReader rs, string[] names, object owner)
        {
            Int32 index = rs.GetOrdinal(names[0]);
            if (rs.IsDBNull(index))
            {
                return null;
            }

            return XmlHelper.FromXml<T>((String)rs[index]);
        }

        public void NullSafeSet(System.Data.IDbCommand cmd, object value, int index)
        {
            if (value == null || value == DBNull.Value)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
            }
            else
            {

                NHibernateUtil.String.Set(cmd, XmlHelper.ToXml(value), index);
            }

        }

        public object Replace(object original, object target, object owner) { return original; }

        public Type ReturnedType { get { return typeof(Uri); } }

        public SqlType[] SqlTypes { get { return new [] { NHibernateUtil.String.SqlType }; } }

        #endregion
    }
}
