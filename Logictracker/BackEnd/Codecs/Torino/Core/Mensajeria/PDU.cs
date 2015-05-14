#region Usings

using System;
using Urbetrack.Comm.Core.Transport;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class PDU
    {
        public PDU()
        {
            Options = 0x00;
        }

        public virtual void FinalEncode(ref byte[] buffer, ref int pos)
        {
        }

        public virtual string Trace(string data)
        {
            if (data.Length == 0)
            {
                data = CH == 0x80 ? "ACK" : "PDU";
            }
            return string.Format("{0} O={1} CH={2} CL={3} {4}={5} dir={6} seq={7}",data, Options, CH, CL, (Entrante ? "src" : "dst"), Destino, (Entrante ? "IN" : "OUT"), Seq);
        }

        #region Propiedades Publicas

        public short IdDispositivo { get; set; }

        public byte Seq { get; set; }

        public byte CH { get; set; }

        public byte CL { get; set; }

        public byte Options { get; set; }

        public bool Entrante { get; set; }

        public bool Saliente
        {
            get { return !Entrante; }
            set { Entrante = !value; }
        }

        public virtual bool RequiereACK
        {
            get { return false; }
        }

        public virtual bool RequiereRespuesta
        {
            get { return false; }
        }

        public virtual string IdTransaccion
        {
            get { return String.Format("net:{0}/dev:{1}/seq:{2}", Destino, IdDispositivo, Seq); }
        }

        public String TraxDestino { get; set; } //necesito este dato para armar la respuesta
        #endregion

        #region Direccion de Origen

        public Destino Destino { get; set; }

        public Transporte Transporte { get; set; }

        public PDU SourcePdu { get; set; }

        public int ExtraData { get; set; }

        #endregion
    }
}