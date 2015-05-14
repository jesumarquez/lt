using System;
using System.Drawing;
using Compumap.Controls;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObject.Vehiculos
{
    [Serializable]
    public class MovOdometroVehiculoVO
    {
        public int IdMovOdometro { get; set; }
        public string Descripcion { get; set; }
        public string Kilometros { get; set; }
        public double AjusteKilometros { get; set; }
        public string Dias { get; set; }
        public int AjusteDias { get; set; }
        public string Horas { get; set; }
        public double AjusteHoras { get; set; }
        public DateTime UltimoUpdate { get; set; }
        public DateTime UltimoDisparo { get; set; }
        public bool EsReseteable { get; set; }
        public Color Color { get; set; }

        public MovOdometroVehiculoVO(MovOdometroVehiculo movOdometro)
        {
            IdMovOdometro = movOdometro.Id;

            Descripcion = movOdometro.Odometro.Descripcion;

            Kilometros = GetKilometrs(movOdometro);
            Dias = GetDays(movOdometro);
            Horas = GetHours(movOdometro);
            UltimoUpdate = GetLastUpdate(movOdometro);
            UltimoDisparo = GetLastRaise(movOdometro);
            AjusteKilometros = GetKilometrosAjuste(movOdometro);
            AjusteDias = GetDaysAjuste(movOdometro);
            AjusteHoras = GetHorasAjuste(movOdometro);

            EsReseteable = movOdometro.Odometro.EsReseteable;

            Color = GetBackColor(movOdometro);
        }

        private static Color GetBackColor(MovOdometroVehiculo movOdometro)
        {
            if (movOdometro.Odometro.PorKm && movOdometro.Odometro.PorTiempo) return GetBackColorUsingTimeAndKilometers(movOdometro);

            if (movOdometro.Odometro.PorKm) return GetBackColorUsingKilometers(movOdometro);

            return movOdometro.Odometro.PorTiempo ? GetBackColorUsingTime(movOdometro) : Color.Empty;
        }
        private static Color GetBackColorUsingTime(MovOdometroVehiculo movOdometro)
        {
            var referencia = movOdometro.Odometro.ReferenciaTiempo;
            var valor = movOdometro.Dias;

            if (valor > referencia && !movOdometro.Odometro.EsIterativo) return Color.Empty;

            if (valor > movOdometro.Odometro.Alarma2Tiempo) return String.Format("#{0}", movOdometro.Odometro.Alarma2Rgb).HexToColor();

            return valor > movOdometro.Odometro.Alarma1Tiempo ? String.Format("#{0}", movOdometro.Odometro.Alarma1RGB).HexToColor() : Color.Empty;
        }
        private static Color GetBackColorUsingKilometers(MovOdometroVehiculo movOdometro)
        {
            var referencia = movOdometro.Odometro.ReferenciaKm;
            var valor = movOdometro.Kilometros;

            if (valor > referencia && !movOdometro.Odometro.EsIterativo) return Color.Empty;

            if (valor > movOdometro.Odometro.Alarma2Km) return String.Format("#{0}", movOdometro.Odometro.Alarma2Rgb).HexToColor();

            return valor > movOdometro.Odometro.Alarma1Km ? String.Format("#{0}", movOdometro.Odometro.Alarma1RGB).HexToColor() : Color.Empty;
        }
        private static Color GetBackColorUsingTimeAndKilometers(MovOdometroVehiculo movOdometro)
        {
            var referenciaKilometros = movOdometro.Odometro.ReferenciaKm;
            var valorKilometros = movOdometro.Kilometros;

            var referenciaTiempo = movOdometro.Odometro.ReferenciaTiempo;
            var valorTiempo = movOdometro.Dias;

            if ((valorKilometros > referenciaKilometros || valorTiempo > referenciaTiempo) && !movOdometro.Odometro.EsIterativo) return Color.Empty;

            var alarma2Kilometros = movOdometro.Odometro.Alarma2Km;
            var alarma2Tiempo = movOdometro.Odometro.Alarma2Tiempo;

            if (valorKilometros > alarma2Kilometros || valorTiempo > alarma2Tiempo) return String.Format("#{0}", movOdometro.Odometro.Alarma2Rgb).HexToColor();

            var alarma1Kilometros = movOdometro.Odometro.Alarma1Km;
            var alarma1Tiempo = movOdometro.Odometro.Alarma1Tiempo;

            if (valorKilometros > alarma1Kilometros || valorTiempo > alarma1Tiempo) return String.Format("#{0}", movOdometro.Odometro.Alarma1RGB).HexToColor();

            return Color.Empty;
        }

        private static DateTime GetLastRaise(MovOdometroVehiculo movOdometro)
        {
            return !movOdometro.UltimoDisparo.HasValue ? DateTime.MinValue : movOdometro.UltimoDisparo.Value;
        }
        private static DateTime GetLastUpdate(MovOdometroVehiculo movOdometro)
        {
            return !movOdometro.UltimoUpdate.HasValue ? DateTime.MinValue : movOdometro.UltimoUpdate.Value;
        }

        private static string GetKilometrs(MovOdometroVehiculo movOdometro)
        {
            if (!movOdometro.Odometro.PorKm)
                return "-";

            var diferencia = movOdometro.Odometro.ReferenciaKm + movOdometro.AjusteKilometros - movOdometro.Kilometros;

            if (diferencia < 0 && !movOdometro.Odometro.EsIterativo)
                return "0";

            return diferencia.ToString("0.00");
        }
        private static string GetDays(MovOdometroVehiculo movOdometro)
        {
            if (!movOdometro.Odometro.PorTiempo) return "-";

            var diferencia = movOdometro.Odometro.ReferenciaTiempo + movOdometro.AjusteDias - movOdometro.Dias;

            if (diferencia < 0 && !movOdometro.Odometro.EsIterativo) return "0";

            return diferencia.ToString("#0");
        }
        private static string GetHours(MovOdometroVehiculo movOdometro)
        {
            if (!movOdometro.Odometro.PorHoras) return "-";

            var diferencia = movOdometro.Odometro.ReferenciaHoras + movOdometro.AjusteHoras - movOdometro.Horas;

            if (diferencia < 0 && !movOdometro.Odometro.EsIterativo) return "0.00";

            return diferencia.ToString("0.00");
        }

        private static double GetKilometrosAjuste(MovOdometroVehiculo movOdometro)
        {
            return movOdometro.Odometro.PorKm ? movOdometro.AjusteKilometros : 0;
        }
        private static int GetDaysAjuste(MovOdometroVehiculo movOdometro)
        {
            return movOdometro.Odometro.PorTiempo ? movOdometro.AjusteDias : 0;
        }
        private static double GetHorasAjuste(MovOdometroVehiculo movOdometro)
        {
            return movOdometro.Odometro.PorHoras ? movOdometro.AjusteHoras : 0.0;
        }
    }
}