#region Usings

using System;
using System.ComponentModel;
using System.Management;
using System.Messaging;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Statistics;
using Logictracker.Utils;

#endregion

namespace Logictracker.MsmqMessaging
{
    public class MessageQueueMonitor : Task
    {
        #region States enum

        public enum States
        {
            [Description("Cola Inexistente;Red")] NOT_FOUND,
            [Description("Sin Movimiento;Blue")] STABLE,
            [Description("Circulando;Green")] WORKING,
            [Description("Vacia;DarkGreen")] EMPTY,
            [Description("Cargando;Red")] RISING,
            [Description("Descargando;DarkBlue")] FALLING
        };

        #endregion

        private readonly int queue_interval;
        private readonly string queue_name;
        private readonly Trend trend;

        private int messages_in_queue;

        public MessageQueueMonitor (string _queue_name) : base("MessageQueueMonitor<" + _queue_name + ">")
        {
			trend = new Trend(Config.MessageQueueMonitor.TrendSamples, Config.MessageQueueMonitor.TrendStableThreshold);
			queue_interval = Config.MessageQueueMonitor.QueryInterval;
            queue_name = _queue_name.StartsWith(".") ? String.Format("{0}{1}", Environment.MachineName, _queue_name.Substring(1)) : _queue_name;
            MessagesInQueue = GetMessagesInQueue(queue_name);
            if (State != States.NOT_FOUND && State != States.EMPTY) 
                    State = States.STABLE;
        }

        public States State { get; protected set;}

        public int MessagesInQueue
        {
            get { return messages_in_queue; }

            private set
            {
                trend.NewSample(value);
                messages_in_queue = value;
                if (messages_in_queue == -1)
                {
                    State = States.NOT_FOUND;
                    return;
                }
                if (messages_in_queue == 0)
                {
                    State = States.EMPTY;
                    return;
                }
                if (trend.Secular == Trend.SecularStates.FALLING)
                { 
                    State = States.FALLING;
                    return;  
                }
                if (trend.Secular == Trend.SecularStates.RISING)
                {
                    State = States.RISING;
                    return;
                }
                State = States.STABLE;
            }
        }

        protected override int DoWork(ulong ticks)
        {
            MessagesInQueue = GetMessagesInQueue(queue_name);
            STrace.Debug(typeof(MessageQueueMonitor).FullName, String.Format("MessageQueueMonitor[{0}]: MessagesInQueue={1}", queue_name, MessagesInQueue));
            return queue_interval;
        }

        public static bool Exists(string queue_name)
        {
            try
            {
                return MessageQueue.Exists(queue_name);
            }
            catch
            {
                return false;
            }
        }

        public static int GetMessagesInQueue(string _queue_name)
        {
            try
            {
                var queue_name = _queue_name.StartsWith(".") ? String.Format("{0}{1}", Environment.MachineName, _queue_name.Substring(1)) : _queue_name;
                if (!Exists(queue_name)) return -1;
                var wmi_path = "Win32_PerfRawdata_MSMQ_MSMQQueue.name='" + queue_name + "'";
                var counter = new ManagementObject(wmi_path);
                counter.Get();
                var depth = (uint)counter.GetPropertyValue("MessagesInQueue");
                return Convert.ToInt32(depth);
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(MessageQueueMonitor).FullName, e);
                return -1;
            }
        }
    }
}
