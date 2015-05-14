#region Usings

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Comm.PumpControl
{
    public class PumpProtocol
    {
        public static string QuerySignle(string hostname, int port, string query)
        {
            try
            {
                using (var client = new TcpClient(hostname, port))
                {
                    client.ReceiveTimeout = 60000;
                    client.SendTimeout = 60000;
                    var data = Encoding.ASCII.GetBytes(query);
                    var stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                    data = new byte[65535];
                    var cursor = 0;
                    while (stream.CanRead)
                    {
                        var bytes = stream.Read(data, cursor, data.Length - cursor);
                        if (bytes == 0) break;
                        cursor += bytes;
                    }
                    // Close everything.
                    stream.Close();
                    client.Close();
                    var line = Encoding.ASCII.GetString(data, 0, cursor);
                    T.TRACE(line);
                    return line;
                }
            }
            catch (Exception e)
            {
                T.EXCEPTION(e,"PumpControl.QuerySingle");
            }
            return null;
        }
    }
}