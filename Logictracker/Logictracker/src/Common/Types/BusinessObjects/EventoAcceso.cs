#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class EventoAcceso : IAuditable, IHasEmpleado
    {
        public virtual int Id { get; set; }
        public virtual Empleado Empleado { set; get; }
        public virtual DateTime Fecha { set; get; }
        public virtual Boolean Entrada { set; get; }
        public virtual PuertaAcceso Puerta { set; get; }
        public virtual DateTime Alta { set; get; }
        public virtual DateTime? Baja { set; get; }
        public virtual DateTime? Modificado { set; get; }
        public virtual Usuario Usuario { set; get; }

        public virtual Type TypeOf(){return typeof (EventoAcceso);}
    }
}
