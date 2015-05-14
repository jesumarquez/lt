using System.Collections.Generic;
using C1.Web.UI.Controls.C1GridView;
using System.Web.UI;

namespace Logictracker.Web.BaseClasses.Util
{
    public interface IGridded<T>
    {
        C1GridView Grid { get; }
        List<T> Data { get; set; }
        string SearchString { get; set; }
        StateBag StateBag { get; }
        int PageSize { get; }
        bool SelectableRows { get; }
        bool MouseOverRowEffect { get; }
        OutlineMode GridOutlineMode { get; }
        bool HasTotalRow { get; }
    }
}
