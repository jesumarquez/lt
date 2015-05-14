#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Timers;

#endregion

namespace Logictracker.Scheduler.Core
{
    public class Scheduler
    {
        #region Private Properties

        private static Configuration Config { get; set; }

        private static List<TaskTimer> Timers { get; set; }

        #endregion

        #region Public Properties

        public static Scheduler Instance { get { return _instance ?? (_instance = new Scheduler()); } }
        private static Scheduler _instance;

        #endregion

        #region Public Methods

        public void Start()
        {
            try
            {
                Stop();

                Config = LoadConfiguration();

                CreateTimers();
            }
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e);
			}
        }

        public void Stop()
        {
            if (Timers != null)
            {
                foreach (var taskTimer in Timers) taskTimer.Dispose();
                
                Timers.Clear();
				Timers = null;
            }

            Config = null;
            _instance = null;
        }

        #endregion

        #region Private Methods

        private Configuration LoadConfiguration()
        {
			var config = Logictracker.Configuration.Config.Scheduler.SchedulerTimersConfiguration;

            STrace.Trace(GetType().FullName, String.Format("Leyendo configuracion desde : {0}", config));

            var fs = new FileStream(config, FileMode.Open);

            var xml = new XmlSerializer(typeof(Configuration));

			xml.UnknownAttribute += SerializerUnknownAttribute;

			xml.UnknownNode += SerializerUnknownNode;

			xml.UnknownElement += SerializerUnknownElement;

            var conf = (Configuration)xml.Deserialize(fs);

            fs.Close();

            return conf;
        }

		private void SerializerUnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			STrace.Error(GetType().FullName, String.Format(
				"UnknownAttribute, Name={0} InnerXml={1} LineNumber={2} LinePosition={3} ObjectBeingDeserialized={4} sender={5}",
				e.Attr.Name, 
				e.Attr.InnerXml, 
				e.LineNumber, 
				e.LinePosition, 
				e.ObjectBeingDeserialized,
				sender));
		}

		private void SerializerUnknownNode(object sender, XmlNodeEventArgs e)
		{
			STrace.Error(GetType().FullName, String.Format(
				"UnknownNode, Name={0} LocalName={1} NamespaceUri={2} Text={3} NodeType={4} ObjectBeingDeserialized={5} sender={6}",
				e.Name,
				e.LocalName,
				e.NamespaceURI,
				e.Text,
				e.NodeType,
				e.ObjectBeingDeserialized,
				sender));
		}

		private void SerializerUnknownElement(object sender, XmlElementEventArgs e)
		{
			STrace.Error(GetType().FullName, String.Format(
				"UnknownElement, Name={0} InnerXml={1} LineNumber={2} LinePosition={3} ObjectBeingDeserialized={4} sender={5}",
				e.Element.Name,
				e.Element.InnerXml,
				e.LineNumber,
				e.LinePosition,
				e.ObjectBeingDeserialized,
				sender));
		}

        private static void CreateTimers()
        {
            if (Config.Timer == null || Config.Timer.Count().Equals(0)) return;

            Timers = new List<TaskTimer>();

            foreach (var timer in Config.Timer.Where(timer => timer.IsEnabled())) Timers.Add(new TaskTimer(timer));
        }

        #endregion
    }
}