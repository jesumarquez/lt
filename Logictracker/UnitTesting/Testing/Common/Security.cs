using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Types.SecurityObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.SessionState;
using System.IO;
using System;

namespace Logictracker.Testing.Common
{
    
    
    /// <summary>
    ///This is a test class for WebSecurityTest and is intended
    ///to contain all WebSecurityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Security
    {
        #region IHttpSessionState Implementation
        public class HttpSessionState2 : IHttpSessionState
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            #region Implementation of IHttpSessionState

            public void Abandon() { Clear(); }

            public void Add(string name, object value)
            {
                Remove(name);
                dic.Add(name, value);
            }

            public void Remove(string name)
            {
                if (dic.ContainsKey(name)) dic.Remove(name);
            }

            public void RemoveAt(int index)
            {
                Remove(dic.Keys.ElementAt(index));
            }

            public void Clear() { dic.Clear(); }

            public void RemoveAll() { Clear(); }

            public IEnumerator GetEnumerator() { return dic.GetEnumerator(); }

            public void CopyTo(Array array, int index) { throw new NotImplementedException(); }

            public string SessionID { get { return "1234"; } }

            public int Timeout { get; set; }

            public bool IsNewSession { get { return false; } }

            public SessionStateMode Mode { get { return SessionStateMode.Custom; } }

            public bool IsCookieless { get { return true; } }

            public HttpCookieMode CookieMode { get { return HttpCookieMode.AutoDetect; } }

            public int LCID { get; set; }

            public int CodePage { get; set; }

            public HttpStaticObjectsCollection StaticObjects { get { return new HttpStaticObjectsCollection(); } }

            object IHttpSessionState.this[string name]
            {
                get { return dic.ContainsKey(name) ? dic[name]: null; }
                set { Add(name, value); }
            }

            object IHttpSessionState.this[int index]
            {
                get { return dic[dic.Keys.ElementAt(index)]; }
                set { Add(dic.Keys.ElementAt(index), value); }
            }

            public int Count { get { return dic.Count; } }

            public NameObjectCollectionBase.KeysCollection Keys { get { return null; } }

            public object SyncRoot { get; set; }

            public bool IsReadOnly { get { return false; } }

            public bool IsSynchronized { get { return false; } }

            #endregion
        } 
        #endregion

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            var request = new SimpleWorkerRequest("/dummy", @"c:\inetpub\wwwroot\dummy", "dummy.html", null, new StringWriter());
            HttpContext.Current = new HttpContext(request);
            SessionStateUtility.AddHttpSessionStateToContext(HttpContext.Current, new HttpSessionState2());
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void Login()
        {
            WebSecurity.AuthenticatedUser = null;

            var modules = new[]
                              {
                                  new MovMenu() {Funcion = new Funcion() {Ref = "MONITOR", Sistema = new Sistema() {}}},
                                  new MovMenu() {Funcion = new Funcion() {Ref = "CONSOLA", Sistema = new Sistema() {}}}
                              };
            var securables = new[]
                                 {
                                     new Asegurable() {Id = 1, Referencia = "MONITOR"},
                                     new Asegurable() {Id = 2, Referencia = "CONSOLA"}
                                 };

            WebSecurity.Login(GetUsuario(), new[] { 1 }, modules, securables);

            var user = WebSecurity.AuthenticatedUser;

            Assert.IsNotNull(user);
            Assert.IsTrue(user.Modules.ContainsKey("MONITOR"));
            Assert.IsTrue(user.Modules.ContainsKey("CONSOLA"));
            Assert.IsTrue(user.Securables.ContainsKey("MONITOR"));
            Assert.IsTrue(user.Securables.ContainsKey("CONSOLA")); 
        }

        [TestMethod()]
        public void ValidateLogin()
        {
            //TODO: Hacer este Test
        }

        [TestMethod()]
        public void Logout()
        {
            WebSecurity.AuthenticatedUser = null;

            var modules = new[]
                              {
                                  new MovMenu() {Funcion = new Funcion() {Ref = "MONITOR", Sistema = new Sistema() {}}},
                                  new MovMenu() {Funcion = new Funcion() {Ref = "CONSOLA", Sistema = new Sistema() {}}}
                              };
            var securables = new[]
                                 {
                                     new Asegurable() {Id = 1, Referencia = "MONITOR"},
                                     new Asegurable() {Id = 2, Referencia = "CONSOLA"}
                                 };

            WebSecurity.Login(GetUsuario(), new[] { 1 }, modules, securables);

            var user = WebSecurity.AuthenticatedUser;
            Assert.IsNotNull(user);

            WebSecurity.Logout();
            
            user = WebSecurity.AuthenticatedUser;
            Assert.IsNull(user);
        }

