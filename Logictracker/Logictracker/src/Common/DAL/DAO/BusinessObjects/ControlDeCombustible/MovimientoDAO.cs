#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.ReportObjects.ControlDeCombustible.Exportador;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible
{
    public class MovimientoDAO : GenericDAO<Movimiento>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public MovimientoDAO(ISession session) : base(session) { }

        #endregion

        #region Despachos

        public IEnumerable<Movimiento> FindDespachosByCentroAndDate(List<int> mobiles,String centro, DateTime desde, DateTime hasta)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where mobiles.Contains(m.Coche.Id)
                          && m.Tanque.Linea.Descripcion.Equals(centro)
                          && m.Fecha >= desde && m.Fecha <= hasta
                          && m.TipoMovimiento.Codigo.Equals("D")
                    select m).ToList();
        }

        public IEnumerable<Movimiento> FindConsumosByMotorAndDate(int caudalimetro, DateTime desde, DateTime hasta)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where m.Caudalimetro.Id == caudalimetro
                      && m.Estado == 1
                      && m.Fecha >= desde && m.Fecha <= hasta
                      && m.TipoMovimiento.Codigo.Equals("M")
                    select m).ToList();
        }

        public Double FindTotalMovement(Int32 tanque, DateTime date, DateTime dateFin)
        {
            return Session.Query<Movimiento>().Where(mov => mov.TipoMovimiento.Codigo == "M" && mov.Caudalimetro.Tanque != null && mov.Caudalimetro.Tanque.Id == tanque && mov.Estado == 1
                && !mov.Caudalimetro.EsDeEntrada && mov.Fecha >= date && mov.Fecha < dateFin).Sum(mov => mov.Volumen);
        }

        public Double FindTotalMovementWithinInterval(Int32 tanque, DateTime date, DateTime hasta, TimeSpan interval)
        {
            return Session.Query<Movimiento>().Where(m => m.Estado == 1 && m.Caudalimetro != null && m.Caudalimetro.Tanque != null && m.Caudalimetro.Tanque.Id == tanque
                && (m.TipoMovimiento.Codigo == "M") && !m.Caudalimetro.EsDeEntrada && m.Fecha > date && m.Fecha <= date.Add(interval) && m.Fecha <= hasta && m.Fecha <= DateTime.Now)
                .Sum(m => m.Volumen);
        }

        public Double FindTotalIncome(Int32 caudalimetro, DateTime date, DateTime dateFin)
        {
            return Session.Query<Movimiento>().Where(mov => mov.TipoMovimiento.Codigo == "I" && mov.Caudalimetro.Id == caudalimetro && mov.Estado == 1 && mov.Fecha >= date
                && mov.Fecha < dateFin).Sum(movi => movi.Volumen);
        }

        public Double FindTotalIncomeWithinInterval(Int32 tanque, DateTime date, DateTime hasta, TimeSpan interval)
        {
            return Session.Query<Movimiento>().Where(m => m.Estado == 1 && m.Caudalimetro != null && m.Caudalimetro.Tanque != null && m.Caudalimetro.Tanque.Id == tanque
                && (m.TipoMovimiento.Codigo == "I") && m.Caudalimetro.EsDeEntrada && m.Fecha > date && m.Fecha <= date.Add(interval) && m.Fecha <= hasta && m.Fecha <= DateTime.Now)
                .Sum(m => m.Volumen);
        }

        public Double FindTotalIncomeByTank(Int32 tanque, DateTime date, DateTime dateFin)
        {
            return Session.Query<Movimiento>().Where(mov => mov.TipoMovimiento.Codigo == "I" && mov.Caudalimetro.Tanque != null && mov.Caudalimetro.Tanque.Id == tanque 
                && mov.Estado == 1 && mov.Fecha >= date && mov.Caudalimetro.EsDeEntrada && mov.Fecha < dateFin).Sum(movi => movi.Volumen);
        }

        public Double FindTotalConciliationIncome(Int32 tanque, DateTime desde, DateTime hasta)
        {
            return Session.Query<Movimiento>().Where(mov => mov.TipoMovimiento.Codigo == "C" && mov.Tanque.Id == tanque && mov.Fecha >= desde && mov.Fecha <= hasta).Sum(m => m.Volumen);
        }

        public Double FindTotalConciliationOutcome(Int32 tanque, DateTime desde, DateTime hasta)
        {
            return Session.Query<Movimiento>().Where(mov => mov.TipoMovimiento.Codigo == "E" && mov.Tanque.Id == tanque && mov.Fecha >= desde && mov.Fecha <= hasta).Sum(m => m.Volumen);
        }

        public IEnumerable<Movimiento> FindDespachosBetweenDatesAndMobiles(List<int> mobiles, DateTime desde, DateTime hasta)
        {
            return Session.Query<Movimiento>()
                .Where(m =>
                        m.Fecha >= desde && m.Fecha <= hasta &&
                       m.TipoMovimiento.Codigo == "D" &&
                       mobiles.Contains(m.Coche.Id)
                       ).ToList();
        }

        public IEnumerable<Movimiento> FindDespachosBetweenDatesAndMobilesForABase(List<Int32> mobiles, Int32 planta, DateTime startDate, DateTime endDate)
        {
            return Session.Query<Movimiento>().Where(mov => mov.TipoMovimiento.Codigo == "D" && mov.Coche != null && mobiles.Contains(mov.Coche.Id) && mov.Tanque != null
                && mov.Tanque.Linea != null && (planta == -1 || mov.Tanque.Linea.Id == planta) && mov.Fecha >= startDate && mov.Fecha <= endDate).Select(mov => mov).ToList();
        }

        public IEnumerable<Movimiento> FindConsumosBetweenDatesAndMotores(List<int> motores, DateTime desde, DateTime hasta)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where motores.Contains(m.Caudalimetro.Id)
                      && m.Estado == 1
                      && m.TipoMovimiento.Codigo.Equals("M")
                      && m.Fecha >= desde && m.Fecha <= hasta
                    orderby m.Fecha descending select m).ToList();
        }

        public IEnumerable<Movimiento> FindConsumosWhitinInterval(List<Int32> motores, DateTime desde, DateTime hasta, Double intervalo)
        {
            return Session.Query<Movimiento>().Where(mov => mov.Caudalimetro != null && motores.Contains(mov.Caudalimetro.Id) && mov.TipoMovimiento.Codigo == "M" && mov.Estado == 1
                && mov.Fecha > desde && mov.Fecha <= desde.AddMinutes(intervalo) && mov.Fecha <= hasta).ToList();
        }

        #endregion

        #region Remitos

        public IEnumerable<Movimiento> FindRemitosByCentroAndDate(String centro, DateTime desde, DateTime hasta)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where (m.TipoMovimiento.Codigo.Equals("R") ||m.TipoMovimiento.Codigo.Equals("A"))
                        && m.Tanque.Linea.Descripcion.Equals(centro) 
                    && m.Fecha >= desde && m.Fecha <= hasta
                    select m).ToList();
        }

        public IEnumerable<Movimiento> FindRemitosByDate(DateTime desde, DateTime hasta)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where (m.TipoMovimiento.Codigo.Equals("R") || m.TipoMovimiento.Codigo.Equals("A"))
                        && m.Fecha >= desde && m.Fecha <= hasta
                    select m).ToList();
        }

        #endregion

        #region Ingresos

        /// <summary>
        /// Finds all the pre-calculated ingresos in a selectad range of dates for a given Equipo.
        /// </summary>
        /// <param name="idEquipo"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<Movimiento> FindIngresosByEquipoAndDate(int idEquipo, DateTime desde, DateTime hasta)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where m.TipoMovimiento.Codigo.Equals("I")
                        && m.Estado == 1
                        && m.Caudalimetro.Equipo.Id == idEquipo
                        && m.Fecha >= desde && m.Fecha <= hasta
                    select m).ToList();
        }

        public Movimiento FindUltimaMedicion(int idEquipo, DateTime desde, DateTime hasta)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where m.TipoMovimiento.Codigo.Equals("I")
                        && m.Caudalimetro.Equipo.Id == idEquipo
                        && m.Fecha >= desde && m.Fecha <= hasta
                    orderby m.Fecha descending 
                    select m).SafeFirstOrDefault();
        }

        public IEnumerable<Movimiento> FindConciliacionesByTanqueAndFecha(int tanque, DateTime desde, DateTime hasta)
        {
            var list = (from Movimiento m in Session.Query<Movimiento>().ToList()
                where m.Tanque.Id == tanque 
                &&(m.TipoMovimiento.Codigo.Equals("C") || m.TipoMovimiento.Codigo.Equals("E"))
                && m.Fecha >= desde && m.Fecha <= hasta
                select m).ToList();

            foreach (var c in list){if (c.TipoMovimiento.Codigo.Equals("E")) c.Volumen = -1*c.Volumen;}

            return (from Movimiento m in list select m).ToList();
        }

        /// <summary>
        /// Finds all the pre-calculated ingresos in a selectad range of dates.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<Movimiento> FindIngresosByDate(DateTime desde, DateTime hasta)
        {

            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                        where m.TipoMovimiento.Codigo.Equals("I")
                        && m.Fecha >= desde && m.Fecha <= hasta
                        orderby m.Fecha descending select m).Distinct().ToList();
        }

        #endregion

        #region Conciliaciones

        public IEnumerable<Movimiento> FindNotProcessedConciliaciones(Tanque tanque)
        {
            return (from Movimiento m in Session.Query<Movimiento>().ToList()
                    where m.Tanque != null && m.Tanque.Id == tanque.Id && !m.Procesado &&
                         (m.TipoMovimiento.Codigo.Equals("C") || m.TipoMovimiento.Codigo.Equals("E"))
                    select m);
        }

        public IEnumerable<Movimiento> FindConciliaciones(Int32 tanque, DateTime date, DateTime dateFin)
        {
            return Session.Query<Movimiento>().Where(mov => mov.Tanque != null && mov.Tanque.Id == tanque && (mov.TipoMovimiento.Codigo == "C" || mov.TipoMovimiento.Codigo == "E")
                && mov.Fecha >= date && mov.Fecha < dateFin).ToList();
        }

        #endregion

        #region Exportador de Despachos

        public List<DispatchsViewVO> FindNonExportedDispatchsFromLinea(int linea)
        {
            return Session.Query<Movimiento>().Where(m => m.Tanque != null && m.Tanque.Linea != null && m.Tanque.Linea.Id == linea
                                        && !m.Procesado 
                                        && m.TipoMovimiento.Codigo == "D"
                                        && m.Coche != null).Select(m => new DispatchsViewVO(m))
                                    .OrderBy(m => m.Fecha).ThenBy( m => m.Interno).ToList();
        }

        public List<DispatchsViewVO> FindExportedDispatchsFromLinea(int linea, DateTime desde, DateTime hasta)
        {
            return Session.Query<Movimiento>().Where(m => m.Tanque != null && m.Tanque.Linea != null && m.Tanque.Linea.Id == linea
                                        && m.TipoMovimiento.Codigo == "D"                        
                                        && m.Procesado && m.Fecha >= desde && m.Fecha <= hasta 
                                        && m.Coche != null).Select(m => new DispatchsViewVO(m))
                                    .OrderBy(m => m.Fecha).ThenBy(m => m.Interno).ToList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the las movement associated to the specified engine.
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        public Movimiento GetLastEngineMovement(Caudalimetro engine)
        {
            if (engine == null) return null;

            return Session.Query<Movimiento>().Where(m => m.Caudalimetro.Id == engine.Id && m.Estado == 0)
                .OrderByDescending(m => m.Fecha).FirstOrDefault();
        }

        /// <summary>
        /// Returns the last DataMart Processed Movement for the Caudalimetro.
        /// </summary>
        /// <param name="idCaudalimetro"></param>
        /// <returns></returns>
        public Movimiento FindLastReportedProcessedMovement(int idCaudalimetro)
        {
            return Session.Query<Movimiento>().Where(m => m.Caudalimetro != null && m.Caudalimetro.Id == idCaudalimetro && m.Procesado && m.Estado == 0).OrderByDescending(m => m.Fecha)
                .ThenByDescending(m => m.Id).FirstOrDefault();
        }

        /// <summary>
        /// Returns all the movements not processed for a specific Caudalimetro before the passed date.
        /// </summary>
        /// <param name="idCaudalimetro"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public IEnumerable<Movimiento> FindAllReportedMovementsNotProcessed(int idCaudalimetro, DateTime fecha)
        {
            return Session.Query<Movimiento>()
                .Where(m => m.Caudalimetro != null && m.Caudalimetro.Id == idCaudalimetro
                            && !m.Procesado && m.Estado == 0 && m.Fecha >= fecha)
                .OrderBy(m => m.Fecha);
        }

        /// <summary>
        /// Finds all the movements conciliados not processed for the selected Equipo.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public IEnumerable<Movimiento> FindAllSumarizedMovementsNotProcessed(Equipo e)
        {
            return Session.Query<Movimiento>().Where(m => m.Estado == 1 && !m.Procesado 
                    && (m.TipoMovimiento.Codigo == "M" || m.TipoMovimiento.Codigo == "I")
                    && m.Caudalimetro != null && m.Caudalimetro.Equipo != null && m.Caudalimetro.Equipo.Id == e.Id)
                .OrderBy(m => m.Fecha).ThenBy(m => m.Id);
        }

        public IList FindRemitosAjustesAndDespachosByTanque(int tanque)
        {
            return Session.Query<Movimiento>().Where(
                mov => mov.Tanque != null && mov.Tanque.Id == tanque && !mov.Procesado &&
                       (mov.TipoMovimiento.Codigo == "R" || mov.TipoMovimiento.Codigo == "D" ||
                        mov.TipoMovimiento.Codigo == "A")).OrderBy(mov => mov.Fecha).ToList();
        }

        public IEnumerable<Movimiento> FindRemitosAjustesByTanque(Int32 distrito, Int32 planta, DateTime startDate, DateTime endDate)
        {
            return Session.Query<Movimiento>().Where(movments => movments.Tanque != null && movments.Tanque.Linea != null 
                && (movments.TipoMovimiento.Codigo == "R" || movments.TipoMovimiento.Codigo == "A") && (planta == -1 || movments.Tanque.Linea.Id == planta)
                && (distrito == -1 || movments.Tanque.Linea.Empresa.Id == distrito) && movments.Fecha >= startDate && movments.Fecha <= endDate).Select(movements => movements).ToList();
        }

        #endregion

        #region Datamart Combustible Methods

        /// <summary>
        /// Process all the remaining movements for an specific Caudalimetro.
        /// </summary>
        /// <param name="caudalimetro"></param>
        public void ProcessMovements(Caudalimetro caudalimetro)
        {
            var typeDAO = new TipoMovimientoDAO();

            var idIngreso = typeDAO.GetByCode("I").Id;
            var lastProcessedMovement = FindLastReportedProcessedMovement(caudalimetro.Id);

            var lastVolume = lastProcessedMovement != null ? lastProcessedMovement.Volumen : 0;
            var lastDate = lastProcessedMovement != null ? lastProcessedMovement.Fecha : new DateTime(1900, 1, 1);

            try
            {

                using (var tx = SmartTransaction.BeginTransaction())
                {
                    try
                    {
                        foreach (var m in FindAllReportedMovementsNotProcessed(caudalimetro.Id, lastDate))
                        {
                            if ((m.TipoMovimiento.Id != idIngreso && m.Volumen >= lastVolume) ||
                                (m.TipoMovimiento.Id == idIngreso && m.Volumen > lastVolume))
                            {
                                var newMov = new Movimiento
                                             {
                                                 Fecha = m.Fecha,
                                                 FechaIngresoABase = DateTime.Now,
                                                 Volumen = m.Volumen - lastVolume,
                                                 Observacion = null,
                                                 Estado = 1,
                                                 Tanque = null,
                                                 Caudalimetro = caudalimetro,
                                                 TipoMovimiento = m.TipoMovimiento,
                                                 Coche = null,
                                                 Caudal = m.Caudal,
                                                 Temperatura = m.Temperatura,
                                                 HsEnMarcha = m.HsEnMarcha,
                                                 RPM = m.RPM,
                                                 Procesado = false,
                                                 Motivo = null,
                                                 Empleado = null
                                             };
                                SaveOrUpdate(newMov);
                            }
                            m.Procesado = true;
                            SaveOrUpdate(m);
                            lastVolume = m.Volumen;
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(MovimientoDAO).FullName, ex, "ProcessMovements(Caudalimetro);");
                        try
                        {
                            tx.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            STrace.Exception(typeof(MovimientoDAO).FullName, ex2, "ProcessMovements(Caudalimetro); doing rollback");
                        }
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Session.Transaction != null) Session.Transaction.Rollback();

                throw new Exception(String.Format("Error procesing remaining movements from caudalimetro: {0} ", caudalimetro.Id), ex);
            }
        }

        public IEnumerable<Movimiento> FindByTanque(Int32 tank, DateTime startDate, DateTime endDate)
        {
            return Session.Query<Movimiento>().Where(m => m.Tanque != null && m.Tanque.Id == tank && m.Fecha >= startDate && m.Fecha <= endDate).ToList();
        }

        #endregion
    }
}
