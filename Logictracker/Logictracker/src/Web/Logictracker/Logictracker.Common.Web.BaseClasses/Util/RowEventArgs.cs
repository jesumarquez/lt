using System;
using C1.Web.UI.Controls.C1GridView;

namespace Logictracker.Web.BaseClasses.Util
{
    public class RowEventArgs<T>:EventArgs
    {
        public C1GridView Grid { get; set;}
        public T DataItem { get; set; }
        public C1GridViewRowEventArgs Event { get; set; }

        public RowEventArgs(C1GridView grid, T dataItem, C1GridViewRowEventArgs @event)
        {
            Grid = grid;
            DataItem = dataItem;
            Event = @event;
        }
    }
}
