using System;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico
{
    [Serializable]
    public class ResumenDeRutasVo
    {
        public const int IndexEstado = 0;
        public const int IndexRuta = 1;
        public const int IndexVehiculo = 2;
        public const int IndexInicio = 3;
        public const int IndexFin = 4;
        public const int IndexRecepcion = 5;
        public const int IndexEntregas = 6;
        public const int IndexRealizadas = 7;
        public const int IndexVisitadas = 8;
        public const int IndexRechazadas = 9;
        public const int IndexEnSitio = 10;
        public const int IndexEnZona = 11;
        public const int IndexPorc = 12;
        public const int IndexKm = 13;
        public const int IndexRecorrido = 14;

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true, IsInitialGroup = true)]
        public string Estado { get; set; }

        [GridMapping(Index = IndexRuta, ResourceName = "Labels", VariableName = "RUTA", AllowGroup = false)]
        public string Ruta { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IsAggregate = true, AggregateType = GridAggregateType.Count)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "INICIO")]
        public DateTime? Inicio { get; set; }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "FIN")]
        public DateTime? Fin { get; set; }

        [GridMapping(Index = IndexRecepcion, ResourceName = "Labels", VariableName = "RECEPCION")]
        public DateTime? Recepcion { get; set; }

        [GridMapping(Index = IndexEntregas, ResourceName = "Labels", VariableName = "ENTREGAS", DataFormatString = "{0:#0}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Entregas { get; set; }

        [GridMapping(Index = IndexRealizadas, ResourceName = "Labels", VariableName = "REALIZADOS", DataFormatString = "{0:#0}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Realizados { get; set; }

        [GridMapping(Index = IndexVisitadas, ResourceName = "Labels", VariableName = "VISITADOS", DataFormatString = "{0:#0}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Visitados { get; set; }

        [GridMapping(Index = IndexRechazadas, ResourceName = "Labels", VariableName = "RECHAZADOS", DataFormatString = "{0:#0}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int Rechazados { get; set; }

        [GridMapping(Index = IndexEnSitio, ResourceName = "Labels", VariableName = "ENSITIO", DataFormatString = "{0:#0}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int EnSitio { get; set; }

        [GridMapping(Index = IndexEnZona, ResourceName = "Labels", VariableName = "ENZONA", DataFormatString = "{0:#0}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:#0}")]
        public int EnZona { get; set; }

        [GridMapping(Index = IndexPorc, ResourceName = "Labels", VariableName = "%", DataFormatString = "{0:0%}", InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public double PorcVisitados { get; set; }

        [GridMapping(Index = IndexKm, ResourceName = "Labels", VariableName = "KMS", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}")]
        public double Kms { get; set; }

        [GridMapping(Index = IndexRecorrido, ResourceName = "Labels", VariableName = "RECORRIDO")]
        public TimeSpan Recorrido { get; set; }

        public int Id { get; set; }
        public ViajeDistribucion Viaje { get; set; }

        public ResumenDeRutasVo(ViajeDistribucion viaje, Boolean estadoVerKm)
        {
            Id = viaje.Id;
            Viaje = viaje;
            var dao = new DAOFactory();
            Ruta = viaje.Codigo;
            Vehiculo = viaje.Vehiculo != null 
                ? viaje.Vehiculo.Interno + " - " 
                + viaje.Vehiculo.Patente 
                + (viaje.Vehiculo.Dispositivo == null ? " (Sin dispositivo)" : string.Empty)
                + (viaje.Vehiculo.Estado == Coche.Estados.EnMantenimiento ? " (En Taller)" : string.Empty)
                + (viaje.Vehiculo.Estado == Coche.Estados.Inactivo ? " (Inactivo)" : string.Empty)
                + (viaje.Vehiculo.Estado == Coche.Estados.Revisar ? " (Revisar)" : string.Empty)
                : "Ninguno";
            Inicio = viaje.InicioReal.HasValue ? viaje.InicioReal.Value.ToDisplayDateTime() : (DateTime?) null;
            Fin = viaje.Estado == ViajeDistribucion.Estados.Cerrado ? viaje.Fin.ToDisplayDateTime() : (DateTime?)null;
            Recepcion = viaje.Recepcion.HasValue ? viaje.Recepcion.Value.ToDisplayDateTime() : (DateTime?) null;
            Entregas = viaje.EntregasTotalCount;
            Realizados = viaje.EntregasCompletadosCount;
            Visitados = viaje.EntregasVisitadosCount;
            Rechazados = viaje.EntregasNoCompletadosCount;
            EnSitio = viaje.EntregasEnSitioCount;
            EnZona = viaje.EntregasEnZonaCount;
            var total = Realizados + Visitados;
            PorcVisitados = Entregas > 0 ? Convert.ToDouble(total) / Convert.ToDouble(Entregas) : 0.00;
            Estado = CultureManager.GetLabel(ViajeDistribucion.Estados.GetLabelVariableName(viaje.Estado));
            var kms = 0.0;  
            var recorrido = new TimeSpan(0);

            if (viaje.Vehiculo != null && viaje.InicioReal.HasValue)
            {
                switch (viaje.Estado)
                {
                    case ViajeDistribucion.Estados.EnCurso:
                        if (estadoVerKm)
                        {
                            kms = dao.CocheDAO.GetDistance(viaje.Vehiculo.Id, viaje.InicioReal.Value, DateTime.UtcNow);
                            recorrido = new TimeSpan(0, 0, (int)DateTime.UtcNow.Subtract(viaje.InicioReal.Value).TotalSeconds);
                        }
                        break;    
                      
                    case ViajeDistribucion.Estados.Cerrado:
                        if (estadoVerKm)
                        {
                            if (viaje.InicioReal.Value < DateTime.Today)
                            {
                                var dmViaje = dao.DatamartViajeDAO.GetRecords(viaje.Id).FirstOrDefault();
                                if (dmViaje != null) kms = dmViaje.KmTotales;
                            }
                            else kms = dao.CocheDAO.GetDistance(viaje.Vehiculo.Id, viaje.InicioReal.Value, viaje.Fin);
                            
                            recorrido = new TimeSpan(0, 0, (int)viaje.Fin.Subtract(viaje.InicioReal.Value).TotalSeconds);
                        }
                        break;
                }
            }
            Kms = kms;
            Recorrido = recorrido;
        }
    }
}
