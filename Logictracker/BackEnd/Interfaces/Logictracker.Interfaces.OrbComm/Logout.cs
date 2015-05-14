using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class Logout
    {
        public int Result { get; set;}

        public Logout(XmlNode node)
        {
            if (node.Name.ToUpper() != "LOGOUT") throw new ApplicationException("El nodo no es de tipo LOGOUT");

            Result = node.GetInt32("RESULT");
        }
    }
}
