#region Usings

using System;
using System.Runtime.Serialization;
using Urbetrack.Utils;
using Urbetrack.Statistics;

#endregion

namespace Urbetrack.Backbone
{
    public class DeviceStateUpgradeRequired : Exception
    {
        public DeviceStateUpgradeRequired(int version_required, DeviceState ds)
            : base(
                string.Format(
                    "Es imposible deserializar correctamente DeviceState: Id={0} Code={1}. Fue serializado con una version mas nueva (v{2}), Actualize su version a la ultima disponible para volver a operar.",
                    ds.Id, ds.Code, version_required))
        {
        }
    }

    partial class DeviceState : ISerializable
    {
        protected DeviceState(SerializationInfo info, StreamingContext context)
        {
            if (info.MemberCount == 18) // la version sin serialization custom.-
            {
                Version0(info);
                ObjectVersion = 0;
                FixVersion1();
                return;
            }

            var version = ObjectVersion = info.GetInt32("ObjectVersion");
                
            if (version == 1)
            {
                Version1(info);
                FixVersion1();
            }

            if (version == 2)
            {
                Version2(info);
            }

            if (version == 3)
            {
                Version3(info);
                AverageConnectedTime = new ArithmeticMean(32);
            }

            if (version == 4)
            {
                Version4(info);
            }


            if (version == 5)
            {
                Version5(info);
            }

            //STrace.Trace(GetType().FullName,9, "DeviceState[{0}]: Deserializado.", LogId);

            if (version == 6)
            {
                Version6(info);
            }

            if (version > 6)
            {
                Version1(info);
                throw new DeviceStateUpgradeRequired(version, this);
            }
        }

        private void FixVersion1()
        {
            Organization = "";
            FlashCounter = new Gauge64();
            CrapReceivedCounter = new Gauge64();
            StatesChanges = new Gauge64();
            PERMANENTCounter = new Gauge64();
            MAINTCounter = new Gauge64();
            OFFLINECounter = new Gauge64();
            CONNECTEDCounter = new Gauge64();
            ONLINECounter = new Gauge64();
            ONNETCounter = new Gauge64();
            SHUTDOWNCounter = new Gauge64();
            SYNCINGCounter = new Gauge64();
            OUTOFSERVICECounter = new Gauge64();
            //TransitionTrend = new Toolkit.TransitionTrend<ServiceStates>();
            AverageConnectedTime = new ArithmeticMean(32);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ObjectVersion", 6);
            info.AddValue("RequireCommit", requireCommit);
            info.AddValue("FirstActivity", firstActivity);
            info.AddValue("lastKnownGPSPoint", lastKnownGPSPoint, typeof(GPSPoint));
            info.AddValue("LastLogin", LastLogin);
            info.AddValue("LastCommit", LastCommit);
            info.AddValue("LastReceivedTrackingData", LastReceivedTrackingData);
            info.AddValue("LastReceivedEventData", LastReceivedEventData);
            info.AddValue("LastLoginGPSPoint", LastLoginGPSPoint, typeof(GPSPoint));
            info.AddValue("AutomaticUpdateQtree", AutomaticUpdateQtree);
            info.AddValue("AutomaticUpdateFirmware", AutomaticUpdateFirmware);
            info.AddValue("XBeeEnabled", XBeeEnabled);
            info.AddValue("Id", Id);
            info.AddValue("FirmwareVersion", FirmwareVersion);
            info.AddValue("HardwareEmergencyMode", HardwareEmergencyMode);
            info.AddValue("QTreeRevision", QTreeRevision);
            info.AddValue("Code", Code);
            info.AddValue("DeviceIdentifier", DeviceIdentifier);
            info.AddValue("ServiceState", ServiceState);
            info.AddValue("AdminState", AdminState);
            info.AddValue("Type", Type);
            info.AddValue("QTreeState", QTreeState);
            info.AddValue("LifeTime", LifeTime);

            // informacion administrativa
            info.AddValue("Vehicle", Vehicle);
            info.AddValue("VehicleType", VehicleType);
            info.AddValue("Organization", Organization);
            info.AddValue("OrganizationBase", OrganizationBase);
            info.AddValue("OrganizationUnit", OrganizationUnit);
            info.AddValue("Carrier", Carrier);

            // caracteristicas del HW
            info.AddValue("XBeeFirmware", XBeeFirmware);
            info.AddValue("HaveDisplay", HaveDisplay);

            // desplazamiento de reloj
            info.AddValue("ClockSlice", ClockSlice, typeof(TimeSpan));
            
            // contadores
            info.AddValue("FlashGauge",         FlashCounter);
            info.AddValue("CrapReceivedGauge",  CrapReceivedCounter);
            info.AddValue("StatesChangesGauge", StatesChanges);
            info.AddValue("PERMANENTGauge",     PERMANENTCounter);
            info.AddValue("MAINTGauge",         MAINTCounter);
            info.AddValue("OFFLINEGauge",       OFFLINECounter);
            info.AddValue("CONNECTEDGauge",     CONNECTEDCounter);
            info.AddValue("ONLINEGauge",        ONLINECounter);
            info.AddValue("ONNETGauge",         ONNETCounter);
            info.AddValue("SHUTDOWNGauge",      SHUTDOWNCounter);
            info.AddValue("SYNCINGGauge",       SYNCINGCounter);
            info.AddValue("OUTOFSERVICECounter", OUTOFSERVICECounter);
            info.AddValue("AverageConnectedTime",AverageConnectedTime);
            info.AddValue("TransientDeviceNetworkPath", TransientDeviceNetworkPath);
            
        }

