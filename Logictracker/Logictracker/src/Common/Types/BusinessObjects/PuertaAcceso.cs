using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class PuertaAcceso : IAuditable, ISecurable, IHasZonaAccesoEntrada, IHasZonaAccesoSalida
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual short Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual ZonaAcceso ZonaAccesoEntrada { get; set; }
        public virtual ZonaAcceso ZonaAccesoSalida { get; set; }
    }
}
