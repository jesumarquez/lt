#region Usings

using System.Collections;

#endregion

namespace Logictracker.Web.Documentos.Interfaces
{
    public interface IReportStrategy
    {
        IList GetData(params object[] paremeters);
    }
}