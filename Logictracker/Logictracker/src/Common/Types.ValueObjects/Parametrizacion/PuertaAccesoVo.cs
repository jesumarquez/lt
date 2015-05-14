using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class PuertaAccesoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexZonaAccesoEntrada = 2;
        public const int IndexZonaAccesoSalida = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", IncludeInSearch = true, AllowGroup = false)]
        public int Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexZonaAccesoEntrada, ResourceName = "Labels", VariableName = "ENTRA_A", AllowGroup = true, IncludeInSearch = true)]
        public string ZonaAccesoEntrada { get; set; }

        [GridMapping(Index = IndexZonaAccesoSalida, ResourceName = "Labels", VariableName = "SALE_A", AllowGroup = true, IncludeInSearch = true)]
        public string ZonaAccesoSalida { get; set; }

        public PuertaAccesoVo(PuertaAcceso puerta)
        {
            Id = puerta.Id;
            Codigo = puerta.Codigo;
            Descripcion = puerta.Descripcion;
            ZonaAccesoEntrada = puerta.ZonaAccesoEntrada != null ? puerta.ZonaAccesoEntrada.Descripcion : string.Empty;
            ZonaAccesoSalida = puerta.ZonaAccesoSalida != null ? puerta.ZonaAccesoSalida.Descripcion : string.Empty;
        }
    }
}
