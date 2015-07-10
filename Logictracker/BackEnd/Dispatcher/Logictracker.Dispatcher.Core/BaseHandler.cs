using System;
using System.Collections.Generic;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Runtime;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

namespace Logictracker.Dispatcher.Core
{
    /// <summary>
    /// Dispatcher handler base class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseHandler<T> : FrameworkElement, IMessageHandler<T> where T : IMessage
    {
        #region Private Properties

        /// <summary>
        /// Data access factory.
        /// </summary>
        private DAOFactory _daoFactory;
        private readonly object _lockDaoFactory = new object();

        /// <summary>
        /// Logictracker messages saver.
        /// </summary>
        private IMessageSaver _messageSaver;
        private IM2mMessageSaver _m2MMessageSaver;

        #endregion

        #region Protected Properties

        /// <summary>
        /// Data access factory.
        /// </summary>
        protected DAOFactory DaoFactory { get { lock (_lockDaoFactory) { return _daoFactory ?? (_daoFactory = new DAOFactory());} } }
        
        /// <summary>
        /// Logictracker messages saver.
        /// </summary>
        protected IMessageSaver MessageSaver { get { return _messageSaver ?? (_messageSaver = new MessageSaver(DaoFactory)); } }

        protected IM2mMessageSaver M2MMessageSaver { get { return _m2MMessageSaver ?? (_m2MMessageSaver = new M2mMessageSaver(DaoFactory)); } } 

        #endregion

        private static readonly Dictionary<int, object> LocksByDevice = new Dictionary<int, object>();

        protected static object GetLockByDevice(int device)
        {
            lock (LocksByDevice)
            {
                if (!LocksByDevice.ContainsKey(device)) LocksByDevice.Add(device, new object());
                return LocksByDevice[device];
            }
        }

        #region Public Methods

		/// <summary>
        /// Performs all necesary actions for handling the givenn message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public HandleResults HandleMessage(T message)
		{
		    //Update Performance Counters
		    //var cat = new BackendCategory();
		    //PerformanceCounterHelper.Increment(cat.CategoryName, cat.DispatcherCount, cat.DispatcherProm, STrace.Module + message.GetType().Name);
            STrace.Debug(GetType().FullName, message.DeviceId,
                         "HandleMessage (BaseHandler) dt=" + message.Tiempo + " T=" + message.GetType().Name + " Id=" +
                         message.DeviceId);

		    lock (GetLockByDevice(message.DeviceId))
		    {
		        using (var transaction = SmartTransaction.BeginTransaction())
		        {
		            try
		            {
		                var result = OnHandleMessage(message);
		                transaction.Commit();
		                return result;
		            }
		            catch (Exception e)
		            {
		                STrace.Exception(GetType().FullName, e, message.DeviceId);
		                try
		                {
		                    transaction.Rollback();
		                }
		                catch (Exception ex2)
		                {
		                    STrace.Exception(GetType().FullName, ex2, message.DeviceId, "HandleMessage(T); Doing rollback");
		                }
		                return HandleResults.SilentlyDiscarded;
		            }
		            finally
		            {
		                DisposeResources();
		            }
		        }
		    }
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Steps to execute when handling the current message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected abstract HandleResults OnHandleMessage(T message);

        #endregion

        #region Private Methods

        /// <summary>
        /// Dispose all allocated resources.
        /// </summary>
		private void DisposeResources()
        {
            _messageSaver = null;
            _m2MMessageSaver = null;
        	lock (_lockDaoFactory)
        	{
        	    if (_daoFactory == null) return;
        	    _daoFactory = null;
        	}
        }

    	#endregion
    }
}
