using System;
using System.Collections.Generic;
using Urbetrack.DatabaseTracer.Core;
using XbeeCore.Exceptions;

namespace XbeeCore
{
    internal class XBeeTrLayer
    {
        private readonly Dictionary<byte, XBeeTxTransaction> tr_layer = new Dictionary<byte, XBeeTxTransaction>();
        private readonly Queue<byte> sequences = new Queue<byte>();

        internal XBeeTrLayer()
        {
            // el 0 (cero) no se usa, segun la especificacion del api.
            for (byte b = 1; b < byte.MaxValue; b++)
            {
                sequences.Enqueue(b);
            }
        }

        internal int Count
        {
            get { return tr_layer.Count; }
        }

        internal byte NextSequence
        {
            get
            {
                lock (this)
                {
                    if (sequences.Count > 0)
                    {
                        return sequences.Dequeue();
                    }
                    throw new NoSequenceException();
                }
            }
        }

        internal void Create(XBeePDU pdu, int timeout)
        {
            #if ENABLE_XBEE_TR_LAYER
            lock (this) {
                pdu.Sequence = NextSequence;
                var tr = new XBeeTxTransaction(this, pdu, timeout);
                STrace.Debug(GetType().FullName, String.Format("Creando Transaccion id={0} timeout={1}", pdu.Sequence, timeout));
                Stack._active_transactions++;
                tr_layer.Add(pdu.Sequence, tr);
            }
            #else
            pdu.Sequence = 0;
            #endif
        }

        internal XBeeTxTransaction Get(byte id)
        {
            #if ENABLE_XBEE_TR_LAYER
            lock (this)
            {
                if (tr_layer.ContainsKey(id))
                {
                    STrace.Debug(GetType().FullName, String.Format("Se encontro la Transaccion id={0}", id));
                    return tr_layer[id];
                }
                STrace.Debug(GetType().FullName, String.Format("Falta Transaccion id={0}", id));
                return null;
            }
            #else
            return null;
            #endif
        }

        internal void Remove(byte id)
        {
            #if ENABLE_XBEE_TR_LAYER
            lock (this)
            {
                if (!tr_layer.ContainsKey(id)) return;
                STrace.Debug(GetType().FullName, String.Format("Eliminando Transaccion id={0}", id));
                tr_layer.Remove(id);
                Stack._active_transactions--;
                sequences.Enqueue(id);
            }
            #endif
        }
        
    }
}
