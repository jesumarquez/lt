#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{
    [Serializable]
    public class TipoMovimiento : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual String Descripcion { get; set; }
        public virtual String Codigo { get; set; }

        public virtual Type TypeOf() { return GetType(); }
    }
}
