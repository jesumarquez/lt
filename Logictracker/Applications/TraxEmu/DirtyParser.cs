using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraxEmu
{
    class DirtyParser
    {
        string Parse(string cmd)
        {
            if (cmd.StartsWith(">QGRI"))
               return ResposeIMEI(cmd);
            return string.Empty;
        }

        private string ResposeIMEI(string cmd)
        {
            return "RGRI0123456789012345";
        }
    }
}
