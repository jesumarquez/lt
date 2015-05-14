using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class TipoMedicion : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual UnidadMedida UnidadMedida { get; set; }
        public virtual bool Baja { get; set; }
        public virtual bool ControlaLimites { get; set; }

        public override string ToString() { return Descripcion; }
    }
}