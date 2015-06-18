using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class MensajeIgnorado : IAuditable, IHasDispositivo
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual string Codigo { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
    }
}