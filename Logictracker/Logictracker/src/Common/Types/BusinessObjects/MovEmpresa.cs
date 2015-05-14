using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Urbetrack.Types.BusinessObjects
{
    [Serializable]
    public class MovEmpresa
    {
        public virtual int Id { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
