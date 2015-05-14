#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.Web.Monitor.ContextMenu
{
    [Serializable]
    public class ContextMenu: Control
    {
        private readonly List<ContextMenuItem> items;
        private readonly string style = "olPopupMenu";

        public ContextMenu(string name)
            : this(name, null)
        {
            items = new List<ContextMenuItem>();
        }
        public ContextMenu(string name, string style) : base(name, string.Empty)
        {
            items = new List<ContextMenuItem>();
            if (!string.IsNullOrEmpty(style))
                this.style = style;
        }

        public override string Code
        {
            get { return string.Format("new OL.XM({{items: {0}, style: '{1}' }})", GetItemArray(), style); }
            set { base.Code = value; }
        }

        public void AddItem(ContextMenuItem item)
        {
            items.Add(item);
        }

        private string GetItemArray()
        {
            var it = "new Array(";
            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0) it += ",";
                it += items[i].Code;
            }
            it += ")";
            return it;
        }
    }
}