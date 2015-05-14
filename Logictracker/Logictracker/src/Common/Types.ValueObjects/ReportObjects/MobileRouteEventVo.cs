using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileRouteEventVo
    {
        public const int KeyIndexId = 0;

        public const int IndexIconUrl = 0;
        public const int IndexDriver = 1;
        public const int IndexResponsable = 2;
        public const int IndexEventTime = 3;
        public const int IndexDuration = 4;
        public const int IndexMessage = 5;
        public const int IndexInitialCross = 6;
        public const int IndexFinalCross = 7;

        [GridMapping(Index = IndexIconUrl, HeaderText = "", Width = "40px", AllowGroup = false)]
        public string IconUrl { get; set; }

        [GridMapping(Index = IndexDriver, ResourceName = "Labels", VariableName = "CHOFER")]
        public string Driver { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE")]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexEventTime, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Ascending)]
        public DateTime? EventTime { get; set; }

        [GridMapping(Index = IndexDuration, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public TimeSpan? Duration { get; set; }

        [GridMapping(Index = IndexMessage, ResourceName = "Entities", VariableName = "PAREVEN01")]
        public string Message { get; set; }

        [GridMapping(Index = IndexInitialCross, ResourceName = "Labels", VariableName = "ESQUINA_INICIAL", AllowGroup = false)]
        public string InitialCross { get; set; }

        [GridMapping(Index = IndexFinalCross, ResourceName = "Labels", VariableName = "ESQUINA_FINAL", AllowGroup = false)]
        public string FinalCross { get; set; }

        [GridMapping(IsDataKey = true)]
        public int Id { get; set; }

        public MobileRouteEventVo(MobileEvent mobileEvent)
        {
            IconUrl = mobileEvent.IconUrl;
            Driver = mobileEvent.Driver;
            Responsable = mobileEvent.Responsable;
            EventTime = mobileEvent.EventTime;
            Duration = mobileEvent.Duration;
            Message = mobileEvent.Message;
            InitialCross = mobileEvent.InitialCross;
            FinalCross = mobileEvent.FinalCross;
            Id = mobileEvent.Id;
        }
    }
}
