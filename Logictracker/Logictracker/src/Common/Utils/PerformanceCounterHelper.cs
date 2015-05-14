#region Usings

using System;
using System.Diagnostics;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Utils
{
	public class PerformanceCounterHelper
	{
		public static Boolean Create(Category cat)
		{
			try
			{
				//deshabilitar cuando esten bien definidos los nombres de los Performance Counters
				if (PerformanceCounterCategory.Exists(cat.CategoryName)) PerformanceCounterCategory.Delete(cat.CategoryName);

				if (!PerformanceCounterCategory.Exists(cat.CategoryName)) PerformanceCounterCategory.Create(cat.CategoryName, String.Empty, PerformanceCounterCategoryType.MultiInstance, cat.Counters);
				return true;
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(PerformanceCounterHelper).FullName, e);
				return false;
			}
		}

		public static PerformanceCounter Get(String categoria, String nombre, String instancia)
		{
			return PerformanceCounterCategory.Exists(categoria) ? new PerformanceCounter(categoria, nombre, instancia, false) : null;
		}

		public static Boolean Increment(String categoria, String nombre1, String nombre2, String instancia)
		{
			var pc1 = Get(categoria, nombre1, instancia);
			if (pc1 == null) return false;
			pc1.Increment();
			var pc2 = Get(categoria, nombre2, instancia);
			if (pc2 == null) return false;
			pc2.Increment();
			return true;
		}
	}

	public interface Category
	{
		String CategoryName { get; }
		CounterCreationDataCollection Counters { get; }
	}

	public class BackendCategory : Category
	{
		public String CategoryName { get { return "Backend"; } }

		public CounterCreationDataCollection Counters
		{
			get
			{
				return new CounterCreationDataCollection
				       	{
				       		new CounterCreationData{CounterName = GatewayCount, CounterType = PerformanceCounterType.NumberOfItems32},
				       		new CounterCreationData{CounterName = GatewayProm, CounterType = PerformanceCounterType.RateOfCountsPerSecond32},
				       		new CounterCreationData{CounterName = DispatcherCount, CounterType = PerformanceCounterType.NumberOfItems32},
				       		new CounterCreationData{CounterName = DispatcherProm, CounterType = PerformanceCounterType.RateOfCountsPerSecond32},
				       		new CounterCreationData{CounterName = MsmqCount, CounterType = PerformanceCounterType.NumberOfItems32},
				       		new CounterCreationData{CounterName = MsmqProm, CounterType = PerformanceCounterType.RateOfCountsPerSecond32},
				       	};
			}
		}

		public String GatewayCount { get { return "GatewayUdpRawCount"; } }
		public String GatewayProm { get { return "GatewayUdpRawProm"; } }
		public String DispatcherCount { get { return "DispatcherCount"; } }
		public String DispatcherProm { get { return "DispatcherProm"; } }
		public String MsmqCount { get { return "MsmqCount_DispProcesa_GwyEncola"; } }
		public String MsmqProm { get { return "MsmqProm_DispProcesa_GwyEncola"; } }
	}
}
