#region Usings

using System.Collections.Generic;
using Logictracker.Web.Controls.BindableListControl;

#endregion

namespace Logictracker.Web.Controls
{
    public interface IBindableControl
    {
        BindEntities Entity { get; set; }
        bool AddAllItem { get; set; }
        bool AddNoneItem { get; set; }
        IEnumerable<int> GetParentSelected(BindEntities entity);
        void ClearItems();
        void AddItem(string text, int value);
        void AddItem(string text, int value, OptionGroupInfo optionGroup);
    }
}
