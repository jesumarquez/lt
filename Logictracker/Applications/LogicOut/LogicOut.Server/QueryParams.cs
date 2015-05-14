using System.Collections.Generic;
using System.Linq;

namespace LogicOut.Server
{
    public class QueryParams : Dictionary<string, string>
    {
        public static QueryParams Create(string parameters)
        {
            var pair = parameters.Split(';');
            var list = new QueryParams();
            foreach (var p in pair.Select(s => s.Trim()).Where(p => !string.IsNullOrEmpty(p)))
            {
                if (p.IndexOf('=') < 0)
                {
                    if (list.ContainsKey(p)) list[p] = string.Empty;
                    else list.Add(p, string.Empty);
                }
                else
                {
                    var v = p.Split('=');
                    var k = v[0].Trim();
                    var l = v[1];
                    if (list.ContainsKey(k)) list[k] = l;
                    else list.Add(k, l);
                }
            }
            return list;
        }
    }
}
