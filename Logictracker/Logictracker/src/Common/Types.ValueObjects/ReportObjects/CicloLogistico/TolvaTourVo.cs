using System;
using Logictracker.DAL.DAO.BusinessObjects.Messages;

namespace Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico
{
    [Serializable]
    public class TolvaTourVo
    {
        public static class Index
        {
            public const int Interno = 0;
            public const int Entrada = 1;
            public const int EntradaHora = 2;
            public const int Salida = 3;
            public const int SalidaHora = 4;
            public const int Duracion = 5;
            public const int Ubicacion = 6;
        }

        public static class KeyIndex
        {
            public const int Vehiculo = 0;
        }

        [GridMapping(Index = Index.Interno, ResourceName = "Entities", VariableName = "PARENTI03", IsInitialGroup = true)]
        public string Interno { get; set; }

        [GridMapping(Index = Index.Entrada, ResourceName = "Labels", VariableName = "INICIO", DataFormatString = "{0:d}", AllowGroup = false)]
        public DateTime Entrada { get; set; }

        [GridMapping(Index = Index.EntradaHora, ResourceName = "Labels", VariableName = "HORA", DataFormatString = "{0:T}", AllowGroup = false)]
        public DateTime EntradaHora { get; set; }

        [GridMapping(Index = Index.Salida, ResourceName = "Labels", VariableName = "FIN", DataFormatString = "{0:d}", AllowGroup = false)]
        public DateTime Salida { get; set; }

        [GridMapping(Index = Index.SalidaHora, ResourceName = "Labels", VariableName = "HORA", DataFormatString = "{0:T}", AllowGroup = false)]
        public DateTime SalidaHora { get; set; }

        [GridMapping(Index = Index.Duracion, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public string Duracion { get; set; }

        [GridMapping(Index = Index.Ubicacion, IsTemplate = true, ResourceName = "Labels", VariableName = "UBICACION", AllowGroup = false)]
        public string Ubicacion { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdVehiculo { get; set; }

        public double LatitudInicio { get; set; }
        public double LongitudInicio { get; set; }
        public double LatitudFin { get; set; }
        public double LongitudFin { get; set; }

        public TolvaTourVo(LogMensajeDAO.MobileTour mobileTour)
        {
            Interno = mobileTour.Interno;
            Entrada = mobileTour.Entrada;
            EntradaHora = mobileTour.Entrada;
            Salida = mobileTour.Salida;
            SalidaHora = mobileTour.Salida;
            Duracion = mobileTour.Duracion.Equals(TimeSpan.Zero)
                                           ? string.Empty
                                           : mobileTour.Duracion.ToString(); 
            IdVehiculo = mobileTour.IdMovil;

            LatitudInicio = mobileTour.LatitudInicio;
            LongitudInicio = mobileTour.LongitudInicio;
            LatitudFin = mobileTour.LatitudFin;
            LongitudFin = mobileTour.LongitudFin;
        }
    }
}
