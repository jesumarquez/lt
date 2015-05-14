using System;
using System.Collections.Generic;
using System.Net;
using Urbetrack.Mobile.Comm.Transaccional;

namespace Urbetrack.Mobile.Comm.Transport
{
    /// <summary>
    /// La clase destino, representa un dispositivo o un servidor, dependiento del UT
    /// que la utilize. Contiene las direcciones de enrutamiento para todos los medios validos.
    /// </summary>
    public class Destination
    {
        public override string ToString()
        {
#if XBEE_HABILITADO
            if (xBee != null)
            {
                return String.Format("xbee/{0}", XBee);
            }    
#endif
            if (TCP != null)
            {
                return String.Format("tcp/{0}", TCP);
            }
            if (UDP != null)
            {
                return String.Format("udp/{0}", UDP);
            }
            return "unknow/unknow";
        }

        public IPEndPoint UDP { get; set; }

        public IPEndPoint TCP { get; set; }

        public IPEndPoint UIQ { get; set; }

        private Queue<byte> secuencias;
        public byte TomarSecuencia(TransactionUser ut)
        {
            if (secuencias == null)
            {
                // inicializo la tabla de secuencias.
                // esto ocurre una vez por destination.
                secuencias = new Queue<byte>();
                for (byte i = ut.LimiteInferiorSeq; i < ut.LimiteSuperiorSeq; i++)
                {
                    secuencias.Enqueue(i);
                }
            }
            return secuencias.Dequeue();
        }

        public void LiberarSecuencia(TransactionUser ut, byte secuencia)
        {
            if (secuencias == null) return;
            secuencias.Enqueue(secuencia);
        }

#if XBEE_HABILITADO
        private short xBee;
        public short XBee
        {
            get { return xBee; }
            set { xBee = value; }
        }
#endif


    }
}