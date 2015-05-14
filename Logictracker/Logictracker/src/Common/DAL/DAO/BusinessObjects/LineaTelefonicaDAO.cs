using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class LineaTelefonicaDAO : GenericDAO<LineaTelefonica>
    {
//        public LineaTelefonicaDAO(ISession session) : base(session) { }

        public LineaTelefonica FindByNumero(string numero)
        {
            return Query.FirstOrDefault(x => x.NumeroLinea == numero);
        }

        public List<LineaTelefonica> GetList(IEnumerable<int> empresas)
        {
            var list = Query.Where(p => !p.Baja).ToList();
            return ApplyCommonFilters(list, empresas).ToList();
        }
        public List<LineaTelefonica> GetLibresByEmpresa(IEnumerable<int> empresas)
        {
            var idSubQuery = QueryOver.Of<Dispositivo>()
                .Where(Restrictions.IsNotNull(Projections.Property<Dispositivo>(p => p.LineaTelefonica)))
                .Select(Projections.Distinct(Projections.Property<Dispositivo>(s => s.LineaTelefonica.Id)));

            var list = Session.QueryOver<LineaTelefonica>()
                .Where(l => !l.Baja)
                .WithSubquery.WhereProperty(p => p.Id).NotIn(idSubQuery)
                .List<LineaTelefonica>();

            return ApplyCommonFilters(list, empresas).ToList();
        }
        private IEnumerable<LineaTelefonica> ApplyCommonFilters(IEnumerable<LineaTelefonica> list, IEnumerable<int> empresas)
        {
            return list.Where(p => (empresas.Contains(0)
                                    || empresas.Contains(-1)
                                    || (p.GetVigencia(DateTime.UtcNow) != null
                                        && empresas.Contains(p.GetVigencia(DateTime.UtcNow).Plan.Empresa))));
        }


        public override void Delete(LineaTelefonica obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public bool IsImeiUnique(int idLineaTelefonica, string imei)
        {
	        return !Query.Any(l => l.Id != idLineaTelefonica
	                              && l.Imei == imei
	                              && !l.Baja);
        }

        public bool IsNumberUnique(int idLineaTelefonica, string numero)
        {
	        return !Query.Any(l => l.Id != idLineaTelefonica
	                               && l.NumeroLinea == numero
	                               && !l.Baja);
        }
    }
}