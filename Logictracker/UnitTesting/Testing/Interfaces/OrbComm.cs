using System;
using Logictracker.Interfaces.OrbComm;
using Logictracker.Process.Import.Client.Mapping;
using Logictracker.Process.Import.Client.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.Interfaces
{
    /// <summary>
    /// Summary description for ServiceTest
    /// </summary>
    [TestClass]
    public class ServiceTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
         [TestInitialize]
         public void TestMethodBegin()
         {
             var service = new Service();
             var auth = service.Authenticate("user", "password");
             SessionId = auth.SessionId;
         }
        
        // Use TestCleanup to run code after each test has run
         [TestCleanup]
         public void TestMethodEnd()
         {
             var service = new Service();
             service.Logout(SessionId);
         }
        
        #endregion

        protected String SessionId;

        [TestMethod]
        public void Import()
        {

            //var a = Logictracker.Process.Import.Client.Types.Properties.GetAs("Codigo",typeof (Logictracker.Process.Import.Client.Types.Properties.Cliente));

            //var d = new Data();
            //d.Entity = (int)Entities.ReferenciaGeografica;
            //var b = EntityParserFactory.GetEntityParser(d);
            /*
            var a =
                "planta,nro_pedido,nro_viaje,cod_client,nombre_cli,cod_obra,obra,direccion,latitud,longitud,fecha,cod_prod,nro_remito,cod_chofer,nomb_chof,cod_camion,m3viaje,m3sol,m3ajus,m3entre,m3saldo,abm,gmt";
            var b =
                @"0025,100,1,""40002"",""cliente 40002"",10,""Obra 10"",""Uruguay 540, Buenos Aires, Ciudad Autónoma de Buenos Aires, Argentina"",""-34.6023370"","" -58.3866890"",28/05/2011 00:00:00,""ART 1"",58433,999,""Chofer 999"",900,0,0,0,0,0,"" "",-3";

            var csv = new CsvDataStrategy(new IDataSourceParameter[0]);

            var a1 = csv.Split(a);
            var b1 = csv.Split(b);
            */
            var parser = ParserFactory.GetEntityParser(new Entity {Type = "Cliente"});
        }

        [TestMethod]
        public void Authentication()
        {
            var service = new Service();

            var auth = service.Authenticate("user", "password");

            if (auth.Result == 1) service.Logout(auth.SessionId);

            Assert.AreEqual(1, auth.Result, auth.ResultDescription);
        }

        [TestMethod]
        public void Refresh()
        {
            var service = new Service();

            var refresh = service.Refresh(SessionId);
            Assert.AreEqual(1, refresh.Result, refresh.ResultDescription);
        }

        [TestMethod]
        public void Logout()
        {
            var service = new Service();

            var logout = service.Logout(SessionId);
            Assert.AreEqual(1, logout.Result, "Error");
        }


        [TestMethod]
        public void QueryDeviceStatus()
        {
            var service = new Service();

            var devstat = service.QueryDeviceStatus(SessionId, "M05000000826X1");
            //var devstat = service.QueryDeviceStatus(SessionId, "M05000000773X1");
            Assert.AreEqual(1, devstat.Result, devstat.ResultDescription);
        }

        [TestMethod]
        public void RetrieveMessages()
        {
            var service = new Service();

            var messages = service.RetrieveMessages(SessionId, Service.MessageFlags.Unread, Service.SetMessageFlags.NoAction, Service.MessageStatusFlags.All, -1, true);
            Assert.IsNotNull(messages);
        }
        /*
        public SendMessage SendMessage(string sessionId, string deviceId, string subject, string body, bool bodyBinary)
        {
            const string op = "SendMessage";
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

        public QueryMessageStatus QueryMessageStatus(string sessionId, string confNum)
        {
            const string op = "QueryMessageStatus";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("CONF_NUM", confNum)
                                         .Add("MESSAGE", "1")
                                         .Add("VERSION", "2");
            var res = Request(op, par);
            return new QueryMessageStatus(res.DocumentElement);
        }
        public DeleteMessage DeleteMessage(string sessionId, string confNum)
        {
            const string op = "DeleteMessage";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("CONF_NUM", confNum)
                                         .Add("VERSION", "2");
            var res = Request(op, par);
            return new DeleteMessage(res.DocumentElement);
        }

        
        public SetMessageFlag SetMessageFlag(string sessionId, string select, string criteria, int setMessageFlag)
        {
            const string op = "SetMessageFlag";
            var par = PostParams.Create().Add("SESSION_ID", sessionId)
                                         .Add("SELECT", select)
                                         .Add("CRITERIA", criteria)
                                         .Add("FLAG", setMessageFlag.ToString());
            var res = Request(op, par);
            return new SetMessageFlag(res.DocumentElement);
        }
         * */
    }
}
