using System.Collections.Generic;
using Logictracker.GsTraq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logictracker.Testing.Codecs
{
    [TestClass]
    public class GlobalSat
    {
        [TestMethod]
        public void NextSequence()
        {
			var parser = new Parser();

            var prev = new List<ulong>(1000);
            for(var i = 0; i < 1000; i++)
            {
                var seq = parser.NextSequence;
                Assert.IsTrue(seq >= 500, "Numero de secuencia debe ser >= 500");
                Assert.IsTrue(seq <= 32000, "Numero de secuencia debe ser <= 32000");
                Assert.IsFalse(prev.Contains(seq), "Numero de secuencia repetido " + seq + " - Iteracion: " + i);
                prev.Add(seq);
            }
        }
    }
}
