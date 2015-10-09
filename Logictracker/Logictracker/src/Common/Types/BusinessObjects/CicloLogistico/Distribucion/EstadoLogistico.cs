using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class EstadoLogistico : IAuditable, IHasEmpresa
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }        
        public virtual Mensaje MensajeInicio { get; set; }
        public virtual Mensaje MensajeFin { get; set; }
        public virtual Icono Icono { get; set; }
        public virtual TipoReferenciaGeografica TipoGeocercaInicio { get; set; }
        public virtual TipoReferenciaGeografica TipoGeocercaFin { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual short Demora { get; set; }
        public virtual bool Productivo { get; set; }
        public virtual bool Baja { get; set; }
    }
}