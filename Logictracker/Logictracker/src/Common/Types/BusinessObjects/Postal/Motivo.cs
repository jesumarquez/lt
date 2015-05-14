#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Postal
{
    [Serializable]
    public class Motivo : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool? EsEntrega { get; set; }
        public virtual int Orden { get; set; }
        public virtual bool? EsDevolucion { get; set; }
        public virtual bool? EsGestion { get; set; }
        public virtual DateTime FechaModificacion { get; set; }
        public virtual DateTime? FechaBaja { get; set; }
        public virtual string Codigo { get; set; }

        public virtual Type TypeOf() { return GetType(); }

        public override Boolean Equals(Object obj)
        {
            var castObj = obj as Motivo;

            return castObj != null && castObj.Id.Equals(Id);
        }

        public override int GetHashCode() { return Id.GetHashCode(); }
    }
}
