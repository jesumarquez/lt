#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Model.Utils;
using System.Collections.Concurrent;
using Logictracker.Utils;
using NHibernate;

#endregion

namespace Logictracker.Layers
{
    [FrameworkElement(XName = "MessageDispatcher", IsContainer = true)]
    public class ReflectionIMessageDispatcher : FrameworkElement, IDispatcherLayer
    {
        #region IDispatcherLayer

        public BackwardReply Dispatch(IMessage msg)
        {
            try
            {
                if (msg == null)
                {
                    STrace.Debug(GetType().FullName, "Message: null");
                    return BackwardReply.None;
                }

                var msgType = msg.GetType();
                if (msgType.FullName == null)
                {
                    STrace.Debug(GetType().FullName, "Message: null type");
                    return BackwardReply.None;
                }

                var pcInstanceName = STrace.Module + "_" + msg.GetType().Name;
                //Update Performance Counters
             //   PerformanceCounterHelper.Increment(BackendCategory.Instance.CategoryName, BackendCategory.Instance.DispatcherProcess, pcInstanceName);
                PerformanceCounterHelper.Increment(BackendCategory.Instance.CategoryName, BackendCategory.Instance.DispatcherProcess, STrace.Module);

                var sw = Stopwatch.StartNew();

                var handlers = GetHandlers(msgType);
                //STrace.Debug(GetType().FullName, 0, String.Format("Dispatch {0} T={1} Id={2} HCount={3}", msg.Tiempo, msg.GetType().Name, msg.DeviceId, (Handlers == null) ? -1 : Handlers.Count()));
                if ((handlers != null) && (handlers.Count() != 0))
                {
                    using (var session = NHibernateHelper.GetCurrentSession())
                    {

                        foreach (var handler in handlers)
                        {
                            //lock (handler)
                            {
                                object ret = null;
                                try
                                {
                                    ret = handler(msg);
                                }
                                catch (Exception e)
                                {
                                    STrace.Exception(GetType().FullName, e, msg.DeviceId, String.Format("mType:{0} hType:{1} failure on delivering", msgType.Name, handler.GetType().Name));
                                }
                                if (ret == null || (!(ret is HandleResults))) return BackwardReply.Release;
                                var result = (HandleResults)ret;
                                if (result == HandleResults.UnspecifiedFailure)
                                {
                                    Thread.Sleep(100);
                                    return BackwardReply.Release;
                                }
                                if (result == HandleResults.BreakSuccess) break;
                            }
                        }
                        NHibernateHelper.CloseSession();
                    }
               //     PerformanceCounterHelper.IncrementBy(BackendCategory.Instance.CategoryName,
                 //       BackendCategory.Instance.DispatcherProcess, pcInstanceName, sw.ElapsedTicks);
                    PerformanceCounterHelper.IncrementBy(BackendCategory.Instance.CategoryName,
                          BackendCategory.Instance.DispatcherProcess, STrace.Module, sw.ElapsedTicks);
                
                }
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(ReflectionIMessageDispatcher).FullName, e);
            }
            return BackwardReply.None;
        }

        public bool StackBind(ILayer bottom, ILayer top)
        {
            return true;
        }

        public bool ServiceStart()
        {
            return true;
        }

        public bool ServiceStop()
        {
            return true;
        }

        #endregion

        #region Private Members

        private delegate HandleResults MHandler<T>(T msg);

        private readonly ReaderWriterLockSlim _rwls = new ReaderWriterLockSlim();

