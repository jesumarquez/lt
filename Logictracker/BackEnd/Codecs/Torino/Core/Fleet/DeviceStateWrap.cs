#region Usings

using System;
using Urbetrack.Backbone;
using Urbetrack.Utils;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Fleet
{
    public class DeviceStateWrap
    {
        [Serializable]
        public class WrappedProperty<TYPE>
        {
            public DeviceStateProperty DeviceStateProperty;
            private TYPE localValue;
            public  bool Initialized { get; private set; }

            public TYPE LocalValue
            {
                get { return localValue; }
                set
                {
                    DeviceStateProperty.Value = localValue = value;
                    Initialized = true;
                }
            }

            public WrappedProperty(string PropertyName)
            {
                DeviceStateProperty = new DeviceStateProperty
                {
                    Property = PropertyName,
                    PendingCommits = 0
                };
                LocalValue = default(TYPE);
                Initialized = false;
            }
            
        }

        [Serializable]
        public class WrappedCounter
        {
            public DeviceStateCounter DeviceStateCounter;

            public ulong LocalValue
            {
                get
                {
                    return DeviceStateCounter.PartialValue;
                }
                set
                {
                    DeviceStateCounter.PartialValue = value;
                }
            }

            public WrappedCounter(string CounterName)
            {
                DeviceStateCounter = new DeviceStateCounter
                                          {
                                              Counter = CounterName,
                                              PendingCommits = 0
                                          };
            }
        }

        private const int ChangesToCommit = 10;
        private readonly short id;
        private readonly WrappedProperty<DeviceTypes.QTreeStates> qTreeState;
        private readonly WrappedProperty<DeviceTypes.Types> type;
        private readonly WrappedProperty<bool> storageMediaFailure;
        private readonly WrappedProperty<string> transientDeviceNetworkPath;
        private readonly WrappedProperty<DeviceState.States> state;
        private readonly WrappedProperty<GPSPoint> lastLoginGPSPoint;
        private readonly WrappedProperty<GPSPoint> lastKnownGPSPoint;
        private readonly WrappedProperty<DateTime> lastLogin;
        private readonly WrappedProperty<DateTime> lastReceivedTrackingData;
        private readonly WrappedProperty<DateTime> lastReceivedEventData;
        private readonly WrappedProperty<TimeSpan> clockSlice;  
        private readonly WrappedProperty<int> qtreeRevision;
        private readonly WrappedCounter flashCounter;
        private readonly WrappedCounter crapReceivedCounter;

        private readonly WrappedProperty<string> firmwareVersion;
        private readonly WrappedProperty<string> xbeeFirmware;
        private readonly WrappedProperty<bool>   haveDisplay;

        public string FirmwareVersion {
            get { return firmwareVersion.LocalValue; }
            set
            {
                if (String.IsNullOrEmpty(value)) return;
                firmwareVersion.LocalValue = value;
                firmwareVersion.DeviceStateProperty.PendingCommits++;
            }
        }

        public int QTreeRevision
        {
            get { return qtreeRevision.LocalValue; }
            set
            {
                qtreeRevision.LocalValue = value;
                qtreeRevision.DeviceStateProperty.PendingCommits++;
            }
        }

        public string XBeeFirmware
        {
            get { return xbeeFirmware.LocalValue; }
            set
            {
                if (String.IsNullOrEmpty(value)) return;
                xbeeFirmware.LocalValue = value;
                xbeeFirmware.DeviceStateProperty.PendingCommits++;
            }
        }

        public bool HaveDisplay
        {
            get { return haveDisplay.LocalValue; }
            set
            {
                haveDisplay.LocalValue = value;
                haveDisplay.DeviceStateProperty.PendingCommits++;
            }
        }

        public ulong FlashCounter
        {
            get { return flashCounter.LocalValue; }
            set
            {
                if (flashCounter.LocalValue == value) return;
                flashCounter.LocalValue = value; 
                flashCounter.DeviceStateCounter.PendingCommits++;
                Commit();  
            }
        }

        public ulong CrapReceivedCounter
        {
            get { return crapReceivedCounter.LocalValue; }
            set
            {
                if (crapReceivedCounter.LocalValue == value) return;
                crapReceivedCounter.LocalValue = value;
                crapReceivedCounter.DeviceStateCounter.PendingCommits++;
            }
        }

        private int changes_to_commit;
        public void Touch()
        {
            if (changes_to_commit > 0)
            {
                changes_to_commit--;
                return;
            }
            Commit();
        }

        public DeviceStateWrap(short device_id)
        {
            changes_to_commit = ChangesToCommit;
            id = device_id;
            qTreeState = new WrappedProperty<DeviceTypes.QTreeStates>("QTreeState");
            type = new WrappedProperty<DeviceTypes.Types>("Type");
            storageMediaFailure = new WrappedProperty<bool>("HardwareEmergencyMode");
            transientDeviceNetworkPath = new WrappedProperty<string>("TransientDeviceNetworkPath");
            state = new WrappedProperty<DeviceState.States>("ServiceState");
            lastLoginGPSPoint = new WrappedProperty<GPSPoint>("LastLoginGPSPoint");
            lastKnownGPSPoint = new WrappedProperty<GPSPoint>("LastKnownGPSPoint");
            lastReceivedTrackingData = new WrappedProperty<DateTime>("LastReceivedTrackingData");
            lastReceivedEventData = new WrappedProperty<DateTime>("LastReceivedEventData");
            lastLogin = new WrappedProperty<DateTime>("LastLogin");
            clockSlice = new WrappedProperty<TimeSpan>("ClockSlice");
            qtreeRevision = new WrappedProperty<int>("QTreeRevision");
            flashCounter = new WrappedCounter("FlashCounter");
            crapReceivedCounter = new WrappedCounter("CrapReceivedCounter");
            firmwareVersion = new WrappedProperty<string>("FirmwareVersion");
            xbeeFirmware = new WrappedProperty<string>("XBeeFirmware");
            haveDisplay = new WrappedProperty<bool>("HaveDisplay");
        }

        public void Refresh()
        {
            qTreeState.DeviceStateProperty.PendingCommits++;
            type.DeviceStateProperty.PendingCommits++;
            state.DeviceStateProperty.PendingCommits++;
            storageMediaFailure.DeviceStateProperty.PendingCommits++;
            transientDeviceNetworkPath.DeviceStateProperty.PendingCommits++;
            lastLoginGPSPoint.DeviceStateProperty.PendingCommits++;
            lastKnownGPSPoint.DeviceStateProperty.PendingCommits++;
            lastLogin.DeviceStateProperty.PendingCommits++;
            lastReceivedTrackingData.DeviceStateProperty.PendingCommits++;
            lastReceivedEventData.DeviceStateProperty.PendingCommits++;
            clockSlice.DeviceStateProperty.PendingCommits++;
            qtreeRevision.DeviceStateProperty.PendingCommits++;
            flashCounter.DeviceStateCounter.PendingCommits++;
            crapReceivedCounter.DeviceStateCounter.PendingCommits++;
            firmwareVersion.DeviceStateProperty.PendingCommits++;
            xbeeFirmware.DeviceStateProperty.PendingCommits++;
            haveDisplay.DeviceStateProperty.PendingCommits++;
            //STrace.Trace(typeof(DeviceStateWrap).FullName,0, "DS[{0}]: REFRESH", id);
            Commit();
        }

        public void Commit()
        {
            var queue = new DeviceStatePropertiesQueue();
            var counters_queue = new DeviceStateCountersQueue();
            EnqueueProperty(queue, qTreeState);
            EnqueueProperty(queue, type);
            EnqueueProperty(queue, storageMediaFailure);
            EnqueueProperty(queue, transientDeviceNetworkPath);
            EnqueueProperty(queue, state);
            EnqueueProperty(queue, lastLoginGPSPoint);
            EnqueueProperty(queue, lastKnownGPSPoint);
            EnqueueProperty(queue, lastLogin);
            EnqueueProperty(queue, lastReceivedTrackingData);
            EnqueueProperty(queue, lastReceivedEventData);
            EnqueueProperty(queue, clockSlice);
            EnqueueProperty(queue, qtreeRevision);
            EnqueueProperty(queue, firmwareVersion);
            EnqueueProperty(queue, xbeeFirmware);
            EnqueueProperty(queue, haveDisplay);
            SetDeviceStateProperties(queue);
            // contadores
            EnqueueCounter(counters_queue, flashCounter);
            EnqueueCounter(counters_queue, crapReceivedCounter);
            UpdateDeviceStateCounters(counters_queue);
            changes_to_commit = ChangesToCommit;
            //STrace.Trace(typeof(DeviceStateWrap).FullName,0, "DS[{0}]: COMMIT", id);
        }

        private static void EnqueueProperty<FTYPE>(DeviceStatePropertiesQueue queue, WrappedProperty<FTYPE> property)
        {
            if (property.DeviceStateProperty.PendingCommits <= 0 || !property.Initialized) return;
            queue.Enqueue(property.DeviceStateProperty);
        }

        private static void EnqueueCounter(DeviceStateCountersQueue queue, WrappedCounter counter)
        {
            if (counter.DeviceStateCounter.PendingCommits <= 0) return;
            STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("EnqueueCounter: {0} value={1}", counter.DeviceStateCounter.Counter, counter.DeviceStateCounter.PartialValue));
            counter.DeviceStateCounter.Action = DeviceStateCounter.Actions.Increase;
            queue.Enqueue(counter.DeviceStateCounter);
        }
        
        public DeviceTypes.QTreeStates QTreeState
        {
            get { return qTreeState.LocalValue; }
            set
            {
                qTreeState.LocalValue = value;
                qTreeState.DeviceStateProperty.PendingCommits++;
            }
        }

        public DeviceTypes.Types Type
        {
            get { return type.LocalValue; }
            set {
                type.LocalValue = value;
                type.DeviceStateProperty.PendingCommits++;
            }
        }

        public bool StorageMediaFailure
        {
            get { return storageMediaFailure.LocalValue; }
            set
            {
                storageMediaFailure.LocalValue = value;
                storageMediaFailure.DeviceStateProperty.PendingCommits++;
            }
        }

        public string TransientDeviceNetworkPath
        {
            get { return transientDeviceNetworkPath.LocalValue;  }
            set
            {
                transientDeviceNetworkPath.LocalValue = value;
                transientDeviceNetworkPath.DeviceStateProperty.PendingCommits++;
            }
        }

        public DeviceState.States State
        {
            get { return state.LocalValue; }
            set 
            {
                if (value == DeviceState.States.UNLOADED) return;

                // guara del estado shutdown.
                // es un hack pero muy efectivo, de shutdown solo puede 
                // pasar a SESSION_CONNECTED.
                if (state.LocalValue == DeviceState.States.SHUTDOWN &&
                    value != DeviceState.States.CONNECTED && value != DeviceState.States.MAINT) 
                    return;

                if (value == DeviceState.States.ONLINE || value == DeviceState.States.ONNET)
                {
                    // un dispositivo sincronizado nunca esta en falla de hardware.
                    StorageMediaFailure = false;   
                }

                state.LocalValue = value;
                state.DeviceStateProperty.PendingCommits++;
                Commit();
            }
        }

        public DateTime LastLogin
        {
            get { return lastLogin.LocalValue; }
            set
            {
                if (DateTime.MinValue == value) return;
                lastLogin.LocalValue = value;
                lastLogin.DeviceStateProperty.PendingCommits++;
            }
        }

        public TimeSpan ClockSlice
        {
            get { return clockSlice.LocalValue; }
            set
            {
                clockSlice.LocalValue = value;
                clockSlice.DeviceStateProperty.PendingCommits++;
            }
        }

        public DateTime LastReceivedTrackingData
        {
            get { return lastReceivedTrackingData.LocalValue; }
            set
            {
                if (DateTime.MinValue == value) return;
                lastReceivedTrackingData.LocalValue = value;
                lastReceivedTrackingData.DeviceStateProperty.PendingCommits++;
                Touch();
            }
        }

        public DateTime LastReceivedEventData
        {
            get { return lastReceivedEventData.LocalValue; }
            set
            {
                if (DateTime.MinValue == value) return;
                lastReceivedEventData.LocalValue = value;
                lastReceivedEventData.DeviceStateProperty.PendingCommits++;
                Touch();
            }
        }

        public GPSPoint LastLoginGPSPoint
        {
            get { return lastLoginGPSPoint.LocalValue; }
            set
            {
                lastLogin.LocalValue = DateTime.Now;
                lastLogin.DeviceStateProperty.PendingCommits++;
                if (value == null) return;
                lastLoginGPSPoint.LocalValue = value;
                lastLoginGPSPoint.DeviceStateProperty.PendingCommits++;

            }
        }

        public GPSPoint LastKnownGPSPoint
        {
            get { return lastKnownGPSPoint.LocalValue; }
            set
            {
                if (value == null) return;
                lastKnownGPSPoint.LocalValue = value;
                lastKnownGPSPoint.DeviceStateProperty.PendingCommits++;
                Touch();
            }
        }

        #region Incremento asincronico de contadores del dispositivo.

        private delegate bool UpdateDeviceStateCountersHandler(short device_id, DeviceStateCountersQueue counters);

        private class UpdateDeviceStateCountersData
        {
            public UpdateDeviceStateCountersHandler RemoteAsyncDelegate;
            public DeviceStateCountersQueue RemoteCounters;
        }

        private void UpdateDeviceStateCounters(DeviceStateCountersQueue counters)
        {
            lock (state)
            {
                try
                {
                    if (Devices.NetworkSpine.State == SpineClientWrap.States.DISCONNECTED || counters.Count == 0)
                    {
                        // silenciosamente, ignoro el commit pues no tengo spine a la vista.
                        // o no nay nada en la cola de actualizacion.
                        STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("UpdateDeviceStateCounters[DESCARTA]: id={0} SpineState={1} Counters_Count={2}", id, Devices.NetworkSpine.State, counters.Count));
                        return;
                    }
                    STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("UpdateDeviceStateCounters[INICIA]: id={0} Counters_Count={1} QueryState={2}", id, counters.Count, State));
                    var asyncCallback = new AsyncCallback(UpdateDeviceStateCounterAsyncCallbackHandler);
                    // preparo el cliente.
                    var remoteAsyncDelegate =
                        new UpdateDeviceStateCountersHandler(Devices.NetworkSpine.UpdateDeviceStateCounters);
                    var asyncCallBackData = new UpdateDeviceStateCountersData
                    {
                        RemoteCounters = counters,
                        RemoteAsyncDelegate = remoteAsyncDelegate
                    };
                    // lanzamos el evento.
                    remoteAsyncDelegate.BeginInvoke(id, counters, asyncCallback,
                                                    asyncCallBackData);
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(DeviceStateWrap).FullName, e);
                }
            }    
        }

        private void UpdateDeviceStateCounterAsyncCallbackHandler(IAsyncResult ar)
        {
            var asyncCallBackData = (UpdateDeviceStateCountersData)ar.AsyncState;
            try
            {
                if (asyncCallBackData.RemoteAsyncDelegate.EndInvoke(ar))
                {
                    STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("UpdateDeviceStateCounter[CONFIRMA]: id={0} Counters_Count={1}", id, asyncCallBackData.RemoteCounters.Count));
                    foreach (var p in asyncCallBackData.RemoteCounters)
                    {
                        p.PendingCommits = 0;
                        p.PartialValue = 0;
                    }
                }
                else
                {
                    STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("UpdateDeviceStateCounter[FALLA]: id={0} Counters_Count={1} FAIL", id, asyncCallBackData.RemoteCounters.Count));
                }
                // si llega aca, fue un exito.
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(DeviceStateWrap).FullName, e);
            }    
        }

        #endregion
        #region Envio asincronico de las propiedades del dispositivo.

        private delegate bool SetDeviceStatePropertiesHandler(short device_id, DeviceStatePropertiesQueue properties);
        
        private class SetDeviceStatePropertiesData
        {
            public SetDeviceStatePropertiesHandler RemoteAsyncDelegate;
            public DeviceStatePropertiesQueue RemoteProperties;
        }

        private void SetDeviceStateProperties(DeviceStatePropertiesQueue parameters)
        {
            lock (state)
            {
                try
                {
                    if (Devices.NetworkSpine.State == SpineClientWrap.States.DISCONNECTED
                        || parameters.Count == 0)
                    {
                        // silenciosamente, ignoro el commit pues no tengo spine a la vista.
                        // o no nay nada en la cola de actualizacion.
                        STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("SetDeviceStateProperty[DESCARTA]: id={0} SpineState={1} Paramenters_Count={2}", id, Devices.NetworkSpine.State, parameters.Count));
                        return;
                    }
                    STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("SetDeviceStateProperty[INICIA]: id={0} Properties_Count={1} QueryState={2}", id, parameters.Count, State));
                    var asyncCallback = new AsyncCallback(SetDeviceStatePropertyAsyncCallbackHandler);
                    // preparo el cliente.
                    var remoteAsyncDelegate =
                        new SetDeviceStatePropertiesHandler(Devices.NetworkSpine.SetDeviceStateProperties);
                    var asyncCallBackData = new SetDeviceStatePropertiesData
                                                {
                                                    RemoteProperties = parameters,
                                                    RemoteAsyncDelegate = remoteAsyncDelegate
                                                };
                    // lanzamos el evento.
                    remoteAsyncDelegate.BeginInvoke(id, parameters, asyncCallback,
                                                    asyncCallBackData);
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(DeviceStateWrap).FullName, e);
                }
            }
        }

        // Lanzado por .NET Remoting cuando la llamada remota se completa.
        public void SetDeviceStatePropertyAsyncCallbackHandler(IAsyncResult ar)
        {
            var asyncCallBackData = (SetDeviceStatePropertiesData)ar.AsyncState;
            try
            {
                if (asyncCallBackData.RemoteAsyncDelegate.EndInvoke(ar))
                {
                    STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("SetDeviceStateProperty[CONFIRMA]: id={0} Properties_Count={1}", id, asyncCallBackData.RemoteProperties.Count));
                    foreach(var p in asyncCallBackData.RemoteProperties)
                    {
                        p.PendingCommits = 0;
                    }
                } else
                {
                    STrace.Debug(typeof(DeviceStateWrap).FullName, String.Format("SetDeviceStateProperty[FALLA]: id={0} Properties_Count={1} FAIL", id, asyncCallBackData.RemoteProperties.Count));
                }
                // si llega aca, fue un exito.
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(DeviceStateWrap).FullName, e);
            }
        }

        #endregion

        #region Actualizacion asincronica del estado del dispositivo.
