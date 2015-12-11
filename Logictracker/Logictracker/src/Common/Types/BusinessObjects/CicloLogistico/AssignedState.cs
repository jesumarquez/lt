using System;
using System.Runtime.InteropServices;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    public class AssignedTicketState : ITicketState
    {
        Garmin garmin = new Garmin();
        public SosTicket Ticket { get; set; }

        public const int CodigoEstado = 5;

        public AssignedTicketState(SosTicket ticket)
        {
            Ticket = ticket;
        }

        public string NotifyDriver()
        {
            //S:	20151119303932
            //O:	SAN MARTIN  500, CORDOBA
            //D:	COLON 1000, CORDOBA
            //Di:	CORREA DE DISTRIBUCION
            return string.Format("S:{0}<br>O:{1}<br>D:{2}<br>Di:{3}",
                Ticket.NumeroServicio,
                Ticket.Origen.Direccion + ", " + Ticket.Origen.Localidad,
                Ticket.Destino.Direccion + ", " + Ticket.Destino.Localidad,
                Ticket.Diagnostico);
        }

        public int NotifyServer(int respuestaGarmin)
        {
            return respuestaGarmin;
        }

        public void DriverReject()
        {
            throw new NotImplementedException();
        }

        public void DriverAccept()
        {
            throw new NotImplementedException();
        }

        public void CancelService()
        {
            throw new NotImplementedException();
        }

        public int GetCode()
        {
            return 35;
        }

        public int GetAcceptedCode()
        {
            return 45;
        }
        
        public int GetRejectedCode()
        {
            return 40;
        }
    }
}
