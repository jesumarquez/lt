using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Tracker.Services
{
    public interface IIntegrationService
    {
        void CheckServices();
        void ProcessService(SosTicket ticket);
        void Close();
    }
}