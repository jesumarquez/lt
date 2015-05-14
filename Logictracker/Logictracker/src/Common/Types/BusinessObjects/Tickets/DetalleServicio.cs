#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class DetalleServicio : IAuditable
    {
        public const short TipoTiempo = 0;
        public const short TipoEvento = 1;
        public const short TipoEntradaPoi = 2;
        public const short TipoSalidaPoi = 3;

        public const short EstadoEnCurso = 0;
        public const short EstadoCancelado = 9;

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Servicio Servicio { get; set; }
        public virtual DetalleCiclo DetalleCiclo { get; set; }

        public virtual short Tipo { get; set; }

        public virtual short Estado { get; set; }
        public virtual short Orden { get; set; }
        public virtual bool Obligatorio { get; set; }

        public virtual DateTime Programada {get; set;}
        public virtual DateTime? Real { get; set; }
        public virtual DateTime? Control { get; set; }

        public virtual int Minutos { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual Mensaje Mensaje { get; set; }


        public virtual string Descripcion
        {
            get
            {
                if (DetalleCiclo != null) return DetalleCiclo.Descripcion;
                switch(Tipo)
                {
                    case TipoTiempo: return Minutos + " min.";
                    case TipoEvento: return Mensaje != null ? Mensaje.Descripcion : "Sin definir";
                    case TipoEntradaPoi: return "Entrada: " + ReferenciaGeografica != null ? ReferenciaGeografica.Descripcion : "Sin Definir";
                    case TipoSalidaPoi: return "Salida: " + ReferenciaGeografica != null ? ReferenciaGeografica.Descripcion : "Sin Definir";
                }
                return "Sin Definir";
            }
        }
    }
}
