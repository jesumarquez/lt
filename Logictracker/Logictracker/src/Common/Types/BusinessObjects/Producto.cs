using System;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Producto : IAuditable, ISecurable, IHasBocaDeCarga
    {
        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual BocaDeCarga BocaDeCarga { get; set; }

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual bool Baja { get; set; }
        public virtual bool UsaPrefijo { get; set; }

        public override string ToString() { return Descripcion; }
    }
}