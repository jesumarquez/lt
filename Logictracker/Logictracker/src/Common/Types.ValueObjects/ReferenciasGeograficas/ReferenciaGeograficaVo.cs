using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

namespace Logictracker.Types.ValueObjects.ReferenciasGeograficas
{
    [Serializable]
    public class ReferenciaGeograficaVo
    {
        public const int IndexCheck = 0;
        public const int IndexIconUrl = 1;
        public const int IndexCodigo = 2;
        public const int IndexDescripcion = 3;
        public const int IndexIsPoint = 4;
        public const int IndexIsPolygon = 5;
        public const int IndexLatitud = 6;
        public const int IndexLongitud = 7;

        public int Id { get; set;}

        [GridMapping(Index = IndexCheck, IsTemplate = true, Width = "32px", AllowGroup = false, AllowMove = false)]
        public bool Check { get; set;}

        [GridMapping(Index = IndexIconUrl, IsTemplate = true, Width = "40px", AllowGroup = false)]
        public string IconUrl { get; set;}

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexIsPoint, IsTemplate = true, Width = "32px", AllowGroup = false)]
        public bool IsPoint { get; set;}

        [GridMapping(Index = IndexIsPolygon, IsTemplate = true, Width = "32px", AllowGroup = false)]
        public bool IsPolygon { get; set;}

        [GridMapping(Index = IndexIsPolygon, ResourceName = "Labels", VariableName = "LATITUD", Visible = false)]
        public double Latitud { get; set; }

        [GridMapping(Index = IndexLongitud, ResourceName = "Labels", VariableName = "LONGITUD", Visible = false)]
        public double Longitud { get; set; }

        public string Color { get; set; }

        public int Empresa { get; set; }

        public int Linea { get; set; }

        public ReferenciaGeograficaVo(ReferenciaGeografica georef)
        {
            Id = georef.Id;
            IconUrl = georef.Icono != null ? georef.Icono.PathIcono : string.Empty;
            Codigo = georef.Codigo;
            Descripcion = georef.Descripcion;
            IsPoint = georef.Direccion != null;
            IsPolygon = georef.Poligono != null;
            Color = georef.Color != null ? georef.Color.HexValue : string.Empty;
            Linea = georef.Linea != null ? georef.Linea.Id : -1;
            Empresa = georef.Empresa != null ? georef.Empresa.Id : georef.Linea != null ? georef.Linea.Empresa.Id : -1;

            Latitud = georef.Latitude;
            Longitud = georef.Longitude;
        }
    }
}
