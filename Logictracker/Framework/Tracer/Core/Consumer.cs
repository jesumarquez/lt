using System;
using System.Threading;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.NHibernateManagers;
using Logictracker.DatabaseTracer.Types;
using NHibernate;

namespace Logictracker.DatabaseTracer.Core
{
    /// <summary>
    /// Tracer asyncronous queue consumer.
    /// </summary>
    internal static class Consumer
    {
        #region Private Properties

        /// <summary>
        /// Consumer main thread.
        /// </summary>
        private static readonly Thread ConsumerThread;

        /// <summary>
        /// Event for syncronizing consumer thread.
        /// </summary>
        private static readonly Semaphore Semaphore;

        /// <summary>
        /// Error interval before trying to re enqueue the log.
        /// </summary>
        private static readonly TimeSpan ErrorInterval = Config.Tracer.TracerErrorInterval;

        /// <summary>
        /// Max log retries to trace it to database.
        /// </summary>
        private static readonly Int32 MaxRetries = Config.Tracer.TracerMaxRetries;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialice tracer consumer enviroment.
        /// </summary>
        static Consumer()
        {
            Semaphore = new Semaphore(0, Int32.MaxValue);

            ConsumerThread = new Thread(Consume);

            ConsumerThread.Start();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setps up the consumer enviroment and triggers the consumption of the logs.
        /// </summary>
        public static void Start() { Semaphore.Release(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Consumes a log from the queue.
        /// </summary>
        private static void Consume()
        {
            while (true)
            {
                try
                {
                    Semaphore.WaitOne();
                    using (var session = NHibernateHelper.GetSession())
                    {
                        Log log;
                        while ((log = Queue.Dequeue()) != null)
                            Tracer(log, session);
                        session.Flush();
                        session.Close();
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch
                {
                    //ignore
                }
            }
        }

        private static void Tracer(Log log, ISession session)
        {
            try
            {
                if (log == null) return;


                using (ITransaction tx = session.BeginTransaction())
                {
                    try
                    {
                        session.Evict(log);
                        log.Id = 0;
                        foreach (var c in log.Context)
                        {
                            session.Evict(c);
                            c.Id = 0;
                            c.Log = log;
                        }
                        session.Save(log);
                        tx.Commit();
                        session.Evict(log);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw ex;
                    }

                }
            }
            catch (ThreadAbortException)
            {
            }
            catch
            {
                ReEnqueue(log);
            }
        }

        /// <summary>
        /// Tries to re enqueue the log.
        /// </summary>
        /// <param name="log"></param>
        private static void ReEnqueue(Log log)
        {
            Thread.Sleep(ErrorInterval);
            log.Retries++;
            if (log.Retries < MaxRetries) Queue.Enqueue(log);
            Semaphore.Release();
        }

        #endregion
    }
}