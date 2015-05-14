namespace Urbetrack.InterQ.Core.Protocol
{
    public enum States
    {
        WAITFOR_VERSION,
        WAITFOR_CHUNK,
        READING_CHUNK,
        FAILURE,
        TERMINATED
    };
}