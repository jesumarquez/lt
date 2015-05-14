#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Equipo : IAuditable, ISecurable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        /// <summary>
        /// Codigo de indentificacion del equipo
        /// </summary>
        public virtual string Codigo { get; set; }

        /// <summary>
        /// Descripcion del Equipo
        /// </summary>
        public virtual string Descripcion { get; set; }

        /// <summary>
        /// Estado del equipo. True = Inactivo
        /// </summary>
        public virtual bool Baja { get; set; }

        /// <summary>
        /// Cliente al que pertenece el Equipo
        /// </summary>
        public virtual Cliente Cliente { get; set; }

        /// <summary>
        /// Empleado encargado del Equipo
        /// Puede ser NULL
        /// </summary>
        public virtual Empleado Empleado { get; set; }

        /// <summary>
        /// Tarjeta asignada al Equipo
        /// Puede ser NULL. 
        /// </summary>
        public virtual Tarjeta Tarjeta { get; set; }

        /// <summary>
        /// Ubicacion del Equipo
        /// Puede ser NULL
        /// </summary>
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }

        /// <summary>
        /// Centro de costos al que pertenece el Equipo.
        /// </summary>
        public virtual CentroDeCostos CentroDeCostos { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var obra = obj as Equipo;
            if (obra == null) return false;
            return Id == obra.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}
