using System;
using System.Linq;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class ViajeDistribucionVo
    {
        public const int IndexTipo = 0;
        public const int IndexCodigo = 1;
        public const int IndexInicio = 2;
        public const int IndexInicioReal = 3;
        public const int IndexVehiculo = 4;
        public const int IndexParadas = 5;
        public const int IndexNoNomencladas = 6;
        public const int IndexEstado = 7;

        public int Id { get; set; }
        public int IdEmpresa { get; set; }
        public int IdLinea { get; set; }
        public int IdVehiculo { get; set; }
        public int IdTipoVehiculo { get; set; }

        [GridMapping(Index = IndexTipo, ResourceName = "Labels", VariableName = "TYPE", AllowGroup = true)]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE_DISTRIBUCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:dd/MM/yyyy HH:mm}", AllowGroup = false)]
        public DateTime Inicio { get; set; }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "Inicio", DataFormatString = "{0:dd/MM/yyyy HH:mm}", AllowGroup = false)]
        public DateTime? InicioReal { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexParadas, ResourceName = "Labels", VariableName = "ENTREGAS", AllowGroup = false)]
        public int Paradas { get; set; }

        [GridMapping(Index = IndexNoNomencladas, ResourceName = "Labels", VariableName = "NO_NOMENCLADAS", AllowGroup = false, Visible = false)]
        public int NoNomencladas { get; set; }

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "STATE", Width = "100px", AllowGroup = false, IsTemplate = true)]
        public short Estado { get; set; }

        public bool HasCoche { get; set; }

        public bool Nomenclado { get; set; }

        private double _kmControlado = -1;
        public double KmControlado { get
        {
            return _kmControlado > -1
                       ? _kmControlado
                       : (_kmControlado = Distribucion.Detalles.Where(e => e.KmControlado.HasValue).Sum(e => e.KmControlado).Value);
        } }

        private ViajeDistribucion Distribucion { get; set; }

        public ViajeDistribucionVo(ViajeDistribucion viaje)
        {
            Distribucion = viaje;
            Id = viaje.Id;
            IdEmpresa = viaje.Empresa.Id;
            IdLinea = viaje.Linea.Id;
            IdVehiculo = viaje.Vehiculo != null ? viaje.Vehiculo.Id : 0;
            IdTipoVehiculo = viaje.TipoCoche != null ? viaje.TipoCoche.Id : 0;
            Codigo = viaje.Codigo;
            Inicio = viaje.Inicio.ToDisplayDateTime();
            InicioReal = viaje.InicioReal.HasValue ? viaje.InicioReal.Value.ToDisplayDateTime() : (DateTime?) null;
            Vehiculo = viaje.Vehiculo != null ? viaje.Vehiculo.Interno : string.Empty;
            Paradas = viaje.EntregasTotalCount;
            Estado = viaje.Estado;
            HasCoche = viaje.Vehiculo != null;
            Tipo = viaje.Tipo == ViajeDistribucion.Tipos.Ordenado ? CultureManager.GetLabel("TIPODISTRI_NORMAL")
                : viaje.Tipo == ViajeDistribucion.Tipos.Desordenado ? CultureManager.GetLabel("TIPODISTRI_DESORDENADA")
                : viaje.Tipo == ViajeDistribucion.Tipos.RecorridoFijo ? CultureManager.GetLabel("TIPODISTRI_RECORRIDO_FIJO")
                : string.Empty;

            var entregasNomencladas = viaje.EntregasNomencladasCount;

            Nomenclado = entregasNomencladas == Paradas;
            NoNomencladas = Nomenclado ? 0 : Paradas - entregasNomencladas;
        }
    }
}
