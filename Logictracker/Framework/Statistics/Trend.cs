#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace Logictracker.Statistics
{
    public class Trend
    {
        #region SecularStates enum

        public enum SecularStates
        {
            [Description("Ascendente")] RISING,
            [Description("Descendente")] FALLING,
            [Description("Estable")] STABLE,
            [Description("No es predecible")] UNPREDICTABLE
        }

        #endregion

        private readonly Queue<TrendSample> queue = new Queue<TrendSample>();

        public Trend(int size, double stable_threshold)
        {
            StartTime = DateTime.Now;
            if (size < 10) size = 10;
            Size = size;
            StableThreshold = stable_threshold;
        }

        public int Size { get; protected set; }
        public double StableThreshold  { get; protected set; }
        public DateTime StartTime { get; protected set; }

        /// <summary>
        /// Lee la tendencia secular instantanea de la variable medida.
        /// <remarks>
        /// Esta tendencia puede ser RISING, FALLING, STABLE o UNPREDICTABLE.
        /// Segun el tipo SecularStates. 
        /// </remarks>
        /// </summary>
        public SecularStates Secular
        {
            get
            {
                double a, b;
                TrendSample start, end;
                LinearRegresion(out a, out b, out start, out end);
                if (queue.Count < 5) return SecularStates.UNPREDICTABLE;

                var y1 = (a + b * (queue.Count / 2.0)); // punto medio del muestreo
                var y2 = (a + b * (queue.Count * 2.0)); // contra el futuro del muestreo.

                if (Math.Abs(y1 - y2) < StableThreshold) return SecularStates.STABLE;
                return y1 > y2 ? SecularStates.FALLING : SecularStates.RISING;
            }
        }

        /// <summary>
        /// Inyecta a la tendencia una nueva muestra de la variable medida.
        /// </summary>
        /// <param name="value">valor instantaneo de la variable medida</param>
        /// <remarks>
        /// Para que las funciones de tiempo puedan dar datos fidedignos, 
        /// este metodo debe ser llamado en el preciso momento que la muestra
        /// es medida.
        /// </remarks>
        public void NewSample(int value)
        {
            if (queue.Count > Size)
            {
                queue.Dequeue();
            }
            queue.Enqueue(new TrendSample(value));
        }

        private void LinearRegresion(out double a, out double b, out TrendSample start, out TrendSample end)
        {
            var n = queue.Count;
            var y = queue.ToArray();
            start = y[0];
            end = y[n-1];
            var txy = new double[n];
            var tx2 = new double[n];
            double zty = 0;
            double ztx = 0;
            double ztxy = 0;
            double ztx2 = 0;
            for (var i = 0; i < n; ++i)
            {
                var x = i + 1;
                ztx += x;
                zty += y[i].Value;
                txy[i] = y[i].Value * x;
                ztxy += txy[i];
                tx2[i] = Math.Pow(x, 2);
                ztx2 += tx2[i];
            }
            var abd = (n*ztx2) - Math.Pow(ztx, 2);
            a = ((ztx2*zty) - (ztx * ztxy))/abd;
            b = ((n * ztxy) - (ztx * zty)) / abd;
        }

        #region Nested type: TrendSample

        private class TrendSample
        {
            public readonly DateTime Timestamp;
            public readonly int Value;

            public TrendSample(int value)
            {
                Timestamp = DateTime.Now;
                Value = value;
            }
        }

        #endregion
    }
}