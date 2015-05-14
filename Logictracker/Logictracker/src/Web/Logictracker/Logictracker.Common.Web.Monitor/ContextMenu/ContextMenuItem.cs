#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor.ContextMenu
{
    [Serializable]
    public class ContextMenuItem
    {
        public ContextMenuItem(ContextMenuItemBehaviourTypes behaviourType, string text, string args, string style, bool enabled)
        {
            BehaviourType = behaviourType;
            Text = text;
            BehaviourArguments = args;
            Style = style;
            Enabled = enabled;
        }

        public ContextMenuItemBehaviourTypes BehaviourType { get; set; }

        public string Text { get; set; }

        public string BehaviourArguments { get; set; }

        public string Style { get; set; }

        public bool Enabled { get; set; }

        public string Code
        {
            get
            {
                return string.Format("new OL.XI({0}, '{1}', '{2}', '{3}', {4})", (int)BehaviourType, Text, BehaviourArguments, Style, Enabled?"true":"false");
            }
        }

        public static ContextMenuItem Separator
        {
            get { return new ContextMenuItem(ContextMenuItemBehaviourTypes.None, " ", "", "olContextMenuSeparator", true); }
        }
    }
}