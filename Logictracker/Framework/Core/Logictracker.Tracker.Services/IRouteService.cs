using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Positions;


namespace Logictracker.Tracker.Services
{
    public interface IRouteService
    {
        IList<Mensaje> GetAllMessages(string deviceId);
        Empleado GetEmployeeByDeviceImei(string imei);
        ViajeDistribucion GetDistributionRouteById(int routeId);
        IList<ViajeDistribucion> GetAvailableRoutes(string deviceId);
        string StartRoute(int routeId);
        string FinalizeRoute(int id);
        short ReportDelivery(int routeId, long jobId, Coordinate coord, int messageId, short jobStatus, string deviceId);

        //void CreateGarminMessage(int jobId, Coordinate coordinate, int messageId, short status);
        string ReceiveMessageByRouteAndDelivery(int routeId, string messageCode, string text, DateTime dateTime,
            long deliveryId, float lat, float lon, string deviceId);
        IList<LogMensaje> GetMessagesMobile(string deviceId);
        int GetMobileIdByImei(string deviceId);
    }
}
