using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            novelty.Diagnostico = fields[(int) SosFields.Diagnostico];


            return novelty;
        }
    }
}
