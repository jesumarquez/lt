using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class HistogramaDetencionesDetailVo
    {
        public const int IndexMobileType = 0;
        public const int IndexIntern = 1;
        public const int IndexDriver = 2;
        public const int IndexResponsable = 3;
        public const int IndexEventTime = 4;
        public const int IndexDuration = 5;
        public const int IndexInitialCross = 6;

        [GridMapping(Index = IndexMobileType, ResourceName = "Entities", VariableName = "PARENTI17")]
        public string MobileType { get; set; }

        [GridMapping(Index = IndexIntern, ResourceName = "Labels", VariableName = "INTERNO")]
        public string Intern { get; set; }

        [GridMapping(Index = IndexDriver, ResourceName = "Labels", VariableName = "CHOFER")]
        public string Driver { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", AllowGroup = true)]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexEventTime, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:G}", InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime? EventTime { get; set; }

        [GridMapping(Index = IndexDuration, ResourceName = "Labels", VariableName = "DURACION")]
        public TimeSpan? Duration { get; set; }

        [GridMapping(Index = IndexInitialCross, ResourceName = "Labels", VariableName = "ESQUINA_INICIAL")]
        public string InitialCross { get; set; }

        [GridMapping(IsDataKey = true)]
        public int Id { get; set; }

        public HistogramaDetencionesDetailVo(MobileEvent mobileEvent)
        {
            Intern = mobileEvent.Intern;
            Driver = mobileEvent.Driver;
            Responsable = mobileEvent.Responsable;
            EventTime = mobileEvent.EventTime;
            Duration = mobileEvent.Duration;
            InitialCross = mobileEvent.InitialCross;
            MobileType = mobileEvent.MobileType;
            Id = mobileEvent.Id;
        }
    }
}
