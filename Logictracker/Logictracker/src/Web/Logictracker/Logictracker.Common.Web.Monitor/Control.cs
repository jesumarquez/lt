#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor
{
    [Serializable]
    public class Control
    {
        private string code;
        public Control(string name, string code)
        {
            Name = name;
            this.code = code;
        }

        public string Name { get; set; }

        public virtual string Code
        {
            get { return code; }
            set { code = value; }
        }
    }
}