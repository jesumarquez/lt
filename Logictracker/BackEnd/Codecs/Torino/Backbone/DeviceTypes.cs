namespace Urbetrack.Backbone
{
    public class DeviceTypes
    {
        public enum Types
        {
            UNKNOW_DEVICE,
            UNETEL_v1,
            UNETEL_v2,
            SISTELCOM_v1,
            SISTELCOM_v2,
            URBMOBILE_v0_1,
            URB_v0_5,            
            URB_v0_7, 
            URBETRACK_v0_8n,
            URBETRACK_v0_8,
            URBETRACK_v1_0,
            GTE_TRAX_S6SG_v1_0,
            NGN_DEVICE
        }

        public enum States
        {
            UNLOADED,
            OFFLINE,
            ONLINE,
            ONNET
        }

        public enum QTreeStates
        {
            UNKNOWN,
            REQUIRES_UPDATE,
            TRANSFERING,
            UPTODATE
        };
    }
}
