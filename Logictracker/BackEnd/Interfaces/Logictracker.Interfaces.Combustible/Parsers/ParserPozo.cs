#region Usings

using System;
using System.Collections.Generic;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Interfaces.Combustible.Parsers
{
    public class ParserPozo
    {
        #region Private Properties

		private const String EstadoCode = "E";
		private const String LogCode = "L";

		private const String EmptyMessage = "0002";

        #endregion

        #region Public Methods

		public PozoMessage Parse(String str, DAOFactory daoFactory)
        {
            var header = str.Split('@');

            if (ProcessEvents(header) == 1) return new PozoMessage();

            var centerCode = header[0];
            var headerparameters = header[2].Split('|');

            var type = headerparameters[1];

            if (headerparameters[2].Equals(EmptyMessage)) return new PozoMessage();

            var body = str.Remove(0, header[1].Length + header[2].Length + 7).TrimEnd('.').TrimEnd('F').TrimEnd('@');

            switch (type)
            {
                case EstadoCode:
                    {
                        return ParserPozoEstado.Parse(centerCode + "*" + body, daoFactory);
                    }
                case LogCode:
                    {
                        var logParser = new ParserPozoLog();

                        return logParser.Parse(centerCode + "*" + body, daoFactory);
                    }
                default:
                    {
						STrace.Trace(GetType().FullName, String.Format("A message of type pozo with an invalid code was detected: {0}", str));
                        break;
                    }
            }

            return new PozoMessage();
        }
            
        /// <summary>
        /// process the events. Por el momento solo loguea
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
		private int ProcessEvents(IList<String> header)
        {
            if (header[1].Contains("ONLINE"))
            {
                STrace.Debug(GetType().FullName, String.Format("Data centralizer ONLINE at pozo: {0}", header[0]));

                return 1;
            }

            if (header[1].Contains("OFFLINE"))
            {
				STrace.Debug(GetType().FullName, String.Format("Data centralizer OFFLINE at pozo: {0}", header[0]));

                return 1;
            }

            if (header[1].Contains("START"))
            {
				STrace.Debug(GetType().FullName, String.Format("Gateway ONLINE at pozo: {0}", header[0]));

                return 1;
            }

            if (header[1].Contains("STOP"))
            {
				STrace.Debug(GetType().FullName, String.Format("Gateway OFFLINE at pozo: {0}", header[0]));

                return 1;
            }

            if (header[1].Contains("KEEPALIVE"))
            {
				STrace.Debug(GetType().FullName, String.Format("Gateway KEEPALIVE at pozo: {0}", header[0]));

                return 1;
            }

            return 0;
        }

        #endregion
    }
}
