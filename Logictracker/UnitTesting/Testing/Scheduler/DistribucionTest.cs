using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.Scheduler
{
    [TestClass]
    public class DistribucionTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("Logictracker.Scheduler.Tasks.GeneracionTareas.dll")]
        public void OnExecuteTest1()
        {
            var task = new Logictracker.Scheduler.Tasks.GeneracionTareas.Task();
            task.Execute(null);
        }

        [TestMethod]
        [DeploymentItem("Logictracker.Scheduler.Tasks.InicioAutomatico.dll")]
        public void OnExecuteTest2()
        {
            var task = new Logictracker.Scheduler.Tasks.InicioAutomatico.Task();
            task.Execute(null);
        }

        [TestMethod]
        [DeploymentItem("Logictracker.Scheduler.Tasks.ControlSalida.dll")]
        public void OnExecuteTest3()
        {
            var task = new Logictracker.Scheduler.Tasks.ControlSalida.Task();
            task.Execute(null);
        }

        [TestMethod]
        [DeploymentItem("Logictracker.Scheduler.Tasks.CierreAutomatico.dll")]
        public void OnExecuteTest4()
        {
            var task = new Logictracker.Scheduler.Tasks.CierreAutomatico.Task();
            task.Execute(null);
        }
    }
}
