using Logictracker.Scheduler.Tasks.PumpControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.Interfaces
{
    [TestClass]
    public class PumpWebServiceTest
    {
        [TestMethod]
        [DeploymentItem("Logictracker.Scheduler.Tasks.PumpControl.dll")]
        public void OnExecuteTest()
        {
            var target = new Task_Accessor();
            target.OnExecute(null);
        }
    }
}
