using System;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.Security;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileTourVo
    {
        public const int KeyIndexIdMovil = 0;

        public const int IndexInterno = 0;
        public const int IndexEntrada = 1;
        public const int IndexEntradaHora = 2;
        public const int IndexSalida = 3;
        public const int IndexSalidaHora = 4;
        public const int IndexDuracion = 5;

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03", IsInitialGroup = true)]
        public string Interno { get; set; }

        [GridMapping(Index = IndexEntrada, ResourceName = "Labels", VariableName = "INICIO", DataFormatString = "{0:d}", AllowGroup = false)]
        public DateTime Entrada { get; set; }

        [GridMapping(Index = IndexEntradaHora, ResourceName = "Labels", VariableName = "HORA", DataFormatString = "{0:T}", AllowGroup = false)]
        public DateTime EntradaHora { get; set; }

        [GridMapping(Index = IndexSalida, ResourceName = "Labels", VariableName = "FIN", DataFormatString = "{0:d}", AllowGroup = false)]
        public DateTime Salida { get; set; }

        [GridMapping(Index = IndexSalidaHora, ResourceName = "Labels", VariableName = "HORA", DataFormatString = "{0:T}", AllowGroup = false)]
        public DateTime SalidaHora { get; set; }

        [GridMapping(Index = IndexDuracion, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public string Duracion { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdMovil { get; set; }

        public MobileTourVo(LogMensajeDAO.MobileTour mobileTour)
        {
            var entrada = mobileTour.Entrada.ToDisplayDateTime();
            var salida = mobileTour.Salida.ToDisplayDateTime();
            Interno = mobileTour.Interno;
            Entrada = entrada;
            EntradaHora = entrada;
            Salida = salida;
            SalidaHora = salida;
            Duracion = mobileTour.Duracion.Equals(TimeSpan.Zero)
                                           ? string.Empty
                                           : mobileTour.Duracion.ToString(); 
            IdMovil = mobileTour.IdMovil;
        }
    }
}
