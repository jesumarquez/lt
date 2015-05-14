using System;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Model.EnumTypes;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.ValueObject.Positions;

namespace Logictracker.Interfaces.AssistCargo
{
    /// <summary>
    /// Handler for porcessing fuel message packages.
    /// </summary>
    [FrameworkElement(XName = "AssistCargoEventsHandler", IsContainer = false)]
    public class Handler : BaseHandler<TextEvent>
    {
        #region Protected Methods

        /// <summary>
        /// Process handler main tasks.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override HandleResults OnHandleMessage(TextEvent message)
        {
            var coche = DaoFactory.CocheDAO.FindMobileByDevice(message.DeviceId);
            var posicion = new LogPosicionVo(new LogPosicion
                               {
                                   Coche = coche,
                                   Dispositivo = coche.Dispositivo,                                  
                                   Altitud = message.GeoPoint.Height.Unpack(),
                                   Latitud = message.GeoPoint.Lat,
                                   Longitud = message.GeoPoint.Lon,
                                   Velocidad = message.GeoPoint.Velocidad,
                                   FechaMensaje = message.GeoPoint.Date,
                                   FechaRecepcion = DateTime.UtcNow
                               });
            var eventName = message.Text;

            var service = new Service();
            var result = service.ReportEvent(eventName, coche, posicion);

			STrace.Debug(typeof(Handler).FullName, coche.Dispositivo.Id, String.Format("AssistCargo Event: {0} -> {1}: {2}", coche.Patente, eventName, result));

            return HandleResults.Success;
        }

        #endregion
    }
}
