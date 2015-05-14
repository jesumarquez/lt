
using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import.EntityParser
{
    public interface IEntityParser
    {
        object Parse(int empresa, int linea, IData data);
        void SaveOrUpdate(object parsedData, int empresa, int linea, IData data);
        void Delete(object parsedData, int empresa, int linea, IData data);
        void Save(object parsedData, int empresa, int linea, IData data);
        void Update(object parsedData, int empresa, int linea, IData data);
    }
}
