#region Usings

using System;
using System.Collections.Generic;
using System.Net;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.Comm.Core.Transport.XBeeRLP;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    /// <summary>
    /// La clase destino, representa un dispositivo o un servidor, dependiento del UT
    /// que la utilize. Contiene las direcciones de enrutamiento para todos los medios validos.
    /// </summary>
    [Serializable]
    public class Destino
    {
        public override string ToString()
        {
            if (XBee != null)
            {
                return String.Format("xbee/{0}", XBee);
            }    
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

        public string GetAddress()
        {
            if (XBee != null)
            {
                return XBee.ToString();
            }
            if (TCP != null)
            {
                return TCP.ToString();
            }
            if (UDP != null)
            {
                return UDP.ToString();
            }
            return "unset";
        }

        public string GetTransport()
        {
            if (XBee != null)
            {
                return "xbee";
            }
            if (TCP != null)
            {
                return "tcp";
            }
            if (UDP != null)
            {
                return "udp";
            }
            return "unset";
        }

        public IPEndPoint UDP { get; set; }

        public IPEndPoint TCP { get; set; }

        public XBeeAddress XBee { get; set; }

        [NonSerialized] 
        private Queue<byte> secuencias;

        public byte TomarSecuencia(TransactionUser ut)
        {
            if (secuencias == null)
            {
                // inicializo la tabla de secuencias.
                // esto ocurre una vez por Destino.
                secuencias = new Queue<byte>();
                for (var i = ut.LimiteInferiorSeq; i < ut.LimiteSuperiorSeq; i++)
                {
                    secuencias.Enqueue(i);
                }
            }
            return secuencias.Dequeue();
        }

        public void LiberarSecuencia(TransactionUser ut, byte secuencia)
        {
            secuencias.Enqueue(secuencia);
        }
    }
}