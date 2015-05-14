using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class CentroDeCostos : IAuditable, ISecurable, IHasDepartamento, IHasEmpleado
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual Empleado Empleado { get; set; }

        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string NombreEmpresa { get; set; }
        public virtual bool Baja { get; set; }

        public virtual bool GeneraDespachos { get; set; }
        public virtual bool InicioAutomatico { get; set; }
        public virtual DateTime? HorarioInicio { get; set; }

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as CentroDeCostos;

            return castObj != null && castObj.Id.Equals(Id) && Id != 0;
        }

        public override int GetHashCode() { return Id.GetHashCode(); }

        #endregion
    }
}

