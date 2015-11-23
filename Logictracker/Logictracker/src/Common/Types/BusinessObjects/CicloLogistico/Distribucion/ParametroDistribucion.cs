using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using System;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class ParametroDistribucion
    {
        public virtual int Id { get; set; }
        public virtual ViajeDistribucion ViajeDistribucion { get; set; }
        public virtual string Adicional { get; set; }
    Prioridad" not-null="true" />
    Diagnostico" not-null="true" />
    Observacion" not-null="true" />
    Operador" not-null="true" />
    Patente" not-null="true" />
    Color" not-null="true" />
    Marca" not-null="true" />
    PreasignacionNotificada" not-null="true" />
    AsignacionNotificada" not-null="true" />
    CancelacionNotificada" not-null="true" />
    Preasignado" not-null="true" />
    Asignado" not-null="true" />
    Cancelado" not-null="true" />
    }
}
