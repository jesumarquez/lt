#region Usings

using System.Web.UI;

#endregion

namespace Logictracker.Web.Documentos.Interfaces
{
    public interface IDocumentView
    {
        Control DocumentContainer { get; }
        bool Enabled { get; set; }
        void RegisterScript(string key, string script);
        ClientScriptManager ClientScript { get; }
    }
}