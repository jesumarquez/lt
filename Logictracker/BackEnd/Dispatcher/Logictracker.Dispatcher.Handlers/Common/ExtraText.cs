using System;
using Logictracker.AVL.Messages;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using System.Globalization;

namespace Logictracker.Dispatcher.Handlers.Common
{
    public class ExtraText
    {
        public DAOFactory DaoFactory { get; set; }

        public ExtraText(DAOFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        public string GetExtraText(Event generico, string code)
        {
            if (code != MessageCode.GenericMessage.GetMessageCode())
            {
                if (code == MessageCode.HardwareChange.GetMessageCode())
                {
                    return GetMalfunctionEventText((byte)generico.GetData());
                }
                if (code == MessageCode.EngineOn.GetMessageCode())
                {
                    return GetStartupEventText((byte)generico.GetData());
                }

                var mi = (MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), generico.GetData().ToString(CultureInfo.InvariantCulture), true);
                if (mi.HasExtraData())
                {
                    return GetExtraDataText(generico, mi);
                }
            }

            return string.Empty;
        }
        public string GetVelocidadExcedidaExtraText(IGeoPoint generico, Coche coche)
        {
            var speed = 0;

            if (generico.GeoPoint != null)
            {
                speed = generico.GeoPoint.Velocidad;
            }
            else
            {
                var lastPosition = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);
                //No hay indice para hacer la consulta por Dispositivo

                if (lastPosition != null) speed = lastPosition.Velocidad;
            }

            return speed.Equals(0) ? string.Empty : string.Format(": {0}km/h", speed);
        }

        public static string GetRfidExtraText(string rfid, Empleado empleado)
        {
            if (string.IsNullOrEmpty(rfid)) return ": RFID invalida - Sin Codigo";

            return empleado != null
                       ? string.Format(": {0}", empleado.Entidad.Descripcion)
                       : string.Format(": RFID invalida {0} - Sin Asignar", rfid);
        }
        private string GetExtraDataText(IExtraData ev, MessageIdentifier messageIdentifier)
        {
            switch (messageIdentifier)
            {
                case MessageIdentifier.BlackcallIncoming:
                case MessageIdentifier.AccelerationEvent:
                case MessageIdentifier.DesaccelerationEvent:
				case MessageIdentifier.BateryInfo:
                    //return String.Format(" - {0}", ((Event)ev).SensorsDataString);
					return ev.Data.Count > 1 ? String.Format(" - {0}", ev.Data[1]) : String.Empty;
                case MessageIdentifier.Picture:
                    var cantidad = ev.Data.Count > 1 ? Convert.ToInt32(ev.Data[1]) : 0;
                    var id = ev.Data.Count > 2  ? Convert.ToInt32(ev.Data[2]) : 0;
                    return GetPictureText(cantidad, id);
                case MessageIdentifier.TemperatureInfo:
                case MessageIdentifier.TemperatureDisconected:
                case MessageIdentifier.TemperaturePowerDisconected:
                case MessageIdentifier.TemperaturePowerReconected:
                case MessageIdentifier.TemperatureThawingButtonPressed:
                case MessageIdentifier.TemperatureThawingButtonUnpressed:
                case MessageIdentifier.CheckpointReached:
                    return String.Format(" - {0:000.00}", ev.GetData() / (float)100.0);
            }

            return String.Empty;
        }

        private string GetPictureText(int cantidad, int data)
        {
            var text = String.Format(" ({0}) ", cantidad);
            if (data > 0)
            {
                try
                {
                    var entrega = DaoFactory.EntregaDistribucionDAO.FindById(data);
                    text += entrega.Descripcion;
                }
                catch{}
            }
            return text;
        }
        private static string GetMalfunctionEventText(int datos)
        {
            // Device malfunction status.
            const int evtdataGpsMalfunction = 0x00;
            const int evtdataXbeeMalfunction = 0x01;
            const int evtdataModemMalfunction = 0x02;

            var text = String.Empty;

            switch (datos)
            {
                case evtdataGpsMalfunction: text = ": Problemas de GPS"; break;
                case evtdataXbeeMalfunction: text = ": Problemas de xBee"; break;
                case evtdataModemMalfunction: text = ": Problemas de Modem"; break;
            }

            return text;
        }

        private static string GetStartupEventText(byte datos)
        {
            var texto = String.Empty;

            if (datos == 0) return texto;

            if (BitHelper.IsBitSet(datos, 1)) texto = String.Concat(texto, " - Watchdog reset");
            if (BitHelper.IsBitSet(datos, 2)) texto = String.Concat(texto, " - Hard reset");
            if (BitHelper.IsBitSet(datos, 3)) texto = String.Concat(texto, " - User reset");
            if (BitHelper.IsBitSet(datos, 4)) texto = String.Concat(texto, " - Kernel panic");
            if (BitHelper.IsBitSet(datos, 5)) texto = String.Concat(texto, " - Sd almost full");
            if (BitHelper.IsBitSet(datos, 6)) texto = String.Concat(texto, " - Sd heavy");
            if (BitHelper.IsBitSet(datos, 7)) texto = String.Concat(texto, " - Sd full");
            if (BitHelper.IsBitSet(datos, 8)) texto = String.Concat(texto, " - Qtree reset");

            return texto;
        }
    }
}
