#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor.ContextMenu
{
    [Serializable]
    public class ContextMenuItemSeparator: ContextMenuItem
    {
        public ContextMenuItemSeparator() : base(ContextMenuItemBehaviourTypes.None, string.Empty,
                                                 string.Empty, string.Empty, false)
        {
        }
    }
}