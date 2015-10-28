﻿#region Usings

using System;
using System.Collections.Concurrent;
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
                if (PerformanceCounterCategory.Exists(cat.CategoryName)) 
                    PerformanceCounterCategory.Delete(cat.CategoryName);

                if (!PerformanceCounterCategory.Exists(cat.CategoryName)) 
                    PerformanceCounterCategory.Create(cat.CategoryName, String.Empty, PerformanceCounterCategoryType.SingleInstance, cat.Counters);
				
                return true;
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(PerformanceCounterHelper).FullName, e);
			    //Console.ReadLine();
				return false;
			}
		}

        public static ConcurrentDictionary<string,PerformanceCounter> PcCache = new ConcurrentDictionary<string, PerformanceCounter>();  

		public static PerformanceCounter Get(String categoria, String nombre)
		{
		    return PcCache.GetOrAdd(categoria + "|" + nombre + "|ron",
		        (s) => new PerformanceCounter(categoria, nombre, false));
		    //return PcCache.GetOrAdd(categoria + "|" + nombre + "|" + instancia + "|ron",(s)=>new PerformanceCounter(categoria, nombre, instancia, false));
		    //return PerformanceCounterCategory.Exists(categoria) ?  : null;
		}

		public static Boolean Increment(String categoria, String counterName)
		{
            var pc1 = Get(categoria, counterName+"_Total_Count");
			pc1.Increment();
            pc1 = Get(categoria, counterName + "_Per_Sec");
            pc1.Increment();
			return true;
		}

        public static Boolean IncrementBy(String categoria, String counterName, String instancia , long elapsedTicks) 
        {
            var pc1 = Get(categoria, counterName + "_AvgTime");
            pc1.IncrementBy(elapsedTicks);
            pc1 = Get(categoria, counterName + "_AvgTime_Base");
            pc1.Increment();
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
        public static BackendCategory Instance = new BackendCategory();

		public String CategoryName { get { return "Backend"; } }

		public CounterCreationDataCollection Counters
		{
			get
			{
				return new CounterCreationDataCollection
				       	{
                            new CounterCreationData{CounterName = GatewayUDP + "_Total_Count", CounterType = PerformanceCounterType.NumberOfItems32},
                            new CounterCreationData{CounterName = GatewayUDP + "_Per_Sec", CounterType = PerformanceCounterType.RateOfCountsPerSecond32},
                            new CounterCreationData{CounterName = GatewayUDP + "_AvgTime", CounterType = PerformanceCounterType.AverageTimer32},
                            new CounterCreationData{CounterName = GatewayUDP + "_AvgTime_Base", CounterType = PerformanceCounterType.AverageBase},
                            

                            new CounterCreationData{CounterName = DispatcherProcess + "_Total_Count", CounterType = PerformanceCounterType.NumberOfItems32},
                            new CounterCreationData{CounterName = DispatcherProcess + "_Per_Sec", CounterType = PerformanceCounterType.RateOfCountsPerSecond32},
                            new CounterCreationData{CounterName = DispatcherProcess + "_AvgTime", CounterType = PerformanceCounterType.AverageTimer32},
                            new CounterCreationData{CounterName = DispatcherProcess + "_AvgTime_Base", CounterType = PerformanceCounterType.AverageBase},
                        
                            new CounterCreationData{CounterName = HandleEvent + "_Total_Count", CounterType = PerformanceCounterType.NumberOfItems32},
                            new CounterCreationData{CounterName = HandleEvent + "_Per_Sec", CounterType = PerformanceCounterType.RateOfCountsPerSecond32},
                            new CounterCreationData{CounterName = HandleEvent + "_AvgTime", CounterType = PerformanceCounterType.AverageTimer32},
                            new CounterCreationData{CounterName = HandleEvent + "_AvgTime_Base", CounterType = PerformanceCounterType.AverageBase },
                        
                        };
			}
		}
        
        public String GatewayUDP { get { return "GatewayUdp"; } }
        public String DispatcherProcess { get { return "Dispatcher_Process"; } }
        public String HandleEvent  { get { return "Dispatcher_Event"; } }
	
    }
}
