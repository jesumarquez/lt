using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static bool IncludesAll(IEnumerable<int> list)
        {
            return list == null || !list.Any() || list.Contains(-1) || list.Contains(0);
        }
        public static bool IncludesNone(IEnumerable<int> list)
        {
            return list.Contains(-2);
        }
        public static bool IncludesAll(IEnumerable<string> list)
        {
            return list == null || !list.Any() || list.Contains("-1") || list.Contains("0");
        }
        public static bool IncludesNone(IEnumerable<string> list)
        {
            return list.Contains("-2");
        }
    }
}
