using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logitracker.Codecs.Sitrack;

namespace Logictracker.Tracker.Services
{
    public interface IReceptionService
    {
        void ParseSitrackPositions(List<SitrackFrame> positions);
        
        void ParseSitrackPositions(List<SitrackFrame> positions, String path);
    }
}
