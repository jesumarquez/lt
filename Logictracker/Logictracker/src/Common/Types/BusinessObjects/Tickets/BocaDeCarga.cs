using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class BocaDeCarga: IAuditable, IHasLinea
    {
        Type IAuditable.TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual int Rendimiento { get; set; }
        public virtual int HorasLaborales { get; set; }
        public virtual int HoraInicioActividad { get; set; }
        public virtual bool Baja { get; set; }
    }
}
