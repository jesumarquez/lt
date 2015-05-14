using System;
using LogicOut.Core;
using LogicOut.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing
{
    
    
    /// <summary>
    ///This is a test class for MolineteTest and is intended
    ///to contain all MolineteTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MolineteTest
    {


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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Process
        ///</summary>
        [TestMethod()]
        public void ProcessTest()
        {
            Server.Login();
            Molinete target = new Molinete("molinete");
            target.Process();
        }
        /// <summary>
        ///A test for Process
        ///</summary>
        [TestMethod()]
        public void CreateInstanceTest()
        {
            var handlerName = "molinete";
            var type = Type.GetType("LogicOut.Handlers.Molinete,LogicOut.Handlers");
            var handler = Activator.CreateInstance(type, handlerName) as IOutHandler;
            Assert.IsNotNull(handler);
            Assert.AreEqual(handlerName, handler.Name);
        }
    }
}
