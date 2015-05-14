using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Dispatcher.Core.AuxClasses
{
    /// <summary>
    /// Class for temporaly caching device parameters values.
    /// </summary>
    [Serializable]
    public class DeviceParameters
    {
        #region Constructors

        /// <summary>
        /// Generates a new device parameters object based on the speficied device.
        /// </summary>
        /// <param name="dispositivo"></param>
        public DeviceParameters(Dispositivo dispositivo) { UpdateParameters(dispositivo); }

        #endregion

        #region Private Properties

        /// <summary>
        /// The last dare of refresh.
        /// </summary>
        private DateTime _lastRefresh;

        /// <summary>
        /// Period in minutes that determines the refresh time span.
        /// </summary>
        private int _refreshMinutes;

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines if the device discards invalid positions.
        /// </summary>
        public bool DiscardsInvalidPositions { get; private set; }

        /// <summary>
        /// Determines if the positions reported by the device require any georeferencation corrections.
        /// </summary>
        public bool CorrectsPositionsGeoreferenciation { get; private set; }

        /// <summary>
        /// Determines if stopped events sould be generated for this device.
        /// </summary>
        public bool GeneratesStoppedEvents { get; private set; }

        /// <summary>
        /// Determines if the odometers of the vehicle associated to the device should be updated.
        /// </summary>
        public bool CalculatesOdometers { get; private set; }

        /// <summary>
        /// Determines if the positions reported by the device contains hdop information.
        /// </summary>
        public bool InformsHdop { get; private set; }

        /// <summary>
        /// Maximun hdop value accepted by the device..
        /// </summary>
        public int MaxHdop { get; private set; }

        /// <summary>
        /// Radius to apply when correctin positions georefferenciation while the device is stopped.
        /// </summary>
        public int StoppedGeocercaRadius { get; private set; }

        /// <summary>
        /// Duration of the stopped event to be considered relevant.
        /// </summary>
        public int StoppedEventTime { get; private set; }

        /// <summary>
        /// Determines up to wich date in the past positions from this device should be acepted.
        /// </summary>
        public int PositionsListExpiration { get; private set; }

        /// <summary>
        /// Determines some future dates tolerance due to time asincronism.
        /// </summary>
        public int FuturePositionMinutes { get; private set; }

        /// <summary>
        /// Determines the minimum speed ticket duration to be considered a valid infraction.
        /// </summary>
        public int MinimumSpeedTicketDuration { get; private set; }

        /// <summary>
        /// Determines the speed delta to be considered a valid infraction without taking into account the minimum speed * 2.
        /// </summary>
		public int DeltaSpeedTicketSpeedOverridesMinimumSpeedTicketDuration { get; private set; }

        /// <summary>
        /// Determines if the device parameters needs to be updated.
        /// </summary>
        public bool NeedsUpdate { get { return DateTime.Now.Subtract(_lastRefresh) > TimeSpan.FromMinutes(_refreshMinutes); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates device parameters values.
        /// </summary>
        /// <param name="dispositivo"></param>
        public void UpdateParameters(Dispositivo dispositivo)
        {
            _lastRefresh = DateTime.Now;
            var dao = new DAOFactory();

            DiscardsInvalidPositions = dao.DetalleDispositivoDAO.GetDiscardsInvalidPositionsValue(dispositivo.Id);
            CorrectsPositionsGeoreferenciation = dao.DetalleDispositivoDAO.GetCorrectsGeoreferenciationValue(dispositivo.Id);
            GeneratesStoppedEvents = dao.DetalleDispositivoDAO.GetGeneratesStoppedEventsValue(dispositivo.Id);
            CalculatesOdometers = dao.DetalleDispositivoDAO.GetCalculatesOdometersValue(dispositivo.Id);
            InformsHdop = dao.DetalleDispositivoDAO.GetInformsHdopValue(dispositivo.Id);
            MaxHdop = dao.DetalleDispositivoDAO.GetMaxHdopValue(dispositivo.Id);
            StoppedGeocercaRadius = dao.DetalleDispositivoDAO.GetStoppedGeocercaRadiusValue(dispositivo.Id);
            StoppedEventTime = dao.DetalleDispositivoDAO.GetStoppedEventTimeValue(dispositivo.Id);
            PositionsListExpiration = dao.DetalleDispositivoDAO.GetPositionsListExpirationValue(dispositivo.Id);
            FuturePositionMinutes = dao.DetalleDispositivoDAO.GetFuturePositionMinutesValue(dispositivo.Id);
            MinimumSpeedTicketDuration = dao.DetalleDispositivoDAO.GetSpeedTicketMinimunDurationValue(dispositivo.Id);
            DeltaSpeedTicketSpeedOverridesMinimumSpeedTicketDuration = dao.DetalleDispositivoDAO.GetSpeedTicketDelvelovermindurValue(dispositivo.Id);

            _refreshMinutes = dao.DetalleDispositivoDAO.GetRefreshMinutesValue(dispositivo.Id);
        }

        #endregion
    }
}
