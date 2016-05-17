using System;
using System.Collections;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Positions;


namespace Logictracker.Tracker.Services
{
    public interface IRouteService
    {
        IList<Mensaje> GetProfileMessages(string deviceId);
        Empleado GetEmployeeByDeviceImei(string imei);
        ViajeDistribucion GetDistributionRouteById(int routeId);
        IList<ViajeDistribucion> GetAvailableRoutes(string deviceId);
        IList<ViajeDistribucion> GetAvailableRoutes(string deviceId, string EmpresaId, string LineaId, string TransportistaId, string Query);
        ViajeDistribucion setRoute(string deviceId, string Patente, string Chofer, string Ruta);

        string StartRoute(int routeId);
        string FinalizeRoute(int id);
        short ReportDelivery(int routeId, long jobId, Coordinate coord, int messageId, short jobStatus, string deviceId, DateTime dateTime);

        //void CreateGarminMessage(int jobId, Coordinate coordinate, int messageId, short status);
        string SendMessageByRouteAndDelivery(int routeId, string messageCode, string text, DateTime dateTime,
            long deliveryId, float lat, float lon, string deviceId);
        IList<LogMensaje> GetMessagesMobile(string deviceId);
        bool SendMessagesMobile(string deviceId, List<LogMensaje> mensajes);
        int GetMobileIdByImei(string deviceId);
        IList<LogMensaje> GetMessagesMobile(string getDeviceId, DateTime dt);
    }
}
