namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    public interface ITicketState
    {

        string NotifyDriver();
        int NotifyServer(int respuestaGarmin);
        void DriverReject();
        void DriverAccept();
        void CancelService();
        int GetCode();
        int GetAcceptedCode();
        int GetRejectedCode();
    }
}
