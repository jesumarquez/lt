#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Xml.Serialization;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Common.QueueSupport
{
    public class Dispatcher : IDisposable
    {
        private static Dispatcher _inst;

        public static Dispatcher Instance 
        {
            get
            {
                if (_inst == null) _inst = new Dispatcher();
                return _inst;
            }
        }

        public DispCfg Config { get; private set; }

        private readonly List<MessageQueue> queues_ = new List<MessageQueue>();
        public List<MessageQueue> Queues
        {
            get { return queues_; }
        }

        private readonly List<IQueueHandler> qhandlers_ = new List<IQueueHandler>();
        public List<IQueueHandler> QHandlers
        {
            get { return qhandlers_; }
        }

        private void CreateQueue()
        {
            // Recorro las queue creandolas si no existen
            foreach (var q in Config.Queue)
            {
                try
                {
                    MessageQueue que;
                    // Verifica las Queue
                    if (MessageQueue.Exists(q.name))
                    {
                        T.TRACE("Existe Queue:" + q.name);
                        que = new MessageQueue(q.name);
                    }
                    else 
                    {
                        T.TRACE("Creando Queue:" + q.name);
                        que = MessageQueue.Create(q.name,q.transacional);
                    }
                    
                    Queues.Add(que);

                    T.TRACE("qHandler = {0}", q.qHandler);

                    // No tiene un queue handler , uso el default
                    if (String.IsNullOrEmpty(q.qHandler))
                    {
                        IQueueHandler qh = new QueueHandler(que, q.handler, q.timeout);

                        QHandlers.Add(qh);
                    }
                    else
                    {
                        var t = Type.GetType(q.qHandler, true);
                        if (t == null)
                        {
                            T.TRACE("No se puede cargar el tipo: " + q.qHandler);
                            continue; // Siguiente queu
                        }
                        var constInfo = t.GetConstructor(new[]{typeof(MessageQueue),typeof(DispCfgQueueHandler[])});

                        if (constInfo == null)
                        {
                            T.TRACE("No se puede construir la clase:" + q.qHandler);
                            continue;
                        }

                        var qh = (IQueueHandler)constInfo.Invoke(new object[] { que, q.handler });
                        QHandlers.Add(qh);
                    }
                }
                catch (Exception ex)
                {
                    T.EXCEPTION(ex);
                    throw;
                }
            }
        }

        public void Stop()
        {
            // Las cierro y disposeo
            foreach (var q in queues_)
            {
                q.Close();
                q.Dispose();
            }
            // Limpio la coleccion
            queues_.Clear();
            // Limpoo os handlers
            qhandlers_.Clear();
        }
        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            // Detengo
            Stop();
            try
            {
                // Leo configuracion 
                var fname = Toolkit.Config.GetConfigurationString("queues", "configuration", "Configuracion.xml");
                T.INFO("Leyendo cfg desde : {0}", fname);
                var fs = new FileStream(fname, FileMode.Open);
                var xml = new XmlSerializer(typeof (DispCfg));
                Config = (DispCfg) xml.Deserialize(fs);
                // Creo as queu
                CreateQueue();
            }
            catch (Exception ex)
            {
                T.EXCEPTION(ex);
            }
        }

        public bool Stoped
        {
            get { return queues_.Count == 0; }
        }

    }
}
