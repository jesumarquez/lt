using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class ViajeDistribucionDAO : GenericDAO<ViajeDistribucion>
    {
        //        public ViajeDistribucionDAO(ISession session) : base(session) { }

        #region Private Methods
        private DetachedCriteria GetDetachedCriteria(int[] coches, short[] estados)
        {
            return GetDetachedCriteria(coches, estados, false);
        }

        private DetachedCriteria GetDetachedCriteria(int[] coches, short[] estados, DateTime dateFrom, DateTime dateTo)
        {
            return GetDetachedCriteria(false, coches, estados, dateFrom, dateTo);
        }

        private DetachedCriteria GetDetachedCriteria(bool cocheMandatory, int[] coches, short[] estados, DateTime dateFrom, DateTime dateTo)
        {
            return GetDetachedCriteria(cocheMandatory, coches, estados, false, dateFrom, dateTo);
        }

        private DetachedCriteria GetDetachedCriteria(int[] coches, short[] estados, bool excludeEstados, DateTime dateFrom, DateTime dateTo)
        {
            return GetDetachedCriteria(false, coches, estados, excludeEstados, dateFrom, dateTo);
        }

        private DetachedCriteria GetDetachedCriteria(bool cocheMandatory, int[] coches, short[] estados, bool excludeEstados, DateTime dateFrom, DateTime dateTo)
        {
            var dc = GetDetachedCriteria(cocheMandatory, coches, estados, excludeEstados)
                .Add(Restrictions.Between("Inicio", dateFrom, dateTo));

            return dc;
        }

        private DetachedCriteria GetDetachedCriteria(int[] coches, short[] estados, bool excludeEstados)
        {
            return GetDetachedCriteria(false, coches, estados, excludeEstados);
        }

        private DetachedCriteria GetDetachedCriteria(bool cocheMandatory, int[] coches, short[] estados, bool excludeEstados)
        {
            var dc = DetachedCriteria.For<ViajeDistribucion>("dvd")
                .Add(Restrictions.EqProperty("dvd.Id", "vd.Id"))
                .SetProjection(Projections.Property("Id"));

            if (cocheMandatory || coches.Length > 0)
                dc.CreateAlias("Vehiculo", "c", JoinType.InnerJoin);

            if (coches.Length > 0)
                dc.Add(Restrictions.In("c.Id", coches));

            if (estados.Length > 0)
            {
                if (excludeEstados)
                    dc.Add(Restrictions.Not(Restrictions.In("Estado", estados)));
                else
                    dc.Add(Restrictions.In("Estado", estados));
            }
            return dc;
        }

        private ICriteria GetCriteria(int top, DetachedCriteria dc, Order order)
        {
            var c = Session.CreateCriteria<ViajeDistribucion>("vd")
                .Add(Subqueries.Exists(dc));
            if (top > 0)
                c.SetMaxResults(top);
            if (order != null)
                c.AddOrder(order);
            return c;
        }

        #endregion

        #region Find Methods

        public ViajeDistribucion FindEnCurso(Coche coche)
        {
            if (coche.KeyExists(ViajeDistribucion.CurrentCacheKey))
            {
                var key = coche.Retrieve<object>(ViajeDistribucion.CurrentCacheKey);
                return key == null ? null : FindById((int)key);
            }

            var dc = GetDetachedCriteria(new[] { coche.Id }, new[] { ViajeDistribucion.Estados.EnCurso });

            var current = GetCriteria(1, dc, Order.Asc("Inicio")).UniqueResult<ViajeDistribucion>();

            StoreInCache(coche, current);

            return current;
        }

        public ViajeDistribucion FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                .Where(t => t.Estado != ViajeDistribucion.Estados.Eliminado)
                .FirstOrDefault(t => t.Codigo == codigo);
        }

        public List<ViajeDistribucion> FindByCodigos(IEnumerable<int> empresa, IEnumerable<int> linea, IEnumerable<string> codigos)
        {
            return Query.FilterEmpresa(Session, empresa, null)
                        .FilterLinea(Session, empresa, linea, null)
                        .Where(t => t.Estado != ViajeDistribucion.Estados.Eliminado)
                        .Where(t => codigos.Contains(t.Codigo))
                        .ToList();
        }

        public IList<ViajeDistribucion> FindList(int vehicleId, DateTime desde, DateTime hasta)
        {

            var dc = GetDetachedCriteria(new[] { vehicleId }, new[] { ViajeDistribucion.Estados.Eliminado }, true, desde, hasta);

            var c = GetCriteria(1, dc, Order.Asc("Inicio"));

            return c.List<ViajeDistribucion>();
        }

        public ViajeDistribucion FindPendiente(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> vehiculos, DateTime desde, DateTime hasta)
        {
            return Session.Query<ViajeDistribucion>()
                          .FilterLinea(Session, empresas, lineas)
                          .FilterVehiculo(Session, empresas, lineas, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, vehiculos)
                          .Where(t => t.Estado == ViajeDistribucion.Estados.Pendiente
                                   && t.Inicio >= desde
                                   && t.Inicio <= hasta)
                          .OrderBy(t => t.Inicio)
                          .Take(1)
                          .FirstOrDefault();
        }

        #endregion

        #region Get Methods

        public List<ViajeDistribucion> GetByTexto(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> coches, DateTime fechaDesde, DateTime fechaHasta, string texto)
        {
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas);

            if (!QueryExtensions.IncludesAll(transportistas) || !QueryExtensions.IncludesAll(coches))
                q = q.FilterVehiculo(Session, empresas, lineas, transportistas, new[] { -1 }, new[] { -1 }, new[] { -1 }, coches);

            q = q.Where(t => t.Inicio >= fechaDesde
                          && t.Inicio < fechaHasta
                          && t.Estado != ViajeDistribucion.Estados.Eliminado);

            var viajes = q.ToList();

            if (string.IsNullOrEmpty(texto)) return viajes;

            var textoUi = texto.ToUpperInvariant();
            return viajes.Where(t => t.Codigo.ToUpperInvariant().Contains(textoUi)
                                  || t.NumeroViaje.ToString("#0").ToUpperInvariant().Contains(textoUi))
                         .ToList();
        }

        public List<ViajeDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> subCentrosDeCosto, IEnumerable<int> vehiculos, DateTime? desde, DateTime? hasta)
        {
            return GetList(empresas, lineas, transportistas, departamentos, centrosDeCosto, subCentrosDeCosto, vehiculos, new[] { -1 }, desde, hasta);
        }

        public List<ViajeDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> subCentrosDeCosto, IEnumerable<int> vehiculos, IEnumerable<int> estados, DateTime? desde, DateTime? hasta)
        {
            return GetList(empresas, lineas, transportistas, departamentos, centrosDeCosto, subCentrosDeCosto, vehiculos, new[] { -1 }, estados, desde, hasta);
        }

        public List<ViajeDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> subCentrosDeCosto, IEnumerable<int> vehiculos, IEnumerable<int> empleados, IEnumerable<int> estados, DateTime? desde, DateTime? hasta)
        {
            return GetList(empresas, lineas, transportistas, departamentos, centrosDeCosto, subCentrosDeCosto, vehiculos, empleados, estados, new[] { -1 }, desde, hasta);
        }

        public List<ViajeDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> subCentrosDeCosto, IEnumerable<int> vehiculos, IEnumerable<int> empleados, IEnumerable<int> estados, IEnumerable<int> tiposDistribucion, DateTime? desde, DateTime? hasta)
        {
            var tiposVehiculo = new[] { -1 };
            var tiposEmpleado = new[] { -1 };

            var estadosList = estados.ToList();
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas)
                         .FilterVehiculo(Session, empresas, lineas, transportistas, departamentos, centrosDeCosto, tiposVehiculo, vehiculos);

            if (!QueryExtensions.IncludesAll(empleados) || !QueryExtensions.IncludesAll(transportistas))
                q = q.FilterEmpleado(Session, empresas, lineas, transportistas, tiposEmpleado, empleados);

            if (!QueryExtensions.IncludesAll(departamentos) || !QueryExtensions.IncludesAll(centrosDeCosto))
                q = q.FilterCentroDeCostos(Session, empresas, lineas, departamentos, centrosDeCosto);

            if (!QueryExtensions.IncludesAll(subCentrosDeCosto))
                q = q.FilterSubCentroDeCostos(Session, empresas, lineas, departamentos, centrosDeCosto, subCentrosDeCosto);

            if (!estados.Contains(-1))
                q = q.Where(t => estadosList.Contains(t.Estado));

            if (!tiposDistribucion.Contains(-1))
                q = q.Where(t => tiposDistribucion.Contains(t.Tipo));

            if (desde.HasValue) q = q.Where(t => t.Inicio >= desde);
            if (hasta.HasValue) q = q.Where(t => t.Inicio < hasta);

            return q.Where(t => t.Estado != ViajeDistribucion.Estados.Eliminado).ToList();
        }
        public List<ViajeDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> vehiculos, IEnumerable<int> empleados)
        {
            var departamentos = new[] { -1 };
            var transportistas = new[] { -1 };
            var centrosDeCosto = new[] { -1 };
            var tiposVehiculo = new[] { -1 };
            var tiposEmpleado = new[] { -1 };

            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas);

            if (!QueryExtensions.IncludesAll(transportistas) || !QueryExtensions.IncludesAll(centrosDeCosto)
             || !QueryExtensions.IncludesAll(tiposVehiculo) || !QueryExtensions.IncludesAll(vehiculos))
                q = q.FilterVehiculo(Session, empresas, lineas, transportistas, departamentos, centrosDeCosto, tiposVehiculo, vehiculos);

            if (!QueryExtensions.IncludesAll(transportistas) || !QueryExtensions.IncludesAll(tiposEmpleado)
              || !QueryExtensions.IncludesAll(empleados))
                q = q.FilterEmpleado(Session, empresas, lineas, transportistas, tiposEmpleado, empleados);

            return q.ToList();
        }

        public IList<ViajeDistribucion> GetListForDatamart(IEnumerable<int> empresas, DateTime desde, DateTime hasta)
        {
            var dc = GetDetachedCriteria(true, new int[] { }, new short[] { }, desde, hasta);

            if (!QueryExtensions.IncludesAll(empresas))
                dc.CreateAlias("Empresa", "e", JoinType.InnerJoin)
                    .Add(Restrictions.In("e.id", empresas.ToArray()));


            var c = GetCriteria(0, dc, Order.Asc("Inicio"));

            return c.List<ViajeDistribucion>();
        }

        public ViajeDistribucion GetById(IEnumerable<int> empresas, IEnumerable<int> lineas, int id)
        {
            return Query.FilterEmpresa(Session, empresas)
                       .FilterLinea(Session, empresas, lineas)
                       .Where(v => v.Id.Equals(id)).FirstOrDefault();
        }

        public IList<ViajeDistribucion> FindByIds(IEnumerable<int> ids)
        {
            return Query.Where(v => ids.Contains(v.Id)).ToList();
        }

        public IList<ViajeDistribucion> GetListActivos(int empresa, int linea, DateTime desde, DateTime hasta)
        {
            var dc = GetDetachedCriteria(new int[] { }, new[] { ViajeDistribucion.Estados.EnCurso, ViajeDistribucion.Estados.Pendiente }, desde, hasta);

            if (empresa > 0)
            {
                dc.CreateAlias("Empresa", "e", JoinType.InnerJoin)
                    .Add(Restrictions.Eq("e.Id", empresa));
            }

            if (linea > 0)
            {
                dc.CreateAlias("Linea", "l", JoinType.InnerJoin)
                    .Add(Restrictions.Eq("l.Id", linea));
            }
            
            var c = GetCriteria(0, dc, Order.Asc("Inicio"));

            return c.List<ViajeDistribucion>();
        }

        #endregion
        #region Override Methods

        public override void Delete(ViajeDistribucion obj)
        {
            if (obj == null) return;
            obj.Estado = ViajeDistribucion.Estados.Eliminado;
            SaveOrUpdate(obj);
            UpdateCache(obj);
        }

        public override void SaveOrUpdate(ViajeDistribucion obj)
        {
            base.SaveOrUpdate(obj);
            UpdateCache(obj);
        }

        #endregion

        #region Other Methods

        private static void UpdateCache(ViajeDistribucion distribucion)
        {
            if (distribucion == null || distribucion.Vehiculo == null) return;
            var coche = distribucion.Vehiculo;
            if (coche.KeyExists(ViajeDistribucion.CurrentCacheKey)) coche.Delete(ViajeDistribucion.CurrentCacheKey);
            if (distribucion.Estado == ViajeDistribucion.Estados.EnCurso) StoreInCache(coche, distribucion);
        }

        private static void StoreInCache(Coche coche, ViajeDistribucion distribucion)
        {
            if (coche == null) return;
            coche.Store(ViajeDistribucion.CurrentCacheKey, distribucion == null ? null : (object)distribucion.Id, DateTime.Now.AddHours(1));
        }

        #endregion

        public IEnumerable<ViajeDistribucion> FindByCodeLike(int empresa, int linea, string codigo)
        {
            Empresa e = null;
            Linea l = null;
            var q= Session.QueryOver<ViajeDistribucion>()
                .JoinAlias(v => v.Empresa, () => e)
                .JoinAlias(w => w.Linea, () => l)
                .Where(() => e.Id == empresa)
                .Where(() => l.Id == linea)
                .WhereRestrictionOn(x=>x.Codigo).IsLike(codigo+"%")
                .WhereRestrictionOn(x => x.Estado).IsIn(new[] { ViajeDistribucion.Estados.EnCurso , ViajeDistribucion.Estados.Pendiente})
                .Fetch(v => v.Vehiculo).Eager
                .Future();
           
            return q;
        }
    }
}
