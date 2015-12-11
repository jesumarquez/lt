using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    [Serializable]
    public class SosTicket : IAuditable
    {
        public virtual int Id { get; set; }

        Type IAuditable.TypeOf()
        {
            return GetType();
        }

        private ITicketState TicketState;
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

        public virtual string NotifyDriver()
        {
            return TicketState.NotifyDriver();
        }

        public virtual int NotifyServer(int respuestaGarmin)
        {
            return TicketState.NotifyServer(respuestaGarmin);
        }

        public virtual void DriverReject()
        {
            TicketState.DriverReject();
        }

        public virtual void DriverAccept()
        {
            TicketState.DriverAccept();
        }

        public virtual void CancelService()
        {
            TicketState.CancelService();
        }

        public virtual void SetStatus()
        {
            switch (EstadoServicio)
            {
                case 1: //servicio asignado 
                    //if()
                    TicketState = new AssignedTicketState(this);
                    break;
                case 2: //servicio prea asignado 
                    TicketState = new PreassignedTicketState();
                    break;
                case 3: //asignación cancelada 
                case 4: //pre asignado cancelado
                case 5: //asignación y pre asignado canceladas
                default:
                    TicketState = new CanceledTicketState();
                    break;
            }
        }

        //public virtual ITicketState SetStatus(int oldStatus)
        //{
        //    if (oldStatus == 0) return SetStatus();

        //    switch (EstadoServicio)
        //    {
        //        case 1: //servicio asignado 
        //            return new AssignedTicketState(this);
        //        case 2: //servicio prea asignado 
        //            return new PreassignedTicketState();
        //        case 3: //asignación cancelada 
        //        case 4: //pre asignado cancelado
        //        case 5: //asignación y pre asignado canceladas
        //        default:
        //            return new CanceledTicketState();
        //    }
        //}
    }
}