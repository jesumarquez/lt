using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Positions;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Types.ReportObjects.RankingDeOperadores;
using Logictracker.Utils;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    internal static class DatamartDAOX
    {
        #region Criteria

        public static ICriteria FilterByVehicle(this ICriteria dc, int cId)
        {
            return dc.FilterByVehicles(new[] {cId});
        }

        public static ICriteria FilterByVehicles(this ICriteria dc, int[] cIds)
        {
            if (cIds.Count() > 0)
            {
                dc.Add(Restrictions.In("Vehicle.Id", cIds));
            }
            return dc;
        }

        public static ICriteria FilterBeginBetween(this ICriteria dc, DateTime inicioFrom, DateTime inicioTo)
        {
            return dc.Add(Restrictions.Between("Begin", inicioFrom, inicioTo));
        }

        public static ICriteria FilterBeginGeLt(this ICriteria dc, DateTime inicioFrom, DateTime inicioTo)
        {
            return dc.Add(Restrictions.Ge("Begin", inicioFrom)).Add(Restrictions.Lt("Begin", inicioTo));
        }

        public static ICriteria FilterUpToEnd(this ICriteria dc, DateTime finTo)
        {
            return dc.Add(Restrictions.Le("End", finTo));
        }

        public static ICriteria FilterBeginEndBetween(this ICriteria dc, DateTime inicioFrom, DateTime finTo)
        {
            return dc.Add(Restrictions.Ge("Begin", inicioFrom)).Add(Restrictions.Lt("End", finTo));
        }

        public static ICriteria FilterBegin1DayFrom(this ICriteria dc, DateTime dateFrom)
        {
            return dc.Add(Restrictions.Between("Begin", dateFrom, dateFrom.AddDays(1)));
        }

        #endregion Criteria

        #region DetachedCriteria

        public static DetachedCriteria FilterBeginBetween(this DetachedCriteria dc, DateTime inicioFrom, DateTime inicioTo)
        {
            return dc.Add(Restrictions.Between("Begin", inicioFrom, inicioTo));
        }

        public static DetachedCriteria FilterBeginGeLt(this DetachedCriteria dc, DateTime inicioFrom, DateTime inicioTo)
        {
                return dc.Add(Restrictions.Ge("Begin", inicioFrom)).Add(Restrictions.Lt("Begin", inicioTo));
        }

        public static DetachedCriteria FilterUpToEnd(this DetachedCriteria dc, DateTime finTo)
        {
            return dc.Add(Restrictions.Le("End", finTo));
        }

        public static DetachedCriteria FilterBeginEndBetween(this DetachedCriteria dc, DateTime inicioFrom, DateTime finTo)
        {
            return dc.Add(Restrictions.Ge("Begin", inicioFrom)).Add(Restrictions.Lt("End", finTo));
        }

        public static DetachedCriteria FilterBegin1DayFrom(this DetachedCriteria dc, DateTime dateFrom)
        {
            return dc.Add(Restrictions.Between("Begin", dateFrom, dateFrom.AddDays(1)));
        }

        #endregion DetachedCriteria
    }


    /// <summary>
    /// Datamart data access class.
    /// </summary>
    public class DatamartDAO : GenericDAO<Datamart>
    {
        private const short SQLParameterLimit = 2050;

        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public DatamartDAO(ISession session) : base(session) { }

        #endregion

        #region Private Methods
        private DetachedCriteria GetDatamartDetachedCriteria()
        {
            DetachedCriteria dc =
                DetachedCriteria.For<Datamart>("ddm")
                .Add(Restrictions.EqProperty("ddm.Id", "dm.Id"))
                .Add(Restrictions.EqProperty("ddm.Begin", "dm.Begin"))
                .SetProjection(Projections.Property("Id"));
            return dc;
        }

        private DetachedCriteria GetDatamartDetachedCriteriaForEmployee(int eId)
        {
            return GetDatamartDetachedCriteriaForEmployee(new[] {eId});
        }

        private DetachedCriteria GetDatamartDetachedCriteriaForEmployee(int[] eIds)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria();

            if (eIds.Count() > 0)
                dc.CreateAlias("Employee", "ch", JoinType.InnerJoin).Add(Restrictions.In("ch.Id", eIds));
            return dc;
        }

        private DetachedCriteria GetDatamartDetachedCriteria(int cId)
        {
            return GetDatamartDetachedCriteria(new[] {cId});
        }

        private DetachedCriteria GetDatamartDetachedCriteria(int[] cIds)
        {
            return GetDatamartDetachedCriteria(false, GetVehiclesDetachedCriteria(cIds, new int[] {}, new int[] {}));
        }

        private DetachedCriteria GetVehiclesDetachedCriteria(int[] vehiclesIds, int[] empresaIds, int[] lineaIds)
        {
            DetachedCriteria dc = DetachedCriteria.For<Coche>("c").SetProjection(Projections.Property("Id"));

            if (vehiclesIds.Count() > 0)
                dc.Add(Restrictions.In("Id", vehiclesIds));

            if (empresaIds.Count() > 0)
            {
                AbstractCriterion nullEmpresa = Restrictions.IsNull("Empresa.Id");
                int[] empresasIds = (empresaIds[0] == 0 ? empresaIds.Skip(1).ToArray() : empresaIds);
                AbstractCriterion empresasRestriction = Restrictions.In("Empresa.Id", empresasIds);

                if (empresaIds.Count() == 1)
                    if (empresaIds[0] != 0)
                        dc.Add(empresasRestriction);
                    else if (empresaIds[0] == 0)
                        dc.Add(Restrictions.Or(nullEmpresa, empresasRestriction));
                    else
                        dc.Add(empresasRestriction);
            }

            if (lineaIds.Count() > 0)
            {
                AbstractCriterion nulllinea = Restrictions.IsNull("Linea.Id");
                int[] lineasIds = (lineaIds[0] == 0 ? lineaIds.Skip(1).ToArray() : lineaIds);
                AbstractCriterion lineasRestriction = Restrictions.In("Linea.Id", lineasIds);

                if (lineaIds.Count() == 1)
                    if (lineaIds[0] != 0)
                        dc.Add(lineasRestriction);
                    else if (lineaIds[0] == 0)
                        dc.Add(Restrictions.Or(nulllinea, lineasRestriction));
                    else
                        dc.Add(lineasRestriction);
            }

            return dc;
        }

        private DetachedCriteria GetDatamartDetachedCriteria(bool vehicleMandatory, DetachedCriteria dcv)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria();

            if (vehicleMandatory)
            {
                dc.CreateAlias("Vehicle", "c", JoinType.InnerJoin);
            }

            if (dcv != null)
            {
                dc.Add(Subqueries.PropertyIn("Vehicle.Id", dcv));
            }
            return dc;
        }

        private ICriteria GetDatamartCriteria(int top, DetachedCriteria dc, ProjectionList pl, Order order)
        {
            ICriteria crit = GetDatamartCriteria(top, dc, order);
            crit.SetProjection(pl);
            return crit;
        }

        private ICriteria GetDatamartCriteria(int top, DetachedCriteria dc, ProjectionList pl, Order order, Type T)
        {
            ICriteria crit = GetDatamartCriteria(top, dc, pl, order);
            crit.SetResultTransformer(Transformers.AliasToBean(T));

            return crit;
        }

        private ICriteria GetDatamartCriteria(int top, DetachedCriteria dc, Order order)
        {
            ICriteria c = Session.CreateCriteria<Datamart>("dm");

            if (dc != null)
                c.Add(Subqueries.Exists(dc));

            if (top > 0)
                c.SetMaxResults(top);

            if (order != null)
                c.AddOrder(order);
            return c;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Gets the last datamart update for the specified vehicle.
        /// </summary>
        /// <param name="coche"></param>
        /// <returns></returns>
        public DateTime GetLastDatamartUpdate(int coche)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria(coche);
            ICriteria crit = GetDatamartCriteria(1, dc, Order.Desc("Begin"));

            var data = crit.UniqueResult<Datamart>();

            //Adds a minute to avoid regenerating the last hour.
            if (data != null) return data.End.AddMinutes(1);

            var posicionesDao = new LogPosicionDAO();

            LogPosicion firtPosition = posicionesDao.GetFirtPosition(coche);

            DateTime result = firtPosition == null ? DateTime.MinValue : firtPosition.FechaMensaje;

            return result;
        }

        /// <summary>
        /// Deletes datamart records older than the specified date.
        /// </summary>
        /// <param name="date"></param>
        public void DeleteOldDatamartRecords(DateTime date)
        {
            using (SmartTransaction tx = SmartTransaction.BeginTransaction())
            {
            try
            {
                Session.CreateSQLQuery("delete from opeposi07 where opeposi07_inicio <= :date ; select @@rowcount as count;")
                    .AddScalar("count", NHibernateUtil.Int32)
                    .SetParameter("date", date)
                    .SetTimeout(0)
                    .UniqueResult();
                    tx.Commit();
            }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw new Exception("Error deleting datamart records older than one year ago: ", ex);
        }
            }
        }

        /// <summary>
        /// Deletes records for the specified vehicle and time span.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        public void DeleteRecords(Int32 vehicle, DateTime desde, DateTime hasta)
        {
            using (SmartTransaction tx = SmartTransaction.BeginTransaction())
            {
                try
                {
                    Session.CreateSQLQuery(
                        "delete from opeposi07 where rela_parenti03 = :coche and opeposi07_inicio >= :desde and opeposi07_inicio <= :hasta ; select @@rowcount as count;")
                .AddScalar("count", NHibernateUtil.Int32)
                .SetParameter("coche", vehicle)
                .SetParameter("desde", desde)
                .SetParameter("hasta", hasta)
                .SetTimeout(0)
                .UniqueResult();
                    tx.Commit();
        }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw new Exception(String.Format("Error deleting datamart records for vehicle {0} from {1} to {2} ", vehicle, desde, hasta), ex);
                }
            }
        }

        /// <summary>
        /// Gets the last datamart record previos to the current period.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="desde"></param>
        /// <returns></returns>
        public Datamart GetLastDatamart(Int32 id, DateTime desde)
        {
            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_DatamartDAO_GetLastDatamart] @vehicleId = :vehicleId, @fecha = :fecha;")
                              .AddEntity(typeof(Datamart))
                              .SetInt32("vehicleId", id)
                              .SetDateTime("fecha", desde);
            var results = sqlQ.List<Datamart>();
            return results.FirstOrDefault();

            //DetachedCriteria dc = GetDatamartDetachedCriteria(id).FilterUpToEnd(desde);
            //ICriteria crit = GetDatamartCriteria(1, dc, Order.Desc("Begin"));
            //var result = crit.UniqueResult<Datamart>();
            //return result;
        }

        /// <summary>
        /// Gets datamart regeneration periods.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="refference"></param>
        /// <returns></returns>
        public List<RegenerateDatamart> GetDaysToRegenerate(Coche vehicle, DateTime from, DateTime to, DateTime refference)
        {
            var posicionesDao = new LogPosicionDAO();

            int maxMonths;

            try
            {
                maxMonths = vehicle != null && vehicle.Empresa != null ? vehicle.Empresa.MesesConsultaPosiciones : 3;
            }
            catch (Exception)
            {
                maxMonths = 3;
            }

            var te = new TimeElapsed();
            var start = posicionesDao.GetRegenerationStartDate(vehicle.Id, from, to, refference, maxMonths);
            var ts = te.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Error("Logictracker.Scheduler.Tasks.Mantenimiento.DatamartGeneration", string.Format("GetRegenerationStartDate en {0} segundos", ts));

            if (!start.HasValue) return new List<RegenerateDatamart>();

            var startDate = start.Value;

            te.Restart();
            var endDate = posicionesDao.GetRegenerationEndDate(vehicle.Id, from, to, maxMonths);
            ts = te.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Error("Logictracker.Scheduler.Tasks.Mantenimiento.DatamartGeneration", string.Format("GetRegenerationEndDate en {0} segundos", ts));

            var result = new List<RegenerateDatamart>();

            while (startDate < endDate)
            {
                var period = new RegenerateDatamart(startDate);
                result.Add(period);
                startDate = startDate.AddDays(1);
            }

            return result;
        }

        /// <summary>
        /// Get datamart records for the specified vehicle and timespan.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IList<Datamart> GetBetweenDates(Int32 vehicle, DateTime desde, DateTime hasta)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria(vehicle).FilterBeginBetween(desde, hasta);
            ICriteria crit = GetDatamartCriteria(0, dc, Order.Asc("Begin"));

            IList<Datamart> result = crit.List<Datamart>();
            return result;
        }

        public IList<Datamart> GetBetweenDates(List<int> ids, DateTime desde, DateTime hasta)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria(ids.ToArray()).FilterBeginEndBetween(desde, hasta);
            ICriteria crit = GetDatamartCriteria(0, dc, Order.Asc("Begin"));

            IList<Datamart> result = crit.List<Datamart>();
            return result;
        }

        /// <summary>
        /// Get datamart records with movement for the specified vehicle and timespan.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<Datamart> GetBetweenDatesWithMovement(Int32 vehicle, DateTime desde, DateTime hasta)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria(vehicle).FilterBeginBetween(desde, hasta);
            dc.Add(Restrictions.Gt("AverageSpeed", 0));
            ICriteria crit = GetDatamartCriteria(0, dc, Order.Asc("Begin"));

            IList<Datamart> result = crit.List<Datamart>();
            return result;
        }

        private IEnumerable<Datamart> GetBetweenDatesWithEmployee(bool EmployeeNull, Int32 vehicle, DateTime desde, DateTime hasta)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria(vehicle).FilterBeginBetween(desde, hasta);
            if (!EmployeeNull)
                dc.Add(Restrictions.Not(Restrictions.IsNull("Employee")));
            else
                dc.Add(Restrictions.IsNull("Employee"));
            ICriteria crit = GetDatamartCriteria(0, dc, Order.Asc("Begin"));

            IList<Datamart> result = crit.List<Datamart>();

            return result;
        }

        /// <summary>
        /// Get datamart records with employees for the specified vehicle and timespan.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<Datamart> GetBetweenDatesWithEmployee(Int32 vehicle, DateTime desde, DateTime hasta)
        {
            return GetBetweenDatesWithEmployee(false, vehicle, desde, hasta);
        }

        /// <summary>
        /// Get datamart records without employees for the specified vehicle and timespan.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<Datamart> GetBetweenDatesWithoutEmployee(Int32 vehicle, DateTime desde, DateTime hasta)
        {
            return GetBetweenDatesWithEmployee(true, vehicle, desde, hasta);
        }

        /// <summary>
        /// Gets sumarized information about the activity of the specified vehicles within the givenn timespan.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="ids"></param>
        /// <param name="km"></param>
        /// <returns></returns>
        public List<MobileActivity> GetMobileActivities(DateTime desde, DateTime hasta, Int32 empresa, Int32 linea, List<Int32> ids, Int32 km)
        {
            MobileActivity ma = null;
            DetachedCriteria vdc = GetVehiclesDetachedCriteria(ids.Count <= SQLParameterLimit ? ids.ToArray() : new int[] {}, new[] {empresa},
                                                  (linea != 0 ? new[] {0, linea} : new int[] {}));
            DetachedCriteria dc = GetDatamartDetachedCriteria(false, vdc).FilterBeginEndBetween(desde, hasta);

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => ma.HsActivos));
            pl.Add(Projections.Sum<Datamart>(dm => dm.StoppedHours).WithAlias(() => ma.HsInactivos));
            pl.Add(Projections.Sum<Datamart>(dm => dm.InfractionMinutes).WithAlias(() => ma.HsInfraccion));
            pl.Add(Projections.Sum<Datamart>(dm => dm.NoReportHours).WithAlias(() => ma.HsSinReportar));
            pl.Add(Projections.Sum<Datamart>(dm => dm.Infractions).WithAlias(() => ma.Infracciones));
            IProjection recorridoProjection = Projections.Sum<Datamart>(dm => dm.Kilometers).WithAlias(() => ma.Recorrido);
            pl.Add(recorridoProjection);
            pl.Add(Projections.Max<Datamart>(dm => dm.MaxSpeed).WithAlias(() => ma.VelocidadMaxima));
            pl.Add(Projections.Group<Datamart>(dm => dm.Vehicle.Id).WithAlias(() => ma.Id));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (MobileActivity));
            IList<MobileActivity> results = crit.List<MobileActivity>();

            var cocheDAO = new CocheDAO();
            foreach (MobileActivity r in results)
            {
                Coche coche = cocheDAO.FindById(r.Id);
                r.VelocidadPromedio = r.HsActivos > 0 ? Convert.ToInt32(r.Recorrido/r.HsActivos) : 0;
                r.HsInfraccion /= 60;
                r.CentroDeCostos = coche.CentroDeCostos != null ? coche.CentroDeCostos.Descripcion : null;
                r.TipoVehiculo = coche.TipoCoche != null ? coche.TipoCoche.Descripcion : null;
                r.Movil = coche.Interno;
                r.Patente = coche.Patente;
            }

            List<MobileActivity> result = (ids.Count > SQLParameterLimit ? results.Where(x => ids.Contains(x.Id)) : results).OrderBy(r => r.Movil).ToList();
            return result;
        }

        public IEnumerable<MobilesKilometers> GetMobilesKilometers(DateTime desde, DateTime hasta, List<Int32> ids)
        {
            return GetMobilesKilometers(desde, hasta, ids, false);
        }

        /// <summary>
        /// Gets the aggregate value for the kilometers of each vehicle.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="ids"></param>
        /// <param name="enCiclo"> </param>
        /// <returns></returns>
        public IEnumerable<MobilesKilometers> GetMobilesKilometers(DateTime desde, DateTime hasta, List<Int32> ids, bool enCiclo)
        {
            var table = Ids2DataTable(ids);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_DatamartDAO_GetMobilesKilometers] :ids, :dateFrom, :dateTo, :enCiclo;");
            sqlQ.SetStructured("ids", table);
            sqlQ.SetDateTime("dateFrom", desde);
            sqlQ.SetDateTime("dateTo", hasta);
            sqlQ.SetBoolean("enCiclo", enCiclo);
            sqlQ.SetResultTransformer(Transformers.AliasToBean(typeof(MobilesKilometers)));
            var results = sqlQ.List<MobilesKilometers>();
            return results;
        }

        public IEnumerable<KilometrosDiarios> GetKilometrosDiarios(DateTime desde, DateTime hasta, List<Int32> ids, int gmt)
        {
            var table = Ids2DataTable(ids);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_DatamartDAO_GetKilometrosDiarios] :ids, :dateFrom, :dateTo, :GMT;");
            sqlQ.SetStructured("ids", table);
            sqlQ.SetDateTime("dateFrom", desde);
            sqlQ.SetDateTime("dateTo", hasta);
            sqlQ.SetInt32("GMT", gmt);
            sqlQ.SetResultTransformer(Transformers.AliasToBean(typeof(KilometrosDiarios)));
            var results = sqlQ.List<KilometrosDiarios>();
            return results;
        }

        public SummarizedDatamart GetSummarizedDatamart(DateTime desde, DateTime hasta, int id)
        {
            SummarizedDatamart sd = null;
            DetachedCriteria dc = GetDatamartDetachedCriteria(id).FilterBeginEndBetween(desde, hasta);

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => sd.HsMovimiento));
            pl.Add(Projections.Sum<Datamart>(dm => dm.StoppedHours).WithAlias(() => sd.HsDetenido));
            pl.Add(Projections.Sum<Datamart>(dm => dm.Kilometers).WithAlias(() => sd.Kilometros));
            pl.Add(Projections.Sum<Datamart>(dm => dm.Consumo).WithAlias(() => sd.Consumo));
            pl.Add(Projections.Sum<Datamart>(dm => dm.HorasMarcha).WithAlias(() => sd.HsMarcha));
            pl.Add(Projections.Max<Datamart>(dm => dm.MaxSpeed).WithAlias(() => sd.MaxSpeed));
            pl.Add(Projections.Avg<Datamart>(dm => dm.AverageSpeed).WithAlias(() => sd.AverageSpeed));
            pl.Add(Projections.Group<Datamart>(dm => dm.Vehicle.Id).WithAlias(() => sd.Id));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (SummarizedDatamart));

            var km = crit.UniqueResult<SummarizedDatamart>();

            SummarizedDatamart result = km ?? new SummarizedDatamart(id);

            return result;
        }

        /// <summary>
        /// Gets the aggregate value for the times of each vehicle.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<MobilesTime> GetMobilesTimes(DateTime desde, DateTime hasta, List<Int32> ids)
        {
            MobilesTime mt = null;

            DetachedCriteria dc = GetDatamartDetachedCriteria(ids.ToArray()).FilterBeginEndBetween(desde, hasta);

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => mt.ElapsedTime));
            pl.Add(Projections.Group<Datamart>(dm => dm.Vehicle.Id).WithAlias(() => mt.Movil));
