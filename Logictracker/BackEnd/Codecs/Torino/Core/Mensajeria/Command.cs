#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class Command : PDU
    {
        public enum Comandos
        {
            Syncronize,
            DeviceData,
            DeviceUpgradeFirmware,
            _NUM_COMANDOS_
        }

        public Command(byte tipo)
        {
            CH = (byte)Codes.HighCommand.Command;
            CL = tipo; // tipo de acarreo.
        }

        public Command(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            var size = UrbetrackCodec.DecodeShort(buffer, ref pos);
            Datos = size > 0 ? UrbetrackCodec.DecodeBytes(buffer, ref pos, size) : null;
        }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            if (Datos != null && Datos.GetLength(0) > 0)
            {
                UrbetrackCodec.EncodeShort(ref buffer, ref pos, Convert.ToInt16(Datos.GetLength(0)));
                UrbetrackCodec.EncodeBytes(ref buffer, ref pos, Datos);
            } else
            {
                UrbetrackCodec.EncodeShort(ref buffer, ref pos, 0);
            }
        }

        #region Propiedades Publicas

        public byte[] Datos { get; set; }

        public override bool RequiereACK
        {
            get { return false; }
        }

        public override bool RequiereRespuesta
        {
            get { return false; }
        }

        #endregion

    }
}