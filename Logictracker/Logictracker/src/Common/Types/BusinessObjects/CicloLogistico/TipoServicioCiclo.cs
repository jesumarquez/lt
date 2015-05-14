using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    public class TipoServicioCiclo: IAuditable, ISecurable
    {
        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual int Demora { get; set; }
        public virtual bool Default { get; set; }
        public virtual bool Baja { get; set; }
    }
}
