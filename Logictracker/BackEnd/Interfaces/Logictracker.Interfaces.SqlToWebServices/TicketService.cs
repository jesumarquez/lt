using System;
using System.Data.SqlTypes;
using Logictracker.Interfaces.SqlToWebServices.Service;
using Microsoft.SqlServer.Server;

namespace Logictracker.Interfaces.SqlToWebServices
{
    public class TicketService
    {
        private static TicketsSoapClient GetService()
        {
            return new TicketsSoapClient(ServiceConfig.GetHttpBinding(), ServiceConfig.GetEndpointAddress("Tickets.asmx"));
        }

        [SqlProcedure]
        public static void Login(SqlString username, SqlString password, out SqlString sessionId, out SqlString errorMessage)
        {
            var service = GetService();

            var respuesta = service.Login(username.ToString(), password.ToString());
			sessionId = new SqlString(respuesta.RespuestaOk ? respuesta.Resultado : String.Empty);
            errorMessage = new SqlString(respuesta.Mensaje);
        }

        [SqlProcedure]
        public static void IsActive(SqlString sessionId, out SqlBoolean active, out SqlString errorMessage)
        {
            var service = GetService();

            var respuesta = service.IsActive(sessionId.ToString());
            active = new SqlBoolean(respuesta.RespuestaOk && respuesta.Resultado);
            errorMessage = new SqlString(respuesta.Mensaje);
        }

        [SqlProcedure]
        public static void Logout(SqlString sessionId, out SqlBoolean done, out SqlString errorMessage)
        {
            var service = GetService();

            var respuesta = service.Logout(sessionId.ToString());
            done = new SqlBoolean(respuesta.RespuestaOk && respuesta.Resultado);
            errorMessage = new SqlString(respuesta.Mensaje);
        }

        [SqlProcedure]
        public static void AssignAndInit(SqlString sessionId, SqlString company, SqlString branch, SqlDateTime utcDate,  SqlString clientCode, SqlString deliveryPointCode, SqlInt32 tripNo, SqlString vehicleDomain, SqlString driver, SqlString load, out SqlBoolean done, out SqlString errorMessage)
        {
            var service = GetService();

            var respuesta = service.AssignAndInit(sessionId.ToString(), company.ToString(), branch.ToString(), utcDate.Value, clientCode.ToString(), deliveryPointCode.ToString(), tripNo.Value, vehicleDomain.ToString(), driver.ToString(), load.ToString());
            done = new SqlBoolean(respuesta.RespuestaOk && respuesta.Resultado);
            errorMessage = new SqlString(respuesta.Mensaje);
        }
    }
}
