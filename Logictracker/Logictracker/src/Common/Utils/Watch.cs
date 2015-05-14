﻿#region Usings

using System;
using System.Diagnostics;

#endregion

namespace Logictracker.Utils
{
    public static class Watch
    {
        private static Stopwatch _watch;
        public static TimeSpan Elapsed = TimeSpan.Zero;
        public static void Start()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _watch = Stopwatch.StartNew();
        }
        public static TimeSpan Stop()
        {
            Elapsed = _watch.Elapsed;
            return Elapsed;
        }

    }
}
