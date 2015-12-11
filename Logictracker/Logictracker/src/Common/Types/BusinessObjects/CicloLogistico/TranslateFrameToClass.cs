using System;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    public static class TranslateFrameToClass
    {
        public enum SosFields
        {
            //SERVICIO,MOVIL,PATENTE,MARCA, PRIORIDAD,	ADICIONAL,OBSERVACION,ORIGEN_DIRECCION,	ORIGEN_LOCALIDAD, DESTINO_DIRECCION,DESTINO_LOCALIDAD,OPERADOR,	DIAGNOSTICO, COLOR,	TIPO,HORA_PEDIDO,ORIGEN_LATITUD,ORIGEN_LONGITUD,DESTINO_LATITUD,DESTINO_LONGITUD,TIPO_MENSAJE
            Servicio=0, 
            Movil=1, 
            Patente=2, 
            Marca=3, 
            Prioridad=4, 
            Adicional=5, 
            Observacion=6, 
            OrigenDireccion=7, 
            OrigenLocalidad=8, 
            DestinoDireccion=9, 
            DestinoLocalidad=10, 
            Operador=11, 
            Diagnostico=12, 
            Color=13, 
            Tipo=14, 
            HoraPedido=15, 
            OrigenLatitud=16, 
            OrigenLongitud=17, 
            DestinoLatitud=18, 
            DestinoLongitud=19, 
            Estado=20
        }

        //20151119303926,1271,IAZ 581,CITROEN C 4 2.0 I BVA EXCLUSIV,NORMAL,290.4,SOBRE MANO DERECHA FRENTE CONCESIONARIA,SAN MARTIN  500,CORDOBA,COLON   1000,CORDOBA,PALACIO HEREDIA FERNANDO JAVIE,CORREA DE DISTRIBUCION,AMARILLO,TRASLADO,19/11/2015 09:00,-31.3241410993924,-63.9724202099609,-31.3101359203789,-63.8783077646484,1

        public static SosTicket ParseFrame(string alert)
        {
            var fields = alert.Split(',');
            var novelty = new SosTicket();

            novelty.NumeroServicio = fields[(int)SosFields.Servicio];
            novelty.Movil = int.Parse(fields[(int)SosFields.Movil]);
            novelty.Diagnostico = fields[(int)SosFields.Diagnostico];
            novelty.Prioridad = fields[(int)SosFields.Prioridad];
            novelty.HoraServicio = DateTime.Parse(fields[(int) SosFields.HoraPedido]);
            novelty.CobroAdicional = fields[(int) SosFields.Adicional];
            novelty.Patente = fields[(int)SosFields.Patente];
            novelty.Color = fields[(int)SosFields.Color];
            novelty.Marca= fields[(int)SosFields.Marca];
            novelty.Origen = new LocationSos(fields[(int)SosFields.OrigenLatitud], fields[(int)SosFields.OrigenLongitud], fields[(int)SosFields.OrigenDireccion], fields[(int)SosFields.OrigenLocalidad]);
            //novelty.Origen.Referencia = fields[(int)SosFields.Observacion];            
            novelty.Destino = new LocationSos(fields[(int)SosFields.DestinoLatitud], fields[(int)SosFields.DestinoLongitud], fields[(int)SosFields.DestinoDireccion], fields[(int)SosFields.DestinoLocalidad]);
            novelty.Operador = fields[(int)SosFields.Operador];
            novelty.Tipo = fields[(int)SosFields.Tipo];
            novelty.Observacion= fields[(int)SosFields.Observacion];
            novelty.EstadoServicio = int.Parse(fields[(int)SosFields.Estado]);

            //novelty.Preasignado = null;
            //novelty.Asignado = null;
            //novelty.Cancelado = null;

            return novelty;
        }

    }
}
