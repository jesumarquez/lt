#region Usings

using System;
using System.Text;
using Urbetrack.Comm.Core.Mensajeria;
using Posicion=Urbetrack.Comm.Core.Codecs.Unetel.Posicion;

#endregion

namespace Urbetrack.Comm.Core.Codecs.Unetel
{
    public class Entrante
    {
        public static Evento Parse(byte[] ins)
        {
            var partes = Encoding.ASCII.GetString(ins).Split(";".ToCharArray());
            var codigo_legacy = partes[3];
            var evt = new Evento
                          {
                              SubTipoAuxiliar = ((char) ins[1]),
                              Seq = Convert.ToByte(partes[1]),
                              IdDispositivo = Convert.ToInt16(partes[2]),
                              Posicion = Posicion.ParsearPosicionCorta(partes[4]),
                              Datos = 0,
                              Saliente = false
                          };
            switch (codigo_legacy)
            {
                case "94":
                    evt.CodigoEvento = 0x02; // EVT_SWITCHING_ON
                    break;
                case "95":
                    evt.CodigoEvento = 0x01; // EVT_SWITCHING_OFF
                    break;
                case "92":
                    evt.CodigoEvento = 0x802; // EVT_BEACON_ON
                    break;
                case "91":
                    evt.CodigoEvento = 0x801; // EVT_BEACON_OFF
                    break;
                default:
                    evt.CodigoEvento = 0x7FFF; // EVT_VOID
                    evt.Datos = Convert.ToInt32(codigo_legacy);
                    break;
            }
            return evt;
        }
        
        /*public List<string> FormatoGatewayJava()
        {
            var lista = new List<string> {String.Format("MI,{0},{1},", IdEquipo, Codigo)};
            return lista;
        }*/
    }
}