#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Metrics.Core.AuxClasses;
using Logictracker.Metrics.Enums;
using Logictracker.Metrics.NHibernateManagers;
using Logictracker.Metrics.Types;

#endregion

namespace Logictracker.Metrics.Core
{
    /// <summary>
    /// Class for consuming and processing metrics data collected.
    /// </summary>
    internal static class Consumer
    {
        #region Private Properties

        /// <summary>
        /// Consumer main thread.
        /// </summary>
        private static Thread _consumerThread;

        /// <summary>
        /// Metrics saver thread.
        /// </summary>
        private static Thread _saverThread;

        /// <summary>
        /// Event for syncronizing consumer thread.
        /// </summary>
        private static Semaphore _semaphore;

        /// <summary>
        /// Error interval before trying to re enqueue the log.
        /// </summary>
        private static readonly TimeSpan ErrorInterval = Config.Metrics.MetricsErrorInterval;

        /// <summary>
        /// Max log retries to trace it to database.
        /// </summary>
        private static readonly Int32 MaxRetries = Config.Metrics.MetricsMaxRetries;

        /// <summary>
        /// List of the currently configured metrics.
        /// </summary>
        private static readonly List<MetricConfiguration> Configuration = new List<MetricConfiguration>();

        /// <summary>
        /// List for recording all current ocurrences o a metric.
        /// </summary>
        private static readonly Dictionary<String, MetricValue> Metrics = new Dictionary<String, MetricValue>();

        /// <summary>
        /// List of custom performance counter defined for the configured metrics.
        /// </summary>
        private static readonly Dictionary<String, PerformanceCounter> PerformanceCounters = new Dictionary<String, PerformanceCounter>();

        private static String _categoryName;

        #endregion

        #region Public Properties

