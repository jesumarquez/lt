#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class DetalleCiclo : IAuditable
    {
        public const short TipoTiempo = 0;
        public const short TipoEvento = 1;
        public const short TipoEntradaPoi = 2;
        public const short TipoSalidaPoi = 3;
        public const short TipoCicloLogistico = 4;

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual CicloLogistico CicloLogistico { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual short Tipo { get; set; }
        public virtual short Repeticion { get; set; }
        public virtual short Orden { get; set; }
        public virtual bool Obligatorio { get; set; }
        public virtual bool Baja { get; set; }
        public virtual int Duracion { get; set; }

        public virtual int Minutos { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual CicloLogistico EstadoCicloLogistico { get; set; }
        public virtual Mensaje Mensaje { get; set; }

        public virtual bool ObligatorioControl { get; set; }
        public virtual Mensaje MensajeControl { get; set; }
    }
}
