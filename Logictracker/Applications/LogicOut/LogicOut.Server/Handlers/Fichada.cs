using System;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.Layers.MessageQueue;
using Logictracker.Messaging;
using Convert = LogicOut.Server.Convert;

namespace LogicOut.Server.Handlers
{
    public class Fichada : IOutHandler
    {
        protected DAOFactory DaoFactory { get; set; }

        public Fichada(DAOFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        public OutData[] Process(int empresa, int linea, QueryParams parameters)
        {
            var data = new OutData("fichada");
            try
            {
                var server = parameters.ContainsKey("server") ? parameters["server"] : string.Empty;

                if (!parameters.ContainsKey("data"))
                {
                    throw new ApplicationException("No se encontraron los datos de la fichada");
                }
                var pq = parameters["data"];
                var parts = pq.Split(':');
                if (pq[0] != 'F' || parts.Length != 7)
                {
                    throw new ApplicationException("Format de paquete incorrecto");
                }
                var equipo = System.Convert.ToInt16(parts[1]);
                var point = parts[2];
                var tipo = parts[3];
                var fecha = parts[4];
                var tarjeta = System.Convert.ToInt32(parts[5]);
                var direccion = parts[6];
                var datetime = new DateTime(System.Convert.ToInt32(fecha.Substring(0, 4)),
                                            System.Convert.ToInt32(fecha.Substring(4, 2)),
                                            System.Convert.ToInt32(fecha.Substring(6, 2)),
                                            System.Convert.ToInt32(fecha.Substring(8, 2)),
                                            System.Convert.ToInt32(fecha.Substring(10, 2)),
                                            System.Convert.ToInt32(fecha.Substring(12, 2)));

                
                var puerta = DaoFactory.PuertaAccesoDAO.FindByCodigo(empresa, linea, equipo);
                if(puerta == null)
                {
                    throw new ApplicationException("No se encontró una puerta con el código " + equipo);
                }
                if(puerta.Vehiculo == null)
                {
                    throw new ApplicationException("La puerta " + equipo + " no tiene un vehiculo asignado");
                }
                if (puerta.Vehiculo.Dispositivo == null)
                {
                    throw new ApplicationException("La puerta " + equipo + " no tiene un dispositivo asignado");
                }
                var device = puerta.Vehiculo.Dispositivo.Id;
                var tarj = DaoFactory.TarjetaDAO.FindList(new[] {empresa}, new[] {linea}).FirstOrDefault(trj => Convert.GetPinCerbero(trj) == tarjeta);
                var pin = tarj != null ? tarj.Pin : tarjeta.ToString();
                var t = direccion == "0" ? 3 : 4; //3: Login Empleado, 4: Logout Empleado
                var msg = MessageIdentifierX.FactoryRfid(device, 0, null, datetime, pin, t);

                var queue = GetDispatcherQueue();
                if(queue == null)
                {
                    throw new ApplicationException("No se pudo crear la cola");
                }
                queue.Send(msg);
                
                data.AddProperty("done", "true");

                return new[] {data};
            }
            catch(Exception ex)
            {
                data.AddProperty("done", "false");
                data.AddProperty("message", ex.ToString());
                return new[] {data};
            }
        }
        private IMessageQueue GetDispatcherQueue()
        {
            var queueName = Config.LogicOut.Fichada.QueueName;
            var queueType = Config.LogicOut.Fichada.QueueType;
            if (String.IsNullOrEmpty(queueName)) return null;

            var umq = new IMessageQueue(queueName);
            if (queueType.ToLower() == "xml") umq.Formatter = "XmlMessageFormatter";

            return !umq.LoadResources() ? null : umq;
        }
    }
}
