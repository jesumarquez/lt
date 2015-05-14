using System;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class ConsumoCCVo
    {
        public const int IndexFecha = 0;
        public const int IndexCentroDeCosto = 1;
        public const int IndexVehiculo = 2;
        public const int IndexNumeroFactura = 3;
        public const int IndexKmDeclarados = 4;
        public const int IndexImporteTotal = 5;
        public const int IndexProveedor = 6;

        public int Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexCentroDeCosto, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true, IsInitialGroup = true, IncludeInSearch = true)]
        public string CentroDeCosto { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Labels", VariableName = "VEHICULO", AllowGroup = true, IsInitialGroup = true, IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexNumeroFactura, ResourceName = "Labels", VariableName = "NRO_FACTURA", IncludeInSearch = true)]
        public string NumeroFactura { get; set; }

        [GridMapping(Index = IndexKmDeclarados, ResourceName = "Labels", VariableName = "KM_DECLARADOS", DataFormatString = "{0:0.00 Km}", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00 Km}")]
        public double KmDeclarados { get; set; }

        [GridMapping(Index = IndexImporteTotal, ResourceName = "Labels", VariableName = "IMPORTE_TOTAL", DataFormatString = "{0:$0.00}", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:$0.00}")]
        public double ImporteTotal { get; set; }

        [GridMapping(Index = IndexProveedor, ResourceName = "Entities", VariableName = "PARENTI59", AllowGroup = true, IncludeInSearch = true)]
        public string Proveedor { get; set; }

        public ConsumoCCVo(ConsumoCabecera consumo, int centroDeCostoId)
        {
            Id = consumo.Id;
            Fecha = consumo.Fecha.ToDisplayDateTime();
            KmDeclarados = consumo.KmDeclarados;
            NumeroFactura = consumo.NumeroFactura;
            ImporteTotal = consumo.ImporteTotal;
            Proveedor = consumo.Proveedor != null
                            ? consumo.Proveedor.ToString()
                            : consumo.Deposito != null
                                  ? consumo.Deposito.ToString()
                                  : string.Empty;
            Vehiculo = consumo.Vehiculo != null
                           ? consumo.Vehiculo.ToString()
                           : consumo.DepositoDestino != null
                                 ? consumo.DepositoDestino.ToString()
                                 : string.Empty;
            var dao = new DAOFactory();
            var centroDeCosto = dao.CentroDeCostosDAO.FindById(centroDeCostoId);
            CentroDeCosto = centroDeCosto != null ? centroDeCosto.Descripcion : string.Empty;
        }
    }
}
