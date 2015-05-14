using System;

namespace Urbetrack.Types.BusinessObjects
{
    [Serializable]
    public class MovLinea
    {
        public virtual int Id { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Linea Linea { get; set; }
    }
}
