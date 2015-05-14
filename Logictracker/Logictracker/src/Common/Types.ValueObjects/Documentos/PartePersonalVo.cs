using System;
using Logictracker.Security;
using Logictracker.Types.ValueObjects.Documentos.Partes;

namespace Logictracker.Types.ValueObjects.Documentos
{
    [Serializable]
    public class PartePersonalVo
    {
        public const int IndexCodigo = 0;
        public const int IndexFecha = 1;
        public const int IndexVehiculo = 2;
        public const int IndexTipoServicio = 3;
        public const int IndexEquipo = 4;
        public const int IndexCentroCostos = 5;
        public const int IndexEmpresa = 6;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:G}", IncludeInSearch = true)]
        public DateTime Fecha { get; set;}

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IncludeInSearch = true)]
        public string Vehiculo { get; set;}

        [GridMapping(Index = IndexTipoServicio, ResourceName = "Labels", VariableName = "TIPOSERVICIOPARTE", IncludeInSearch = true)]
        public string TipoServicio { get; set; }

        [GridMapping(Index = IndexEquipo, ResourceName = "Entities", VariableName = "PARENTI19", IncludeInSearch = true)]
        public string Equipo { get; set;}

        [GridMapping(Index = IndexCentroCostos, ResourceName = "Entities", VariableName = "PARENTI37", IncludeInSearch = true)]
        public string CentroCostos { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI07", IncludeInSearch = true)]
        public string Empresa { get; set;}

        public short Estado { get; set; }

        public PartePersonalVo(PartePersonal parte)
        {
            Id = parte.Id;
            Codigo = parte.Codigo;
            Fecha = parte.Fecha.ToDisplayDateTime();
            Vehiculo = parte.Interno;
            Equipo = parte.Equipo;
            Empresa = parte.Empresa;
            Estado = parte.Estado;
            TipoServicio = parte.TipoServicio;
            CentroCostos = parte.CentroCostos;
        }
    }
}
