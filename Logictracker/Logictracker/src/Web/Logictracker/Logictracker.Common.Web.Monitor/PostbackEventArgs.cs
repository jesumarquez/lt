#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor
{
    public class PostbackEventArgs: EventArgs
    {
        public PostbackEventArgs(string commandArguments, double latitud, double longitud)
        {
            CommandArguments = commandArguments;
            Latitud = latitud;
            Longitud = longitud;
        }

        public string CommandArguments { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
