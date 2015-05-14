using System;
namespace Geocoder.Core
{
	public static class Prefix
	{
		private static int PrefixCharValue(char c)
		{
			int result;
			if (char.IsNumber(c))
			{
				result = (int)(c - '0');
			}
			else
			{
				if (c < 'q')
				{
					result = (int)(c - 'W');
				}
				else
				{
					if (c < 'v')
					{
						result = (int)(c - 'X');
					}
					else
					{
						if (c < 'x')
						{
							result = (int)(c - 'Y');
						}
						else
						{
							result = (int)(c - 'Z');
						}
					}
				}
			}
			return result;
		}
		private static int PrefixMin(string normalized, int index)
		{
			int num = 0;
			int num2 = normalized.Length - index;
			int num3 = (num2 > 6) ? 6 : num2;
			int num4 = 0;
			for (int i = index; i < num3; i++)
			{
				num += Prefix.PrefixCharValue(normalized[i]) << (5 - num4) * 5;
				num4++;
			}
			return num;
		}
		private static int PrefixMax(string normalized, int index)
		{
			int num = normalized.Length;
			int num2 = Prefix.PrefixMin(normalized, index);
			int result;
			if ((num = ((num > 6) ? 6 : num)) == 6)
			{
				result = num2;
			}
			else
			{
				result = (int)((long)num2 + (long)((ulong)(4294967295u >> 2 + num * 5)));
			}
			return result;
		}
		public static int Prefix6Min(string normalized)
		{
			return Prefix.PrefixMin(normalized, 0);
		}
		public static int Prefix6Max(string normalized)
		{
			return Prefix.PrefixMax(normalized, 0);
		}
		public static int Prefix12Min(string normalized)
		{
			return Prefix.PrefixMin(normalized, 6);
		}
		public static int Prefix12Max(string normalized)
		{
			return Prefix.PrefixMax(normalized, 6);
		}
		public static int Prefix18Min(string normalized)
		{
			return Prefix.PrefixMin(normalized, 12);
		}
		public static int Prefix18Max(string normalized)
		{
			return Prefix.PrefixMax(normalized, 12);
		}
		public static bool IsPrefix(string normalized, string normalizedTestForPrefix)
		{
			bool result;
			if (Prefix.Prefix6Min(normalizedTestForPrefix) > Prefix.Prefix6Min(normalized) || Prefix.Prefix6Max(normalized) > Prefix.Prefix6Max(normalizedTestForPrefix))
			{
				result = false;
			}
			else
			{
				int length = normalizedTestForPrefix.Length;
				result = (length <= 6 || (Prefix.Prefix12Min(normalizedTestForPrefix) <= Prefix.Prefix12Min(normalized) && Prefix.Prefix12Max(normalized) <= Prefix.Prefix12Max(normalizedTestForPrefix) && (length <= 12 || (Prefix.Prefix18Min(normalizedTestForPrefix) <= Prefix.Prefix18Min(normalized) && Prefix.Prefix18Max(normalized) <= Prefix.Prefix18Max(normalizedTestForPrefix)))));
			}
			return result;
		}
	}
}
