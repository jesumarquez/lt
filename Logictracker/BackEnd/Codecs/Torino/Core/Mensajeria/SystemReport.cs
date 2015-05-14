#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class SystemReport : PDU
    {
        public int SystemResets { get; set; }
        public int WatchDogResets { get; set; }
        public int GPS_FixedSeconds { get; set; }
        public int GPS_BlindSeconds { get; set; }
        public int GPS_Resets { get; set; }
        public int NETWORK_UDP_ReceivedBytes { get; set; }
        public int NETWORK_UDP_SentBytes { get; set; }
        public int NETWORK_UDP_ReceivedDgrams { get; set; }
        public int NETWORK_UDP_SentDgrams { get; set; }
        public int NETWORK_Resets { get; set; }
        public int MODEM_Resets { get; set; }

        public SystemReport(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            SystemResets = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            WatchDogResets = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            GPS_FixedSeconds = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            GPS_BlindSeconds = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            GPS_Resets = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            NETWORK_UDP_ReceivedBytes = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            NETWORK_UDP_SentBytes = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            NETWORK_UDP_ReceivedDgrams = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            NETWORK_UDP_SentDgrams = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            NETWORK_Resets = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            MODEM_Resets = UrbetrackCodec.DecodeInteger(buffer, ref pos);
        }
    }
}