        private void Version0(SerializationInfo info)
        {
            requireCommit = info.GetBoolean("requireCommit");
            firstActivity = info.GetDateTime("firstActivity");
            lastKnownGPSPoint = info.GetValue("lastKnownGPSPoint",typeof(GPSPoint)) as GPSPoint;
            LastLogin = info.GetDateTime("<LastLogin>k__BackingField");
            // este campo fue renombrado.
            LastCommit = info.GetDateTime("<LastActivity>k__BackingField");
            LastLoginGPSPoint = info.GetValue("<LastLoginGPSPoint>k__BackingField",typeof(GPSPoint)) as GPSPoint;
            AutomaticUpdateQtree = info.GetBoolean("<AutomaticUpdateQtree>k__BackingField");
            AutomaticUpdateFirmware = info.GetBoolean("<AutomaticUpdateFirmware>k__BackingField");
            XBeeEnabled = info.GetBoolean("<XBeeEnabled>k__BackingField");
            Id = info.GetInt16("<Id>k__BackingField");
            FirmwareVersion = info.GetString("<FirmwareVersion>k__BackingField");
            HardwareEmergencyMode = info.GetBoolean("<HardwareEmergencyMode>k__BackingField");
            QTreeRevision = info.GetInt32("<QTreeRevision>k__BackingField");
            Code = info.GetString("<Code>k__BackingField");
            DeviceIdentifier = info.GetString("<DeviceIdentifier>k__BackingField");
            serviceState = (States) info.GetValue("_state",typeof(States));
            Type = (DeviceTypes.Types) info.GetValue("<Type>k__BackingField",typeof(DeviceTypes.Types));
            QTreeState = (DeviceTypes.QTreeStates) info.GetValue("<QTreeState>k__BackingField",typeof(DeviceTypes.QTreeStates));
        }

        private void Version1(SerializationInfo info)
        {
            // en esta version se cambiaron los nombres a un texto mas simple.
            requireCommit = info.GetBoolean("RequireCommit");
            firstActivity = info.GetDateTime("FirstActivity");
            lastKnownGPSPoint = info.GetValue("lastKnownGPSPoint", typeof(GPSPoint)) as GPSPoint;
            LastLogin = info.GetDateTime("LastLogin");
            LastCommit = info.GetDateTime("LastCommit");
            LastReceivedTrackingData = info.GetDateTime("LastReceivedTrackingData");
            LastReceivedEventData = info.GetDateTime("LastReceivedEventData");
            LastLoginGPSPoint = info.GetValue("LastLoginGPSPoint", typeof(GPSPoint)) as GPSPoint;
            AutomaticUpdateQtree = info.GetBoolean("AutomaticUpdateQtree");
            AutomaticUpdateFirmware = info.GetBoolean("AutomaticUpdateFirmware");
            XBeeEnabled = info.GetBoolean("XBeeEnabled");
            Id = info.GetInt16("Id");
            FirmwareVersion = info.GetString("FirmwareVersion");
            HardwareEmergencyMode = info.GetBoolean("HardwareEmergencyMode");
            QTreeRevision = info.GetInt32("QTreeRevision");
            Code = info.GetString("Code");
            DeviceIdentifier = info.GetString("DeviceIdentifier");
            serviceState = (States)info.GetValue((ObjectVersion < 6 ? "State" : "ServiceState"), typeof(States));
            Type = (DeviceTypes.Types)info.GetValue("Type", typeof(DeviceTypes.Types));
            QTreeState = (DeviceTypes.QTreeStates)info.GetValue("QTreeState", typeof(DeviceTypes.QTreeStates));
            // campos agregados en v1.
            Vehicle = info.GetString("Vehicle");
            OrganizationBase = info.GetString((ObjectVersion < 6 ? "Base" : "OrganizationBase"));
            XBeeFirmware = info.GetString("XBeeFirmware");
            HaveDisplay = info.GetBoolean("HaveDisplay");
            ClockSlice = (TimeSpan) info.GetValue("ClockSlice", typeof(TimeSpan));
        }

