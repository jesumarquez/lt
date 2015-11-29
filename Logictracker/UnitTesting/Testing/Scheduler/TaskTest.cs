using Logictracker.Scheduler.Tasks.ReportsScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.Scheduler
{
    [TestClass]
    public class TaskTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("Logictracker.Scheduler.Tasks.ReportsScheduler.dll")]
        public void OnExecuteTest1()
        {
            //var target = new Task_Accessor();
            //target.OnExecute(null);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
