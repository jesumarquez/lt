using System;
using System.IO;
using System.Net;
using System.Text;

namespace Logictracker.Scheduler.Tasks.ControlAcceso
{
    public class Pct100Dumper
    {
        private readonly Uri _webInterface;
        private readonly string _userName;
        private readonly string _password;
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webInterface"></param>
        /// <param name="password"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="userName"></param>
        /// <example> 
        /// var q = new PCT100Dumper("http://192.168.10.66", "admin","admin", new DateTime(2012,12,13),  new DateTime(2012,12,14));
        /// q.Execute();
        /// </example>
        public Pct100Dumper(Uri webInterface, string userName, string password, DateTime startTime, DateTime endTime)
        {
            _webInterface = webInterface;
            _userName = userName;
            _password = password;
            _startTime = startTime;
            _endTime = endTime;
        }

        public Uri GetUri()
        {
            const string page = "if.cgi";

            var query = new StringBuilder(
                "redirect=UserLog.htm&failure=fail.htm&type=search_user_log&type=0" +
                "&sel=1&u_id=&even=0&even=0&even=0&even=0&even=0");

            query.AppendFormat("&year={0}&mon={1}&day={2}", _startTime.Year - 2000, _startTime.Month, _startTime.Day);

            query.AppendFormat("&year={0}&mon={1}&day={2}", _endTime.Year - 2000, _endTime.Month, _endTime.Day);

            query.Append("&card=0&card=0&card=0&card=0&card=0&card=0&card=0&card=0&fun_t=1&e_t=0");

            var uriBuilder = new UriBuilder(_webInterface) { Path = page, Query = query.ToString() };

            return uriBuilder.Uri;
        }

        public Stream Execute()
        {
            var req = WebRequest.Create(GetUri());
            req.Credentials = new NetworkCredential(_userName, _password);

            return req.GetResponse().GetResponseStream();
        }

        public void SaveToFile(string fileName)
        {
            var f = File.OpenWrite(fileName);
            Execute().CopyTo(f);
            f.Close();
        }
    }
}