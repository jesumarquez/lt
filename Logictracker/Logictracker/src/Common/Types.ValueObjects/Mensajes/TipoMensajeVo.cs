using System;
using Logictracker.Types.BusinessObjects.Messages;

namespace Logictracker.Types.ValueObjects.Mensajes
{
    [Serializable]
    public class TipoMensajeVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public TipoMensajeVo(TipoMensaje tipoMensaje)
        {
            Id = tipoMensaje.Id;
            Codigo = tipoMensaje.Codigo;
            Descripcion = tipoMensaje.Descripcion;
        }
    }
}
