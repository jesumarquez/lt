#region Usings

using System;
using System.Threading;

#endregion

namespace Logictracker.Utils 
{
    public class SyncQueue<TYPE>
    {
        private readonly TYPE[] queue;
        private readonly Semaphore sema_NotEmpty;
        private readonly Semaphore sema_NotFull;
        private readonly int size;
        private readonly AutoResetEvent syncro;

        private int getFromIndex;
        private int putToIndex;

        public SyncQueue(int Capacity)
        {
            if (Capacity <= 0)
                throw new ArgumentOutOfRangeException("Capacity");

            queue = new TYPE[Capacity];
            sema_NotEmpty = new Semaphore(0, Capacity);
            sema_NotFull = new Semaphore(Capacity, Capacity);
            syncro = new AutoResetEvent(true);
            getFromIndex = 0;
            putToIndex = 0;
            size = Capacity;
            Count = 0;
        }

        public int Count { get; private set; }

        public bool Contains(TYPE item)
        {
            lock (queue)
            {
                if (Count == 0) return false;
                
                syncro.WaitOne();

                for (var i = 0; i < Count; i++)
                {
                    if (!queue[i].Equals(item)) continue;
                    syncro.Set();
                    return true;
                }

                syncro.Set();
                return false;
            }
        }

        /// <summary>
        /// Encola sincronicamente.
        /// </summary>
        /// <param name="item">objeto tipo TYPE a encolarse.</param>
        /// <returns>Retorna verdadero si la cola estaba vacia, si ya tenia elementos retorna falso.</returns>
        public void Enqueue(TYPE item)
        {
            sema_NotFull.WaitOne();
            lock (queue)
            {
                syncro.WaitOne();

                queue[putToIndex++] = item;

                if (putToIndex == size)
                    putToIndex = 0;

                Count++;

                syncro.Set();

            }
            sema_NotEmpty.Release();
        }

        public TYPE Dequeue()
        {
            return Dequeue(0);
        }

        public TYPE WaitOne(int ms_time_out)
        {
            return Dequeue(ms_time_out);
        }
        public TYPE WaitOne()
        {
            return Dequeue(-1);
        }

        public TYPE Dequeue(int ms_time_out)
        {
            TYPE item;

            if (!sema_NotEmpty.WaitOne(ms_time_out)) return default(TYPE);
            
            lock (queue)
            {
                syncro.WaitOne();

                item = queue[getFromIndex++];

                if (getFromIndex == size)
                    getFromIndex = 0;

                Count--;

                syncro.Set();

            }
            sema_NotFull.Release();

            return item;
        }

        public TYPE Peek()
        {
            return Peek(0);
        }

        public TYPE Peek(int time_out)
        {
            TYPE item;
            // si esta vacia la cola sale de inmediato con null o lo que sea.
            if (!sema_NotEmpty.WaitOne(time_out)) return default(TYPE);
            lock (queue)
            {
                syncro.WaitOne();

                syncro.Set();

                item = queue[getFromIndex];

                syncro.Set();
            }
            sema_NotEmpty.Release();

            return item;
        }
    }
}