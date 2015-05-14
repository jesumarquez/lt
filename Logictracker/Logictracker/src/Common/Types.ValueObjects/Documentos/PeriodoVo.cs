#region Usings

using System;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.ValueObjects.Documentos
{
    [Serializable]
    public class PeriodoVo
    {
        public const int IndexDescripcion = 0;
        public const int IndexFechaDesde = 1;
        public const int IndexFechaHasta = 2;
        public const int IndexEmpresa = 3;
        public const int IndexEstado = 4;
        public const int IndexAction = 5;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexFechaDesde, HeaderText = "Desde", AllowGroup = false, DataFormatString = "{0:d}", InitialSortExpression = true, IncludeInSearch = true)]
        public DateTime FechaDesde { get; set;}

        [GridMapping(Index = IndexFechaHasta, HeaderText = "Hasta", AllowGroup = false, DataFormatString = "{0:d}", IncludeInSearch = true)]
        public DateTime FechaHasta { get; set;}

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01")]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexEstado, HeaderText = "Estado")]
        public string Estado { get; set;}

        [GridMapping(Index = IndexAction, AllowGroup = false, IsTemplate = true, IncludeInSearch = true)]
        public bool Action { get; set;}

        
        public PeriodoVo(Periodo periodo)
        {
            Id = periodo.Id;
            Descripcion = periodo.Descripcion;
            FechaDesde = periodo.FechaDesde;
            FechaHasta = periodo.FechaHasta;
            Estado = periodo.Estado == 0 ? "Abierto"
                            : periodo.Estado == 1 ? "Cerrado"
                            : "Liquidado";
            Action = periodo.Estado == 0;
            Empresa = periodo.Empresa != null ? periodo.Empresa.RazonSocial : string.Empty;
        }
    }
}
