#region Usings

using System.Collections.Generic;

#endregion

namespace Logictracker.Utils
{
    public interface ICustomComparer<T>: IComparer<T>
    {
        bool Descending { get; set; }
    }
}
