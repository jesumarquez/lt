namespace Urbetrack.Model
{
    public interface ICanQueueOutgoingMessages
    {
        ///<summary>
        ///</summary>
        void CancelAllMessages();

        ///<summary>
        ///</summary>
        ///<param name="MessageId"></param>
        void CancelMessage(int MessageId);
    }
}