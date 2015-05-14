#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{
    [Serializable]
    public class EventoCombustible : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime Fecha { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual string MensajeDescri { get; set; }
        public virtual Tanque Tanque { get; set; }
        public virtual Caudalimetro Motor { get; set; }
        public virtual Mensaje Mensaje { get; set; }
        public virtual Accion Accion { get; set; }
    }
}
