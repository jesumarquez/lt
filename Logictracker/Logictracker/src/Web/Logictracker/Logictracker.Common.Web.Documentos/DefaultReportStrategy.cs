#region Usings

using System;
using System.Collections;
using Logictracker.Web.Documentos.Interfaces;

#endregion

namespace Logictracker.Web.Documentos
{
    public class DefaultReportStrategy: IReportStrategy
    {
        #region IReportStrategy Members

        public IList GetData(params object[] paremeters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}