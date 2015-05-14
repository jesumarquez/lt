using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class DetalleVo
    {   
        public const int IndexNombre = 0;
        public const int IndexTipo = 1;
        public const int IndexRepresentacion = 2;
        public const int IndexFiltro = 3;
        public const int IndexObligatorio = 4;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set; }

        [GridMapping(Index = IndexTipo, ResourceName = "Labels", VariableName = "TIPO_DATO", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexRepresentacion, ResourceName = "Labels", VariableName = "REPRESENTACION", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Representacion { get; set; }

        [GridMapping(Index = IndexFiltro, ResourceName = "Labels", VariableName = "ES_FILTRO", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = false)]
        public string Filtro { get; set; }

        [GridMapping(Index = IndexObligatorio, ResourceName = "Labels", VariableName = "OBLIGATORIO", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = false)]
        public string Obligatorio { get; set; }

        public DetalleVo(Detalle detalle)
        {
            Id = detalle.Id;
            Nombre = detalle.Nombre;
            
            switch (detalle.Tipo)
            {
                case 1:
                    Tipo = CultureManager.GetLabel("TEXTO");
                    break;
                case 2:
                    Tipo = CultureManager.GetLabel("NUMERICO");
                    break;
                case 3:
                    Tipo = CultureManager.GetLabel("FECHA");
                    break;
                default:
                    Tipo = CultureManager.GetLabel("TEXTO");
                    break;
            }

            switch (detalle.Representacion)
            {
                case 1:
                    Representacion = CultureManager.GetLabel("TEXTBOX");
                    break;
                case 2:
                    Representacion = CultureManager.GetLabel("LISTA");
                    break;
                case 3:
                    Representacion = CultureManager.GetLabel("SELECCION_MULTIPLE");
                    break;
                default:
                    Representacion = CultureManager.GetLabel("TEXTBOX");
                    break;
            }

            Filtro = detalle.EsFiltro ? CultureManager.GetLabel("SI") : CultureManager.GetLabel("NO");
            Obligatorio = detalle.Obligatorio ? CultureManager.GetLabel("SI") : CultureManager.GetLabel("NO");
        }
    }
}
