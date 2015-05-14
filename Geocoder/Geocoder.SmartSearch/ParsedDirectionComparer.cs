using System;
using System.Collections.Generic;
namespace Geocoder.SmartSearch
{
	public class ParsedDirectionComparer : IComparer<ParsedDirection>
	{
		public int Compare(ParsedDirection x, ParsedDirection y)
		{
			return y.Probabilidad.CompareTo(x.Probabilidad);
		}
	}
}
