using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Support
{
    public class Subcategoria : IAuditable, IHasCategoria
    {
        public virtual Type TypeOf() { return GetType(); }
        public virtual Categoria CategoriaObj { get; set; }

        public virtual int Id { get; set;}
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
    }
}
