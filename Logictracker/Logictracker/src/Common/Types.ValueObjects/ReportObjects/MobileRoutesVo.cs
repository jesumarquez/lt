using System;
using Logictracker.DAL.Factories;
using Logictracker.Services.Helpers;
using Logictracker.Types.ReportObjects;
using System.Globalization;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileRoutesVo
    {
        public const int IndexInitialTime = 0;
        public const int IndexFinalTime = 1;
        public const int IndexDuration = 2;
        public const int IndexEngineStatus = 3;
        public const int IndexStatus = 4;
        public const int IndexDriver = 5;
        public const int IndexGeocerca = 6;
        public const int IndexKilometers = 7;
        public const int IndexMinSpeed = 8;
        public const int IndexAverageSpeed = 9;
        public const int IndexMaxSpeed = 10;
        public const int IndexInfractions = 11;
        public const int IndexInfractionsDuration = 12;
        public const int IndexConsumo = 13;
        public const int IndexHsMarcha = 14;

        [GridMapping(Index = IndexInitialTime, ResourceName = "Labels", VariableName = "INICIO", DataFormatString = "{0:G}", InitialSortExpression = true, AllowGroup = false)]
        public DateTime InitialTime { get; set; }

        [GridMapping(Index = IndexFinalTime, ResourceName = "Labels", VariableName = "FIN", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime FinalTime { get; set; }

        [GridMapping(Index = IndexDuration, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public string Duration { get; set; }

        [GridMapping(Index = IndexEngineStatus, ResourceName = "Labels", VariableName = "ESTADO_MOTOR")]
        public string EngineStatus { get; set; }

        [GridMapping(Index = IndexStatus, ResourceName = "Labels", VariableName = "ESTADO")]
        public string VehicleStatus { get; set; }

        [GridMapping(Index = IndexDriver, ResourceName = "Labels", VariableName = "CHOFER")]
        public string Driver { get; set; }

        [GridMapping(Index = IndexGeocerca, ResourceName = "Labels", VariableName = "GEOCERCA")]
        public string Geocerca 
        { 
            get
            {
                if (!_mobileRoutes.Geocerca.Equals(string.Empty))
                    return _mobileRoutes.Geocerca;

                if (_mobileRoutes.VehicleStatus.Equals("Detenido") && _verDirecciones)
                {
                    var dao = new DAOFactory();
                    var coche = dao.CocheDAO.FindById(_mobileRoutes.Id);
                    var maxMonths = coche != null && coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;
                    var position = dao.LogPosicionDAO.GetFirstPositionOlderThanDate(_mobileRoutes.Id, _mobileRoutes.FinalTime, maxMonths);
                    return GeocoderHelper.GetDescripcionEsquinaMasCercana(position.Latitud, position.Longitud);
                }

                return string.Empty;
            }
        }

        [GridMapping(Index = IndexKilometers, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:2}", AllowGroup = false)]
        public double Kilometers { get; set; }

        [GridMapping(Index = IndexMinSpeed, ResourceName = "Labels", VariableName = "VELOCIDAD_MINIMA", AllowGroup = false)]
        public int MinSpeed { get; set; }

        [GridMapping(Index = IndexAverageSpeed, ResourceName = "Labels", VariableName = "VELOCIDAD_PROMEDIO", AllowGroup = false)]
        public int AverageSpeed { get; set; }

        [GridMapping(Index = IndexMaxSpeed, ResourceName = "Labels", VariableName = "VELOCIDAD_MAXIMA", AllowGroup = false)]
        public int MaxSpeed { get; set; }

        [GridMapping(Index = IndexInfractions, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public int Infractions { get; set; }

        [GridMapping(Index = IndexInfractionsDuration, ResourceName = "Labels", VariableName = "TIEMPO_INFRACCION", AllowGroup = false)]
        public string InfractionsDuration { get; set; }

        [GridMapping(Index = IndexConsumo, ResourceName = "Labels", VariableName = "CONSUMO", AllowGroup = false)]
        public string Consumos { get; set; }

        [GridMapping(Index = IndexHsMarcha, ResourceName = "Labels", VariableName = "HS_MARCHA", AllowGroup = false)]
        public string HsMarcha { get; set; }

        private MobileRoutes _mobileRoutes;
        private bool _verDirecciones;

        public MobileRoutesVo(MobileRoutes mobileRoutes, bool verDirecciones)
        {
            _mobileRoutes = mobileRoutes;
            _verDirecciones = verDirecciones;

            InitialTime = mobileRoutes.InitialTime;
            FinalTime = mobileRoutes.FinalTime;
            Duration = String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(mobileRoutes.Duration).Hours, TimeSpan.FromHours(mobileRoutes.Duration).Minutes, TimeSpan.FromHours(mobileRoutes.Duration).Seconds));
            EngineStatus = mobileRoutes.EngineStatus;
            Driver = mobileRoutes.Driver;
            
            Kilometers = mobileRoutes.Kilometers;
            MinSpeed = mobileRoutes.MinSpeed;
            AverageSpeed = mobileRoutes.AverageSpeed;
            MaxSpeed = mobileRoutes.MaxSpeed;
            Infractions = mobileRoutes.Infractions;
            var ts = TimeSpan.FromMinutes(mobileRoutes.InfractionsDuration);
            InfractionsDuration = String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, ts.Hours, ts.Minutes, ts.Seconds));
            VehicleStatus = mobileRoutes.VehicleStatus;
            Consumos = mobileRoutes.Consumo.ToString("#0.00", CultureInfo.InvariantCulture);
            ts = TimeSpan.FromHours(mobileRoutes.HsMarcha);
            HsMarcha = String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, ts.Hours, ts.Minutes, ts.Seconds));
        }
    }
}
