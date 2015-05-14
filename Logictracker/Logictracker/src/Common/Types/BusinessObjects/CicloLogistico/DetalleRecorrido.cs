using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    [Serializable]
    public class DetalleRecorrido: IAuditable
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual double Latitud { get; set; }
        public virtual double Longitud { get; set; }
        public virtual int Orden { get; set; }
        public virtual double Distancia { get; set; }
    }
}
