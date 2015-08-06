#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class PuntoEntrega : IAuditable, IHasCliente
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Mail { get; set; }
        public virtual string Nombre { get; set; }
        public virtual double Importe { get; set; }
        public virtual bool Baja { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Empleado Responsable { get; set; }

        public virtual bool Nomenclado { get; set; }

        public virtual string DireccionNomenclada { get; set; }

        public virtual string Comentario1 { get; set; }
        public virtual string Comentario2 { get; set; }
        public virtual string Comentario3 { get; set; }

        public PuntoEntrega()
        {
            Nomenclado = true;
        }
    }
}
