using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.Process.Import.Client.Types
{
    [Serializable]
    public class Data: IData
    {
        public int Version { get { return 1; } }
        public int Entity { get; set; }
        public int Operation { get; set; }
        private int[] _properties = new int[0];
        private string[] _values = new string[0];

        public int[] Properties
        {
            get { return _properties; }
        }
        public string[] Values
        {
            get { return _values; }
        }

        [NonSerialized]
        protected Dictionary<int, string> Dictionary = new Dictionary<int, string>();

        public void Add(int property, string value)
        {
            if(property == 0)
            {
                property = Math.Min(0,Dictionary.Keys.Min()) - 1;
            }
            if(Dictionary.ContainsKey(property)) Dictionary[property] = value;
            else Dictionary.Add(property, value);
        }

        public string this[int property]
        {
            get
            {
                if (_properties.Length == 0) Pack();
                for (var i = 0; i < _properties.Length; i++)
                    if (_properties[i] == property) return Values[i];

                return null;
            }
        }

        public Dictionary<string, string> GetCustomFields()
        {
            var dictionary = new Dictionary<string, string>();
            for (var i = 0; i < _properties.Length; i++)
            {
                if (_properties[i] > 0) continue;
                var value = Values[i];
                var idx = value.IndexOf('=');
                if (idx < 0) continue;
                var key = value.Substring(0, idx);
                var val = idx < value.Length - 1 ? value.Substring(idx + 1): string.Empty;
                dictionary.Add(key, val.Trim('"'));
            }

            return dictionary;
        }

        public IData Pack()
        {
            _properties = Dictionary.Keys.ToArray();
            _values = Dictionary.Values.ToArray();
            return this;
        }
    }
}
