#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    /// <summary>
    /// Class that represents a shift that could be asigned to a vehicle.
    /// </summary>
    [Serializable]
    public class Shift : IAuditable, ISecurable
    {
        #region Private properties

        private ISet<Coche> _asignaciones;

        #endregion

        #region Constructors

        public Shift() {}

        public Shift(Shift shift)
        {
            Id = shift.Id;
            Codigo = string.Format("{0} - Duplicado", shift.Codigo);
            Descripcion = shift.Descripcion;
            Baja = shift.Baja;
            Empresa = shift.Empresa;
            Linea = shift.Linea;
            Inicio = shift.Inicio;
            Fin = shift.Fin;
            Lunes = shift.Lunes;
            Martes = shift.Martes;
            Miercoles = shift.Miercoles;
            Jueves = shift.Jueves;
            Viernes = shift.Viernes;
            Sabado = shift.Sabado;
            Domingo = shift.Domingo;
            AplicaFeriados = shift.AplicaFeriados;
        }

        #endregion

        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }

        public virtual double Inicio { get; set; }
        public virtual double Fin { get; set; }

        public virtual bool Lunes { get; set; }
        public virtual bool Martes { get; set; }
        public virtual bool Miercoles { get; set; }
        public virtual bool Jueves { get; set; }
        public virtual bool Viernes { get; set; }
        public virtual bool Sabado { get; set; }
        public virtual bool Domingo { get; set; }
        public virtual bool AplicaFeriados { get; set; }

        /// <summary>
        /// Gets assigned Centers and vehicle types to the current shift.
        /// </summary>
        public virtual ISet<Coche> Asignaciones { get { return _asignaciones ?? (_asignaciones = new HashSet<Coche>()); } }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as Shift;

            if (castObj == null) return false;

            var empresa = Empresa != null ? Empresa.Id : -1;
            var castEmpresa = castObj.Empresa != null ? castObj.Empresa.Id : -1;

            var linea = Linea != null ? Linea.Id : -1;
            var castLinea = castObj.Linea != null ? castObj.Linea.Id : -1;

            return Id != 0 && empresa.Equals(castEmpresa) && linea.Equals(castLinea) && Codigo.Equals(castObj.Codigo);
        }

        public override int GetHashCode()
        {
            var empresa = Empresa != null ? Empresa.Id : -1;
            var linea = Linea != null ? Linea.Id : -1;

            return empresa.GetHashCode() * linea.GetHashCode() * Codigo.GetHashCode();
        }

        /// <summary>
        /// Determines if the shift applies to the given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual bool AppliesToDate(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: return Lunes;
                case DayOfWeek.Tuesday: return Martes;
                case DayOfWeek.Wednesday: return Miercoles;
                case DayOfWeek.Thursday: return Jueves;
                case DayOfWeek.Friday: return Viernes;
                case DayOfWeek.Saturday: return Sabado;
                case DayOfWeek.Sunday: return Domingo;
            }

            return false;
        }
        public virtual bool AppliesToDate(DateTime date, IEnumerable<int> feriados)
        {
            var aplica = AppliesToDate(date);
            if (aplica && feriados != null && feriados.Any())
            {
                aplica = (!feriados.Contains(date.DayOfYear) || AplicaFeriados);
            }
            return aplica;
        }
        public virtual bool AppliesToDateTime(DateTime date, IEnumerable<int> feriados)
        {
            var aplica = AppliesToDate(date) && (Inicio <= date.TimeOfDay.TotalHours && Fin >= date.TimeOfDay.TotalHours);
            if (aplica && feriados != null && feriados.Any())
            {
                aplica = (!feriados.Contains(date.DayOfYear) || AplicaFeriados);
            }
            return aplica;
        }

        /// <summary>
        /// Gets the last shift end datetime previos to the givenn refference.
        /// </summary>
        /// <param name="reffrence"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual DateTime GetLastShiftEnd(DateTime reffrence, TimeZoneInfo culture)
        {
            var offset = culture != null ? -culture.BaseUtcOffset.TotalHours : 0;

            var date = reffrence;

            if (AppliesToDate(date))
            {
                var end = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0).Add(TimeSpan.FromHours(Fin)).AddHours(offset);

                if (end <= reffrence) return end;
            }

            while (true)
            {
                date = date.AddDays(-1);

                if (AppliesToDate(date)) return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0).Add(TimeSpan.FromHours(Fin)).AddHours(offset);
            }
        }

        #endregion
    }
}
