using System;
using System.Linq;

namespace Urbetrack.Comm.Core.Codecs
{
	public static class fix
	{
		public static DateTime DecodePackedDateTime(int PackedDateTime)
		{
			var source = (uint)PackedDateTime;
			if (source == 0)
			{
				throw new ArgumentOutOfRangeException("PackedDateTime");
			}
			
			var work = source;
			work -= GetCorrection(source);
			var year = work / CONST_YEAR_SECS;
			work -= year * CONST_YEAR_SECS;


			var month = 1;
			for (var i = 0; i < 12; ++i)
			{
				var this_month_secs = (uint)(month_days[i] * CONST_DAY_SECS);
				if (i == 1 && year > 0 && ((year % 4) == 0))
					this_month_secs += CONST_DAY_SECS;
				if (work <= this_month_secs)
					break;
				
				work -= this_month_secs;
				month++;
			}
			var day = (work / CONST_DAY_SECS) + 1;
			work -= (day - 1) * CONST_DAY_SECS;
			
			var eldia = diaX(source);
			if (eldia != 0)
			{
				year = (eldia / CONST_YEAR_SECS)-1;
				month = 12;
				day = 31;
				work = source - eldia;
			}			

			var hour = work / CONST_HOUR_SECS;
			work -= hour * CONST_HOUR_SECS;
			var minute = work / CONST_MIN_SECS;
			work -= minute * CONST_MIN_SECS;
			var centenio = (DateTime.UtcNow.Year/100)*100;
			try
			{
				return new DateTime((int)year + centenio, month, (int)day, (int)hour, (int)minute, (int)work, DateTimeKind.Utc);
			}
			catch (ArgumentOutOfRangeException)
			{
				if (month == 12)
				{
					month = 1;
					year++;
				}
				else
				{
					day = 1;
					month++;
				}
				return new DateTime((int)year + centenio, month, (int)day, (int)hour, (int)minute, (int)work, DateTimeKind.Utc);
			}
		}
		
		private static uint diaX(uint source)
		{
			var diasX = new uint[] {
				411004800, 537494400, 663984000, 790473600,
				916963200, 1043452800, 1169942400, 1296432000,
				1422921600, 1549411200, 1675900800, 1802390400,
				1928880000, 2055369600, 2181859200, 2308348800,
				2434838400, 2561328000, 2687817600, 2814307200,
				2940796800, 3067286400,	};

			return diasX.FirstOrDefault(diax => (source >= diax) && (source <= (diax + 86399)));
		}
		
		private static uint GetCorrection(uint source)
		{
			int res;
			
			if (source <= 316137599) res = 9;
			else if (source < 347846401) res = 10;
			else if (source < 379468801) res = 11;
			else if (source < 411004801) res = 12;
			else if (source < 442713601) res = 13;
			else if (source < 474336001) res = 14;
			else if (source < 505958401) res = 15;
			else if (source < 537494401) res = 16;
			else if (source < 569203201) res = 17;	
			else if (source < 600825601) res = 18;
			else if (source < 632448001) res = 19;
			else if (source < 663984001) res = 20;
			else if (source < 695692801) res = 21;
			else if (source < 727315201) res = 22;
			else if (source < 758937601) res = 23;
			else if (source < 790473601) res = 24;
			else if (source < 822182401) res = 25;
			else if (source < 853804801) res = 26;
			else if (source < 885427201) res = 27;
			else if (source < 916963201) res = 28;
			else if (source < 948672001) res = 29;
			else if (source < 980294401) res = 30;
			else if (source < 1011916801) res = 31;
			else if (source < 1043452801) res = 32;			
			else if (source < 1075161601) res = 33;
			else if (source < 1106784001) res = 34;
			else if (source < 1138406401) res = 35;
			else if (source < 1169942401) res = 36;
			else if (source < 1201651201) res = 37;
			else if (source < 1233273601) res = 38;
			else if (source < 1264896001) res = 39;
			else if (source < 1296432001) res = 40;
			else if (source < 1328140801) res = 41;
			else if (source < 1359763201) res = 42;
			else if (source < 1391385601) res = 43;
			else if (source < 1422921601) res = 44;
			else if (source < 1454630401) res = 45;
			else if (source < 1486252801) res = 46;
			else if (source < 1517875201) res = 47;
			else if (source < 1549411201) res = 48;
			else if (source < 1581120001) res = 49;
			else if (source < 1612742401) res = 50;
			else if (source < 1644364801) res = 51;
			else if (source < 1675900801) res = 52;
			else if (source < 1707609601) res = 53;
			else if (source < 1739232001) res = 54;
			else if (source < 1770854401) res = 55;
			else if (source < 1802390401) res = 56;
			else if (source < 1834099201) res = 57;
			else if (source < 1865721601) res = 58;
			else if (source < 1897344001) res = 59;
			else if (source < 1928880001) res = 60;
			else if (source < 1960588801) res = 61;
			else if (source < 1992211201) res = 62;
			else if (source < 2023833601) res = 63;
			else if (source < 2055369601) res = 64;
			else if (source < 2087078401) res = 65;
			else if (source < 2118700801) res = 66;
			else if (source < 2150323201) res = 67;
			else if (source < 2181859201) res = 68;
			else if (source < 2213568001) res = 69;
			else if (source < 2245190401) res = 70;
			else if (source < 2276812801) res = 71;
			else if (source < 2308348801) res = 72;
			else if (source < 2340057601) res = 73;
			else if (source < 2371680001) res = 74;
			else if (source < 2403302401) res = 75;
			else if (source < 2434838401) res = 76;
			else if (source < 2466547201) res = 77;
			else if (source < 2498169601) res = 78;
			else if (source < 2529792001) res = 79;
			else if (source < 2561328001) res = 80;
			else if (source < 2593036801) res = 81;
			else if (source < 2624659201) res = 82;
			else if (source < 2656281601) res = 83;
			else if (source < 2687817601) res = 84;
			else if (source < 2719526401) res = 85;
			else if (source < 2751148801) res = 86;
			else if (source < 2782771201) res = 87;
			else if (source < 2814307201) res = 88;
			else if (source < 2846016001) res = 89;
			else if (source < 2877638401) res = 90;
			else if (source < 2909260801) res = 91;
			else if (source < 2940796801) res = 92;
			else if (source < 2972505601) res = 93;
			else if (source < 3004128001) res = 94;
			else if (source < 3035750401) res = 95;
			else if (source < 3067286401) res = 96;
			else if (source < 3098995201) res = 97;
			else if (source < 3130617601) res = 98;
			else res = 99;
	
			return (uint)res * CONST_DAY_SECS;
		}

		private const int CONST_MIN_SECS = 60;
		private const int CONST_HOUR_SECS = 60 * 60;
		private const int CONST_DAY_SECS = 60 * 60 * 24;
		private const int CONST_YEAR_SECS = 60 * 60 * 24 * 365;
		private static readonly int[] month_days = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
	}
}