#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Logictracker.Utils;

#endregion

namespace Logictracker.Statistics
{
    [Serializable]
    public class Gauge64
    {
        private readonly List<OneHour> last24hsMatrix = new List<OneHour>();

        [NonSerialized]
        private DateTime _time_hack;
        [NonSerialized]
        private bool _time_hack_enabled;

        private OneHour currentHour;
        private DateTime partialstart;
        private DateTime today;

        public Gauge64()
        {
            _time_hack_enabled = false;
        }

        public Gauge64(Counter64 src)
        {
            today = src.GetToday();
            partialstart = src.GetPartialStart();
            _time_hack_enabled = false;
        }

        public DateTime Now
        {
            get
            {
                return !_time_hack_enabled ? DateTime.Now : _time_hack;
            }
        }

        public ulong Last24HoursValue
        { 
            get
            {
                var sum = last24hsMatrix
					.Select(oh => new {oh, hs_diff = Now - oh.Span})
					.Where(@t => @t.hs_diff.TotalHours < 24)
					.Select(@t => @t.oh)
					.Aggregate<OneHour, ulong>(0, (current, oh) => oh.Value + current);
            	PurgeLast24();
                return sum;
            }
        }

        [Conditional("DEBUG")]
        public void HackedNow(DateTime now)
        {
            _time_hack = now;
            _time_hack_enabled = true;
        }

        private void UpdateAll()
        {
            PurgeLast24();
            if (today.Year == Now.Year && today.DayOfYear == Now.DayOfYear) return;
            today = Now;
        }

        public string ListLast24()
        {
        	return last24hsMatrix.Aggregate("- begin -\n", (current, oh) => current + (oh.Span + " => " + oh.Value + "\n"));
        }

        private void PurgeLast24()
        {
            // limpiamos los periodos viejos.
            var toDelete = (from oh in last24hsMatrix where oh != null let diff = Now - oh.Span where diff.TotalHours >= 24.0 select oh).ToList();

	        foreach (var oh in toDelete)
            {
                last24hsMatrix.Remove(oh);
            }

            // agregamos lo nuevo.
            if (currentHour == null)
            {
                currentHour = new OneHour(this);
            }
            else
            {
                // si cambia el año, el dia del año, o la hora del dia,
                // tons.. persisto el dato y reincio el contador.
                if (currentHour.Span.Year != Now.Year || currentHour.Span.DayOfYear != Now.DayOfYear || currentHour.Span.Hour != Now.Hour)
                {
                    last24hsMatrix.Add(currentHour);
                    currentHour = new OneHour(this);
                }
            }
        }

        public void Inc(ulong toadd)
        {
            // actualizo las estructuras internas de datos transientes.
            UpdateAll();
            // incremento los contadores
            currentHour.Value += toadd;
	        UpdateAll();
        }

        public void ResetPartial()
        {
            partialstart = Now;
        }

        public DateTime GetPartialStart()
        {
            return partialstart;
        }

        public void Shrink()
        {
            last24hsMatrix.TrimExcess();
        }

        public void Reset()
        {
            ResetPartial();
            last24hsMatrix.Clear();
            last24hsMatrix.TrimExcess();
            currentHour = new OneHour(this);
        }

        #region Nested type: OneHour

        [Serializable]
        private class OneHour
        {
            public readonly DateTime Span;
            public ulong Value;

            public OneHour(Gauge64 g)
            {
                // seteo el segundo cero de la hora actual como Span.
                Span = g.Now.Date.AddHours(g.Now.Hour);
                Value = 0; // lo explico?
            }
        }

        #endregion
    }
}