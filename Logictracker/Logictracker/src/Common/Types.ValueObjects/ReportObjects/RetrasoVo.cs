using System;
using Logictracker.Process.CicloLogistico;
using Logictracker.Security;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class RetrasoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexCoche = 1;
        public const int IndexCliente = 2;
        public const int IndexPuntoDeEntrega = 3;
        public const int IndexTelefono = 4;
        public const int IndexInicio = 5;
        public const int IndexEnCiclo = 6;
        public const int IndexEnGeocerca = 7;

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexCoche, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, IncludeInSearch = true)]
        public string Coche { get; set; }

        [GridMapping(Index = IndexCliente, ResourceName = "Entities", VariableName = "CLIENT", AllowGroup = true, IncludeInSearch = true)]
        public string Cliente { get; set; }

        [GridMapping(Index = IndexPuntoDeEntrega, ResourceName = "Entities", VariableName = "PARENTI44", AllowGroup = true, IncludeInSearch = true)]
        public string PuntoDeEntrega { get; set; }

        [GridMapping(Index = IndexTelefono, ResourceName = "Labels", VariableName = "TELEFONO", AllowGroup = false, IncludeInSearch = true)]
        public string Telefono { get; set; }

        [GridMapping(Index = IndexInicio, AllowGroup = false, ResourceName = "Labels", VariableName = "INICIO")]
        public string Inicio { get; set; }

        [GridMapping(Index = IndexEnCiclo, AllowGroup = false, ResourceName = "Labels", VariableName = "EN_CICLO")]
        public TimeSpan TiempoEnCiclo { get; set; }

        [GridMapping(Index = IndexEnGeocerca, AllowGroup = false, ResourceName = "Labels", VariableName = "EN_GEOCERCA")]
        public TimeSpan Atraso { get; set; }

        public RetrasoVo(ICicloLogistico cicloLogistico, DateTime fecha)
        {
            Codigo = cicloLogistico.Codigo;
            Coche =  cicloLogistico.Interno;
            Cliente = cicloLogistico.Cliente;
            PuntoDeEntrega = cicloLogistico.PuntoEntrega;
            Telefono = cicloLogistico.Telefono;
            Inicio = cicloLogistico.Iniciado.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            TiempoEnCiclo = fecha.Subtract(cicloLogistico.Iniciado);
            Atraso = cicloLogistico.EnGeocercaDesde.HasValue ? fecha.Subtract(cicloLogistico.EnGeocercaDesde.Value) : new TimeSpan(0, 0, 0);
        }
    }
}
