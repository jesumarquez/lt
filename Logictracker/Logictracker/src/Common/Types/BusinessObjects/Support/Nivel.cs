using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Support
{
    public class Nivel : IAuditable, IHasEmpresa
    {
        public virtual Type TypeOf() { return GetType(); }
        public virtual Empresa Empresa { get; set; }

        public virtual int Id { get; set;}
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
    }
}
