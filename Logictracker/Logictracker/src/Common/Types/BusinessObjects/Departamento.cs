using System;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Departamento: IDataIdentify, IAuditable, IHasEmpresa, IHasLinea, IHasEmpleado
    {
        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Empleado Empleado { get; set; }

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
    }
}
