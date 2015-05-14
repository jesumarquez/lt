#region Usings

using System;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Interfaces.Combustible.Helpers;
using Logictracker.Interfaces.Combustible.Parsers;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

#endregion

namespace Logictracker.Interfaces.Combustible
{
    /// <summary>
    /// Handler for porcessing fuel message packages.
    /// </summary>
    [FrameworkElement(XName = "FuelEventsHandler", IsContainer = false)]
    public class Handler : BaseHandler<UserMessage>
    {
        #region Constants

        /// <summary>
        /// Event identifiers.
        /// </summary>
		private const String NivelCode = "N";
		private const String MovimientoCode = "M";
		private const String PozoCode = "P";

        #endregion

        #region Private Properties

        /// <summary>
        /// Pozo parser.
        /// </summary>
        private static readonly ParserPozo Parser = new ParserPozo();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Process handler main tasks.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override HandleResults OnHandleMessage(UserMessage message)
        {
			var label = message.GetUserSetting("label");
            var body = message.GetUserSetting("body");

            var headerParameters = label.Split(';');

            var type = headerParameters[1];

            switch (type)
            {
                case MovimientoCode: ParseAndSaveMovimiento(body); break;
                case NivelCode: ParseAndSaveNivel(body); break;
                case PozoCode: ParseAndSavePozo(headerParameters[0], ParserHelper.ParseDate(headerParameters[2]), body); break;
                default: throw new Exception("Mensaje con formato incorrecto");
            }

            return HandleResults.Success;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses and Saves a VolumenHistorico Object.
        /// </summary>
        /// <param name="body"></param>
		private void ParseAndSaveNivel(String body)
        {
            var parser = new ParserNivel();

            parser.Parse(body, DaoFactory);
        }

        /// <summary>
        /// Parses and Saves a Movimiento Object.
        /// </summary>
        /// <param name="body"></param>
		private void ParseAndSaveMovimiento(String body)
        {
            var parser = new ParserMovimiento();

            parser.Parse(body, DaoFactory);
        }

        /// <summary>
        /// Parses And Saves the Movimientos and Volumes that came in a PozoMessage.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="date"></param>
        /// <param name="body"></param>
		private void ParseAndSavePozo(String center, DateTime date, String body)
        {
            using (var pozoMesages = Parser.Parse(center + "@" + body, DaoFactory))
            {
                foreach (var m in pozoMesages.getMovimientos())
                {
                    m.Fecha = m.FechaIngresoABase = date;

                    DaoFactory.MovimientoDAO.SaveOrUpdate(m);
                }

                foreach (var v in pozoMesages.getHistoricalVolumes())
                {
                    v.Fecha = date;

                    DaoFactory.VolumenHistoricoDAO.SaveOrUpdate(v);
                }

                foreach (var m in pozoMesages.getMovimientosLog()) DaoFactory.MovimientoDAO.SaveOrUpdate(m);

                foreach (var v in pozoMesages.getHistoricalVolumesLog()) DaoFactory.VolumenHistoricoDAO.SaveOrUpdate(v);
            }
        }

        #endregion
    }
}