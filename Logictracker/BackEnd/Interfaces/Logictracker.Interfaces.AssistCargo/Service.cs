using System;
using Logictracker.Configuration;
using Logictracker.Interfaces.AssistCargo.AssistCargoService;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;

namespace Logictracker.Interfaces.AssistCargo
{
    public class Service
    {
        private ServiceSoapClient _client;
        private ServiceSoapClient Client
        {
            get { return _client ?? (_client = new ServiceSoapClient()); }
        }

        public int ReportPosition(Coche coche, LogUltimaPosicionVo lastPosition)
        {
            return ReportEvent("P", coche, lastPosition);
        }
		public int ReportEvent(String eventName, Coche coche, LogPosicionVo lastPosition)
        {
            if (lastPosition == null) return -999;

            var result = Client.LoginYInsertarEvento(
                    Config.AssistCargo.AssistCargoWebServiceUser,
                    Config.AssistCargo.AssistCargoWebServicePassword,
                    coche.Patente,
                    "-1",
                    eventName,
                    lastPosition.Latitud,
                    lastPosition.Longitud,
                    lastPosition.Altitud,
                    lastPosition.Velocidad,
                    lastPosition.FechaMensaje,
                    lastPosition.FechaRecepcion);

            return result;
        }
        public Command ObtenerComando()
        {
            var comando = Client.LoginYObtenerComando(Config.AssistCargo.AssistCargoWebServiceUser,
                                                      Config.AssistCargo.AssistCargoWebServicePassword);
            return comando != null ? new Command(comando) : null;
        }
		public bool EstadoComando(int numeroComando, String mensaje)
        {
            return Client.LoginYEstadoComando(Config.AssistCargo.AssistCargoWebServiceUser,
                                       Config.AssistCargo.AssistCargoWebServicePassword,
                                       numeroComando, mensaje);
        }
        public bool ComandoProcesado(int numeroComando, bool procesado)
        {
            return Client.LoginYComandoProcesado(Config.AssistCargo.AssistCargoWebServiceUser,
                                          Config.AssistCargo.AssistCargoWebServicePassword,
                                          numeroComando, procesado);
        }
        
    }

	public class Command
	{
		public int Id { get; set; }
		public String Code { get; set; }
		public String Vehicle { get; set; }
		public String Dominio { get; set; }

		public Command(Comando comando)
		{
			Id = comando.NUMERO;
			Code = comando.CMD;
			Vehicle = comando.EQUIPO;
			Dominio = comando.UNIDAD;
		}
	}
}
