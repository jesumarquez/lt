using Logictracker.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Logictracker.Testing.Common
{
    /// <summary>
    ///This is a test class for WindowsSecurityHelperTest and is intended
    ///to contain all WindowsSecurityHelperTest Unit Tests
    ///</summary>
    [TestClass]
    public class WindowsSecurityHelperTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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
        ///A test for UndoImpersonation
        ///</summary>
        [TestMethod]
        public void UndoImpersonationTest()
        {
            var target = new WindowsSecurityHelper();
            target.UndoImpersonation();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RevertToSelf
        ///</summary>
        [TestMethod]
        [DeploymentItem("Logictracker.Security.dll")]
        public void RevertToSelfTest()
        {
            //var actual = WindowsSecurityHelper_Accessor.RevertToSelf();
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LogonUserA
        ///</summary>
        [TestMethod]
        [DeploymentItem("Logictracker.Security.dll")]
        public void LogonUserATest()
        {
            var phToken = new IntPtr();
            var phTokenExpected = new IntPtr();
            Assert.AreEqual(phTokenExpected, phToken);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ImpersonateValidUser
        ///</summary>
        [TestMethod]
        public void ImpersonateValidUserTest()
        {
            var target = new WindowsSecurityHelper();
            const bool expected = false;
            var actual = target.ImpersonateValidUser();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DuplicateToken
        ///</summary>
        [TestMethod]
        [DeploymentItem("Logictracker.Security.dll")]
        public void DuplicateTokenTest()
        {
            new IntPtr();
            var hNewToken = new IntPtr();
            var hNewTokenExpected = new IntPtr();
            //var actual = WindowsSecurityHelper_Accessor.DuplicateToken(hToken, impersonationLevel, ref hNewToken);
            Assert.AreEqual(hNewTokenExpected, hNewToken);
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod]
        public void DisposeTest()
        {
            var target = new WindowsSecurityHelper();
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CloseHandle
        ///</summary>
        [TestMethod]
        [DeploymentItem("Logictracker.Security.dll")]
        public void CloseHandleTest()
        {
            new IntPtr();
            //var actual = WindowsSecurityHelper_Accessor.CloseHandle(handle);
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for WindowsSecurityHelper Constructor
        ///</summary>
        [TestMethod]
        public void WindowsSecurityHelperConstructorTest()
        {
            new WindowsSecurityHelper();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
