using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicOut.Server
{
    [Serializable]
    public class OutData
    {
        public string[] Keys = new string[0];
        public string[] Values = new string[0];

        public string Entity { get; set; }

        [NonSerialized]
        protected readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>();

        public OutData()
        {
         
        }
        public OutData(string entity)
        {
            Entity = entity;
        }
        
        public void AddProperty(string name, string value)
        {
            if (Dictionary.ContainsKey(name)) Dictionary[name] = value;
            else Dictionary.Add(name, value);
            Pack();
        }

        public string this[string key]
        {
            get
            {
                if (Keys.Length == 0) Pack();
                for (var i = 0; i < Keys.Length; i++)
                    if (Keys[i] == key) return Values[i];

                return null;
            }
        }
        private void Pack()
        {
            Keys = Dictionary.Keys.ToArray();
            Values = Dictionary.Values.ToArray();
        }
    }
}
