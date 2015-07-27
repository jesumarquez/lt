using System;
using System.Security;
using TraxEmu.Componentes;

namespace TraxEmu
{
    class DirtyParser
    {
        private IOPorts ports;
        private Disparadores disparadores;

        public DirtyParser(IOPorts ports)
        {
            this.ports = ports;
        }

        string Parse(string cmd)
        {
            return cmd.StartsWith(">Q")
                ? ProcessQuery(cmd.Substring(2))
                : (cmd.StartsWith(">S") ? ProcessSet(cmd.Substring(2)) : ResponseError(cmd));
        }


        private string ProcessQuery(string cmd)
        {
            var code = cmd.Substring(0, 2);
            switch (code)
            {
                case "AD":
                    return QAD();
                case "GS":
                    return QGS(cmd.Substring(2));
                case "TD":
                    return QTD(cmd.Substring(2));
            }
            return string.Empty;
        }

        private string QTD(string cmd)
        {
            throw new NotImplementedException();
        }

        private string ResponseError(string cmd)
        {
            var code = cmd.Substring(0, 2);
            return "E" + code + ":MSGID";
        }

        private string ProcessSet(string cmd)
        {
            var code = cmd.Substring(0, 2);
            switch (code)
            {
                case "GS":
                    return SGS(cmd.Substring(2));
                case "TD":
                    return STD(cmd.Substring(3));
            }
            return string.Empty;
        }

        private string STD(string cmd)
        {
            throw new NotImplementedException();
        }


        private string SGS(string cmd)
        {
            var n = int.Parse(cmd.Substring(0, 2));
            var min = int.Parse(cmd.Substring(2, 3));
            var max = int.Parse(cmd.Substring(5, 3));
            disparadores.GSMax[n] = max;
            disparadores.GSMin[n] = min;
            return RGS(n);
        }

        private string QGS(string cmd)
        {
            var n = int.Parse(cmd.Substring(0, 2));
            return RGS(n);
        }

        private string RGS(int n)
        {
            return string.Format("RGS{0:00}{1:000}{2:000}", n, disparadores.GSMin[n], disparadores.GSMax[n]);
        }


        private string QAD()
        {
            return string.Format("RAD00{0:ddMMyyHHmmss}{1:0000}{2:0000}{3:0000}{4:0000}", DateTime.UtcNow, ports.IN4,
                ports.IN5, ports.AuxBat, ports.MainBat);
        }
    }
}
