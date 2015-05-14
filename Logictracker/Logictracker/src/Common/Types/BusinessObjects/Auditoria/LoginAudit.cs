#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Auditoria
{
    [Serializable]
    public class LoginAudit : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual string IP { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime? FechaFin { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual Type TypeOf()
        {
            return GetType();
        }
    }
}
