using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Sync
{
    [Serializable]
    public class OutState: IAuditable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual OutQueue OutQueue { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual string Server { get; set; }
        public virtual bool Sincronizado { get; set; }
        public virtual bool Ok { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Observaciones { get; set; }
    }
}
