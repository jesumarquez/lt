#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Logictracker.Statistics
{
    [Serializable]
    public class ArithmeticMean 
    {
        private readonly Queue<ulong> values = new Queue<ulong>();

        public ArithmeticMean()
        {
            ItemsLimit = 0;
        }

        public ArithmeticMean(int items_limit)
        {
            ItemsLimit = items_limit;
        }

        public int ItemsLimit { get; private set; }

        public ulong MeanValue
        {
            get
            {
                if (Items == 0) return 0; // evito la division por cero.
                return values.Aggregate((ulong)0,(a, b) => a + b) / (uint) Items;
            }
        }

        public int Items
        {
            get
            {
                return values.Count();
            }
        }

        public void Reset()
        {
            values.Clear();
            Shrink();
        }

        public void NewSample(ulong value)
        {
            if (ItemsLimit > 0)
            {
                while (Items >= (ItemsLimit - 1))
                {
                    values.Dequeue();
                }
            }
            values.Enqueue(value);
        }


        public void Shrink()
        {
            values.TrimExcess();
        }
    }
}