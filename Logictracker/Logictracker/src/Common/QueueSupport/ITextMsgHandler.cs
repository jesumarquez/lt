namespace Urbetrack.Common.QueueSupport
{
    /// <summary>
    /// Handler for text messages.
    /// </summary>
    public interface ITextMsgHandler
    {
        /// <summary>
        /// Process the givenn message.
        /// </summary>
        /// <param name="msg"></param>
        void DoIt(string label, string body);
    }
}
