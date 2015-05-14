#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Vehiculos
{
    /// <summary>
    /// Shift data access class.
    /// </summary>
    public class ShiftDAO : GenericDAO<Shift>
    {
        #region Constructor

    	/// <summary>
    	/// Instanciates a new data access class using the provided nhibernate sessions.
    	/// </summary>
    	/// <param name="session"></param>
//    	public ShiftDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        public Shift FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .Where(d => d.Codigo == code)
                        .Where(d => !d.Baja)
                        .Cacheable()
						.SafeFirstOrDefault();
        }

        /// <summary>
        /// Find active shifts assigned to the specified locations.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public List<Shift> FindActive(Empresa empresa, Linea linea, Usuario usuario)
        {
            if (usuario != null && usuario.AppliesFilters && empresa == null && linea == null) return FindByUsuario(usuario);

            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;
            var lin = linea != null ? linea.Id : -1;

            return FindActive(new List<int> { emp }, new List<int> { lin });
        }

        /// <summary>
        /// Deletes the shift associated to the specified id.
        /// </summary>
        /// <param name="shift"></param>
        public override void Delete(Shift shift)
        {
            if (shift == null) return;

            shift.Baja = true;

            SaveOrUpdate(shift);
        }

        /// <summary>
        /// Gets all assigned shifts for the current vehicle.
        /// </summary>
        /// <param name="vehiculo"></param>
        /// <returns></returns>
        public List<Shift> GetVehicleShifts(Coche vehiculo)
        {
            //return Session.Query<MovShiftsAssignments>()
            //    .Where(assigment => !assigment.Shift.Baja && assigment.VehicleType == vehiculo.TipoCoche && assigment.CostCenter == vehiculo.CentroDeCostos)
            //    .Select(s => s.Shift)
            //    .Distinct()
            //    .ToList();

            return vehiculo.Turnos.OfType<Shift>().Where(t => !t.Baja).ToList();
        }

        /// <summary>
        /// Determines if the givenn position is within any shift.
        /// </summary>
        /// <param name="vehiculo"></param>
        /// <param name="positionDate"></param>
        /// <param name="gmtModifier"></param>
        /// <returns></returns>
        public bool IsPositionWithinShift(Coche vehiculo, DateTime positionDate, double gmtModifier)
        {
            var date = positionDate.AddHours(gmtModifier);

            var shifts = GetVehicleShifts(vehiculo);

            if (shifts == null || shifts.Count.Equals(0)) return true;

            return shifts.Any(shift => AppliesToDate(vehiculo, date, shift) && shift.Inicio <= date.TimeOfDay.TotalHours && shift.Fin >= date.TimeOfDay.TotalHours);
        }

        /// <summary>
        /// Detemrines if the specified vehicle has any assigned shifts.
        /// </summary>
        /// <param name="coche"></param>
        /// <returns></returns>
        public bool HasShifts(Coche coche)
        { 
            return Session.Query<MovShiftsAssignments>().Any(assigment => assigment.VehicleType == coche.TipoCoche && assigment.CostCenter == coche.CentroDeCostos);
        }

        /// <summary>
        /// Gets the last shift for the specified position.
        /// </summary>
        /// <param name="vehiculo"></param>
        /// <param name="posicionDate"></param>
        /// <param name="gmtModifier"></param>
        /// <returns></returns>
        public Shift GetLastShift(Coche vehiculo, DateTime posicionDate, double gmtModifier)
        {
            var shifts = GetVehicleShifts(vehiculo);

            if (shifts == null || shifts.Count.Equals(0)) return null;

            var date = posicionDate.AddHours(gmtModifier);

            return shifts.Where(shift => AppliesToDate(vehiculo, date, shift) && shift.Fin <= date.TimeOfDay.TotalHours).OrderByDescending(shift => shift.Fin)
                .FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehiculo"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="gmtModifier"></param>
        /// <returns></returns>
        public TimeSpan TimeByVehicleAndDate(Coche vehiculo, DateTime desde, DateTime hasta, double gmtModifier)
        {
            var shifts = GetVehicleShifts(vehiculo);

            var hours = 0.0;

            hasta = hasta.AddHours(gmtModifier);

            var start = desde.AddHours(gmtModifier);

            while (start < hasta)
            {
                hours += shifts.Where(shift => AppliesToDate(vehiculo, start, shift)).Sum(shift => (shift.Fin - shift.Inicio));

                start = start.AddDays(1);
            }

            return TimeSpan.FromHours(hours);
        }

        /// <summary>
        /// Checks if the selected combination overlapps with an existing Shift for vehicle types, Base, District and CostCenter.
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="vehicleTypes"></param>
        /// <param name="costCenters"></param>
        /// <returns></returns>
        public bool ShiftsOverlapped(Shift shift, List<int> vehicleTypes, List<int> costCenters)
        {
            var idEmpresa = shift.Empresa != null ? shift.Empresa.Id : 0;
            var idLinea = shift.Linea != null ? shift.Linea.Id : 0;

            var assignedShifts = Session.Query<MovShiftsAssignments>()
                .Where(s => shift.Id != s.Shift.Id && !s.Shift.Baja
                    && costCenters.Contains(s.CostCenter.Id) && vehicleTypes.Contains(s.VehicleType.Id)
                    && (idEmpresa == 0 || s.Shift.Empresa == null || idEmpresa == s.Shift.Empresa.Id)
                    && (idLinea == 0 || s.Shift.Linea == null || idLinea == s.Shift.Linea.Id)
                    ).ToList();

            return assignedShifts.Select(o => o.Shift)
                .Any(tAnterior => ((tAnterior.Lunes && shift.Lunes) || (tAnterior.Martes && shift.Martes) || (tAnterior.Miercoles && shift.Miercoles)
                    || (tAnterior.Jueves && shift.Jueves) || (tAnterior.Viernes && shift.Viernes) || (tAnterior.Sabado && shift.Sabado) || (tAnterior.Domingo && shift.Domingo))
                 && (!((shift.Inicio <= tAnterior.Inicio && shift.Fin <= tAnterior.Inicio) || (shift.Inicio >= tAnterior.Fin && shift.Fin >= tAnterior.Fin))));
        }

        /// <summary>
        /// Determines if the givenn shift is a valid one.
        /// </summary>
        /// <param name="vehiculo"></param>
        /// <param name="date"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public bool AppliesToDate(Coche vehiculo, DateTime date, Shift shift)
        {
            var feriadoDao = new FeriadoDAO();

            return shift.AppliesToDate(date) && (!feriadoDao.EsFeriado(vehiculo.Empresa, vehiculo.Linea, date) || shift.AplicaFeriados);
        }

        public List<Shift> GetList(List<int> empresas, List<int> lineas)
        {
            return FindActive(empresas, lineas);
        }

        #endregion

        #region Private Methods

        /*/// <summary>
        /// Arrenge days for the new shift.
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="newShift"></param>
        private static void AdjustDays(Shift shift, Shift newShift)
        {
            newShift.Lunes = false;
            newShift.Martes = false;
            newShift.Miercoles = false;
            newShift.Jueves = false;
            newShift.Viernes = false;
            newShift.Sabado = false;
            newShift.Domingo = false;

            if (shift.Lunes) newShift.Martes = true;
            if (shift.Martes) newShift.Miercoles = true;
            if (shift.Miercoles) newShift.Jueves = true;
            if (shift.Jueves) newShift.Viernes = true;
            if (shift.Viernes) newShift.Sabado = true;
            if (shift.Sabado) newShift.Domingo = true;
            if (shift.Domingo) newShift.Lunes = true;
        }//*/

        /// <summary>
        /// Fins all shifts assigned to the user visible locations.
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private List<Shift> FindByUsuario(Usuario usuario)
        {
            var emp = usuario.PorEmpresa ? usuario.Empresas.OfType<Empresa>().Select(e => e.Id).ToList() : new List<int> {-1};
            var lin = usuario.PorLinea ? usuario.Lineas.OfType<Linea>().Select(l => l.Id).ToList() : new List<int> { -1 };

            return FindActive(emp, lin);
        }

        /// <summary>
        /// Find active shifts assigned to the specified locations ids.
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="lin"></param>
        /// <returns></returns>
        private List<Shift> FindActive(List<int> emp, List<int> lin)
        {
            var sql = "from Shift s where s.Baja = 0";

            if (emp.Count > 0 && !emp.Contains(-1)) sql = string.Concat(sql, " and (s.Empresa is null or s.Empresa.Id in (:emp))");
            if (lin.Count > 0 && !lin.Contains(-1)) sql = string.Concat(sql, " and (s.Linea is null or s.Linea.Id in (:lin))");

            var query = Session.CreateQuery(sql);

            if (emp.Count > 0 && !emp.Contains(-1)) query.SetParameterList("emp", emp);
            if (lin.Count > 0 && !lin.Contains(-1)) query.SetParameterList("lin", lin);

            return query.SetCacheable(true).List<Shift>().ToList();
        }

        #endregion
    }
}
