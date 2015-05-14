using System;
using System.IO;
using System.Net;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.Net
{
    public class Toolkit
    {
        public const string UrbetrackWebURL = "http://www.urbetrack.com.ar";
        public const string UrbetrackNetworkWatchdogURL = "http://www.urbetrack.com.ar/wd/";
        public const string UrbetrackUpgradeURL = "http://www.urbetrack.com.ar/upgrade/";

        public static bool IsConnectionWorking
        {
            get
            {
                try
                {
                    T.INFO("NETWORK: probando IsConnectionWorking.");
                    var destino = new Uri(UrbetrackNetworkWatchdogURL);
                    var myWebRequest = (HttpWebRequest)WebRequest.Create(destino);
                    var myWebResponse = myWebRequest.GetResponse();
                    myWebResponse.Close();
                    return true;
                }
                catch (Exception e)
                {
                    T.EXCEPTION(e, "IsConnectionWorking");
                }
                return false;
            }
        }

        public static bool WebGet(string url, string filename)
        {
            try
            {
                var destino = new Uri(url);
                var myWebRequest = (HttpWebRequest)WebRequest.Create(destino);
                var myWebResponse = myWebRequest.GetResponse();
                var receiveStream = myWebResponse.GetResponseStream();
                var read = new byte[256];
                var count = receiveStream.Read(read, 0, 256);
                var file = File.Create(filename);
                while (count > 0)
                {
                    file.Write(read, 0, count);
                    count = receiveStream.Read(read, 0, 256);
                }
                file.Flush();
                file.Close();
                myWebResponse.Close();
                return true;
            }
            catch (Exception e)
            {
                T.EXCEPTION(e, "WebGet");
            }
            return false;
        }
    }
}
