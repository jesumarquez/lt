using System;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileActivityVo
    {
        public const int KeyIndexIdMovil = 0;

        public const int IndexTipoVehiculo = 0;
        public const int IndexCentroDeCostos = 1;
        public const int IndexMovil = 2;
        public const int IndexPatente = 3;
        public const int IndexRecorrido = 4;
        public const int IndexCosto = 5;
        public const int IndexHorasActivo = 6;
        public const int IndexHorasDetenido = 7;
        public const int IndexInfracciones = 8;
        public const int IndexLeves = 9;
        public const int IndexMedias = 10;
        public const int IndexGraves = 11;
        public const int IndexHorasInfraccion = 12;
        public const int IndexVelocidadPromedio = 13;
        public const int IndexVelocidadMaxima = 14;

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true, IsInitialGroup = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true)]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexMovil, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, InitialSortExpression = true, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "Vehículos: {0}")]
        public string Movil { get; set; }

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", AllowGroup = true, InitialSortExpression = true, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "Vehículos: {0}")]
        public string Patente { get; set; }

        [GridMapping(Index = IndexRecorrido, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:2} km", AllowGroup = false, IsAggregate = true, AggregateTextFormat = "Kilómetros: {0:0.00}")]
        public double Recorrido { get; set; }

        [GridMapping(Index = IndexCosto, ResourceName = "Labels", VariableName = "COSTO", DataFormatString = "$ {0:0.00}", AllowGroup = false, IsAggregate = true, AggregateTextFormat = "$ {0:0.00}")]
        public double Costo { get; set; }

        [GridMapping(Index = IndexHorasActivo, ResourceName = "Labels", VariableName = "MOVIMIENTO", AllowGroup = false)]
        public string HorasActivo { get; set; }

        [GridMapping(Index = IndexHorasDetenido, ResourceName = "Labels", VariableName = "DETENCION", AllowGroup = false)]
        public string HorasDetenido { get; set; }

        [GridMapping(Index = IndexInfracciones, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public int Infracciones { get; set; }

        [GridMapping(Index = IndexLeves, ResourceName = "Labels", VariableName = "LEVES", AllowGroup = false)]
        public int Leves { get; set; }

        [GridMapping(Index = IndexMedias, ResourceName = "Labels", VariableName = "MEDIAS", AllowGroup = false)]
        public int Medias { get; set; }

        [GridMapping(Index = IndexGraves, ResourceName = "Labels", VariableName = "GRAVES", AllowGroup = false)]
        public int Graves { get; set; }

        [GridMapping(Index = IndexHorasInfraccion, ResourceName = "Labels", VariableName = "TIEMPO_INFRACCION", AllowGroup = false)]
        public string HorasInfraccion { get; set; }

        [GridMapping(Index = IndexVelocidadPromedio, ResourceName = "Labels", VariableName = "VELOCIDAD_PROMEDIO", AllowGroup = false)]
        public double VelocidadPromedio { get; set; }

        [GridMapping(Index = IndexVelocidadMaxima, ResourceName = "Labels", VariableName = "VELOCIDAD_MAXIMA", AllowGroup = false)]
        public double VelocidadMaxima { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdMovil { get; set; }

        public MobileActivityVo(MobileActivity t)
        {
            IdMovil = t.Id;
            VelocidadMaxima = t.VelocidadMaxima;
            VelocidadPromedio = t.VelocidadPromedio;
            Infracciones = t.Infracciones;
            Movil = t.Movil;
            Patente = t.Patente;
            CentroDeCostos = t.CentroDeCostos;
            TipoVehiculo = t.TipoVehiculo;
            Recorrido = t.Recorrido;
            var dao = new DAOFactory();
            var coche = dao.CocheDAO.FindById(t.Id);
            Costo = coche != null && coche.CocheOperacion != null ? Recorrido * coche.CocheOperacion.CostoKmHistorico : 0.0;
            HorasActivo = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasActivo.Days, t.HorasActivo.Hours, t.HorasActivo.Minutes, t.HorasActivo.Seconds);
            HorasDetenido = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasDetenido.Days, t.HorasDetenido.Hours, t.HorasDetenido.Minutes, t.HorasDetenido.Seconds);
            HorasInfraccion = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasInfraccion.Days, t.HorasInfraccion.Hours, t.HorasInfraccion.Minutes, t.HorasInfraccion.Seconds);
        }

        public MobileActivityVo(MobileActivity t, DateTime desde, DateTime hasta, bool verDetalle)
        {
            IdMovil = t.Id;
            VelocidadMaxima = t.VelocidadMaxima;
            VelocidadPromedio = t.VelocidadPromedio;
            Infracciones = t.Infracciones;
            Movil = t.Movil;
            Patente = t.Patente;
            CentroDeCostos = t.CentroDeCostos;
            TipoVehiculo = t.TipoVehiculo;
            Recorrido = t.Recorrido;
            var dao = new DAOFactory();
            var coche = dao.CocheDAO.FindById(t.Id);
            Costo = coche != null && coche.CocheOperacion != null ? Recorrido * coche.CocheOperacion.CostoKmHistorico : 0.0;
            HorasActivo = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasActivo.Days, t.HorasActivo.Hours,t.HorasActivo.Minutes, t.HorasActivo.Seconds);
            HorasDetenido = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasDetenido.Days, t.HorasDetenido.Hours, t.HorasDetenido.Minutes, t.HorasDetenido.Seconds);
            HorasInfraccion = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasInfraccion.Days, t.HorasInfraccion.Hours, t.HorasInfraccion.Minutes, t.HorasInfraccion.Seconds);

            if (verDetalle)
            {
                var infracciones = dao.InfraccionDAO.GetByVehiculo(IdMovil, Infraccion.Codigos.ExcesoVelocidad, desde, hasta);
                Leves = infracciones.Count(inf => (inf.Alcanzado - inf.Permitido) <= 9);
                Medias = infracciones.Count(inf => (inf.Alcanzado - inf.Permitido) > 9
                                                   && (inf.Alcanzado - inf.Permitido) <= 17);
                Graves = infracciones.Count(inf => (inf.Alcanzado - inf.Permitido) > 17);
            }
        }
    }
}
