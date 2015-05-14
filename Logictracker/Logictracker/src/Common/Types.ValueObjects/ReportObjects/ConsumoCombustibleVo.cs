using System;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ConsumoCombustibleVo
    {
        public const int IndexFecha = 0;
        public const int IndexModelo = 1;
        public const int IndexVehiculo = 2;
        public const int IndexNumeroFactura = 3;
        public const int IndexProveedor = 4;
        public const int IndexInsumo = 5;
        public const int IndexUnidadMedida = 6;
        public const int IndexCapacidad = 7;
        public const int IndexCantidad = 8;
        public const int IndexImporteUnitario = 9;
        public const int IndexImporteTotal = 10;
        public const int IndexRendimiento = 11;
        public const int IndexKmDeclarados = 12;
        public const int IndexConsumoCalculadoDeclarado = 13;
        public const int IndexKmGps = 14;
        public const int IndexConsumoCalculadoGps = 15;

        public int Id { get; set; }
        public int IdCabecera { get; set; }
        public int IdVehiculo { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexKmGps, ResourceName = "Labels", VariableName = "KM_GPS", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double KmGps
        {
            get
            {
                var df = new DAOFactory();
                return df.ConsumoCabeceraDAO.GetDistance(IdCabecera);
            }
        }

        [GridMapping(Index = IndexKmDeclarados, ResourceName = "Labels", VariableName = "KM_DECLARADOS", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double KmDeclarados
        {
            get
            {
                var df = new DAOFactory();
                return df.ConsumoCabeceraDAO.GetKmDeclarados(IdCabecera);
            }
        }

        [GridMapping(Index = IndexUnidadMedida, ResourceName = "Labels", VariableName = "UNIDAD_MEDIDA", AllowGroup = true)]
        public string UnidadMedida { get; set; }

        [GridMapping(Index = IndexCantidad, ResourceName = "Labels", VariableName = "CANTIDAD", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double Cantidad { get; set; }

        [GridMapping(Index = IndexNumeroFactura, ResourceName = "Labels", VariableName = "NRO_FACTURA", AllowGroup = true)]
        public string NumeroFactura { get; set; }

        [GridMapping(Index = IndexImporteUnitario, ResourceName = "Labels", VariableName = "IMPORTE_UNITARIO", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double ImporteUnitario { get; set; }

        [GridMapping(Index = IndexImporteTotal, ResourceName = "Labels", VariableName = "IMPORTE_TOTAL", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double ImporteTotal { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Labels", VariableName = "VEHICULO", AllowGroup = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexModelo, ResourceName = "Entities", VariableName = "PARENTI61", AllowGroup = true)]
        public string Modelo { get; set; }

        [GridMapping(Index = IndexInsumo, ResourceName = "Entities", VariableName = "PARENTI58", AllowGroup = true)]
        public string Insumo { get; set; }

        [GridMapping(Index = IndexProveedor, ResourceName = "Entities", VariableName = "PARENTI59", AllowGroup = true)]
        public string Proveedor { get; set; }

        [GridMapping(Index = IndexRendimiento, ResourceName = "Labels", VariableName = "RENDIMIENTO_LTS_KM", AllowGroup = true, DataFormatString = "{0:0.00}")]
        public double Rendimiento { get; set; }

        [GridMapping(Index = IndexCapacidad, ResourceName = "Labels", VariableName = "CAPACIDAD", AllowGroup = true, DataFormatString = "{0:0.00}")]
        public double Capacidad { get; set; }

        [GridMapping(Index = IndexConsumoCalculadoDeclarado, ResourceName = "Labels", VariableName = "CONSUMO_CALCULADO_DECLARADO", AllowGroup = true, DataFormatString = "{0:0.00}")]
        public double ConsumoCalculadoDeclarado { get; set; }

        [GridMapping(Index = IndexConsumoCalculadoGps, ResourceName = "Labels", VariableName = "CONSUMO_CALCULADO_GPS", AllowGroup = true, DataFormatString = "{0:0.00}")]
        public double ConsumoCalculadoGps 
        { 
            get
            {
                return (KmGps > 0 && Rendimiento > 0) ? (KmGps / 100.0) * Rendimiento : 0.0;
            }
        }

        public ConsumoCombustibleVo(ConsumoDetalle consumo)
        {
            Id = consumo.Id;
            IdCabecera = consumo.ConsumoCabecera.Id;
            Fecha = consumo.ConsumoCabecera.Fecha.ToDisplayDateTime();
            Cantidad = consumo.Cantidad;
            NumeroFactura = consumo.ConsumoCabecera.NumeroFactura;
            ImporteUnitario = consumo.ImporteUnitario;
            ImporteTotal = consumo.ImporteTotal;
            Vehiculo = consumo.ConsumoCabecera.Vehiculo != null ? consumo.ConsumoCabecera.Vehiculo.ToString() : string.Empty;
            IdVehiculo = consumo.ConsumoCabecera.Vehiculo != null ? consumo.ConsumoCabecera.Vehiculo.Id : 0;
            Insumo = consumo.Insumo != null ? consumo.Insumo.ToString() : string.Empty;
            Proveedor = consumo.ConsumoCabecera.Proveedor != null ? consumo.ConsumoCabecera.Proveedor.ToString() : string.Empty;
            Modelo = (consumo.ConsumoCabecera.Vehiculo != null && consumo.ConsumoCabecera.Vehiculo.Modelo != null) ? consumo.ConsumoCabecera.Vehiculo.Modelo.Descripcion : string.Empty;
            Capacidad = (consumo.ConsumoCabecera.Vehiculo != null && consumo.ConsumoCabecera.Vehiculo.Modelo != null) ? consumo.ConsumoCabecera.Vehiculo.Modelo.Capacidad : 0.0;
            
            if (consumo.ConsumoCabecera.Vehiculo != null)
            {
                if (consumo.ConsumoCabecera.Vehiculo.CocheOperacion != null && consumo.ConsumoCabecera.Vehiculo.CocheOperacion.Rendimiento > 0.0)
                    Rendimiento = consumo.ConsumoCabecera.Vehiculo.CocheOperacion.Rendimiento;
                else
                    Rendimiento = consumo.ConsumoCabecera.Vehiculo.Modelo != null ? consumo.ConsumoCabecera.Vehiculo.Modelo.Rendimiento : 0.0;
            }
            else
                Rendimiento = 0.0;

            ConsumoCalculadoDeclarado = (consumo.ConsumoCabecera.Vehiculo != null && KmDeclarados > 0 && Rendimiento > 0) ? (KmDeclarados / 100.0) * Rendimiento : 0.0;
        }
    }
}
