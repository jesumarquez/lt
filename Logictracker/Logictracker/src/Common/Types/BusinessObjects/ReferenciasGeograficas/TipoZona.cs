using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class TipoZona: IAuditable, ISecurable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
    }
}