        /// <summary>
        ///A test for ShowDriver
        ///</summary>
        [TestMethod()]
        public void ShowDriverTest()
        {
            var user = GetUsuario();
            var usd = new UserSessionData(user, new List<int> { 1 });
            WebSecurity.AuthenticatedUser = usd;

            bool actual = WebSecurity.ShowDriver;
            Assert.IsFalse(actual);


            user.Client = "AGC";
            usd = new UserSessionData(user, new List<int> { 1 });
            WebSecurity.AuthenticatedUser = usd;

            actual = WebSecurity.ShowDriver;
            Assert.IsTrue(actual);
        }

        /// <summary>
        ///A test for AuthenticatedUser
        ///</summary>
        [TestMethod()]
        public void AuthenticatedUserTest()
        {
            var expected = new UserSessionData(GetUsuario(), new List<int> { 1 });
            UserSessionData actual;
            WebSecurity.AuthenticatedUser = expected;
            actual = WebSecurity.AuthenticatedUser;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Authenticated
        ///</summary>
        [TestMethod()]
        public void AuthenticatedTest()
        {
            var usd = new UserSessionData(GetUsuario(), new List<int> { 1 });
            WebSecurity.AuthenticatedUser = usd;
            bool actual = WebSecurity.Authenticated;
            Assert.IsTrue(actual);

            WebSecurity.AuthenticatedUser = null;
            actual = WebSecurity.Authenticated;
            Assert.IsFalse(actual);
        }

        /// <summary>
        ///A test for IsSecuredAllowed
        ///</summary>
        [TestMethod()]
        public void IsSecuredAllowedTest()
        {
            string securable = Securables.ViewIds;

            var usd = new UserSessionData(GetUsuario(), new List<int> { 1 });
            usd.Securables.Add(securable, true);
            WebSecurity.AuthenticatedUser = usd;

            bool actual = WebSecurity.IsSecuredAllowed(string.Empty);
            Assert.IsTrue(actual);

            actual = WebSecurity.IsSecuredAllowed(null);
            Assert.IsTrue(actual);

            actual = WebSecurity.IsSecuredAllowed(securable);
            Assert.IsTrue(actual);

            usd.Securables.Remove(securable);

            actual = WebSecurity.IsSecuredAllowed(securable);
            Assert.IsFalse(actual);
        }

        /// <summary>
        ///A test for GetUserModuleByRef
        ///</summary>
        [TestMethod()]
        public void GetUserModuleByRefTest()
        {
            var moduleOk = "MONITOR";
            var moduleBad = "NOT_EXISTING_MODULE";
            var moduleMultiple1 = "MONITOR,CONSOLA";
            var moduleMultiple2 = "MONITOR,NOT_EXISTING_MODULE";
            var moduleMultiple3 = "NOT_EXISTING_MODULE,CONSOLA";
            WebSecurity.AuthenticatedUser = null;

            Module actual = WebSecurity.GetUserModuleByRef(moduleOk);
            Assert.IsNull(actual);         

            var usd = new UserSessionData(GetUsuario(), new List<int> { 1 });
            usd.Modules.Add("MONITOR", new Module(new MovMenu(){Funcion = new Funcion(){Ref = "MONITOR", Sistema = new Sistema(){}}}));
            usd.Modules.Add("CONSOLA", new Module(new MovMenu() { Funcion = new Funcion() { Ref = "CONSOLA", Sistema = new Sistema() { } } }));

            WebSecurity.AuthenticatedUser = usd;

            actual = WebSecurity.GetUserModuleByRef(moduleBad);
            Assert.IsNull(actual);

            actual = WebSecurity.GetUserModuleByRef(moduleOk);
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.RefName, moduleOk);

            actual = WebSecurity.GetUserModuleByRef(moduleMultiple1);
            Assert.IsNotNull(actual);
            Assert.IsTrue(moduleMultiple1.Contains(actual.RefName));

            actual = WebSecurity.GetUserModuleByRef(moduleMultiple2);
            Assert.IsNotNull(actual);
            Assert.IsTrue(moduleMultiple2.Contains(actual.RefName));

            actual = WebSecurity.GetUserModuleByRef(moduleMultiple3);
            Assert.IsNotNull(actual);
            Assert.IsTrue(moduleMultiple3.Contains(actual.RefName));

        }

