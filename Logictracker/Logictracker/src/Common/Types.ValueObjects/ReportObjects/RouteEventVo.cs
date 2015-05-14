using System;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class RouteEventVo
    {
        public const int IndexIcono = 0;
        public const int IndexInitialDate = 1;
        public const int IndexFinalDate = 2;
        public const int IndexElapsedTime = 3;
        public const int IndexDistance = 4;
        public const int IndexMinimumSpeed = 5;
        public const int IndexMaximumSpeed = 6;
        public const int IndexAverageSpeed = 7;
        public const int IndexInitialCorner = 8;
        public const int IndexFinalCorner = 9;

        [GridMapping(Index = IndexIcono, IsTemplate = true, AllowGroup = false)]
        public string Icono { get; set; }

        [GridMapping(Index = IndexInitialDate, ResourceName = "Labels", VariableName = "INICIO", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true)]
        public DateTime InitialDate { get; set; }

        [GridMapping(Index = IndexFinalDate, ResourceName = "Labels", VariableName = "FIN", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime FinalDate { get; set; }

        [GridMapping(Index = IndexElapsedTime, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public TimeSpan ElapsedTime { get; set; }

        [GridMapping(Index = IndexDistance, ResourceName = "Labels", VariableName = "DISTANCIA", DataFormatString = "{0:0.00} km", AllowGroup = false, ExcelTemplateName="Distancia_Col")]
        public double Distance { get; set; }

        [GridMapping(Index = IndexMinimumSpeed, ResourceName = "Labels", VariableName = "VELOCIDAD_MINIMA", DataFormatString = "{0:0.00} km/h", AllowGroup = false)]
        public double MinimumSpeed { get; set; }

        [GridMapping(Index = IndexMaximumSpeed, ResourceName = "Labels", VariableName = "VELOCIDAD_MAXIMA", DataFormatString = "{0:0.00} km/h", AllowGroup = false)]
        public double MaximumSpeed { get; set; }

        [GridMapping(Index = IndexAverageSpeed, ResourceName = "Labels", VariableName = "VELOCIDAD_PROMEDIO", DataFormatString = "{0:0.00} km/h", AllowGroup = false, ExcelTemplateName = "Velocidad_Promedio_Col")]
        public double AverageSpeed { get; set; }

        [GridMapping(Index = IndexInitialCorner, ResourceName = "Labels", VariableName = "ESQUINA_INICIAL", AllowGroup = false)]
        public string InitialCorner { get { return _initialCorner ?? (_initialCorner = GeocoderHelper.GetDescripcionEsquinaMasCercana(InitialLatitude, InitialLongitude)); } }

        [GridMapping(Index = IndexFinalCorner, ResourceName = "Labels", VariableName = "ESQUINA_FINAL", AllowGroup = false)]
        public string FinalCorner
        {
            get {
                return Distance != 0
                           ? (_finalCorner ?? (_finalCorner = GeocoderHelper.GetDescripcionEsquinaMasCercana(FinalLatitude, FinalLongitude)))
                           : _initialCorner;
            }
        }


        public double MaxSpeed { get; set; }
        public double Direction { get; set; }
        public double InitialLatitude { get; set; }
        public double InitialLongitude { get; set; }
        public double FinalLatitude { get; set; }
        public double FinalLongitude { get; set; }

        private string _finalCorner;
        private string _initialCorner;

        public RouteEventVo(RouteEvent routeEvent)
        {
            InitialDate = routeEvent.InitialDate.ToDisplayDateTime();
            FinalDate = routeEvent.FinalDate.ToDisplayDateTime();
            ElapsedTime = routeEvent.ElapsedTime;
            Distance = routeEvent.Distance;
            MinimumSpeed = routeEvent.MinimumSpeed;
            MaximumSpeed = routeEvent.MaximumSpeed;
            AverageSpeed = routeEvent.AverageSpeed;
            MaxSpeed = routeEvent.MaximumSpeed;
            Direction = routeEvent.Direction;
            InitialLatitude = routeEvent.InitialLatitude;
            FinalLatitude = routeEvent.FinalLatitude;
            InitialLongitude = routeEvent.InitialLongitude;
            FinalLongitude = routeEvent.FinalLongitude;
        }
    }
}
