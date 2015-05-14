#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Web.Controls.BindableListControl;

#endregion

namespace Logictracker.Web.Controls
{
    public interface IBindableListControl : IBindableControl
    {
        IEnumerable<int> Selected { get; set; }
        event EventHandler SelectedIndexChanged;
        void DataBind();
        IBindableListControl GetParent(BindEntities entity);
        string ParentControls { get; set; }
        void RegisterChild(IBindableListControl control);
        void ClearSelection();
        void SelectItem(int value);
    }
}
