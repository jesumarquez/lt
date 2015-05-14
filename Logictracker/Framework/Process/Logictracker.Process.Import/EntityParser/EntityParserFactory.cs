using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import.EntityParser
{
    public static class EntityParserFactory
    {
        private static readonly Assembly Assembly;
        private static readonly Dictionary<int, IDictionary<int, string>> ParserTypes = new Dictionary<int, IDictionary<int, string>>();
 
        static EntityParserFactory()
        {
            Assembly = typeof (EntityParserFactory).Assembly;
        }

        public static IEntityParser GetEntityParser(IData data)
        {
            try
            {
                if(!ParserTypes.ContainsKey(data.Entity))
                {
                    int i;
                    var baseClassName = GetEntityParserClassName(data.Entity);
                    var versionDict = Assembly.GetTypes()
                        .Where(t => t.FullName.StartsWith(baseClassName + "V"))
                        .Where(t => int.TryParse(t.FullName.Substring(baseClassName.Length + 1), out i))
                        .ToDictionary(t => Convert.ToInt32(t.FullName.Substring(baseClassName.Length + 1)), t => t.FullName);

                    ParserTypes.Add(data.Entity, versionDict);
                }

                var versionDictionary = ParserTypes[data.Entity];

                for (var i = data.Version; i > 0; i--)
                {
                    if (versionDictionary.ContainsKey(i))
                    {
                        return Assembly.CreateInstance(versionDictionary[i]) as IEntityParser;
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string GetEntityParserClassName(int entity)
        {
            var className = Enum.GetName(typeof (Entities), entity);
            return typeof(IEntityParser).Namespace + "." + className;
        }
    }
}