#if false
        private delegate DeviceState LoadDeviceStateHandler(short device_id);

        private void AsyncCommitDeviceState()
        {
            try
            {
                if (Devices.NetworkSpine.State == SpineClientWrap.States.DISCONNECTED)
                {
                    // silenciosamente, ignoro el commit pues no tengo spine a la vista.
                    return;
                }
                var asyncCallback = new AsyncCallback(LoadDeviceStateAsyncCallbackHandler);
                // preparo el cliente.
                var remoteAsyncDelegate = new LoadDeviceStateHandler(Devices.NetworkSpine.GetDeviceState);
                // lanzamos el evento.
                remoteAsyncDelegate.BeginInvoke(id, asyncCallback, remoteAsyncDelegate);
            } 
            catch (Exception e)
            {
                SimpleTrace.Exception(typeof(DeviceStateWrap).FullName,e);
            }
        }

        // Lanzado por .NET Remoting cuando la llamada remota se completa.
        public void LoadDeviceStateAsyncCallbackHandler(IAsyncResult ar)
        {
            var asyncCallBackData = (LoadDeviceStateHandler)ar.AsyncState;
            try
            {
                var ds = asyncCallBackData.EndInvoke(ar);
                if (ds == null) return;
                if (!initialized)
                {
                    Type = ds.Type;
                    State = ds.State;
                    QTreeState = ds.QTreeState;
                    LastLoginGPSPoint = ds.LastLoginGPSPoint;
                    LastKnownGPSPoint = ds.LastKnownGPSPoint;
                    initialized = true;
                    return;
                }
                ds.Type = Type;
                ds.State = State;
                ds.QTreeState = QTreeState;
                ds.LastLoginGPSPoint = LastLoginGPSPoint;
                ds.LastKnownGPSPoint = LastKnownGPSPoint;
                // si llega aca, fue un exito.
            }
            catch (Exception ex)
            {
                SimpleTrace.Exception(typeof(DeviceStateWrap).FullName,ex);
            }
        }
#endif
        #endregion

    }
}