        /// <summary>
        /// The system performance counters category for the current application.
        /// </summary>
        public static String CategoryName
        {
            set
            {
                var createCounters = String.IsNullOrEmpty(_categoryName);

                _categoryName = value;

                if (createCounters) CreatePerformanceCounters();
            }
            private get { return _categoryName; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialice tracer consumer enviroment.
        /// </summary>
        static Consumer()
        {
            try
            {
                LoadConfiguration();

                StartConsumer();

                StartSaver();
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setps up the consumer enviroment and triggers the consumption of the logs.
        /// </summary>
        public static void Start() { _semaphore.Release(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates performance counter for monitoring the system behaivour.
        /// </summary>
        private static void CreatePerformanceCounters()
        {
            try
            {
                lock (PerformanceCounters)
                {
                    var counters = GetCounters();

                    CreatePerformanceCounterCategory(counters);

                    CreatePerformanceCounterInstance(counters);
                }
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);
            }
        }

        /// <summary>
        /// Creates a instance of each defined performance counter.
        /// </summary>
        /// <param name="counters"></param>
        private static void CreatePerformanceCounterInstance(IEnumerable<CounterCreationData> counters)
        {
            foreach (var counter in counters)
            {
                var counterInstance = GetCounterInstance(counter);

                PerformanceCounters.Add(counter.CounterName, counterInstance);
            }
        }

        /// <summary>
        /// Creates an instance of the defined performance counter.
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        private static PerformanceCounter GetCounterInstance(CounterCreationData counter) { return new PerformanceCounter(CategoryName, counter.CounterName, false); }

        /// <summary>
        /// Creates a performance counter category for holding all the configured performance counters.
        /// </summary>
        /// <param name="counters"></param>
        private static void CreatePerformanceCounterCategory(CounterCreationData[] counters)
        {
            var counterData = new CounterCreationDataCollection();

            counterData.AddRange(counters);

            if (PerformanceCounterCategory.Exists(CategoryName)) PerformanceCounterCategory.Delete(CategoryName);

            PerformanceCounterCategory.Create(CategoryName, "Performance counters defined for Logictracker.", PerformanceCounterCategoryType.SingleInstance, counterData);
        }

        /// <summary>
        /// Gets all configured performance counters.
        /// </summary>
        /// <returns></returns>
        private static CounterCreationData[] GetCounters() { return Configuration.Where(metric => metric.PublishCounter).Select(metric => GetCounter(metric)).ToArray(); }

        /// <summary>
        /// Gets a information about the speficied counter,
        /// </summary>
        /// <param name="metricConfiguration"></param>
        /// <returns></returns>
        private static CounterCreationData GetCounter(MetricConfiguration metricConfiguration)
        {
            return new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = GetCounterName(metricConfiguration.Class, metricConfiguration.Name) };
        }

        /// <summary>
        /// Gets the name of the counter.
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetCounterName(String clazz, String name) { return String.Format("{0}/{1}", clazz, name); }    

        /// <summary>
        /// Starts metrics generation thread.
        /// </summary>
        private static void StartSaver()
        {
            _saverThread = new Thread(GenerateMetrics);

            _saverThread.Start();
        }

        /// <summary>
        /// Checks the currently recorded metrics ocurrences to check if any metric should be saved into database.
        /// </summary>
        private static void GenerateMetrics()
        {
            try
            {
                Thread.Sleep(Config.Metrics.MetricsGenerationInterval);

                lock (Metrics)
                {
                    var metricsToDelete = new List<String>();

                    foreach (var metric in Metrics)
                    {
                        var configuration = GetMetricConfiguration(metric.Value);

                        var end = GetEnd(metric.Value, configuration);

                        if (!GenerateMetric(metric.Value.FirstInstance, end, configuration)) continue;

                        SaveMetric(metric.Value, end);

                        metricsToDelete.Add(metric.Key);
                    }

                    foreach (var metric in metricsToDelete) Metrics.Remove(metric);
                }
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);
            }
            finally
            {
            	GenerateMetrics();
            }
        }

        /// <summary>
        /// Gets the current end for the specified metric.
        /// </summary>
        /// <param name="metricValue"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static DateTime GetEnd(MetricValue metricValue, MetricConfiguration configuration)
        {
            return metricValue.LastInstance.Subtract(metricValue.FirstInstance) >= configuration.Interval.TimeOfDay ? metricValue.LastInstance : DateTime.UtcNow;
        }

        /// <summary>
        /// Determines if a metric should be generated.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static bool GenerateMetric(DateTime start, DateTime end, MetricConfiguration configuration) { return end.Subtract(start) >= configuration.Interval.TimeOfDay; }

        /// <summary>
        /// Save the specified metric with the givenn end datetime.
        /// </summary>
        /// <param name="metricValue"></param>
        /// <param name="end"></param>
        private static void SaveMetric(MetricValue metricValue, DateTime end) { Save(GetCurrentMetric(metricValue, end)); }

        /// <summary>
        /// Saves the givenn metric into database.
        /// </summary>
        /// <param name="metric"></param>
        private static void Save(Metric metric)
        {
            try
            {
                NHibernateHelper.GetSession().Save(metric);
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);
            }

            UpdatePerformanceCounter(metric);
        }

        /// <summary>
        /// Updates the performance counter associated to the specified metric.
        /// </summary>
        /// <param name="metric"></param>
        private static void UpdatePerformanceCounter(Metric metric)
        {
            try
            {
                lock (PerformanceCounters)
                {
                    var name = GetCounterName(metric.Component, metric.Name);

                    if (PerformanceCounters.ContainsKey(name)) PerformanceCounters[name].RawValue = (long) metric.Value;
                }
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);
            }
        }

        /// <summary>
        /// Get a new metric with its current value.
        /// </summary>
        /// <param name="metricValue"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static Metric GetCurrentMetric(MetricValue metricValue, DateTime end)
        {
            return new Metric
                       {
                           Component = metricValue.Component,
                           DeviceType = metricValue.DeviceType,
                           End = end,
                           Module = metricValue.Module,
                           Name = metricValue.Name,
                           Start = metricValue.FirstInstance,
                           Value = GetFinalValue(metricValue),
                           VehicleType = metricValue.VehicleType
                       };
        }

        /// <summary>
        /// Gets the final metric value.
        /// </summary>
        /// <param name="metricValue"></param>
        /// <returns></returns>
        private static Double GetFinalValue(MetricValue metricValue)
        {
            switch (metricValue.Type)
            {
                case MetricType.Average: return metricValue.CurrentValue/metricValue.Ocurrences;
                case MetricType.Sum: case MetricType.Max: case MetricType.Min: return metricValue.CurrentValue;
                default: return metricValue.Ocurrences;
            }
        }

        /// <summary>
        /// Gets the loaded configuration for the current metric instance.
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        private static MetricConfiguration GetMetricConfiguration(MetricValue metric)
        {
            return Configuration.First(configuration => configuration.Class.Equals(metric.Component) && configuration.Name.Equals(metric.Name));
        }

        /// <summary>
        /// Starts main consumer thread.
        /// </summary>
        private static void StartConsumer()
        {
            _semaphore = new Semaphore(0, Int32.MaxValue);

            _consumerThread = new Thread(Consume);

            _consumerThread.Start();
        }

        /// <summary>
        /// Loads metrics configuration.
        /// </summary>
        /// <returns></returns>
        private static void LoadConfiguration()
        {
            try
            {
                var metricsFile = Config.Metrics.LogictrackerMetricsConfiguration;

                var fs = new FileStream(metricsFile, FileMode.Open);

                var xml = new XmlSerializer(typeof (Configuration));

                var configuration = (Configuration)xml.Deserialize(fs);

                SaveConfiguration(configuration);
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);
            }
        }

