#region Usings

using System.Collections;
using NHibernate;
using NHibernate.Expression;
using Nullables;
using Urbetrack.Types.BusinessObjects;

#endregion

namespace Urbetrack.DAL.DAO.BusinessObjects
{
    public class TipoDomicilioDAO: GenericDAO<TipoDomicilio>
    {
        public TipoDomicilioDAO(ISession sess) : base(sess)
        {
        }
        public bool HasChilds(TipoDomicilio tipo)
        {
            return HasChilds(tipo.Id);
        }
        public bool HasChilds(int tipo)
        {
            if (tipo <= 0) return false;

            return ((int)sess.CreateQuery("select count(d) from MovDomicilio d where d.Tipo.Id = :tipo")
                             .SetParameter("tipo", tipo)
                             .UniqueResult()) > 0;
        }

        public TipoDomicilio GetParticular() { return GetHC("EsParticular"); }
        
        public TipoDomicilio GetPlanta() { return GetHC("EsPlanta"); }
        
        public TipoDomicilio GetCliente() { return GetHC("EsCliente"); }
        
        public TipoDomicilio GetObra() { return GetHC("EsObra"); }
        
        public TipoDomicilio GetTaller() { return GetNullableHC("EsTaller"); }

        public TipoDomicilio GetTransportista() { return GetNullableHC("EsTransportista"); }

        private TipoDomicilio GetNullableHC(string property)
        {
            var l = sess.CreateCriteria(typeof (TipoDomicilio))
                .Add(Expression.Eq(property, new NullableBoolean(true)))
                .List();

            return l.Count > 0 ? l[0] as TipoDomicilio : null;
        }

        private TipoDomicilio GetHC(string property)
        {
            var l = sess.CreateCriteria(typeof (TipoDomicilio))
                .Add(Expression.Eq(property, true))
                .List();

            return l.Count > 0 ? l[0] as TipoDomicilio : null;
        }
        public TipoDomicilio GetByCodigo(string codigo)
        {
            var l = sess.CreateCriteria(typeof(TipoDomicilio))
                .Add(Expression.Eq("Codigo", codigo))
                .List();

            return l.Count > 0 ? l[0] as TipoDomicilio : null;
        }
        public TipoDomicilio GetByDescripcion(string descri)
        {
            var l = sess.CreateCriteria(typeof(TipoDomicilio))
                .Add(Expression.Eq("Descripcion", descri))
                .List();

            return l.Count > 0 ? l[0] as TipoDomicilio : null;
        }

        public IList FindByEmpresaAndLinea(int empresa, int linea)
        {
            if (linea > 0) return empresa > 0 ? FindByEmpresaYLinea(empresa, linea) : FindByLinea(linea);

            return empresa > 0 ? FindByEmpresa(empresa) : FindAll();
        }

        private IList FindByEmpresaYLinea(int empresa, int linea)
        {
            var query = "from TipoDomicilio t where ((t.Linea.Id = :linea or t.Linea is null) and t.Empresa.Id = :empresa) or (t.Linea is null and t.Empresa is null)";

            return sess.CreateQuery(query).SetParameter("linea", linea).SetParameter("empresa", empresa).List();
        }

        private IList FindByLinea(int linea)
        {
            var lineaDAO = new LineaDAO(sess);

            var empresa = lineaDAO.FindById(linea).Empresa;

            var query = "from TipoDomicilio t where t.Linea.Id = :linea or (t.Linea is null and (t.Empresa is null or t.Empresa = :empresa))";

            return sess.CreateQuery(query).SetParameter("linea", linea).SetParameter("empresa", empresa).List();
        }

        private IList FindByEmpresa(int empresa)
        {
            var query = "from TipoDomicilio t where t.Empresa.Id = :empresa or (t.Linea is null and t.Empresa is null)";

            return sess.CreateQuery(query).SetParameter("empresa", empresa).List();
        }
    }
}