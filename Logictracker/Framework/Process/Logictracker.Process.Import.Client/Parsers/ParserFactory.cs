using System;
using Logictracker.Process.Import.Client.Mapping;
using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import.Client.Parsers
{
    public static class ParserFactory
    {
        public static BaseParser GetEntityParser(Entity entity)
        {
            var ent = (Entities) Enum.Parse(typeof (Entities), entity.Type, true);
            return new BaseParser(ent, entity);
        }
    }
}
