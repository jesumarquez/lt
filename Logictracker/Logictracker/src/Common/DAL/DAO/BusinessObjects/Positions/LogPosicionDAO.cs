using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Positions
{
    public class LogPosicionDAO : MaintenanceDAO<LogPosicion>
    {
        //public LogPosicionDAO(ISession session) : base(session) { }

        private const int FirstRow = 1;

        private ICriteria InternalGetLastNPositionsBetweenDates(int? vehicleId, int? n, DateTime? dateFrom, DateTime? dateTo, int? maxMonths, Order order, DateTime? dateFromReceived, DateTime? dateToReceived)
        {
            if (maxMonths != null)
            {
                var limite = DateTime.UtcNow.AddMonths(-maxMonths.Value);
                dateFrom = dateFrom == null ? limite : (dateFrom.Value < limite ? limite : dateFrom.Value);

                if (dateFromReceived != null || dateToReceived != null)
                {
                    var limiteReceived = DateTime.UtcNow.AddMonths(-maxMonths.Value);
                    dateFromReceived = dateFromReceived == null ? limiteReceived : (dateFromReceived.Value < limiteReceived ? limiteReceived : dateFromReceived.Value);
                }
            }
            var dc = DetachedCriteria.For<LogPosicion>("dlp")
                                     .SetProjection(Projections.Property("Id"));

            if (vehicleId != null) 
                dc.Add(Restrictions.Eq("Coche.Id", vehicleId));

            if (dateFrom != null && dateTo != null)
            {
                dc.Add(Restrictions.Between("dlp.FechaMensaje", dateFrom, dateTo));
            }
            else
            {
                if (dateFrom != null)
                    dc.Add(Restrictions.Ge("dlp.FechaMensaje", dateFrom));
                else if (dateTo != null)
                {
                    dc.Add(Restrictions.Le("dlp.FechaMensaje", dateTo));
                }
            }

            if (dateFromReceived != null || dateToReceived != null)
            {
                dc.Add(dateToReceived != null
                           ? Restrictions.Between("dlp.FechaRecepcion", dateFromReceived, dateToReceived)
                           : Restrictions.Ge("dlp.FechaRecepcion", dateFromReceived));
            }
            else
            {
                dc.Add(Restrictions.EqProperty("dlp.FechaMensaje", "lp.FechaMensaje"));
            }

            dc.Add(Restrictions.EqProperty("dlp.Id", "lp.Id"));                

            var rc = Session.CreateCriteria<LogPosicion>("lp")
                .Add(Subqueries.Exists(dc));

            if (n != null)
                rc.SetMaxResults(n.Value);

            if (order != null)
                rc.AddOrder(order);

            return rc;
        }

        public IEnumerable<LogPosicionBase> GetLastNPositions(int vehicleId, int n, int maxMonths)
        {
            var p = InternalGetLastNPositionsBetweenDates(vehicleId, n, null, null, maxMonths, Order.Desc("lp.FechaMensaje"), null, null);
            var positions = p.List<LogPosicionBase>();
            return positions;
        }

        public Dictionary<int, LogUltimaPosicionVo> GetLastVehiclesPositions(IEnumerable<Coche> coches)
        {
            return coches.ToDictionary(coche => coche.Id, coche => GetLastOnlineVehiclePosition(coche));
        }

        public LogUltimaPosicionVo GetLastOnlineVehiclePosition(Coche coche)
        {
            try
            {
                if (coche == null) return null;

                LogUltimaPosicionVo lastPositionVo = null;

                if (coche.IsLastPositionInCache())
                    lastPositionVo = coche.RetrieveLastPosition();

                if (lastPositionVo != null) // if a position existed in cached, then return it else go find last one on DB.
                    return lastPositionVo;

                var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;

                var q = InternalGetLastNPositionsBetweenDates(coche.Id, FirstRow, null, null, maxMonths, Order.Desc("lp.FechaMensaje"), null, null);
                var fe = q.UniqueResult<LogPosicion>();

                if (fe != null)
                    lastPositionVo = new LogUltimaPosicionVo(fe);

                coche.StoreLastPosition(lastPositionVo);

                return lastPositionVo;
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e);
                return null;
            }
        }

        public LogUltimaPosicionVo GetLastVehiclePosition(Coche coche)
        {
            try
            {
                if (coche == null) return null;

                LogUltimaPosicionVo lastPositionVo = null;
                
                if (coche.IsLastPositionInCache())
                    lastPositionVo = coche.RetrieveLastPosition();

                if (coche.IsNewestPositionReceivedInCache())
                {
                    var newestReceivedPos = coche.RetrieveNewestReceivedPositionVo();
                    if (newestReceivedPos != null)
                    {
                        newestReceivedPos.Coche = newestReceivedPos.Coche + " *";

                        if (lastPositionVo == null ||
                            newestReceivedPos.FechaMensaje > lastPositionVo.FechaMensaje)
                            lastPositionVo = newestReceivedPos;
                    }
                }
                
                if (lastPositionVo != null) // if a position existed in cached, then return it else go find last one on DB.
                    return lastPositionVo;

                var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;

                var te = new TimeElapsed();
                var q = InternalGetLastNPositionsBetweenDates(coche.Id, FirstRow, null, null, maxMonths, Order.Desc("lp.FechaMensaje"), null, null);
                var fe = q.UniqueResult<LogPosicion>();
                var totalSecs = te.getTimeElapsed().TotalSeconds;
                if (totalSecs > 1) STrace.Error("DispatcherLock", coche.Dispositivo.Id, "InternalGetLastNPositionsBetweenDates: " + totalSecs);

                if (fe != null)
                    lastPositionVo = new LogUltimaPosicionVo(fe);

                te.Restart();
                coche.StoreLastPosition(lastPositionVo);
                totalSecs = te.getTimeElapsed().TotalSeconds;
                if (totalSecs > 1) STrace.Error("DispatcherLock", coche.Dispositivo.Id, "StoreLastPosition: " + totalSecs);

                return lastPositionVo;
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e);
                return null;
            }
        }

        public IList<LogPosicion> GetPositionsBetweenDates(int vehicleId, DateTime desde, DateTime hasta)
        {
            var q = InternalGetLastNPositionsBetweenDates(vehicleId, null, desde, hasta, null, Order.Asc("lp.FechaMensaje"), null, null);
            return q.List<LogPosicion>();
        }

        public IList<LogPosicion> GetPositionsBetweenDates(int vehicleId, DateTime desde, DateTime hasta, int maxMonths)
        {
            var limite = DateTime.UtcNow.AddMonths(-maxMonths);
            desde = desde < limite ? limite : desde;
            hasta = hasta < limite ? limite : hasta;
            
            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogPosicionDAO_GetPositionsBetweenDates_3] @vehicleId = :vehicleId, @desde = :desde, @hasta = :hasta;")
                              .AddEntity(typeof(LogPosicion))
                              .SetInt32("vehicleId", vehicleId)
                              .SetDateTime("desde", desde)
                              .SetDateTime("hasta", hasta);
            var results = sqlQ.List<LogPosicion>();
            return results;

            //var q = InternalGetLastNPositionsBetweenDates(vehicleId, null, desde, hasta, maxMonths, Order.Asc("lp.FechaMensaje"), null, null);
            //return q.List<LogPosicion>();
        }

        public IList<LogPosicion> GetPositionsForDate(DateTime date, int maxMonths)
        {
            var from = date;
            var to = date.AddDays(1);

            var q = InternalGetLastNPositionsBetweenDates(null, null, from, to, maxMonths, null, null, null);
            return q.List<LogPosicion>();
        }

        public DateTime? GetRegenerationStartDate(int vehicleId, DateTime from, DateTime to, DateTime refference, int maxMonths)
        {
            var limite = DateTime.UtcNow.AddMonths(-maxMonths);
            from = from < limite ? limite : from;
            to = to < limite ? limite : to;

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogPosicionDAO_GetRegenerationStartDate_2] @vehicleId = :vehicleId, @desde = :desde, @hasta = :hasta, @recepcionDesde = :recepcionDesde, @recepcionHasta = :recepcionHasta;")
                              .AddEntity(typeof(LogPosicion))
                              .SetInt32("vehicleId", vehicleId)
                              .SetDateTime("desde", from)
                              .SetDateTime("hasta", to)
                              .SetDateTime("recepcionDesde", from)
                              .SetDateTime("recepcionHasta", to);
            var results = sqlQ.List<LogPosicion>();
            var firstPosition = results.FirstOrDefault();

            //var q = InternalGetLastNPositionsBetweenDates(vehicleId, FirstRow, refference, from, maxMonths, Order.Asc("lp.FechaMensaje"), from, to);

            //var firstPosition = q.UniqueResult<LogPosicion>();
            if (firstPosition == null) return null;
            return new DateTime(firstPosition.FechaMensaje.Year, firstPosition.FechaMensaje.Month, firstPosition.FechaMensaje.Day, 0, 0, 0);
        }

        public DateTime GetRegenerationEndDate(int vehicleId, DateTime from, DateTime to, int maxMonths)
        {
            var limite = DateTime.UtcNow.AddMonths(-maxMonths);
            var desde = from < limite ? limite : from;
            var hasta = to < limite ? limite : to;

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogPosicionDAO_GetRegenerationEndDate_2] @vehicleId = :vehicleId, @desde = :desde, @recepcionDesde = :recepcionDesde, @recepcionHasta = :recepcionHasta;")
                              .AddEntity(typeof(LogPosicion))
                              .SetInt32("vehicleId", vehicleId)
                              .SetDateTime("desde", desde)
                              .SetDateTime("recepcionDesde", desde)
                              .SetDateTime("recepcionHasta", hasta);
            var results = sqlQ.List<LogPosicion>();

            //var q = InternalGetLastNPositionsBetweenDates(vehicleId, FirstRow, from, null, maxMonths, Order.Desc("lp.FechaMensaje"), from, to);

            //var firstPosition = q.UniqueResult<LogPosicion>();
            var firstPosition = results.FirstOrDefault();
            return firstPosition != null 
                        ? new DateTime(firstPosition.FechaMensaje.Year, firstPosition.FechaMensaje.Month, firstPosition.FechaMensaje.Day, 23, 59, 59, 99) 
                        : to;
        }

        public LogPosicion GetFirstPositionOlderThanDate(int vehicleId, DateTime date, int maxMonths)
        {
            var limite = DateTime.UtcNow.AddMonths(-maxMonths);
            date = date < limite ? limite : date;

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogPosicionDAO_GetFirstPositionOlderThanDate_2] @vehicleId = :vehicleId, @date = :date;")
                              .AddEntity(typeof(LogPosicion))
                              .SetInt32("vehicleId", vehicleId)
                              .SetDateTime("date", date);
            var results = sqlQ.List<LogPosicion>();
            return results.FirstOrDefault();

            //var q = InternalGetLastNPositionsBetweenDates(vehicleId, FirstRow, null, fecha, maxMonths, Order.Desc("lp.FechaMensaje"), null, null);
            //return q.UniqueResult<LogPosicion>();
        }

        public LogPosicion GetFirstPositionNewerThanDate(int vehicleId, DateTime date, int maxMonths)
        {
            var limite = DateTime.UtcNow.AddMonths(-maxMonths);
            date = date < limite ? limite : date;
            
            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogPosicionDAO_GetFirstPositionNewerThanDate_2] @vehicleId = :vehicleId, @date = :date;")
                              .AddEntity(typeof(LogPosicion))
                              .SetInt32("vehicleId", vehicleId)
                              .SetDateTime("date", date);
            var results = sqlQ.List<LogPosicion>();
            return results.FirstOrDefault();

            //var q = InternalGetLastNPositionsBetweenDates(vehicleId, FirstRow, date, null, maxMonths, Order.Asc("lp.FechaMensaje"), null, null);
            //return q.UniqueResult<LogPosicion>();
        }

        public LogPosicion GetFirtPosition(int vehicleId)
        {
            var q = InternalGetLastNPositionsBetweenDates(vehicleId, FirstRow, null, null, null, Order.Asc("lp.FechaMensaje"), null, null);
            return q.UniqueResult<LogPosicion>();
        }

        public int GetVehiclesInside(IEnumerable<int> vehicles, Geocerca geocerca, DateTime fecha)
        {
            var count = 0;            

            foreach (var vehicleId in vehicles)
            {
                var position = GetFirstPositionOlderThanDate(vehicleId, fecha, 1);
                if (position != null)
                {
                    var latitud = position.Latitud;
                    var longitud = position.Longitud;
                    var point = new PointF((float)longitud, (float)latitud);
                    var inside = geocerca.IsInBounds(point) && geocerca.Contains(latitud, longitud);
                    if (inside) count++;
                }
            }

            return count;
        }

        protected override String GetDeleteCommand()
        {
            return "delete top(:n) from opeposi01 where opeposi01_fechora <= :date ; select @@ROWCOUNT as count;";
        }
    }
}