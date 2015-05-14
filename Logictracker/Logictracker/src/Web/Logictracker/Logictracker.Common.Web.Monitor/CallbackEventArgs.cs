#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor
{
    public class CallbackEventArgs: EventArgs
    {
        public CallbackEventArgs(string commandName, string commandArguments, double latitud, double longitud)
        {
            CommandName = commandName;
            CommandArguments = commandArguments;
            Latitud = latitud;
            Longitud = longitud;
        }

        public string CommandName { get; set; }
        public string CommandArguments { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