        /// <summary>
        /// Saves into memory the currently active configurations.
        /// </summary>
        /// <param name="configuration"></param>
        private static void SaveConfiguration(Configuration configuration)
        {
            if (configuration.MetricConfiguration == null || configuration.MetricConfiguration.Length.Equals(0)) return;

            Configuration.AddRange(configuration.MetricConfiguration.Where(metric => metric.Enabled).ToList());
        }

        /// <summary>
        /// Consumes a log from the queue.
        /// </summary>
        private static void Consume()
        {
            try
            {
                _semaphore.WaitOne();

                ProcessInstance(Queue.Dequeue());
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);
            }
            finally
            {
            	Consume();
            }
        }

        /// <summary>
        /// Trace the specified log into database.
        /// </summary>
        /// <param name="metricInstance"></param>
        private static void ProcessInstance(MetricInstance metricInstance)
        {
            try
            {
                if (metricInstance == null || !IsActive(metricInstance)) return;

                if (metricInstance.Type.Equals(MetricType.Unique)) SaveUniqueInstanceMetric(metricInstance);
                else RecordMetric(metricInstance);
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(Consumer).FullName, e);

                ReEqnueue(metricInstance);
            }
        }

        /// <summary>
        /// Records the current metric
        /// </summary>
        /// <param name="metricInstance"></param>
        private static void RecordMetric(MetricInstance metricInstance)
        {
            lock (Metrics)
            {
                var key = GenerateKey(metricInstance);

                if (Metrics.ContainsKey(key)) UpdateMetricVaslues(metricInstance, key);
                else AddNewMetric(metricInstance, key);
            }
        }

        /// <summary>
        /// Adds a new metric value for the current metric instance.
        /// </summary>
        /// <param name="metricInstance"></param>
        /// <param name="key"></param>
        private static void AddNewMetric(MetricInstance metricInstance, String key) { Metrics.Add(key, GetNewCurrentValue(metricInstance)); }

        /// <summary>
        /// Gets a new current value for the specified metric.
        /// </summary>
        /// <param name="metricInstance"></param>
        /// <returns></returns>
        private static MetricValue GetNewCurrentValue(MetricInstance metricInstance)
        {
            return new MetricValue
                       {
                           Module = metricInstance.Module,
                           Component = metricInstance.Component,
                           Name = metricInstance.Name,
                           FirstInstance = metricInstance.DateTime,
                           LastInstance = metricInstance.DateTime,
                           CurrentValue = metricInstance.Value,
                           Ocurrences = 1,
                           DeviceType = metricInstance.DeviceType,
                           VehicleType = metricInstance.VehicleType,
                           Type = metricInstance.Type
                       };
        }

        /// <summary>
        /// Updates the specified metric value with the givenn metric instance.
        /// </summary>
        /// <param name="metricInstance"></param>
        /// <param name="key"></param>
        private static void UpdateMetricVaslues(MetricInstance metricInstance, String key)
        {
            Metrics[key].Ocurrences++;

            Metrics[key].CurrentValue = GetMetricCurrentValue(Metrics[key], metricInstance);

            Metrics[key].LastInstance = metricInstance.DateTime;
        }

        /// <summary>
        /// Calculates the new metric current value.
        /// </summary>
        /// <param name="metricValue"></param>
        /// <param name="metricInstance"></param>
        /// <returns></returns>
        private static Double GetMetricCurrentValue(MetricValue metricValue, MetricInstance metricInstance)
        {
            var newValue = metricValue.CurrentValue;

            switch (metricInstance.Type)
            {
                case MetricType.Average: case MetricType.Sum: newValue += metricInstance.Value; break;
                case MetricType.Max: newValue = Math.Max(newValue, metricInstance.Value); break;
                case MetricType.Min: newValue = Math.Min(newValue, metricInstance.Value); break;
            }

            return newValue;
        }

        /// <summary>
        /// Gets the key associated to the current metric instance.
        /// </summary>
        /// <param name="metricInstance"></param>
        /// <returns></returns>
        private static String GenerateKey(MetricInstance metricInstance)
        {
            var vehicleType = metricInstance.VehicleType.HasValue ? metricInstance.VehicleType.Value.ToString() : "-1";
            var deviceType = metricInstance.DeviceType.HasValue ? metricInstance.DeviceType.Value.ToString() : "-1";

            return String.Format("{0} - {1} - {2} - {3}", metricInstance.Component, metricInstance.Name, vehicleType, deviceType);
        }

        /// <summary>
        /// Saves a unique ocurrence metric into database.
        /// </summary>
        /// <param name="metricInstance"></param>
        private static void SaveUniqueInstanceMetric(MetricInstance metricInstance) { Save(GetUniqueInstance(metricInstance)); }

        /// <summary>
        /// Determines if the givenn metric is an active configured metric that is currently receiving ocurrences.
        /// </summary>
        /// <param name="metricInstance"></param>
        /// <returns></returns>
        private static bool IsActive(MetricInstance metricInstance)
        {
            return Configuration.Any(metric => metric.Name.Equals(metricInstance.Name)
                && metric.Class.Equals(metricInstance.Component)
                && (metric.StartDate.Equals(default(DateTime)) || metric.StartDate <= metricInstance.DateTime)
                && (metric.EndDate.Equals(default(DateTime)) || metric.EndDate >= metricInstance.DateTime));
        }

        /// <summary>
        /// Gets a unique ocurrence metric.
        /// </summary>
        /// <param name="metricInstance"></param>
        /// <returns></returns>
        private static Metric GetUniqueInstance(MetricInstance metricInstance)
        {
            return new Metric
                       {
                           Component = metricInstance.Component,
                           DeviceType = metricInstance.DeviceType,
                           End = metricInstance.DateTime,
                           Module = metricInstance.Module,
                           Name = metricInstance.Name,
                           Start = metricInstance.DateTime,
                           Value = metricInstance.Value,
                           VehicleType = metricInstance.VehicleType
                       };
        }

        /// <summary>
        /// Tries to re enqueue the log.
        /// </summary>
        /// <param name="metricInstance"></param>
        private static void ReEqnueue(MetricInstance metricInstance)
        {
            Thread.Sleep(ErrorInterval);

            metricInstance.Retries++;

            if (metricInstance.Retries < MaxRetries) Queue.Enqueue(metricInstance);

            _semaphore.Release();
        }

        #endregion
    }
}