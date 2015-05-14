using System;
using System.Collections.Generic;

namespace Logictracker.Web.Controls
{
    public interface IAutoBindeable3
    {
        AutoBindingMode AutoBindingMode { get; }
        IEnumerable<int> ParentSelectedValues(AutoBindingMode mode);
        IAutoBindeable3 GetParent(AutoBindingMode mode);
        IEnumerable<int> SelectedValues { get; }
        void AddItem(ComboBoxItem item);
        void Clear();
        event EventHandler SelectedIndexChanged;
        bool AddAllItem { get; }
        bool AddNoneItem { get; }
    }
    public interface IAutoBindeableBase
    {
        event EventHandler SelectedIndexChanged;
    }
}
