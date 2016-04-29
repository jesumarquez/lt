using System;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Process.Geofences;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;

namespace Logictracker.Dispatcher.Handlers
{
    [FrameworkElement(XName = "TicketsHandler", IsContainer = false)]
    public class SpeedTickets : DeviceBaseHandler<SpeedingTicket>
    {
        #region Protected Methods

        /// <summary>
        /// Process speeding ticket messages events.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override HandleResults OnDeviceHandleMessage(SpeedingTicket message) { return Coche == null ? HandleResults.SilentlyDiscarded : ProcessVelocidadExcedidaEvent(message); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Process the givenn message as a VelocidadExcedida event.
        /// </summary>
        /// <param name="velocidadExcedida">A message.</param>
        private HandleResults ProcessVelocidadExcedidaEvent(SpeedingTicket velocidadExcedida)
        {
            var inicio = velocidadExcedida.StartPoint ?? velocidadExcedida.TicketPoint;

            if (IsInvalidVelocidadExcedidaMessage(inicio, velocidadExcedida)) return HandleResults.BreakSuccess;

            var texto = GetVelocidadExcedidaText(velocidadExcedida);
            var chofer = GetChofer(velocidadExcedida.GetRiderId());
            var velocidadPermitida = Convert.ToInt32(velocidadExcedida.SpeedLimit);
            var velocidadAlcanzada = Convert.ToInt32(velocidadExcedida.SpeedReached);
            var estado = GeocercaManager.CalcularEstadoVehiculo(Coche, inicio, DaoFactory);
            var zona = estado != null && estado.ZonaManejo != null && estado.ZonaManejo.ZonaManejo > 0
                    ? DaoFactory.ZonaDAO.FindById(estado.ZonaManejo.ZonaManejo) : null;

            if (Coche.Empresa.AsignoInfraccionPorAgenda)
            {
                var reserva = DaoFactory.AgendaVehicularDAO.FindByVehicleAndDate(Coche.Id, inicio.Date);
                if (reserva != null)
                    chofer = reserva.Empleado;
            }

            var evento = MessageSaver.Save(velocidadExcedida, MessageCode.SpeedingTicket.GetMessageCode(), Dispositivo, Coche, chofer, inicio.Date, inicio, velocidadExcedida.EndPoint, texto, velocidadPermitida, velocidadAlcanzada, null, zona);

            var infraccion = new Infraccion
                                 {
                                     Vehiculo = Coche,
                                     Alcanzado = velocidadAlcanzada,
                                     CodigoInfraccion = Infraccion.Codigos.ExcesoVelocidad,
                                     Empleado = evento.Chofer,
                                     Fecha = inicio.Date,
                                     Latitud = inicio.Lat,
                                     Longitud = inicio.Lon,
                                     FechaFin = velocidadExcedida.EndPoint.Date,                                   
                                     LatitudFin = velocidadExcedida.EndPoint.Lat,
                                     LongitudFin = velocidadExcedida.EndPoint.Lon,
                                     Permitido = velocidadPermitida,
                                     Zona = zona,
                                     FechaAlta = DateTime.UtcNow
                                 };

            DaoFactory.InfraccionDAO.Save(infraccion);

            return HandleResults.Success;
        }

        /// <summary>
        /// Determines if the givenn speed message has valid values.
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="velocidadExcedida"></param>
        /// <returns></returns>
        private Boolean IsInvalidVelocidadExcedidaMessage(GPSPoint inicio, SpeedingTicket velocidadExcedida)
        {
            var discardReason = DiscardReason.None;

            if (velocidadExcedida.EndPoint == null || inicio == null) discardReason = DiscardReason.MissingPositions;
			else if (FechaInvalida(velocidadExcedida.EndPoint.Date, DeviceParameters)) discardReason = DiscardReason.InvalidDate;
			else if (FechaInvalida(inicio.Date, DeviceParameters)) discardReason = DiscardReason.InvalidDate;
            else if (FueraDelGlobo(inicio)) discardReason = DiscardReason.OutOfGlobe;
            else if (FueraDelGlobo(velocidadExcedida.EndPoint)) discardReason = DiscardReason.OutOfGlobe;
            else if (VelocidadInvalida(velocidadExcedida)) discardReason = DiscardReason.InvalidSpeed;
            else if (InvalidDuration(velocidadExcedida, inicio)) discardReason = DiscardReason.InvalidDuration;
			else if (InvalidTicketData(velocidadExcedida, inicio)) discardReason = DiscardReason.InvalidTicketData;

            if (discardReason.Equals(DiscardReason.None)) return false;

            MessageSaver.Discard(MessageCode.SpeedingTicket.GetMessageCode(), Dispositivo, Coche, GetChofer(velocidadExcedida.GetRiderId()), inicio.GetDate(), inicio, velocidadExcedida.EndPoint, discardReason);

            return true;
        }

		private Boolean InvalidTicketData(SpeedingTicket velocidadExcedida, GPSPoint inicio)
        {
			if ((velocidadExcedida.SpeedReached - velocidadExcedida.SpeedLimit) > DeviceParameters.DeltaSpeedTicketSpeedOverridesMinimumSpeedTicketDuration)
				return false;
			return velocidadExcedida.EndPoint.Date.Subtract(inicio.Date).TotalSeconds <= (2 * DeviceParameters.MinimumSpeedTicketDuration);
        }

        /// <summary>
        /// Determines if the current ticket has a valid duration or not.
        /// </summary>
        /// <param name="velocidadExcedida"></param>
        /// <param name="inicio"></param>
        /// <returns></returns>
        private Boolean InvalidDuration(ITicket velocidadExcedida, GPSPoint inicio)
        {
            if (velocidadExcedida.EndPoint.Date.Subtract(inicio.Date).TotalSeconds < 10)
                return true;

            return velocidadExcedida.EndPoint.Date.Subtract(inicio.Date).TotalSeconds <= DeviceParameters.MinimumSpeedTicketDuration;
        }

        /// <summary>
        /// Determines if the givenn message has valid speeds.
        /// </summary>
        /// <param name="velocidadExcedida"></param>
        /// <returns></returns>
        private bool VelocidadInvalida(SpeedingTicket velocidadExcedida)
        {
            var maximaVelocidadAlcanzable = Coche.TipoCoche.MaximaVelocidadAlcanzable;

            var velocidadMaximaInvalida = velocidadExcedida.SpeedLimit > maximaVelocidadAlcanzable || velocidadExcedida.SpeedLimit < 0;

            var velocidadAlcanzadaInvalida = velocidadExcedida.SpeedReached > maximaVelocidadAlcanzable || velocidadExcedida.SpeedReached < 0;

            return velocidadMaximaInvalida || velocidadAlcanzadaInvalida;
        }

        /// <summary>
        /// Gets a message that indicates allowed speed, reached speed and duration of the event.
        /// </summary>
        /// <param name="velocidadExcedida"></param>
        /// <returns></returns>
		private static String GetVelocidadExcedidaText(SpeedingTicket velocidadExcedida)
        {
            var inicio = velocidadExcedida.StartPoint ?? velocidadExcedida.TicketPoint;
            var duracion = velocidadExcedida.EndPoint.Date.Subtract(inicio.Date);

			return String.Format(": Permitida {0} - Alcanzada {1} - Duracion: {2}", Convert.ToInt32(velocidadExcedida.SpeedLimit), Convert.ToInt32(velocidadExcedida.SpeedReached), duracion);
        }

        #endregion
    }
}
