#region Usings

using System;
using Logictracker.DAL.Factories;
using Logictracker.Interfaces.Combustible.Helpers;

#endregion

namespace Logictracker.Interfaces.Combustible.Parsers
{
    public class ParserPozoEstado
    {
        #region Constants

        #region Type Codes

		private const String Motor = "M";
		private const String Medidor = "V";
		private const String Tanque = "T";

        #endregion

        #region Protocol Indexes

        private const int ID = 0;
        private const int TIPO = 1;
        private const int ALIAS_O_FIRM = 2;
        private const int FIRM = 3;
        private const int DATA = 4;

        private const int T_EVENTS = 0;
        private const int T_VOLUMEN_BRUTO = 1;
        private const int T_VOLUMEN_AGUA = 3;

        private const int V_VOLUMEN_NETO = 0;
        private const int V_EVENTS = 2;

        private const int M_VOLUMEN = 0;
        private const int M_CAUDAL = 2;
        private const int M_HS_EN_MARCHA = 3;
        private const int M_TEMP = 4;
        private const int M_RPM = 5;
        private const int M_ENTRIES = 6;
        private const int M_EVENTS = 7;
        #endregion

        #region Firmware Versions

		private const String V1_0_0_2 = "1.0.0.2";
		private const String V1_0_0_3 = "1.0.0.3";
		private const String V1_0 = "1.0";

        #endregion

        #endregion

        #region Private Properties

        private static readonly ParserHelper Parser = new ParserHelper();

        #endregion

        #region Public Methods

		public static PozoMessage Parse(String str, DAOFactory daoFactory)
        {
            var pozoMessage = new PozoMessage();

            var msg = str.Split('*');
            var center = msg[0];

            var dataFields = msg[1].Split('@');

            foreach (var d in dataFields)
            {
                var parameters = d.Split('|');
                var id = parameters[ID].TrimStart('D');
                var type = parameters[ID].Equals("T") ? "T" : parameters[TIPO];
                var dataIndex = (parameters[ALIAS_O_FIRM].Equals(V1_0_0_2) || parameters[ALIAS_O_FIRM].Equals(V1_0_0_3) || parameters[ALIAS_O_FIRM].Equals(V1_0)) ? DATA - 1 : DATA;
                var dataString = parameters[dataIndex];
                var data = dataString.Split(';');
                var alias = parameters[ALIAS_O_FIRM];

                switch (type)
                {
                    case Motor:
                        {
                            var m = Parser.ProcessMotor(daoFactory,
                                                                center,
                                                                id,
                                                                alias,
                                                                Convert.ToDouble(data[M_VOLUMEN]),
                                                                DateTime.Now,
                                                                Convert.ToDouble(data[M_CAUDAL]),
                                                                Convert.ToDouble(data[M_HS_EN_MARCHA]),
                                                                Convert.ToDouble(data[M_TEMP]),
                                                                Convert.ToDouble(data[M_RPM]),
                                                                data[M_ENTRIES],
                                                                data[M_EVENTS]);
                            if (m != null) pozoMessage.addMovimiento(m);
                            break;
                        }
                    case Medidor:
                        {
                            var v = Parser.ProcessMedidor(daoFactory,
                                                                center,
                                                                id,
                                                                alias,
                                                                Convert.ToDouble(data[V_VOLUMEN_NETO]),
                                                                "Ingreso",
                                                                DateTime.Now,
                                                                data[V_EVENTS]);
                            if (v != null) pozoMessage.addMovimiento(v);
                            break;
                        }
                    case Tanque:
                        {
                            var tankCode = parameters[1];
                            var t = Parser.ProcessTanque(daoFactory,
                                                               center,
                                                               tankCode,
                                                               alias,
                                                               Convert.ToDouble(data[T_VOLUMEN_BRUTO]),
                                                               Convert.ToDouble(data[T_VOLUMEN_AGUA]),
                                                               DateTime.Now,
                                                               data[T_EVENTS]);
                            if (t != null) pozoMessage.addHistoricalVolume(t);
                            break;
                        }
                }
            }
            return pozoMessage;
        }

        #endregion
    }
}
