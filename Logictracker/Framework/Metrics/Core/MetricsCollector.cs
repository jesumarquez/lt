#region Usings

using System;
using Logictracker.Metrics.Core.AuxClasses;
using Logictracker.Metrics.Enums;

#endregion

namespace Logictracker.Metrics.Core
{
    /// <summary>
    /// Class for performing metrics data collection.
    /// </summary>
    public class MetricsCollector
    {
        #region Private Properties

        /// <summary>
        /// The metric module associated to the current collector.
        /// </summary>
        private readonly MetricModule _module;

        /// <summary>
        /// The component of the metric module of the current collector.
        /// </summary>
        private readonly String _component;

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new metrics data collector for the specified module and component.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="component"></param>
        public MetricsCollector(MetricModule module, String component)
        {
            _module = module;
            _component = component;

            Consumer.CategoryName = module.GetCategoryName();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new metric data with the specified collection type and name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public void AddMetric(MetricType type, String name) { AddMetric(type, name, 1); }

        /// <summary>
        /// Adds a new metric data with the specified collection type, name and value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddMetric(MetricType type, String name, Double value) { AddMetric(type, name, value, null, null); }

        /// <summary>
        /// Adds a new metric data with the specified collection type, name and value, associating it to the specified vehicle and device ids.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="vehicleType"></param>
        /// <param name="deviceType"></param>
        public void AddMetric(MetricType type, String name, Double value, Int32? vehicleType, Int32? deviceType)
        {
            Queue.Enqueue(GetMetricInstance(type, name, value, vehicleType, deviceType));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a new metric instance with the provided values.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="vehicleType"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        private MetricInstance GetMetricInstance(MetricType type, String name, Double value, Int32? vehicleType, Int32? deviceType)
        {
            return new MetricInstance
                       {
                           Component = _component,
                           DeviceType = deviceType,
                           DateTime = DateTime.UtcNow,
                           Module = _module.GetDescription(),
                           Name = name,
                           Type = type,
                           Value = value,
                           VehicleType = vehicleType
                       };
        }

        #endregion
    }
}