using System;
using System.Collections.Generic;
using System.Threading;

namespace Geocoder.Logic
{
	public static class SpecialWords
	{
		private static readonly object LockObject = new object();
		private static readonly List<string> SmallWords = new List<string>(new string[]
		{
			"el",
			"la",
			"los",
			"las",
			"de",
			"del",
			"i",
			"e",
			"a",
			"pje",
			"jose",
			"juan",
			"bautista",
			"calle",
			"sin",
			"nombre",
			"ax",
			"s/n",
			"luis"
		});
		public static bool IsSmallWord(string word)
		{
			object lockObject;
			Monitor.Enter(lockObject = SpecialWords.LockObject);
			bool result;
			try
			{
				result = SpecialWords.SmallWords.Contains(word);
			}
			finally
			{
				Monitor.Exit(lockObject);
			}
			return result;
		}
	}
}
