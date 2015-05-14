#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor
{
    [Serializable]
    public class Layer
    {
        public Layer(string name, string code, bool requireGoogleMapsScript)
        {
            Name = name;
            Code = code;
            RequireGoogleMapsScript = requireGoogleMapsScript;
        }
        public Layer(string name, string code)
        {
            Name = name;
            Code = code;
            RequireGoogleMapsScript = false;
        }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool RequireGoogleMapsScript { get; set; }

        public string PostCode { get; set; }

        public bool executePostCode()
        {
            return !String.IsNullOrEmpty(PostCode);
        }
    }
}