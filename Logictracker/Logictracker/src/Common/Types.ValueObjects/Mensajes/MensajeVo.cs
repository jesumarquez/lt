using System;
using Logictracker.Types.BusinessObjects.Messages;

namespace Logictracker.Types.ValueObjects.Mensajes
{
    [Serializable]
    public class MensajeVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexTipo = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexTipo, ResourceName = "Labels", VariableName = "TYPE", IncludeInSearch = true)]
        public string Tipo { get; set; }

        public short Destino { get; set; }

        public byte Origen { get; set; }

        public short Ttl { get; set; }

        public bool IsParent { get; set; }

        public bool IsGeneric { get; set; }

        public MensajeVo (Mensaje msj)
        {
            Id = msj.Id;
            Descripcion = msj.Descripcion;
            Destino = msj.Destino;
            Origen = msj.Origen;
            Codigo = msj.Codigo;
            Ttl = msj.Ttl;
            IsParent = msj.Empresa == null && msj.Linea == null;
            IsGeneric = msj.TipoMensaje.EsGenerico;
            Tipo = msj.TipoMensaje.Descripcion;
        }
    }
}