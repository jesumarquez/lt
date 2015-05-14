using System;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ReporteDespachoVo
    {
        public const int IndexVehiculo = 0;
        public const int IndexFecha = 1;
        public const int IndexPeriodo = 2;
        public const int IndexConsumo = 3;
        public const int IndexDespacho = 4;
        public const int IndexDiferencia = 5;
        public const int IndexDifPorc = 6;
        
        public int Id { get; set; }
        public Coche Coche { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IncludeInSearch = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexFecha, InitialSortExpression = true, Visible = false)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexPeriodo, ResourceName = "Labels", VariableName = "PERIODO", IncludeInSearch = true, AllowGroup = false, AllowMove = false)]
        public string Periodo { get; set; }

        [GridMapping(Index = IndexConsumo, ResourceName = "Labels", VariableName = "CONSUMO", DataFormatString = "{0:#0.00}", AllowGroup = false, AllowMove = false)]
        public double Consumo { get; set; }

        [GridMapping(Index = IndexDespacho, ResourceName = "Labels", VariableName = "DESPACHO", DataFormatString = "{0:#0.00}", AllowGroup = false, AllowMove = false)]
        public double Despacho { get; set; }

        [GridMapping(Index = IndexDiferencia, ResourceName = "Labels", VariableName = "DIFERENCIA", DataFormatString = "{0:#0.00}", AllowGroup = false, AllowMove = false)]
        public double Diferencia { get; set; }

        [GridMapping(Index = IndexDifPorc, ResourceName = "Labels", VariableName = "PORCENTUAL", DataFormatString = "{0:#0.00}", AllowGroup = false, AllowMove = false)]
        public double DifPorc { get; set; }

        public ReporteDespachoVo(ConsumoDetalle anterior, ConsumoDetalle consumo)
        {
            var dao = new DAOFactory();
            Id = consumo.Id;
            Coche = consumo.ConsumoCabecera.Vehiculo;
            Fecha = consumo.ConsumoCabecera.Fecha;
            var entidad = dao.EntidadDAO.FindByDispositivo(new[] { Coche.Empresa != null ? Coche.Empresa.Id : -1 },
                                                           new[] { Coche.Linea != null ? Coche.Linea.Id : -1 },
                                                           new[] { Coche.Dispositivo != null ? Coche.Dispositivo.Id : -1 });
            var usaTelemetria = entidad != null && entidad.Id != 0;

            Vehiculo = Coche.Interno;
            Periodo = anterior.ConsumoCabecera.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + " a " + consumo.ConsumoCabecera.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            Despacho = consumo.Cantidad;
            var dm = dao.DatamartDAO.GetSummarizedDatamart(anterior.ConsumoCabecera.Fecha, consumo.ConsumoCabecera.Fecha, Coche.Id);

            if (usaTelemetria)
                Consumo = dm.Consumo;
            else
            {
                var km = dm.Kilometros;
                var rendimiento = Coche.CocheOperacion != null && Coche.CocheOperacion.Rendimiento > 0.0
                                      ? Coche.CocheOperacion.Rendimiento
                                      : Coche.Modelo != null
                                            ? Coche.Modelo.Rendimiento
                                            : 0.0;
                Consumo = km > 0 && rendimiento > 0 ? (km / 100.0) * rendimiento: 0.0;
            }
            Diferencia = Consumo - Despacho;
            DifPorc = Despacho > 0.00 ? Diferencia / Despacho * 100 : 0.00;
        }
    }
}
