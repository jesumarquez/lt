#region Usings

using System;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class HistoriaGeoRef : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual Direccion Direccion { get; set; }
        public virtual Poligono Poligono { get; set; }
        public virtual Vigencia Vigencia { get; set; }

        public HistoriaGeoRef()
        {
            Vigencia = new Vigencia();
        }
    }
}
