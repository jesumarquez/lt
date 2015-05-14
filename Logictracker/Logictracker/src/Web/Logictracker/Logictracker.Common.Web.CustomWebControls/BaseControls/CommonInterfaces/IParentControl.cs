#region Usings

using System.Collections.Generic;

#endregion

namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces
{
    public interface IParentControl
    {
        IEnumerable<IAutoBindeable> ParentControls { get; }
    }
}
