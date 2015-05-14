#region Usings

using System;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Interfaces.Combustible.Helpers;

#endregion

namespace Logictracker.Interfaces.Combustible.Parsers
{
    public class ParserPozoLog
    {
        #region Constants

		private const String Motor = "M";
		private const String Medidor = "V";
		private const String Tanque = "T";

        private const int Tipomsg = 0;
        private const int Id = 1;
        private const int Tipo = 2;
        private const int Alias = 3;

        private const int MFecha = 4;
        private const int MVolumen = 5;
        private const int MCaudal = 6;
        private const int MHsEnMarcha = 7;
        private const int MTemperatura = 8;
        private const int MRpm = 9;
        private const int MEntries = 10;
        private const int MEvents = 11;

        private const int VFecha = 4;
        private const int VVolumen = 5;
        private const int VEvents = 7;

        private const int IdParameter = 2;
        private const int FechaParameter = 4;
        private const int EventsParameter = 5;
        private const int VolumenAguaParameter = 8;
        private const int VolumenBrutoParameter = 6;
        
        #endregion

        #region Private Properties

        private static readonly ParserHelper Parser = new ParserHelper();

        #endregion

        #region Public Methods

		public PozoMessage Parse(String str, DAOFactory daoFactory)
        {
            var pozoMessage = new PozoMessage();

            var msg = str.Split('*');
            var center = msg[0];

            var dataFields = msg[1].Split('@');

            foreach (var parameters in dataFields.Select(d => d.Split('|')))
            {
                if (parameters[Tipomsg].TrimStart('D').Equals("E")) return pozoMessage;

                var type = parameters[Id].Equals("T") ? "T" : parameters[Tipo];

                switch (type)
                {
                    case Motor:
                        {
                            var m = Parser.ProcessMotor(daoFactory,
                                                              center,
                                                              parameters[Id],
                                                              parameters[Alias],
                                                              Convert.ToDouble(parameters[MVolumen]),
                                                              Convert.ToDateTime(parameters[MFecha]),
                                                              Convert.ToDouble(parameters[MCaudal]),
                                                              Convert.ToDouble(parameters[MHsEnMarcha]),
                                                              Convert.ToDouble(parameters[MTemperatura]),
                                                              Convert.ToDouble(parameters[MRpm]),
                                                              parameters[MEntries],
                                                              parameters[MEvents]);

                            if (m != null) pozoMessage.addMovimientoLog(m);

                            break;
                        }
                    case Medidor:
                        {
                            var v = Parser.ProcessMedidor(daoFactory,
                                                                center,
                                                                parameters[Id],
                                                                parameters[Alias],
                                                                Convert.ToDouble(parameters[VVolumen]),
                                                                "Ingreso",
                                                                Convert.ToDateTime(parameters[VFecha]),
                                                                parameters[VEvents]);

                            if (v != null) pozoMessage.addMovimientoLog(v);

                            break;
                        }
                    case Tanque:
                        {
                            var t = Parser.ProcessTanque(daoFactory,
                                                               center,
                                                               parameters[IdParameter],
                                                               parameters[Alias],
                                                               Convert.ToDouble(parameters[VolumenBrutoParameter]),
                                                               Convert.ToDouble(parameters[VolumenAguaParameter]),
                                                               Convert.ToDateTime(parameters[FechaParameter]),
                                                               parameters[EventsParameter]);

                            if (t != null) pozoMessage.addHistoricalVolumeLog(t);

                            break;
                        }
                    default:
                        {
                           STrace.Error(GetType().FullName, String.Format("Invalid formated message detected for a pozo: {0}", str));

                            break;
                        }
                }
            }

            return pozoMessage;
        }

        #endregion
    }
}
