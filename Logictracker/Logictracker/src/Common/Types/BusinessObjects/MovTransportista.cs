using System;

namespace Urbetrack.Types.BusinessObjects
{
    [Serializable]
    public class MovTransportista
    {
        public virtual int Id {get;set;}
        public virtual Usuario Usuario { get; set; }
        public virtual Transportista Transportista { get; set; }
    }
}
