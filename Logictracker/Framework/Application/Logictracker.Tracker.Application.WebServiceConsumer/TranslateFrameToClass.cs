using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging.Simple;

namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public static class TranslateFrameToClass
    {
        public enum SosFields
        {
            Servicio, Movil, Patente, Marca, Prioridad, Adicional, Observacion, OrigenDireccion, OrigenLocalidad, DestinoDireccion, DestinoLocalidad, Operador, Diagnostico, Color, Tipo, HoraPedido, OrigenLatitud, OrigenLongitud, DestinoLatitud, DestinoLongitud
        }

        public static Novelty ParseFrame(string alert)
        {
            var fields = alert.Split(',');
            var novelty = new Novelty();

            novelty.NumeroServicio = int.Parse(fields[(int) SosFields.Servicio]);
            novelty.Diagnostico = fields[(int)SosFields.Diagnostico];
            novelty.Prioridad = fields[(int)SosFields.Prioridad];
            novelty.HoraServicio = DateTime.Parse(fields[(int) SosFields.HoraPedido]);
            novelty.CobroAdicional = fields[(int) SosFields.Adicional];
            novelty.Estado = GetStateCode(fields[(int)SosFields.Observacion]); //NO COINCIDE REALMENTE
            novelty.Vehiculo = new VehicleDataSos(fields[(int)SosFields.Patente], fields[(int)SosFields.Color], fields[(int)SosFields.Marca]);
            novelty.Origen = new LocationSos(fields[(int)SosFields.OrigenLatitud], fields[(int)SosFields.OrigenLongitud], fields[(int)SosFields.OrigenDireccion], fields[(int)SosFields.OrigenLocalidad]);
            novelty.Destino = new LocationSos(fields[(int)SosFields.DestinoLatitud], fields[(int)SosFields.DestinoLongitud], fields[(int)SosFields.DestinoDireccion], fields[(int)SosFields.DestinoLocalidad]);
            
            return novelty;
        }

        private static int GetStateCode(string estado)
        {
            return 1;
        }
    }
}
