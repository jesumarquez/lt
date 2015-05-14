namespace Urbetrack.Model
{
    /// <summary>
    /// Define los mecanismos de "editorial" (del ingles Publisher)   
    /// </summary>
    /// <remarks>
    /// patron de diseño Publisher/Subscriber.
    /// </remarks>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Crea una suscripcion para un mensaje concreto.
        /// </summary>
        /// <typeparam name="TYPE">Tipo concreto de mensaje al que suscribe
        /// el suscriptor.</typeparam>
        void Subscribe<TYPE>(IMessageSubscriber<TYPE> messageSubscriber) where TYPE : IMessage;
    }
}