        private void Version2(SerializationInfo info)
        {   
            Version1(info);
            Organization = info.GetString((ObjectVersion < 6 ? "District" : "Organization"));
            FlashCounter = new Gauge64((Counter64)info.GetValue("FlashCounter", typeof(Counter64)));
            CrapReceivedCounter = new Gauge64((Counter64)info.GetValue("CrapReceivedCounter", typeof(Counter64)));
            StatesChanges = new Gauge64((Counter64) info.GetValue("StatesChanges", typeof(Counter64)));
            PERMANENTCounter = new Gauge64((Counter64)info.GetValue("PERMANENTCounter", typeof(Counter64)));
            MAINTCounter = new Gauge64((Counter64)info.GetValue("MAINTCounter", typeof(Counter64)));
            OFFLINECounter = new Gauge64((Counter64)info.GetValue("OFFLINECounter", typeof(Counter64)));
            CONNECTEDCounter = new Gauge64((Counter64)info.GetValue("CONNECTEDCounter", typeof(Counter64)));
            ONLINECounter = new Gauge64((Counter64)info.GetValue("ONLINECounter", typeof(Counter64)));
            ONNETCounter = new Gauge64((Counter64)info.GetValue("ONNETCounter", typeof(Counter64)));
            SHUTDOWNCounter = new Gauge64((Counter64)info.GetValue("SHUTDOWNCounter", typeof(Counter64)));
            SYNCINGCounter = new Gauge64((Counter64)info.GetValue("SYNCINGCounter", typeof(Counter64)));
            OUTOFSERVICECounter = new Gauge64((Counter64)info.GetValue("STOCKCounter", typeof(Counter64)));
            //TransitionTrend = (Toolkit.TransitionTrend<ServiceStates>)info.GetValue("TransitionTrend", typeof(Toolkit.TransitionTrend<ServiceStates>));
            AverageConnectedTime = new ArithmeticMean(32);
        }

        private void Version3(SerializationInfo info)
        {
            Version1(info);
            Organization = info.GetString((ObjectVersion < 6 ? "District" : "Organization"));
            FlashCounter = (Gauge64)info.GetValue("FlashGauge", typeof(Gauge64));
            CrapReceivedCounter = (Gauge64)info.GetValue("CrapReceivedGauge", typeof(Gauge64));
            StatesChanges = (Gauge64)info.GetValue("StatesChangesGauge", typeof(Gauge64));
            PERMANENTCounter = (Gauge64)info.GetValue("PERMANENTGauge", typeof(Gauge64));
            MAINTCounter = (Gauge64)info.GetValue("MAINTGauge", typeof(Gauge64));
            OFFLINECounter = (Gauge64)info.GetValue("OFFLINEGauge", typeof(Gauge64));
            CONNECTEDCounter = (Gauge64)info.GetValue("CONNECTEDGauge", typeof(Gauge64));
            ONLINECounter = (Gauge64)info.GetValue("ONLINEGauge", typeof(Gauge64));
            ONNETCounter = (Gauge64)info.GetValue("ONNETGauge", typeof(Gauge64));
            SHUTDOWNCounter = (Gauge64)info.GetValue("SHUTDOWNGauge", typeof(Gauge64));
            SYNCINGCounter = (Gauge64)info.GetValue("SYNCINGGauge", typeof(Gauge64));
            OUTOFSERVICECounter = (Gauge64)info.GetValue(ObjectVersion < 6 ? "STOCKGauge" : "OUTOFSERVICECounter", typeof(Gauge64));
            //var x = (Urbetrack.Toolkit.TransitionTrend<ServiceStates>)info.GetValue("TransitionTrend", typeof(Urbetrack.Toolkit.TransitionTrend<ServiceStates>));
            //TransitionTrend = 
        }

        private void Version4(SerializationInfo info)
        {
            Version3(info);
            AverageConnectedTime = (ArithmeticMean)info.GetValue("AverageConnectedTime", typeof(ArithmeticMean));
        }

        private void Version5(SerializationInfo info)
        {
            Version4(info);
            TransientDeviceNetworkPath = info.GetString("TransientDeviceNetworkPath");
        }

        private void Version6(SerializationInfo info)
        {
            Version5(info);
            adminState = (AdminStates)info.GetValue("AdminState", typeof(AdminStates));
            VehicleType = info.GetString("VehicleType");
            OrganizationUnit = info.GetString("OrganizationUnit");
            Carrier = info.GetString("Carrier");
            LifeTime = info.GetInt32("LifeTime");
        }

        /// <summary>
        /// Reduce el tamaño de las listas a la minima expresion posible,
        /// para asi optimizar la memoria, las transferncias por red y el
        /// almacenamiento en disco del estado de dispositivo.
        /// Este metodo es convocado automaticamente por la infraestructura 
        /// del spine.
        /// </summary>
        public void Shrink()
        {
            FlashCounter.Shrink();
            CrapReceivedCounter.Shrink();
            StatesChanges.Shrink();
            PERMANENTCounter.Shrink();
            MAINTCounter.Shrink();
            OFFLINECounter.Shrink();
            CONNECTEDCounter.Shrink();
            ONLINECounter.Shrink();
            ONNETCounter.Shrink();
            SHUTDOWNCounter.Shrink();
            SYNCINGCounter.Shrink();
            OUTOFSERVICECounter.Shrink();
            //TransitionTrend.Shrink();
            AverageConnectedTime.Shrink();
        }
    }
}
