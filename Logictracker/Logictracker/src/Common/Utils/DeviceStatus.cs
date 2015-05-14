#region Usings

using System;
using System.Text;

#endregion

namespace Logictracker.Utils
{
    /// <summary>
    /// Objeto que representa el estado de un dispositivo incluyendo posicion del mismo
    /// objeto en movimiento, representado en el tiempo.
    /// </summary>
    [Serializable]
    public class DeviceStatus
    {     
        #region Public Members

        public Int32? DeviceId;
        public byte FiredEventNumber;
        public bool? GarminConnected;
        public DigitalInputStatus MainPowerStatus;
        public DigitalInputStatus IgnitionStatus;
        public DigitalInputStatus[] DigitalInputs;
        public GPSPoint Position;

        public IgnitionStatus getEngineStatus()
        {
            if (Position == null)
                return Utils.IgnitionStatus.Unknown;            
            return Position.IgnitionStatus;
        }

        public static DigitalInputStatus decodeInputStatus(byte value, byte bit, bool invert)
        {
            return BitHelper.IsBitSet(value, bit)
                ? (invert ? DigitalInputStatus.Off : DigitalInputStatus.On)
                : (invert ? DigitalInputStatus.On : DigitalInputStatus.Off);
        }
        #endregion

        #region Constructores

        public DeviceStatus(Int32? deviceId, GPSPoint p, byte evento, byte digitalInputs)
        {
            DeviceId = deviceId;
            Position = p;
            FiredEventNumber = evento;
            IgnitionStatus = decodeInputStatus(digitalInputs, 7, false);
            MainPowerStatus = decodeInputStatus(digitalInputs, 6, false);
            DigitalInputs = translateDigitalInputs(digitalInputs);
        }
		#endregion

	    #region Private Members
        private DigitalInputStatus[] translateDigitalInputs(byte? digitalInputs)
        {
            if (digitalInputs != null)
            {
                return new[]
                           {
                               decodeInputStatus(digitalInputs.Value, 0, true),
                               decodeInputStatus(digitalInputs.Value, 1, true),
                               decodeInputStatus(digitalInputs.Value, 2, true),
                               decodeInputStatus(digitalInputs.Value, 3, true),
                               decodeInputStatus(digitalInputs.Value, 4, false),
                               decodeInputStatus(digitalInputs.Value, 5, false)
                           };
            }

            return new[]
                       {
                           DigitalInputStatus.Unknown,
                           DigitalInputStatus.Unknown,
                           DigitalInputStatus.Unknown,
                           DigitalInputStatus.Unknown,
                           DigitalInputStatus.Unknown,
                           DigitalInputStatus.Unknown
                       };
        }
	    
	    #endregion

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Device: {0} ", DeviceId);
            if (Position != null)
            {
                sb.AppendFormat(" = HDOP: {0} ", Position.HDOP);
                sb.AppendFormat(" = LAT: {0} ", Position.Lat);
                sb.AppendFormat(" = LON: {0} ", Position.Lon);
            }
            sb.AppendFormat(" = DI: ");
            for (var i = 0; i < DigitalInputs.Length; i++)
                sb.AppendFormat("{1}{2}", i, DigitalInputs[i], (i<DigitalInputs.Length-1?"-":""));
            sb.AppendFormat(" MainPower: {0} - Ignition: {1}{2}", MainPowerStatus, IgnitionStatus,
                (GarminConnected != null ? " - " + (GarminConnected.Value ? "GarminConnected: True" : "GarminConnected: False") : ""));
            return sb.ToString();
        }        
	}

    public enum DigitalInputStatus
    {
        Off = 0,
        On = 1,
        Unknown = 2,
    }
}
