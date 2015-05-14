#region Usings

using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

#endregion

namespace Logictracker.Dispatcher.Handlers
{
    /// <summary>
    /// Handler for hacking the torino devices reported positons to solve the detected datetime conversion issue.
    /// </summary>
    [FrameworkElement(XName = "SaiDatetimeHack", IsContainer = false)]
    public class SaiDateTimeHack : DeviceBaseHandler<IMessage>
    {
        #region Protected Methods

        /// <summary>
        /// Arrange all the datetimes of the current message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override HandleResults OnDeviceHandleMessage(IMessage message)
        {
            /*if ((message as Position) != null) ArrangePositionDate(message as Position);
            if ((message as Event) != null) ArrangeEventDate(message as Event);
            if ((message as SpeedingTicket) != null) ArrangeSpeedingTicketDate(message as SpeedingTicket);//*/

            return HandleResults.Success;
        }

        #endregion

		/*#region Private Methods

        /// <summary>
        /// Arrange all speeding ticket positions date.
        /// </summary>
        /// <param name="message"></param>
        private static void ArrangeSpeedingTicketDate(ITicket message)
        {
            if (message.TicketPoint != null) message.TicketPoint.Date = message.TicketPoint.Date.AddDays(-2);
            if (message.StartPoint != null) message.StartPoint.Date = message.StartPoint.Date.AddDays(-2);
            if (message.EndPoint != null) message.EndPoint.Date = message.EndPoint.Date.AddDays(-2);
        }

        /// <summary>
        /// Arranges the date of the position of the current event.
        /// </summary>
        /// <param name="message"></param>
        private static void ArrangeEventDate(IGeoPoint message) { if (message.GeoPoint != null) message.GeoPoint.Date = message.GeoPoint.Date.AddDays(-2); }

        /// <summary>
        /// Arrage the date for each position of the current package.
        /// </summary>
        /// <param name="message"></param>
        private static void ArrangePositionDate(IGeoMultiPoint message) { foreach (var position in message.GeoPoints) position.Date = position.Date.AddDays(-2); }

        #endregion//*/
    }
}
