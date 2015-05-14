#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible
{
    public class VolumenHistoricoDAO : GenericDAO<VolumenHistorico>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public VolumenHistoricoDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        public VolumenHistorico GetLastTankRealLevel(Tanque tank,DateTime hasta)
        {
            if (tank == null) return null;

            var list = Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == tank.Id && !v.EsTeorico && v.Fecha < hasta)
                .OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id);

            return list.FirstOrDefault();
        }

        /// <summary>
        /// If not Teoric Volume is found returns the nearest Real volume found, else null.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="maxDate"></param>
        /// <returns></returns>
        public VolumenHistorico GetLastTankTeoricMovement(Tanque tank, DateTime maxDate)
        {
            if (tank == null) return null;

            var list = Session.Query<VolumenHistorico>().Where( v => v.Tanque.Id == tank.Id && v.EsTeorico && v.Fecha <= maxDate)
                .OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id);

            if (!list.Any())
            {
                list = Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == tank.Id && !v.EsTeorico && v.Fecha <= maxDate)
                    .OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id);
            }

            return list.FirstOrDefault();
        }

        public List<VolumenHistorico> FindAllTeoricVolumeEvents(int tankId)
        {

            var lastVolume = FindLastTeoricVolume();
            
            return Session.Query<VolumenHistorico>().Where( v => v.Tanque != null && v.Tanque.Id == tankId && v.EsTeorico 
                                      && (lastVolume == null || v.Fecha > lastVolume.Fecha)).OrderBy(v => v.Fecha).ToList();          
        }

        public VolumenHistorico FindLastTeoricVolume()
        {
            var list = Session.Query<VolumenHistorico>().Where(v => v.EsTeorico).OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id);

            return list.FirstOrDefault();
        }

        public VolumenHistorico FindLastTeoricVolumeUnderCapacity(int tankId, DateTime maxDate)
        {
            var list = Session.Query<VolumenHistorico>().Where(v => v.EsTeorico && v.Fecha < maxDate && v.Tanque.Capacidad != 0
                     && v.Volumen <= v.Tanque.Capacidad).OrderByDescending(v => v.Fecha);

            return list.FirstOrDefault();
        }

        public VolumenHistorico FindLastTeoricVolumeBelowRealAndTeoricDifference(int idTank, DateTime maxDate,DateTime minDate, double volumeDiff)
        {
            if (volumeDiff.Equals(0)) return null;

            var realVolumes = Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == idTank && !v.EsTeorico).ToList();

            var teoricVolumes = Session.Query<VolumenHistorico>().Where(o => o.Tanque.Id == idTank && !o.EsTeorico && o.Fecha > minDate && o.Fecha < maxDate).ToList();

            return (from vol in teoricVolumes
                    let realvolume = (from o in realVolumes where o.Fecha <= vol.Fecha orderby o.Fecha select o).FirstOrDefault()
                    where realvolume != null && volumeDiff < Math.Abs(vol.Volumen - realvolume.Volumen)
                    select vol).FirstOrDefault();
        }

        /// <summary>
        /// /// If not Teoric Volume is found returns the nearest Real volume found, else null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VolumenHistorico GetLastTankTeoricLevel(int id)
        {
            var list = Session.Query<VolumenHistorico>().Where(t => t.Tanque.Id == id && t.EsTeorico).OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id);

            if (!list.Any()) list = Session.Query<VolumenHistorico>().Where(t => t.Tanque.Id == id && !t.EsTeorico).OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id);

            return list.FirstOrDefault();
        }

        /// <summary>
        /// Founds All real volumes between given dates for the given tank.
        /// </summary>
        /// <param name="tankId"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<VolumenHistorico> FindAllRealVolumes(int tankId, DateTime desde, DateTime hasta)
        {
            return Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == tankId && !v.EsTeorico && v.Fecha >= desde && v.Fecha <= hasta)
                .OrderBy(v => v.Fecha).ThenBy(v => v.Id).ToList();
        }

        /// <summary>
        /// Gets initial real volume for the specified tank and timespan.
        /// </summary>
        /// <param name="tankId"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public VolumenHistorico FindInitialRealVolume(int tankId, DateTime desde, DateTime hasta)
        {
            return Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == tankId && !v.EsTeorico && v.Fecha >= desde && v.Fecha <= hasta)
                .OrderBy(v => v.Fecha).ThenBy(v => v.Id).FirstOrDefault();
        }

        /// <summary>
        /// Gets last real volume for the specified tank and timespan.
        /// </summary>
        /// <param name="tankId"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public VolumenHistorico FindLastRealVolume(int tankId, DateTime desde, DateTime hasta)
        {
            return Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == tankId && !v.EsTeorico && v.Fecha >= desde && v.Fecha <= hasta)
                .OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id).FirstOrDefault();
        }

        /// <summary>
        /// Finds the actual volume for the specified tank.
        /// </summary>
        /// <param name="tanque"></param>
        /// <param name="date"></param>
        /// <param name="hasta"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public VolumenHistorico FindActualVolume(Int32 tanque, DateTime date, DateTime hasta, TimeSpan interval)
        {
            return Session.Query<VolumenHistorico>().Where(v => !v.EsTeorico && v.Tanque != null && v.Tanque.Id == tanque && v.Fecha > date && v.Fecha <= date.Add(interval) 
                && v.Fecha <= hasta && v.Fecha <= DateTime.Now).OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id).FirstOrDefault();
        }

        /// <summary>
        /// Gets initial theoric volume for the specified tank and timespan.
        /// </summary>
        /// <param name="tankId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public VolumenHistorico FindInitialTheoricVolume(int tankId, DateTime date)
        {
            return Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == tankId && v.EsTeorico && v.Fecha <= date).OrderByDescending(v => v.Fecha)
                .ThenByDescending(v => v.Id).FirstOrDefault();
        }

        /// <summary>
        /// Founds All real volumes between given dates for the given tank.
        /// Shows only the latest volume for the interval.
        /// </summary>
        /// <param name="tankId"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="groupByXMinutes"></param>
        /// <returns></returns>
        public List<VolumenHistorico> FindAllRealVolumes(int tankId, DateTime desde, DateTime hasta,int groupByXMinutes)
        {
            var actualDate = desde;
            var endDate = actualDate.AddMinutes(groupByXMinutes);
            var results = new List<VolumenHistorico>();

            var initialVolume = Session.Query<VolumenHistorico>().Where(v => v.Tanque.Id == tankId && !v.EsTeorico && v.Fecha >= actualDate && v.Fecha <= endDate)
                .OrderBy(v => v.Fecha).ThenBy(v => v.Id).FirstOrDefault();

            if(initialVolume != null) results.Add(initialVolume);

            while(actualDate <= hasta)
            {
                var volume = Session.Query<VolumenHistorico>().Where( v => v.Tanque.Id == tankId && !v.EsTeorico && v.Fecha >= actualDate && v.Fecha <= endDate)
                    .OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id).FirstOrDefault();

                if (volume != null) results.Add(volume);

                actualDate = endDate;
                endDate = actualDate.AddMinutes(groupByXMinutes);
            }

            return results;
        }

        /// <summary>
        /// Finds all the volumes for the tank after the passed date.
        /// </summary>
        /// <param name="tanque"></param>
        /// <param name="desde"></param>
        /// <returns></returns>
        public IEnumerable<VolumenHistorico> FindAllTeoricVolumesAfterDate(Tanque tanque, DateTime desde)
        {
            if (tanque == null) return new List<VolumenHistorico>();

            return Session.Query<VolumenHistorico>().Where(v => tanque.Id == v.Tanque.Id && v.EsTeorico && v.Fecha > desde);
        }

        public double ObtainLastTeoricVolumeByTanqueTipoAndFecha(int tanque, bool tipo, DateTime fecha)
        {
            var vol = Session.Query<VolumenHistorico>().Where(
                    mov => mov.Tanque.Id == tanque && mov.EsTeorico == tipo && mov.Fecha <= fecha).OrderByDescending(
                        mov => mov.Fecha).ThenByDescending(mov => mov.Id).FirstOrDefault();

            return vol != null ? vol.Volumen : -1;
        }

        public void UpdateVolumes(int tanque, DateTime fecha, double volumen)
        {
            var dao = new DAOFactory();
            var volumes = Session.Query<VolumenHistorico>().Where(
                mov => mov.Tanque.Id == tanque && mov.EsTeorico && mov.Fecha > fecha).ToList();

            foreach (var vol in volumes)
            {
                vol.Volumen = vol.Volumen + volumen;

                dao.VolumenHistoricoDAO.SaveOrUpdate(vol);
            }
        }

        public IEnumerable<VolumenHistorico> GetVolumes(Int32 tank, DateTime actualDate, DateTime endDate)
        {
            return Session.Query<VolumenHistorico>().Where(vol => vol.Tanque.Id == tank && vol.Fecha <= actualDate && vol.Fecha <= endDate).ToList();
        }

        #endregion

        #region DatamartCombustible Methods

        #region Private properties

        private readonly TimeSpan _timeDifferenceForInvalidConsumo = new TimeSpan(0, 5, 0);

        #endregion

        #region Public Methods

        /// <summary>
        /// Process all the movements for the givenn tank, Generating is teoric Volume.
        /// </summary>
        /// <param name="tanque"></param>
        public void GenerateTanksByPlantaTeoricVolumes(Tanque tanque)
        {
            try
            {
                var movementDao = new MovimientoDAO();
                var movimientos = movementDao.FindRemitosAjustesAndDespachosByTanque(tanque.Id);

                var firstTime = true;

                if (movimientos.Count.Equals(0)) return;
                using (var transaction = SmartTransaction.BeginTransaction())
                {
                    try
                    {
                        foreach (Movimiento mov in movimientos)
                        {

                            var lastMovement = GetLastTankTeoricMovement(tanque, mov.Fecha);
                            var volumenAnterior = lastMovement != null ? lastMovement.Volumen : 0;

                            /*if its the first tank teoric volume inserts the Most Nearest of the Real Volumes*/
                            if (firstTime &&
                                lastMovement != null &&
                                !lastMovement.EsTeorico)
                            {
                                var v = new VolumenHistorico {Fecha = mov.Fecha, Volumen = volumenAnterior, EsTeorico = true, Tanque = tanque};

                                SaveOrUpdate(v);
                            }

                            var movementVolume = mov.TipoMovimiento.Codigo.Equals("D") ? -1*mov.Volumen : mov.Volumen;

                            SaveOrUpdate(new VolumenHistorico {Fecha = mov.Fecha, Volumen = volumenAnterior + movementVolume, EsTeorico = true, Tanque = tanque});

                            UpdateTeoricVolumesAfterDate(tanque, mov.Fecha, movementVolume);

                            mov.Procesado = true;
                            movementDao.SaveOrUpdate(mov);
                            firstTime = false;

                        }
                        try
                        {
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            STrace.Exception(typeof (LineaDAO).FullName, ex, "Exception in GenerateTanksByPlantaTeoricVolumes(Tanque) -> transaction.Commit();");
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof (LineaDAO).FullName, ex, "Exception in GenerateTanksByPlantaTeoricVolumes(Tanque)");
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            STrace.Exception(typeof (LineaDAO).FullName, ex2,
                                "Exception in GenerateTanksByPlantaTeoricVolumes(Tanque) -> transaction.Rollback();");
                        }
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error procesing teoric volume for tank by Planta {0} ", tanque.Id), ex);
            }
        }

        /// <summary>
        /// Checks all invalid tank volumes and discards them if considered necesary.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="lastDate"></param>
        /// <param name="user"></param>
        public void CheckInvalidTankVolumes(Tanque tank, DateTime lastDate, Usuario user)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    CheckInvalidTankVolumesWithoutTransaction(tank, lastDate, user);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof (VolumenHistoricoDAO).FullName, ex,
                            "Exception in CheckInvalidTankVolumes(Tanque, DateTime, Usuario) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof (VolumenHistoricoDAO).FullName, ex, "Exception in CheckInvalidTankVolumes(Tanque, DateTime, Usuario)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof (VolumenHistoricoDAO).FullName, ex2,
                            "Exception in CheckInvalidTankVolumes(Tanque, DateTime, Usuario) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        private void CheckInvalidTankVolumesWithoutTransaction(Tanque tank, DateTime lastDate, Usuario user)
        {
            try
            {
                var invalidVolumesDAO = new VolumenHistoricoInvalidoDAO();

                var lastReporting = GetLastTankRealLevel(tank, lastDate);
                var lastReportVolume = lastReporting != null ? lastReporting.Volumen : 0;
                var lastReportDate = lastReporting != null ? lastReporting.Fecha : new DateTime(1900, 1, 1);

                var realVolumes = FindAllRealVolumes(tank.Id, lastDate, DateTime.MaxValue);

                foreach (var m in realVolumes)
                {
                    if (((m.Volumen - lastReportVolume) >= 5000 || (lastReportVolume - m.Volumen >= 4000)) &&
                        m.Fecha.Subtract(lastReportDate) <= _timeDifferenceForInvalidConsumo)
                    {
                        var mDesc = new VolumenHistoricoInvalido(m, 0);

                        invalidVolumesDAO.SaveOrUpdate(mDesc);
                        Delete(m);
                    }
                    else
                    {
                        lastReportDate = m.Fecha;
                        lastReportVolume = m.Volumen;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error checking Invalid Tank Volumes for tank by Equipo {0} ", tank.Id), ex);
            }

        }

        public void GenerateTeoricVolumes(Tanque tanque)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    GenerateTeoricVolumesWithoutTransaction(tanque);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex,
                            "Exception in GenerateTeoricVolumes(Tanque) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex, "Exception in GenerateTeoricVolumes(Tanque)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex2,
                            "Exception in GenerateTeoricVolumes(Tanque) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }            
        }

        /// <summary>
        /// Generates all the teoric volumes for the given tank using the unprocessed movements assigned to it.
        /// </summary>
        /// <param name="tanque"></param>
        public void GenerateTeoricVolumesWithoutTransaction(Tanque tanque)
        {
            {
                try
                {
                    var movementDAO = new MovimientoDAO();
                    var tanqueDAO = new TanqueDAO();

                    var list = movementDAO.FindAllSumarizedMovementsNotProcessed(tanque.Equipo);

                    if (!list.Any()) return;

                    var lastTeoricMovement = GetLastTankTeoricMovement(tanque, list.First().Fecha);
                    var lastTeoricVolume = lastTeoricMovement != null ? lastTeoricMovement.Volumen : 0;
                    var firstTime = true;

                    foreach (var m in list)
                    {
                        /*if its the first tank teoric volume inserts the Most Nearest of the Real Volumes*/
                        if (firstTime &&
                            lastTeoricMovement != null &&
                            !lastTeoricMovement.EsTeorico)
                        {
                            var v = new VolumenHistorico {Fecha = m.Fecha, EsTeorico = true, Tanque = tanque, Volumen = lastTeoricVolume, VolumenAgua = 0};

                            SaveOrUpdate(v);
                        }

                        lastTeoricVolume = m.TipoMovimiento.Codigo.Equals("M") ? lastTeoricVolume - m.Volumen : lastTeoricVolume + m.Volumen;

                        var volHistorico = new VolumenHistorico
                                           {
                                               Fecha = m.Fecha,
                                               EsTeorico = true,
                                               Tanque = tanque,
                                               Volumen = lastTeoricVolume,
                                               VolumenAgua = 0
                                           };

                        SaveOrUpdate(volHistorico);

                        m.Procesado = true;

                        movementDAO.SaveOrUpdate(m);

                        firstTime = false;
                    }

                    tanque.VolTeorico = lastTeoricVolume;

                    tanqueDAO.SaveOrUpdate(tanque);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Error generating teoric volumes for tank by Equipo {0} ", tanque.Id), ex);
                }
            }
        }

        public void ProcessConciliaciones(Tanque tanque)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    ProcessConciliacionesWithoutTransaction(tanque);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex, "Exception in ProcessConciliaciones(Tanque) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex, "Exception in ProcessConciliaciones(Tanque)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex2, "Exception in ProcessConciliaciones(Tanque) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Process All conciliaciones for the given tank.
        /// </summary>
        /// <param name="tanque"></param>
        public void ProcessConciliacionesWithoutTransaction(Tanque tanque)
        {
            try
            {
                var movementDAO = new MovimientoDAO();

                foreach (var m in movementDAO.FindNotProcessedConciliaciones(tanque))
                {
                    var lastTeoricMovement = GetLastTankTeoricMovement(tanque, m.Fecha);
                    var lastTeoricVolume = lastTeoricMovement != null ? lastTeoricMovement.Volumen : 0;

                    /*if its the first tank teoric volume inserts the Most Nearest of the Real Volumes*/
                    if (lastTeoricMovement != null &&
                        !lastTeoricMovement.EsTeorico)
                    {
                        var v = new VolumenHistorico {Fecha = m.Fecha, EsTeorico = true, Tanque = tanque, Volumen = lastTeoricVolume, VolumenAgua = 0};

                        SaveOrUpdate(v);
                    }

                    var volHistorico = new VolumenHistorico
                                       {
                                           Fecha = m.Fecha,
                                           EsTeorico = true,
                                           Tanque = tanque,
                                           Volumen =
                                               m.TipoMovimiento.Codigo.Equals("E")
                                                   ? lastTeoricVolume - m.Volumen
                                                   : lastTeoricVolume + m.Volumen,
                                           VolumenAgua = 0
                                       };

                    SaveOrUpdate(volHistorico);

                    UpdateTeoricVolumesAfterDate(tanque, m.Fecha, m.TipoMovimiento.Codigo.Equals("E") ? -1*m.Volumen : m.Volumen);

                    m.Procesado = true;
                    movementDAO.SaveOrUpdate(m);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error processing Conciliaciones for tank by Equipo {0} ", tanque.Id), ex);
            }
        }

        #endregion

        #region Private Methods

        private void UpdateTeoricVolumesAfterDate(Tanque tanque, DateTime date, double volumeToAdd)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    UpdateTeoricVolumesAfterDateWithoutTransaction(tanque, date, volumeToAdd);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex, "Exception in UpdateTeoricVolumesAfterDate(Tanque, DateTime, double) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex, "Exception in UpdateTeoricVolumesAfterDate(Tanque, DateTime, double)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(VolumenHistoricoDAO).FullName, ex2, "Exception in UpdateTeoricVolumesAfterDate(Tanque, DateTime, double) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        private void UpdateTeoricVolumesAfterDateWithoutTransaction(Tanque tanque, DateTime date, double volumeToAdd)
        {
            var volumes = FindAllTeoricVolumesAfterDate(tanque, date);
            foreach (var vol in volumes)
            {
                vol.Volumen = vol.Volumen + volumeToAdd;
                SaveOrUpdate(vol);
            }
        }

        #endregion

        #endregion
    }
}