        /// <summary>
        /// carga todos los handlers para este tipo de IMessage (lazy loading pattern)
        /// </summary>
        private MHandler<IMessage>[] GetHandlers(Type mtype)
        {
            Debug.Assert(mtype != null);
            Debug.Assert(mtype.FullName != null);
            var mtypename = mtype.FullName;

            _rwls.EnterReadLock();
            try
            {
                if (DHandlers.ContainsKey(mtypename)) return DHandlers[mtypename];
            }
            finally
            {
                _rwls.ExitReadLock();
            }

            _rwls.EnterUpgradeableReadLock();
            try
            {
                //repito por si otro thread me gano de mano en instanciar los handlers
                if (DHandlers.ContainsKey(mtypename)) return DHandlers[mtypename];

                _rwls.EnterWriteLock();
                try
                {
                    var pre = Elements();
                    var pre2 = pre
                        .Where(i => i.GenericType != null && i.GenericType == mtype);
                    var handlers = pre2
                        .Where(hh => HandlesThisMessageType(hh.GetType(), mtype))
                        .Select(handler => BuildDelegate(handler, mtype, FeDHandler))
                        .ToArray();

                    DHandlers.AddOrUpdate(mtypename, handlers, (k, v) => v);
                    return handlers;
                }
                finally
                {
                    _rwls.ExitWriteLock();
                }
            }
            finally
            {
                _rwls.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Lamba que llama a la funcion "HandleMessage" para todos los "IMessageHandler" que manejan un tipo de mensaje que es la clave del diccionario
        /// </summary>
        private ConcurrentDictionary<String, MHandler<IMessage>[]> _dhandlers;
        private ConcurrentDictionary<String, MHandler<IMessage>[]> DHandlers { get { return _dhandlers ?? (_dhandlers = new ConcurrentDictionary<String, MHandler<IMessage>[]>()); } }

        /// <summary>
        /// Tabla de correspondencia entre FrameworkElement - MHandler, utilizando como key el atributo xml string x:Key y como value el delegate
        /// </summary>
        private Dictionary<String, MHandler<IMessage>> _feDHandler;
        private Dictionary<String, MHandler<IMessage>> FeDHandler { get { return _feDHandler ?? (_feDHandler = new Dictionary<String, MHandler<IMessage>>()); } }

        private static MHandler<IMessage> BuildDelegate(FrameworkElement handler, Type mtype, Dictionary<String, MHandler<IMessage>> feDHandler)
        {
            if (feDHandler.ContainsKey(handler.StaticKey)) return feDHandler[handler.StaticKey];

            //STrace.Debug(typeof(ReflectionIMessageDispatcher).FullName, "Building LambdaCallingDelegate using reflection: {0} for {1}", handler.GetType().Name, mtype.Name);

            var methodName = ((Func<FrameworkElement, MHandler<IMessage>>)BuildLambda<IMessage>).Method.Name;
            var del = typeof(ReflectionIMessageDispatcher).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(mtype)
                .Invoke(null, new object[] { handler })
                as MHandler<IMessage>;

            feDHandler.Add(handler.StaticKey, del);
            return del;
        }

        private static MHandler<IMessage> BuildLambda<TParam>(FrameworkElement handler)
        {
            var methodinfo = handler.GetType().GetMethod("HandleMessage", new[] { typeof(TParam) });
            var sdi = (MHandler<TParam>)Delegate.CreateDelegate(typeof(MHandler<TParam>), handler, methodinfo);
            return param => sdi((TParam)param);
        }

        /// <summary>
        /// averigua si este dispatcher procesa este tipo de mensaje
        /// </summary>
        /// <param name="htype">handler.GetType().FullName</param>
        /// <param name="mtype">msg.GetType().FullName</param>
        /// <returns></returns>
        private static bool HandlesThisMessageType(Type htype, Type mtype)
        {
            while (htype != null)
            {
                //obtener todos los argumentos genericos para este tipo y las interfaces que implementa
                var genArgs = htype.GetGenericArguments();
                var allArgs = htype.GetInterfaces().SelectMany(i => i.GetGenericArguments()).Union(genArgs);

                if (allArgs.Any(arg => arg.IsAssignableFrom(mtype)))
                {
                    return true;
                }

                htype = htype.BaseType;
            }
            return false;
        }

        #endregion
    }
}
