#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;

#endregion

namespace Logictracker.Utils.NHibernate
{
    public static class StructuredExtensions
    {
        private static readonly Sql2008Structured structured = new Sql2008Structured();

        public static IQuery SetStructured(this IQuery query, string name, DataTable dt)
        {
            return query.SetParameter(name, dt, structured);
        }
    }

    public class Sql2008Structured : IType
    {
        private static readonly SqlType[] x = new[] { new SqlType(DbType.Object) };
        public SqlType[] SqlTypes(IMapping mapping)
        {
            return x;
        }

        public bool IsXMLElement { get; private set; }

        public bool IsCollectionType
        {
            get { return true; }
        }

        public bool IsComponentType { get; private set; }
        public bool IsEntityType { get; private set; }
        public bool IsAnyType { get; private set; }

        public int GetColumnSpan(IMapping mapping)
        {
            return 1;
        }

        public bool IsDirty(object old, object current, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public bool IsModified(object oldHydratedState, object currentState, bool[] checkable, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session)
        {
            var s = st as SqlCommand;
            if (s != null)
            {
                s.Parameters[index].SqlDbType = SqlDbType.Structured;
                s.Parameters[index].TypeName = "IntTable";
                s.Parameters[index].Value = value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string ToLoggableString(object value, ISessionFactoryImplementor factory)
        {
            throw new NotImplementedException();
        }

        public object DeepCopy(object val, EntityMode entityMode, ISessionFactoryImplementor factory)
        {
            throw new NotImplementedException();
        }

        public object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public object ResolveIdentifier(object value, ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public object SemiResolve(object value, ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready)
        {
            throw new NotImplementedException();
        }

        public object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
        {
            throw new NotImplementedException();
        }

        public bool IsSame(object x, object y, EntityMode entityMode)
        {
            throw new NotImplementedException();
        }

        public bool IsEqual(object x, object y, EntityMode entityMode)
        {
            throw new NotImplementedException();
        }

        public bool IsEqual(object x, object y, EntityMode entityMode, ISessionFactoryImplementor factory)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(object x, EntityMode entityMode)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(object x, EntityMode entityMode, ISessionFactoryImplementor factory)
        {
            throw new NotImplementedException();
        }

        public int Compare(object x, object y, EntityMode? entityMode)
        {
            throw new NotImplementedException();
        }

        public IType GetSemiResolvedType(ISessionFactoryImplementor factory)
        {
            throw new NotImplementedException();
        }

        public void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory)
        {
            throw new NotImplementedException();
        }

        public object FromXMLNode(XmlNode xml, IMapping factory)
        {
            throw new NotImplementedException();
        }

        public bool[] ToColumnNullness(object value, IMapping mapping)
        {
            throw new NotImplementedException();
        }

        public string Name { get; private set; }
        public Type ReturnedClass { get; private set; }
        public bool IsMutable { get; private set; }
        public bool IsAssociationType { get; private set; }

        public object Disassemble(object value, ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public object Assemble(object cached, ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public void BeforeAssemble(object cached, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }
    }

    
    public static class NHibernateUtils
    {
        public static string GenerateSQL(ICriteria criteria)
        {
            var criteriaImpl = (CriteriaImpl) criteria;
            ISessionImplementor session = criteriaImpl.Session;
            ISessionFactoryImplementor factory = session.Factory;

            var translator =
                new CriteriaQueryTranslator(
                    factory,
                    criteriaImpl,
                    criteriaImpl.EntityOrClassName,
                    CriteriaQueryTranslator.RootSqlAlias);

            String[] implementors = factory.GetImplementors(criteriaImpl.EntityOrClassName);

            var walker = new CriteriaJoinWalker(
                (IOuterJoinLoadable) factory.GetEntityPersister(implementors[0]),
                translator,
                factory,
                criteriaImpl,
                criteriaImpl.EntityOrClassName,
                session.EnabledFilters);

            return walker.SqlString.ToString();
        }

        public class ArithmeticOperatorProjection : OperatorProjection
        {
            public ArithmeticOperatorProjection(string op, IType returnType, params IProjection[] args)
                : base(op, returnType, args)
            {
                if (args.Length < 2)
                    throw new ArgumentOutOfRangeException("args", args.Length, "Requires at least 2 projections");
            }

            public override string[] AllowedOperators
            {
                get { return new[] {"+", "-", "*", "/", "%"}; }
            }
        }

        public class BitwiseOperatorProjection : OperatorProjection
        {
            public BitwiseOperatorProjection(string op, IType returnType, params IProjection[] args)
                : base(op, returnType, args)
            {
                if (args.Length < 2)
                    throw new ArgumentOutOfRangeException("args", args.Length, "Requires at least 2 projections");
            }

            public override string[] AllowedOperators
            {
                get { return new[] {"&", "|", "^"}; }
            }
        }

        public abstract class OperatorProjection : SimpleProjection
        {
            private readonly IProjection[] args;
            private readonly IType returnType;

            private string op;
            private string Op
            {
                get { return op; }
                set
                {
                    var trimmed = value.Trim();
                    if (System.Array.IndexOf(AllowedOperators, trimmed) == -1)
                        throw new ArgumentOutOfRangeException("value", trimmed, "Not allowed operator");
                    op = " " + trimmed + " ";
                }
            }

            public abstract string[] AllowedOperators { get; }

            protected OperatorProjection(string op, IType returnType, params IProjection[] args)
            {
                this.Op = op;
                this.returnType = returnType;
                this.args = args;
            }

            public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
            {
                SqlStringBuilder sb = new SqlStringBuilder();
                sb.Add("(");

                for (int i = 0; i < args.Length; i++)
                {
                    int loc = (position + 1) * 1000 + i;
                    SqlString projectArg = GetProjectionArgument(criteriaQuery, criteria, args[i], loc, enabledFilters);
                    sb.Add(projectArg);

                    if (i < args.Length - 1)
                        sb.Add(Op);
                }
                sb.Add(")");
                sb.Add(" as ");
                sb.Add(GetColumnAliases(position)[0]);
                return sb.ToSqlString();
            }

            private static SqlString GetProjectionArgument(ICriteriaQuery criteriaQuery, ICriteria criteria,
                                                           IProjection projection, int loc,
                                                           IDictionary<string, IFilter> enabledFilters)
            {
                SqlString sql = projection.ToSqlString(criteria, loc, criteriaQuery, enabledFilters);
                return sql.Substring(0, sql.LastIndexOfCaseInsensitive(" as "));
            }

            public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
            {
                return new IType[] { returnType };
            }

            public override bool IsAggregate
            {
                get { return false; }
            }

            public override bool IsGrouped
            {
                get
                {
                    foreach (IProjection projection in args)
                    {
                        if (projection.IsGrouped)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
            {
                SqlStringBuilder buf = new SqlStringBuilder();
                foreach (IProjection projection in args)
                {
                    if (projection.IsGrouped)
                    {
                        buf.Add(projection.ToGroupSqlString(criteria, criteriaQuery, enabledFilters)).Add(", ");
                    }
                }
                if (buf.Count >= 2)
                {
                    buf.RemoveAt(buf.Count - 1);
                }
                return buf.ToSqlString();
            }
        }
    }
}