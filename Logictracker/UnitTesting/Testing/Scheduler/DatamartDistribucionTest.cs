using Logictracker.Scheduler.Tasks.Mantenimiento;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.Scheduler
{
    [TestClass]
    public class DatamartDistribucionTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("Logictracker.Scheduler.Tasks.Mantenimiento.dll")]
        public void OnExecuteTest1()
        {
            var task = new DatamartDistribucionTask();            
            task.Execute(null);
        }
    }
}
