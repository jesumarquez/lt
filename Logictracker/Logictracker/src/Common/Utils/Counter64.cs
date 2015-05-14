#region Usings

using System;

#endregion

namespace Logictracker.Utils
{
    [Serializable]
    public class Counter64
    {
        private DateTime partialstart;
        private DateTime today;

        private ulong todayValue;
        public ulong TodayValue
        {
            get
            {
                ManageToday();
                return todayValue;
            }
            private set { todayValue = value; }
        }

        public ulong PartialValue  { get; private set; }
        public ulong HistoricValue  { get; private set; }

        private void ManageToday()
        {
            var now = DateTime.Now;
            if (today.Year == now.Year && today.DayOfYear == now.DayOfYear) return;
            todayValue = 0;
            today = DateTime.Now;
        }

        public void Inc(ulong toadd)
        {
            ManageToday();
            HistoricValue += toadd;
            PartialValue += toadd;
            TodayValue += toadd;
        }

        public void ResetPartial()
        {
            partialstart = DateTime.Now;
            PartialValue = 0;
        }

        public DateTime GetPartialStart()
        {
            return partialstart;
        }

        public DateTime GetToday()
        {
            return today;
        }
        
    }
}