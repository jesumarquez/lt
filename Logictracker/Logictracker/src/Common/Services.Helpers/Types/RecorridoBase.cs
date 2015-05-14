#region Usings

using System.Xml;

#endregion

namespace Logictracker.Services.Helpers.Types
{
    public abstract class RecorridoBase
    {
        public Direccion DireccionInicial { get; set; }
        public Direccion DireccionFinal { get; set; }

        internal abstract void FromXml(XmlNode node);
        public abstract void Calcular();
    }
}
