#region Usings

using System;
using System.Text;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Types;

#endregion

namespace Urbetrack.Comm.Core.Codecs.Unetel
{
    public class HandShake0
    {
        public static LoginRequest Parse(byte[] ins)
        {
            var partes = Encoding.ASCII.GetString(ins).Split(";".ToCharArray());
            var lrq = new LoginRequest
                          {
                              DetectedDeviceType = Devices.Types.UNETEL_v2,
                              IMEI = partes[1],
                              Password = partes[2],
                              Firmware = partes[3],
                              ConfigRevision = Convert.ToInt16(partes[4]),
                              MessagesRevision = Convert.ToInt16(partes[5]),
                              Saliente = false
                          };
            return lrq;
        }
    }
}