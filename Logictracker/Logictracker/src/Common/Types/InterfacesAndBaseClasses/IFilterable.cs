#region Usings

using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.InterfacesAndBaseClasses
{
    public interface IFilterable : IAuditable
    {
        Empresa Empresa { get; set; }
        Linea Linea { get; set; }
        CentroDeCostos CentroDeCostos { get; set; }
    }
}
