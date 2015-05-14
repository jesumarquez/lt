using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico
{
    [Serializable]
    public class ResumenDeEntregasVo
    {
        //public const int IndexDepartamento = 0;
        //public const int IndexCentroDeCostos = 1;
        public const int IndexTransportista = 0;
        public const int IndexVehiculo = 1;
        public const int IndexResponsable = 2;
        public const int IndexCompletados = 3;
        public const int IndexVisitados = 4;
        public const int IndexEnSitio = 5;
        public const int IndexEnZona = 6;
        public const int IndexNoCompletados = 7;
        public const int IndexNoVisitados = 8;
        public const int IndexTotales = 9;
        public const int IndexViajes = 10;
        public const int IndexPorcCompletados = 11;
        public const int IndexPorcVisitados = 12;
        public const int IndexPorcEnSitio = 13;
        public const int IndexPorcEnZona = 14;
        public const int IndexPorcNoCompletados = 15;
        public const int IndexPorcNoVisitados = 16;

        //[GridMapping(Index = IndexDepartamento, ResourceName = "Entities", VariableName = "PARENTI04", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 0)]
        //public string Departamento { get; set; }

        //[GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 1)]
        //public string CentroDeCostos { get; set; }

        //[GridMapping(Index = IndexSubCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI99", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 2)]
        //public string SubCentroDeCostos { get; set; }


        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 0)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, InitialSortExpression = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", AllowGroup = false)]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexCompletados, ResourceName = "Labels", VariableName = "COMPLETADOS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Completados { get; set; }

        [GridMapping(Index = IndexNoCompletados, ResourceName = "Labels", VariableName = "NO_COMPLETADOS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int NoCompletados { get; set; }

        [GridMapping(Index = IndexVisitados, ResourceName = "Labels", VariableName = "VISITADOS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Visitados { get; set; }

        [GridMapping(Index = IndexNoVisitados, ResourceName = "Labels", VariableName = "NO_VISITADOS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int NoVisitados { get; set; }

        [GridMapping(Index = IndexEnSitio, ResourceName = "Labels", VariableName = "EN_SITIO", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int EnSitio { get; set; }

        [GridMapping(Index = IndexEnZona, ResourceName = "Labels", VariableName = "EN_ZONA", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int EnZona { get; set; }

        [GridMapping(Index = IndexTotales, ResourceName = "Labels", VariableName = "TOTAL", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Total { get; set; }

        [GridMapping(Index = IndexViajes, ResourceName = "Labels", VariableName = "VIAJES", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Viajes { get; set; }

        [GridMapping(Index = IndexPorcCompletados, ResourceName = "Labels", VariableName = "PORC_COMPLETADOS", DataFormatString = "{0:0%}")]
        public double PorcCompletados { get; set; }

        [GridMapping(Index = IndexPorcNoCompletados, ResourceName = "Labels", VariableName = "PORC_NO_COMPLETADOS", DataFormatString = "{0:0%}")]
        public double PorcNoCompletados { get; set; }

        [GridMapping(Index = IndexPorcVisitados, ResourceName = "Labels", VariableName = "PORC_VISITADOS", DataFormatString = "{0:0%}")]
        public double PorcVisitados { get; set; }

        [GridMapping(Index = IndexPorcNoVisitados, ResourceName = "Labels", VariableName = "PORC_NO_VISITADOS", DataFormatString = "{0:0%}")]
        public double PorcNoVisitados { get; set; }

        [GridMapping(Index = IndexPorcEnSitio, ResourceName = "Labels", VariableName = "PORC_EN_SITIO", DataFormatString = "{0:0%}")]
        public double PorcEnSitio { get; set; }

        [GridMapping(Index = IndexPorcEnZona, ResourceName = "Labels", VariableName = "PORC_EN_ZONA", DataFormatString = "{0:0%}")]
        public double PorcEnZona { get; set; }

        public ResumenDeEntregasVo(Coche vehiculo, int completados, int noCompletados, int visitados, int noVisitados, int enSitio, int enZona, int viajes)
        {
            if (vehiculo != null)
            {
                Vehiculo = vehiculo.Interno + " - "+vehiculo.Patente ;
                Responsable = vehiculo.Chofer != null && vehiculo.Chofer.Entidad != null
                                  ? vehiculo.Chofer.Entidad.Descripcion
                                  : string.Empty;
                Transportista = vehiculo.Transportista != null ? vehiculo.Transportista.Descripcion : CultureManager.GetLabel("NINGUNO");
                //SubCentroDeCostos = vehiculo.SubCentroDeCostos != null ? vehiculo.SubCentroDeCostos.Descripcion : CultureManager.GetLabel("NINGUNO");
                //Departamento = vehiculo.CentroDeCostos != null && vehiculo.CentroDeCostos.Departamento != null
                //                   ? vehiculo.CentroDeCostos.Departamento.Descripcion
                //                   : CultureManager.GetLabel("NINGUNO");
            }
            
            Completados = completados;
            NoCompletados = noCompletados;
            Visitados = visitados;
            NoVisitados = noVisitados;
            EnSitio = enSitio;
            EnZona = enZona;
            Total = completados + noCompletados + visitados + noVisitados + enSitio + enZona;
            Viajes = viajes;
            if (Total > 0)
            {
                PorcCompletados = Convert.ToDouble(completados)/Convert.ToDouble(Total);
                PorcNoCompletados = Convert.ToDouble(noCompletados) / Convert.ToDouble(Total);
                PorcVisitados = Convert.ToDouble(visitados)/Convert.ToDouble(Total);
                PorcNoVisitados = Convert.ToDouble(noVisitados)/Convert.ToDouble(Total);
                PorcEnSitio = Convert.ToDouble(enSitio) / Convert.ToDouble(Total);
                PorcEnZona = Convert.ToDouble(enZona) / Convert.ToDouble(Total);
            }
        }
    }
}
