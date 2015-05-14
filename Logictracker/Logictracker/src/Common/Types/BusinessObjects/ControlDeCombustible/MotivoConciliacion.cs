#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{
    [Serializable]
    public class MotivoConciliacion : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual String Descripcion { get; set; }
        public virtual bool Asignable { get; set; }

        public virtual Type TypeOf() { return GetType(); }
    }
}
