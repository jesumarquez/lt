using Logictracker.Scheduler.Tasks.VencimientoDocumentos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.Scheduler
{
    [TestClass]
    public class Documents
    {
        public TestContext TestContext { get; set; }

        [TestMethod]

        public void TestMethod1()
        {
            var task = new Task();
            task.Execute(null);
        }
    }
}
