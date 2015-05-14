using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Logictracker.Configuration;
using System.IO;

namespace Logictracker.Interfaces.OrbComm
{
    public class Service
    {
        public static class MessageFlags
        {
            public const int All = 0;
            public const int Read = 1;
            public const int Unread = 2;
            public const int Deleted = 3;
        }
        public static class SetMessageFlags
        {
            public const int NoAction = 0;
            public const int Read = 1;
            public const int Unread = 2;
            public const int Deleted = 3;
        }
        public static class MessageStatusFlags
        {
            public const int All = 0;
            public const int Pending = 1;
            public const int ReceivedOms = 2;
            public const int Delivered = 3;
            public const int InTransit = 4;
            public const int Scheduled = 5;
            public const int MobileOriginated = 6;
            public const int Exhausted = 7;
            public const int Undeliverable = 9;
        }
        public static class SetMessageSelect
        {
            public const int All = 0;
            public const int ByDate = 1;
            public const int ByConfNum = 2;
            public const int ByMessageId = 3;
        }
        public static class MessagePriority
        {
			public const String NonUrgent = "non-urgent";
			public const String Normal = "normal";
			public const String Urgent = "urgent";
        }
        public Authenticate Authenticate(string user, string password)
        {
            const string op = "Authenticate";
            var par = PostParams.Create().Add("LOGIN", user)
                                         .Add("PSSWD", password)
                                         .Add("VERSION", "2");
            var res = Request(op, par);
            return new Authenticate(res.DocumentElement);
        }
		public Refresh Refresh(String sessionId)
        {
			const String op = "Refresh";
            var par = PostParams.Create().Add("SESSION_ID", sessionId);
            var res = Request(op, par);
            return new Refresh(res.DocumentElement);
        }
		public Logout Logout(String sessionId)
        {
			const String op = "Logout";
            var par = PostParams.Create().Add("SESSION_ID", sessionId);
            var res = Request(op, par);
            return new Logout(res.DocumentElement);
        }
		public SendMessage SendMessage(String sessionId, String deviceId, String subject, String body, bool bodyBinary)
        {
			const String op = "SendMessage";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("DEVICE_ID", deviceId)
                                         .Add("NETWORK_ID", "3")
                                         .Add("MESSAGE_SUBJECT", subject)
                                         .Add("MESSAGE_BODY_TYPE", bodyBinary ? "1" : "0")
                                         .Add("MESSAGE_BODY", body)
                                         .Add("SEND_TIME", "************");
            var res = Request(op, par);
            return new SendMessage(res.DocumentElement);
        }

		public QueryMessageStatus QueryMessageStatus(String sessionId, String confNum)
        {
			const String op = "QueryMessageStatus";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("CONF_NUM", confNum)
                                         .Add("MESSAGE", "1")
                                         .Add("VERSION","2");
            var res = Request(op, par);
            return new QueryMessageStatus(res.DocumentElement);
        }
		public DeleteMessage DeleteMessage(String sessionId, String confNum)
        {
			const String op = "DeleteMessage";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("CONF_NUM", confNum)
                                         .Add("VERSION", "2");
            var res = Request(op, par);
            return new DeleteMessage(res.DocumentElement);
        }

		public QueryDeviceStatus QueryDeviceStatus(String sessionId, String deviceId)
        {
			const String op = "QueryDeviceStatus";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("DEVICE_ID", deviceId)
                                         .Add("VERSION", "2");
            var res = Request(op, par);
            return new QueryDeviceStatus(res.DocumentElement);
        }

		public RetrieveMessages RetrieveMessages(String sessionId, int messageFlag, int setMessageFlag, int messageStatusFlag, int messageId, bool returnMessageBody)
        {
			const String op = "RetrieveMessages";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("NETWORK_ID", "3")
                                         .Add("MSG_FLAG",messageFlag.ToString(CultureInfo.InvariantCulture))
                                         .Add("SET_FLAG",setMessageFlag.ToString(CultureInfo.InvariantCulture))
                                         .Add("MSG_STATUS",messageStatusFlag.ToString(CultureInfo.InvariantCulture))
                                         .Add("MESSAGE_ID",messageId.ToString(CultureInfo.InvariantCulture))
                                         .Add("MESSAGE",returnMessageBody ? "1" : "0")
                                         .Add("MTAG","1")
                                         .Add("VERSION", "2");
            var res = Request(op, par);
            return new RetrieveMessages(res.DocumentElement);
        }
		public static SetMessageFlag SetMessageFlag(String sessionId, int select, String criteria, int setMessageFlag)
        {
			const String op = "SetMessageFlag";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("SELECT", select.ToString(CultureInfo.InvariantCulture))
                                         .Add("CRITERIA", criteria)
                                         .Add("FLAG", setMessageFlag.ToString(CultureInfo.InvariantCulture));
            var res = Request(op, par);
            return new SetMessageFlag(res.DocumentElement);
        }


		private static XmlDocument Request(String operation, PostParams par)
        {
            var url = Config.Orbcomm.WebGatewayUrl + operation;

            var encoding = new ASCIIEncoding();
            var data = encoding.GetBytes(par.Serialize());

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;

            try
            {
                var postStream = req.GetRequestStream();
                postStream.Write(data, 0, data.Length);
                postStream.Close();

                var response = (HttpWebResponse)req.GetResponse();
                var responseStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                var xml = new XmlDocument();
                xml.Load(responseStream);

                response.Close();

                return xml;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public class PostParams
        {
			private readonly Dictionary<String, String> _parameters = new Dictionary<String, String>();
			public PostParams Add(String name, String value)
            {
                _parameters.Add(name, value);
                return this;
            }
            public static PostParams Create()
            {
                return new PostParams();
            }
			public String Serialize()
            {
				return String.Join("&", _parameters.Select(p => String.Concat(p.Key, "=", p.Value)).ToArray());
            }
        }
    }
}
