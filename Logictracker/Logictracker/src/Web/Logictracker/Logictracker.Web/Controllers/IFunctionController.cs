using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logictracker.Web.Controllers
{
    public interface IFunctionController
    {
        string VariableName { get; }
        string GetRefference();
    }
}