        #region Security Extensions
        /// <summary>
        ///A test for ToDisplayTime
        ///</summary>
        [TestMethod()]
        public void ToDisplayTimeTest()
        {
            var time = DateTime.Now.TimeOfDay;
            var usd = new UserSessionData(GetUsuario(), new List<int> { 1 });
            usd.GmtModifier = -3;
            WebSecurity.AuthenticatedUser = usd;

            TimeSpan expected = time.Add(TimeSpan.FromHours(usd.GmtModifier));
            TimeSpan actual = time.ToDisplayTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 0;
            WebSecurity.AuthenticatedUser = usd;

            expected = time.Add(TimeSpan.FromHours(usd.GmtModifier));
            actual = time.ToDisplayTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 4;
            WebSecurity.AuthenticatedUser = usd;

            expected = time.Add(TimeSpan.FromHours(usd.GmtModifier));
            actual = time.ToDisplayTime();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToDisplayDateTime
        ///</summary>
        [TestMethod()]
        public void ToDisplayDateTimeTest()
        {
            var date = DateTime.Now;
            var usd = new UserSessionData(GetUsuario(), new List<int> { 1 });
            usd.GmtModifier = -3;
            WebSecurity.AuthenticatedUser = usd;

            DateTime expected = date.AddHours(usd.GmtModifier);
            DateTime actual = date.ToDisplayDateTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 0;
            WebSecurity.AuthenticatedUser = usd;

            expected = date.AddHours(usd.GmtModifier);
            actual = date.ToDisplayDateTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 4;
            WebSecurity.AuthenticatedUser = usd;

            expected = date.AddHours(usd.GmtModifier);
            actual = date.ToDisplayDateTime();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToDataBaseTime
        ///</summary>
        [TestMethod()]
        public void ToDataBaseTimeTest()
        {
            var time = DateTime.Now.TimeOfDay;
            var usd = new UserSessionData(GetUsuario(), new List<int> { 1 });
            usd.GmtModifier = -3;
            WebSecurity.AuthenticatedUser = usd;

            TimeSpan expected = time.Subtract(TimeSpan.FromHours(usd.GmtModifier));
            TimeSpan actual = time.ToDataBaseTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 0;
            WebSecurity.AuthenticatedUser = usd;

            expected = time.Subtract(TimeSpan.FromHours(usd.GmtModifier));
            actual = time.ToDataBaseTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 4;
            WebSecurity.AuthenticatedUser = usd;

            expected = time.Subtract(TimeSpan.FromHours(usd.GmtModifier));
            actual = time.ToDataBaseTime();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToDataBaseDateTime
        ///</summary>
        [TestMethod()]
        public void ToDataBaseDateTimeTest()
        {
            var date = DateTime.Now;
            var usd = new UserSessionData(GetUsuario(), new List<int> { 1 });
            usd.GmtModifier = -3;
            WebSecurity.AuthenticatedUser = usd;

            DateTime expected = date.AddHours(-usd.GmtModifier);
            DateTime actual = date.ToDataBaseDateTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 0;
            WebSecurity.AuthenticatedUser = usd;

            expected = date.AddHours(-usd.GmtModifier);
            actual = date.ToDataBaseDateTime();
            Assert.AreEqual(expected, actual);

            usd.GmtModifier = 4;
            WebSecurity.AuthenticatedUser = usd;

            expected = date.AddHours(-usd.GmtModifier);
            actual = date.ToDataBaseDateTime();
            Assert.AreEqual(expected, actual);
        } 
        #endregion

        private Usuario GetUsuario()
        {
            return new Usuario
            {
                Clave = "CLAVE",
                Client = "Logictracker",
                Culture = "es-AR",
                Entidad = new Entidad
                {
                    Baja = false,
                    Cuil = "cuil",
                    Apellido = "TestUser",
                    Direccion = null,
                    Id = 1,
                    NroDocumento = "1234",
                    TipoDocumento = "DNI"
                },
                FechaAlta = DateTime.UtcNow.AddYears(-1),
                FechaBaja = null,
                FechaExpiracion = null,
                Id = 1,
                Inhabilitado = false,
                InhabilitadoCambiarPass = false,
                InhabilitadoCambiarUso = false
            };
        }
    }
}
