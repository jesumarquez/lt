using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class AtencionEvento : IAuditable
    {
        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual LogMensaje LogMensaje { get; set; }
        public virtual Mensaje Mensaje { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Observacion { get; set; }
    }
}