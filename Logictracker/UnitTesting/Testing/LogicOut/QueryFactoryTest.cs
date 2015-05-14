using System.Linq;
using LogicOut.Handlers;
using LogicOut.Server;
using Logictracker.DAL.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.LogicOut
{
    
    
    /// <summary>
    ///This is a test class for QueryFactoryTest and is intended
    ///to contain all QueryFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QueryFactoryTest
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
        ///A test for GetData
        ///</summary>
        [TestMethod()]
        public void GetDataTest()
        {
            var daoFactory = new DAOFactory();
            QueryFactory target = new QueryFactory(daoFactory);
            var linea = daoFactory.LineaDAO.FindAll().First();
            int idEmpresa = linea.Empresa.Id;
            int idLinea = linea.Id;
            string query = "molinete";
            string parameters = "server=1";
            var actual = target.GetData(idEmpresa, idLinea, query, parameters);
        }

        [TestMethod()]
        public void Molinete()
        {
            var molinete = new Molinete("molinete");
            molinete.Process();
        }
    }
}
