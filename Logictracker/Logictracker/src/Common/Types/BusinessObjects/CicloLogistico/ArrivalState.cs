using System;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    class ArrivalTicketState : ITicketState
    {
        public string NotifyDriver()
        {
            throw new NotImplementedException();
        }

        public int NotifyServer(int respuestaGarmin)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int GetAcceptedCode()
        {
            throw new NotImplementedException();
        }

        public int GetRejectedCode()
        {
            throw new NotImplementedException();
        }
    }
}
