#region Usings

using System;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Logictracker.Description.Runtime;

#endregion

namespace Logictracker.Runtime
{
    /// <summary>
    /// Framework console application for executing applications.
    /// </summary>
    static class MainProgram
    {
        #region Main

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
		public static void Main(String[] args)
        {
            
			ApplicationLoader.LoadAndRun(args);
        }

        #endregion
    }
}