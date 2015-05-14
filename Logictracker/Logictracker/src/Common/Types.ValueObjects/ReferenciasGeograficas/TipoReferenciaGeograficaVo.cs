#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

#endregion

namespace Logictracker.Types.ValueObjects.ReferenciasGeograficas
{
    [Serializable]
    public class TipoReferenciaGeograficaVo
    {
        public const int IndexIconUrl = 0;
        public const int IndexCodigo = 1;
        public const int IndexDescripcion = 2;
        public const int IndexEmpresa = 3;
        public const int IndexLinea = 4;

        public int Id { get; set;}

        [GridMapping(Index = IndexIconUrl, IsTemplate = true, Width = "40px", AllowGroup = false)]
        public string IconUrl { get; set;}

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", IncludeInSearch = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02", IncludeInSearch = true)]
        public string Linea { get; set; }

        public string Color { get; set; }


        public TipoReferenciaGeograficaVo(TipoReferenciaGeografica tipoGeoref)
        {
            Id = tipoGeoref.Id;
            IconUrl = tipoGeoref.Icono != null ? tipoGeoref.Icono.PathIcono : string.Empty;
            Codigo = tipoGeoref.Codigo;
            Descripcion = tipoGeoref.Descripcion;
            Empresa = tipoGeoref.Empresa != null ? tipoGeoref.Empresa.RazonSocial : tipoGeoref.Linea != null ? tipoGeoref.Linea.Empresa.RazonSocial : string.Empty;
            Linea = tipoGeoref.Linea != null ? tipoGeoref.Linea.Descripcion : string.Empty;
            
            Color = tipoGeoref.Color != null ? tipoGeoref.Color.HexValue : string.Empty;
        }
    }
}
