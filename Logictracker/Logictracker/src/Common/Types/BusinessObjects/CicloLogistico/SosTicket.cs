using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    [Serializable]
    public class SosTicket : IAuditable
    {
        public static class EstadosServicio
        {
            public const short Asignado = 1;
            public const short Preasignado = 2;
            public const short AsignacionCancelada = 3;
            public const short PreAsignacionCancelada = 4;
            public const short AsignacionYPreAsignacionCancelada = 5;
        }

        public virtual int Id { get; set; }

        Type IAuditable.TypeOf()
        {
            return GetType();
        }
        
        public virtual string NumeroServicio { get; set; }
        public virtual int Movil { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Color { get; set; }
        public virtual string Marca { get; set; }
        public virtual string Diagnostico { get; set; }
        public virtual string Prioridad { get; set; }
        public virtual string Observacion { get; set; }
        public virtual DateTime HoraServicio { get; set; }
        public virtual string CobroAdicional { get; set; }
        public virtual int EstadoServicio { get; set; }
        public virtual LocationSos Origen { get; set; }
        public virtual LocationSos Destino { get; set; }
        public virtual string Operador { get; set; }
        public virtual string Tipo { get; set; }
        public virtual bool Rechazado { get; set; }
        public virtual bool PreasignacionNotificada { get; set; }
        public virtual bool AsignacionNotificada { get; set; }
        public virtual bool CancelacionNotificada { get; set; }
        public virtual DateTime? Preasignado { get; set; }
        public virtual DateTime? Asignado { get; set; }
        public virtual DateTime? Cancelado { get; set; }
        public virtual ViajeDistribucion Distribucion { get; set; }     

    }
}