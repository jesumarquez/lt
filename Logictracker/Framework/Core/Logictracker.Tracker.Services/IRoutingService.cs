using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Tracker.Services
{
    public interface IRoutingService
    {
        void BuildProblem();
        void FillTable();
    }
}
