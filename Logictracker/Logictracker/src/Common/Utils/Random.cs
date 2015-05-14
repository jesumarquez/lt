using System;

namespace Logictracker.Utils
{
	public static class RandomUtils
	{
		// provee numeros aleatorios de calidad moderada, contempla que halla mas de una instancia del servicio corriendo.
		private static readonly Random RandomProvider = new Random(DateTime.Now.Millisecond);
		private static readonly Object RandomProviderLock = new Object();

		public static ulong RandomNumber(int min, int max)
		{
			lock (RandomProviderLock)
			{
				return (ulong)RandomProvider.Next(min, max);
			}
		}
	}
}
