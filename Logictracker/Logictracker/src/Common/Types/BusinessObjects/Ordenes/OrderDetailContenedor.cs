using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Types.BusinessObjects.Ordenes
{
    [Serializable]
    public class OrderDetailContenedor : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        public virtual int Ajuste { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual Contenedor Contenedor { get; set; }
    }
}
