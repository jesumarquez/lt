#region Usings

using System;
using System.Threading;
using OpenNETCF.Threading;

#endregion

namespace Urbetrack.Mobile.Toolkit
{
    public class SyncQueue<TYPE>
    {
        private readonly Semaphore sema_NotEmpty;
        private readonly Semaphore sema_NotFull;
        private readonly AutoResetEvent syncro;
        private readonly TYPE[] queue;

        private int getFromIndex;
        private int putToIndex;
        private readonly int size;

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
        public bool Enqueue(TYPE item)
        {
            sema_NotFull.WaitOne();
            bool was_empty;
            lock (queue)
            {
                syncro.WaitOne();

                queue[putToIndex++] = item;

                if (putToIndex == size)
                    putToIndex = 0;

                was_empty = Count == 0;
                Count++;

                syncro.Set();

            }
            sema_NotEmpty.Release();
            return was_empty;
        }

        public TYPE Dequeue()
        {
            return Dequeue(0);
        }

        public TYPE Dequeue(int hash_code)
        {
            TYPE item;

            sema_NotEmpty.WaitOne();
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
            TYPE item;
            // si esta vacia la cola sale de inmediato con null o lo que sea.
            if (!sema_NotEmpty.WaitOne(0,false)) return default(TYPE);
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