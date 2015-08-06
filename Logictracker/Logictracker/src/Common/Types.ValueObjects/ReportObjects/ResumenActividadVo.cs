using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ResumenActividadVo
    {
        public const int IndexVehiculo = 0;
        public const int IndexTiempoEncendido = 1;
        public const int IndexTiempoApagado = 2;
        public const int IndexSinInformacion = 3;
        
        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, InitialSortExpression = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexTiempoEncendido, ResourceName = "Labels", VariableName = "TIEMPO_ENCENDIDO", AllowGroup = false)]
        public string TiempoEncendido { get; set; }

        [GridMapping(Index = IndexTiempoApagado, ResourceName = "Labels", VariableName = "TIEMPO_APAGADO", AllowGroup = false)]
        public string TiempoApagado { get; set; }

        [GridMapping(Index = IndexSinInformacion, ResourceName = "Labels", VariableName = "SIN_INFORMACION", AllowGroup = false)]
        public string SinInformacion { get; set; }

        public ResumenActividadVo(string patente, TimeSpan encendido, TimeSpan apagado)
        {
            Vehiculo = patente;
            TiempoEncendido = string.Format("{0}.{1}:{2}:{3}", encendido.Days.ToString("00"), encendido.Hours.ToString("00"), encendido.Minutes.ToString("00"), encendido.Seconds.ToString("00"));
            TiempoApagado = string.Format("{0}.{1}:{2}:{3}", apagado.Days.ToString("00"), apagado.Hours.ToString("00"), apagado.Minutes.ToString("00"), apagado.Seconds.ToString("00"));
            SinInformacion = "00:00:00";
        }

        public ResumenActividadVo(string patente, TimeSpan sinInfo)
        {
            Vehiculo = patente;
            TiempoEncendido = "00:00:00";
            TiempoApagado = "00:00:00";
            SinInformacion = string.Format("{0}.{1}:{2}:{3}", sinInfo.Days.ToString("00"), sinInfo.Hours.ToString("00"), sinInfo.Minutes.ToString("00"), sinInfo.Seconds.ToString("00"));
        }
    }
}