//            pl.Add(Projections.Group<Datamart>(dm => dm.Vehicle.Interno).WithAlias(() => mt.Intern));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (MobilesTime));
            IList<MobilesTime> results = crit.List<MobilesTime>();

            var cocheDAO = new CocheDAO();
            foreach (MobilesTime r in results)
            {
                Coche coche = cocheDAO.FindById(r.Movil);
                r.Intern = coche.Interno;
            }

            return results;
        }

        /// <summary>
        /// Gets aggregate activity for each specified transport company.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<TransportActivity> GetTransportActivities(DateTime desde, DateTime hasta, List<int> ids)
        {
            TransportActivity ta = null;

            DetachedCriteria dc = GetDatamartDetachedCriteria(ids.ToArray()).FilterBeginBetween(desde, hasta);

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => ta.HsActivos));
            pl.Add(Projections.Sum<Datamart>(dm => dm.StoppedHours).WithAlias(() => ta.HsInactivos));
            pl.Add(Projections.Sum<Datamart>(dm => dm.InfractionMinutes).WithAlias(() => ta.HsInfraccion));
            pl.Add(Projections.Sum<Datamart>(dm => dm.Infractions).WithAlias(() => ta.Infracciones));
            pl.Add(Projections.Sum<Datamart>(dm => dm.Kilometers).WithAlias(() => ta.Recorrido));
            pl.Add(Projections.Max<Datamart>(dm => dm.MaxSpeed).WithAlias(() => ta.VelocidadMaxima));
            pl.Add(Projections.Avg<Datamart>(dm => dm.AverageSpeed).WithAlias(() => ta.VelocidadPromedio));
            pl.Add(Projections.Group<Datamart>(dm => dm.Vehicle.Id).WithAlias(() => ta.IdVehiculo));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (TransportActivity));
            IList<TransportActivity> results = crit.List<TransportActivity>();

            var cocheDAO = new CocheDAO();
            foreach (TransportActivity r in results)
            {
                Coche coche = cocheDAO.FindById(r.Id);
                r.HsInfraccion /= 60;
                r.Transport = coche.Transportista.Descripcion;
                r.CentroDeCostos = coche.CentroDeCostos != null ? coche.CentroDeCostos.Descripcion : null;
                r.TipoVehiculo = coche.TipoCoche != null ? coche.TipoCoche.Descripcion : null;
            }
            return results;
        }

        /// <summary>
        /// Gets mobile maintenance aggregat data for the specified vehicles.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="vehiculos"></param>
        /// <returns></returns>
        public IEnumerable<MobileMaintenance> GetMobileMaintenanceData(DateTime desde, DateTime hasta, IEnumerable<Coche> vehiculos)
        {
            MobileMaintenance mm = null;
            int[] ids = vehiculos.Select(c => c.Id).ToArray();

            DetachedCriteria dc = GetDatamartDetachedCriteria(ids).FilterBeginBetween(desde, hasta);

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => mm.HsMarcha));
            pl.Add(Projections.Sum<Datamart>(dm => dm.Kilometers).WithAlias(() => mm.Kilometros));
            pl.Add(Projections.Group<Datamart>(dm => dm.Vehicle.Id).WithAlias(() => mm.IdVehiculo));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (MobileMaintenance));
            IList<MobileMaintenance> results = crit.List<MobileMaintenance>();

            var cocheDAO = new CocheDAO();
            foreach (MobileMaintenance r in results)
            {
                Coche coche = cocheDAO.FindById(r.IdVehiculo);
                r.Interno = coche.Interno;
                r.TipoVehiculo = coche.TipoCoche != null ? coche.TipoCoche.Descripcion : null;
                r.Patente = coche.Patente;
                r.Referencia = coche.Referencia;
            }
            return results;
        }

        private IEnumerable<Datamart> GetBetweenDatesInternal(bool withShift, int coche, DateTime iniDate, DateTime finDate, bool enableUndefined)
        {
            var statuses = new List<String> {Datamart.EstadosMotor.Apagado};
            if (enableUndefined)
                statuses.Add(Datamart.EstadosMotor.Indefinido);

            DetachedCriteria dc = GetDatamartDetachedCriteria(coche).FilterBeginBetween(iniDate, finDate);
            dc.Add(withShift ? Restrictions.Not(Restrictions.IsNull("Shift")) : Restrictions.IsNull("Shift"));

            dc.Add(Restrictions.In("EngineStatus", statuses));

            ICriteria crit = GetDatamartCriteria(0, dc, null);
            IList<Datamart> results = crit.List<Datamart>();
            return results;
        }

        /// <summary>
        /// Gets datamart records for the specified vehicle within the specified timespan that are associated to a shift.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="iniDate"></param>
        /// <param name="finDate"></param>
        /// <param name="enableUndefined"></param>
        /// <returns></returns>
        public IEnumerable<Datamart> GetBetweenDatesWithShift(Int32 coche, DateTime iniDate, DateTime finDate, Boolean enableUndefined)
        {
            return GetBetweenDatesInternal(true, coche, iniDate, finDate, enableUndefined);
        }

        /// <summary>
        /// Gets datamart records for the specified vehicle within the specified timespan that are not associated to a shift.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="iniDate"></param>
        /// <param name="finDate"></param>
        /// <param name="enableUndefined"></param>
        /// <returns></returns>
        public IEnumerable<Datamart> GetBetweenDatesWithoutShift(Int32 coche, DateTime iniDate, DateTime finDate, Boolean enableUndefined)
        {
            return GetBetweenDatesInternal(false, coche, iniDate, finDate, enableUndefined);
        }

        ///// <summary>
        ///// Gets operator ranking information for the current tmespan.
        ///// </summary>
        ///// <param name="desde"></param>
        ///// <param name="hasta"></param>
        ///// <param name="tipos"></param>
        ///// <param name="transportistas"></param>
        ///// <param name="bases"></param>
        ///// <param name="distritos"></param>
        ///// <returns></returns>
        //public IEnumerable<OperatorRanking> GetOperatorsRanking(DateTime desde, DateTime hasta, List<Int32> tipos, List<Int32> transportistas, List<Int32> bases, List<Int32> distritos)
        //{
        //    return Session.Query<Datamart>()
        //        .Where(data => data.Employee != null && ((data.Employee.TipoEmpleado == null && tipos.Contains(-2)) || (data.Employee.TipoEmpleado != null && tipos.Contains(data.Employee.TipoEmpleado.Id)))
        //            && ((data.Employee.Transportista == null && transportistas.Contains(-2)) || (data.Employee.Transportista != null && transportistas.Contains(data.Employee.Transportista.Id)))
        //            && (data.Employee.Linea == null || bases.Contains(data.Employee.Linea.Id)) && (data.Employee.Empresa == null || distritos.Contains(data.Employee.Empresa.Id))
        //            && data.Begin >= desde && data.Begin <= hasta)
        //        .GroupBy(data => new { data.Employee.Id, data.Employee.Entidad.Descripcion, data.Employee.Legajo })
        //        .Select(data => new OperatorRanking
        //            {
        //                Kilometros = data.Sum(datamart => datamart.Kilometers),
        //                Hours = data.Sum(datamart => datamart.MovementHours),
        //                IdOperador = data.Key.Id,
        //                Operador = data.Key.Descripcion,
        //                Legajo = data.Key.Legajo
        //            })
        //        .ToList();
        //}

        /// <summary>
        /// Gets operator ranking information for the current tmespan.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="tipos"></param>
        /// <param name="transportistas"></param>
        /// <param name="bases"></param>
        /// <param name="distritos"></param>
        /// <returns></returns>
        public IEnumerable<OperatorRanking> GetOperatorsRanking(
            DateTime desde, DateTime hasta, List<Int32> tipos, List<Int32> transportistas, List<Int32> bases, List<Int32> distritos)
        {
            OperatorRanking or = null;
            Empleado ch = null;

            DetachedCriteria dc =
                GetDatamartDetachedCriteria()
                    .FilterBeginBetween(desde, hasta)
                .CreateAlias("Employee", "ch", JoinType.InnerJoin)
                .CreateAlias("Employee.Entidad", "en", JoinType.InnerJoin);


            // Tipo de Empleado
            bool tipoNone = tipos.Contains(-2);
            bool tipoOther = tipos.Any(t => t > 0);
            bool tipoAll = tipos.Contains(-1);

            if (!tipoAll)
            {
                if (tipoNone && tipoOther)
                {
                    dc.Add(Restrictions.Or(Restrictions.IsNull(Projections.Property<Empleado>(p => ch.TipoEmpleado)),
                        Restrictions.In(Projections.Property<Empleado>(p => ch.TipoEmpleado.Id), tipos)));
                }
                else
                {
                    if (tipoNone) dc.Add(Restrictions.IsNull(Projections.Property<Empleado>(p => ch.TipoEmpleado)));
                    if (tipoOther)
                        dc.Add(Restrictions.In(Projections.Property<Empleado>(p => ch.TipoEmpleado.Id), tipos));
                }
            }

            // Transportista
            bool transNone = transportistas.Contains(-2);
            bool transOther = transportistas.Any(t => t > 0);
            bool transAll = transportistas.Contains(-1);

            if (!transAll)
            {
                if (transNone && transOther)
                {
                    dc.Add(Restrictions.Or(Restrictions.IsNull(Projections.Property<Empleado>(p => ch.Transportista)),
                        Restrictions.In(Projections.Property<Empleado>(p => ch.Transportista.Id), transportistas)));
                }
                else
                {
                    if (tipoNone) dc.Add(Restrictions.IsNull(Projections.Property<Empleado>(p => ch.Transportista)));
                    if (tipoOther) dc.Add(Restrictions.InG(Projections.Property<Empleado>(p => ch.Transportista.Id), transportistas));
                }
            }

            if (!bases.Contains(-1))
            {
                dc.Add(Restrictions.Or(Restrictions.IsNull(Projections.Property<Empleado>(p => ch.Linea)),
                    Restrictions.In(Projections.Property<Empleado>(p => ch.Linea.Id), bases)));
            }

            if (!distritos.Contains(-1))
            {
                dc.Add(Restrictions.Or(Restrictions.IsNull(Projections.Property<Empleado>(p => ch.Empresa)),
                    Restrictions.In(Projections.Property<Empleado>(p => ch.Empresa.Id), distritos)));
            }


            ProjectionList pl = Projections.ProjectionList();

            pl.Add(Projections.Sum<Datamart>(dm => dm.Kilometers).WithAlias(() => or.Kilometros));
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => or.Hours));
            pl.Add(Projections.Group<Datamart>(dm => dm.Employee.Id).WithAlias(() => or.IdOperador));            
            pl.Add(Projections.Group<Datamart>(dm => dm.Employee.Legajo).WithAlias(() => or.Legajo));
            pl.Add(Projections.Group<Datamart>(dm => dm.Employee.Entidad.Apellido).WithAlias(() => or.Operador));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (OperatorRanking));

            IList<OperatorRanking> results = crit.List<OperatorRanking>();
            return results;
        }

        public IEnumerable<OperatorRanking> GetOperatorsRanking(DateTime desde, DateTime hasta, List<int> choferes)
        {
            OperatorRanking or = null;

            DetachedCriteria dc = GetDatamartDetachedCriteria().FilterBeginBetween(desde, hasta);

            if (choferes.Count <= SQLParameterLimit)
                dc.Add(Restrictions.In(Projections.Property<Datamart>(p => p.Employee.Id), choferes));

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.Kilometers).WithAlias(() => or.Kilometros));
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => or.Hours));
            pl.Add(Projections.Group<Datamart>(dm => dm.Employee.Id).WithAlias(() => or.IdOperador));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (OperatorRanking));
            IList<OperatorRanking> results = crit.List<OperatorRanking>();

            var empleadoDAO = new EmpleadoDAO();
            foreach (OperatorRanking r in results)
            {
                Empleado empleado = empleadoDAO.FindById(r.IdOperador);
                r.Operador = empleado.Entidad.Apellido;
                r.Legajo = empleado.Legajo;
            }
            IEnumerable<OperatorRanking> result = (choferes.Count > SQLParameterLimit ? results.Where(c => choferes.Contains(c.IdOperador)) : results);
            return result;
        }

        /// <summary>
        /// Gets aggregate operator activities for the current timespan.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="distrito"></param>
        /// <param name="planta"></param>
        /// <param name="ids"></param>
        /// <param name="km"></param>
        /// <returns></returns>
        public IEnumerable<OperatorActivity> GetOperatorActivities(DateTime desde, DateTime hasta, Int32 distrito, Int32 planta, List<Int32> ids, Double km)
        {
            OperatorActivity oa = null;

            DetachedCriteria dc = GetDatamartDetachedCriteria().FilterBeginBetween(desde, hasta).CreateAlias("Employee", "ch", JoinType.InnerJoin);

            if (distrito != -1)
                dc.Add(Restrictions.Or(Restrictions.IsNull("ch.Empresa"), Restrictions.Eq("ch.Empresa.Id", distrito)));

            if (planta != -1)
                dc.Add(Restrictions.Or(Restrictions.IsNull("ch.Linea"), Restrictions.Eq("ch.Linea.Id", distrito)));

            if (ids.Count <= SQLParameterLimit)
                dc.Add(Restrictions.In("ch.Id", ids));

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.StoppedHours).WithAlias(() => oa.StoppedHours));
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours).WithAlias(() => oa.MovementHours));
            pl.Add(Projections.Sum<Datamart>(dm => dm.InfractionMinutes).WithAlias(() => oa.InfractionsMinutes));
            pl.Add(Projections.Sum<Datamart>(dm => dm.Infractions).WithAlias(() => oa.Infractions));
            IProjection kilometersProjection = Projections.Sum<Datamart>(dm => dm.Kilometers).WithAlias(() => oa.Kilometers);
            pl.Add(kilometersProjection);
            pl.Add(Projections.Max<Datamart>(dm => dm.MaxSpeed).WithAlias(() => oa.MaxSpeed));
            pl.Add(Projections.Group<Datamart>(dm => dm.Employee.Id).WithAlias(() => oa.OperatorId));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null, typeof (OperatorActivity));
            crit.Add(Restrictions.Ge(kilometersProjection, km));

            IList<OperatorActivity> results = crit.List<OperatorActivity>();

            var empleadoDAO = new EmpleadoDAO();
            foreach (OperatorActivity r in results)
            {
                Empleado empleado = empleadoDAO.FindById(r.OperatorId);
                r.Operator = empleado.Entidad != null ? empleado.Entidad.Descripcion : null;
            }

            foreach (OperatorActivity r in results.Where(filteredResult => filteredResult.Kilometers > 0 && filteredResult.MovementHours > 0))
                r.AverageSpeed = Convert.ToInt32(r.Kilometers/r.MovementHours);

            IEnumerable<OperatorActivity> result = (ids.Count > SQLParameterLimit ? results.Where(r => ids.Contains(r.OperatorId)) : results);
            return result;
        }

        /// <summary>
        /// Gets all datamart records for the specified employee and timespan.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<Datamart> GetForEmployee(Int32 employee, DateTime desde, DateTime hasta)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteriaForEmployee(new[] {employee}).FilterBeginBetween(desde, hasta);
            ICriteria crit = GetDatamartCriteria(0, dc, null);
            IList<Datamart> result = crit.List<Datamart>();
            return result;
        }

        private Double HoursShift(bool within, int vehicle, DateTime desde, DateTime hasta)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria(vehicle).FilterBeginEndBetween(desde, hasta);
            dc.Add(within ? Restrictions.Not(Restrictions.IsNull("Shift")) : Restrictions.IsNull("Shift"));

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Sum<Datamart>(dm => dm.MovementHours));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null);

            var result = crit.UniqueResult<Double>();
            return result;
        }

        /// <summary>
        /// Gets the total amount of hours within shifts.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public Double HoursWithinShift(Int32 vehicle, DateTime desde, DateTime hasta)
        {
            return HoursShift(true, vehicle, desde, hasta);
        }

        /// <summary>
        /// Gets the total amount of hours without shifts.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public Double HoursWithoutShift(Int32 vehicle, DateTime desde, DateTime hasta)
        {
            return HoursShift(false, vehicle, desde, hasta);
        }

        /// <summary>
        /// Gets total hours within any base for the specified vehicle and timespan.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="geoRefIds"></param>
        /// <returns></returns>
        public Double GetHoursInGeofence(Int32 vehicle, DateTime desde, DateTime hasta, List<Int32> geoRefIds)
        {
            DetachedCriteria dc = GetDatamartDetachedCriteria(vehicle).FilterBeginEndBetween(desde, hasta);
            dc.CreateAlias("GeograficRefference", "gr", JoinType.InnerJoin).Add(Restrictions.In("gr.Id", geoRefIds));


            AggregateProjection p1 = Projections.Sum<Datamart>(dm => dm.MovementHours);
            AggregateProjection p2 = Projections.Sum<Datamart>(dm => dm.StoppedHours);
            AggregateProjection p3 = Projections.Sum<Datamart>(dm => dm.NoReportHours);

            var p = Projections.SqlFunction(new VarArgsSQLFunction("(", "+", ")"), NHibernateUtil.Double, p1, p2, p3);

            ProjectionList pl = Projections.ProjectionList();
            pl.Add(Projections.Alias(p, "SumOfSums"));

            ICriteria crit = GetDatamartCriteria(0, dc, pl, null);

            var result = crit.UniqueResult<Double>();
            return result;
        }

        /// <summary>
        /// Gets the datamart record with the max speed for the specified vehicle and timespan.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <returns></returns>
        public Datamart GetByVehicleMaxSpeed(Int32 vehicle, DateTime desde)
        {
            var dc = GetDatamartDetachedCriteria(vehicle).FilterBeginGeLt(desde, desde.AddDays(1));
            var crit = GetDatamartCriteria(1, dc, Order.Desc("MaxSpeed"));
            var result = crit.UniqueResult<Datamart>();
            return result;
        }

        /// <summary>
        /// Gets the datamart record with the max speed for the specified employee and timespan.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="desde"></param>
        /// <returns></returns>
        public Datamart GetByEmployeeMaxSpeed(Int32 employee, DateTime desde)
        {
            var dc = GetDatamartDetachedCriteriaForEmployee(employee).FilterBeginGeLt(desde, desde.AddDays(1));
            var crit = GetDatamartCriteria(1, dc, Order.Desc("MaxSpeed"));
            var result = crit.UniqueResult<Datamart>();
            return result;
        }

        /// <summary>
        /// Gets all records associated to vehicles stopped within the specified base.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="idBase"></param>
        /// <param name="showUndefined"></param>
        /// <returns></returns>
        public IEnumerable<Datamart> GetVehiclesStoppedInBase(DateTime start, DateTime end, Int32 idBase, Boolean showUndefined)
        {
            var statuses = new List<String> {Datamart.EstadosMotor.Apagado};
            if (showUndefined)
                statuses.Add(Datamart.EstadosMotor.Indefinido);

            DetachedCriteria dc =
                GetDatamartDetachedCriteria()
                .FilterBeginBetween(start, end)
                .CreateAlias("Vehicle", "c", JoinType.InnerJoin)
                .CreateAlias("GeograficRefference", "gr", JoinType.InnerJoin)
                .Add(Restrictions.Eq("gr.Id", idBase))
                .Add(Restrictions.In("EngineStatus", statuses));

            ICriteria crit = GetDatamartCriteria(0, dc, null);

            IList<Datamart> result = crit.List<Datamart>();
            return result;
        }

        #endregion

        [Serializable]
        public class SummarizedDatamart
        {
            public SummarizedDatamart()
            {
            }

            public SummarizedDatamart(int id)
            {
                Id = id;
                Kilometros = 0;
                Consumo = 0;
                HsMarcha = 0;
                HsMovimiento = 0;
                HsDetenido = 0;
                MaxSpeed = 0;
                AverageSpeed = 0;
            }

            public int Id { get; set; }
            public double Kilometros { get; set; }
            public double Consumo { get; set; }
            public double HsMarcha { get; set; }
            public double HsMovimiento { get; set; }
            public double HsDetenido { get; set; }
            public double MaxSpeed { get; set; }
            public double AverageSpeed { get; set; }
        }
    }
}