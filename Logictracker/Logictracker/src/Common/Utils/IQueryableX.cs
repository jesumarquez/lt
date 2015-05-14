using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;

namespace Logictracker.Utils
{
	public static class IQueryableX
	{
		public static T SafeFirstOrDefault<T>(this IEnumerable<T> me)
		{
			var l = me.ToList();

			if (l.Count > 1)
			{
				var context = new Dictionary<String, String>
					{
						{"StackTrace", Environment.StackTrace },
					};

				for (var i = 0; i < l.Count; i++)
				{
					var el = l[i];
					var data = String.Format("Type: {0}, ToString: {1}", el.GetType(), el);
					/*if (el is IAuditable)
					{
						var tt = el as IAuditable;
						data = "Id: " + tt.Id + " " + data;
					}
					else if (el is IDataIdentify)
					{
						var tt = el as IDataIdentify;
						data = "Id: " + tt.Id + " " + data;
					}*/
					context.Add(String.Format("Elem{0:D2}", i), data);
				}

				STrace.Log(typeof(IQueryableX).FullName, null, 0, LogTypes.Trace, context, "Se detecto mas de un elemento antes de FirstOrDefault!");
			}

			return l.FirstOrDefault();
		}

		public static T SafeFirstOrDefault<T>(this IQueryable<T> me)
		{
			return me.AsEnumerable().SafeFirstOrDefault();
		}
	}
}
