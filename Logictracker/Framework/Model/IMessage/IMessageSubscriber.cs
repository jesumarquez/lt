namespace Urbetrack.Model
{
    /// <summary>
    /// Define los mecanismos de un suscriptor
    /// </summary>
    /// <typeparam name="TYPE">Tipo concreto de mensaje que suscribe.</typeparam>
    /// <remarks>
    /// patron de diseño Publisher/Subscriber.
    /// </remarks>
    public interface IMessageSubscriber<TYPE> where TYPE : IMessage
    {
        /// <summary>
        /// Receptor del mensaje suscripto.
        /// </summary>
        /// <param name="messagePublisher">"editorial" que publico el mensaje.</param>
        /// <param name="message">mensaje publicado.</param>
        void Publish(IMessagePublisher messagePublisher, TYPE message);
    }
}
