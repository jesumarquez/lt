#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Culture;

#endregion

namespace Logictracker.Types.ValueObjects.Vehiculos
{
    [Serializable]
    public class CocheVo
    {
        public const int IndexIcono = 0;
        public const int IndexBase = 1;
        public const int IndexTipo = 2;
        public const int IndexInterno = 3;
        public const int IndexPatente = 4;
        public const int IndexResponsable = 5;
        public const int IndexDispositivo = 6;
        public const int IndexTransportista = 7;
        public const int IndexDepartamento = 8;
        public const int IndexCentroDeCostos = 9;
        public const int IndexSubCentroDeCostos = 10;
        public const int IndexReferencia = 11;
        public const int IndexRendimiento = 12;
        public const int IndexCostoKm = 13;

        public int Id { get; set; }

        [GridMapping(Index = IndexIcono, HeaderText = "", IsTemplate = true, Width = "40px", AllowGroup = false)]
        public string Icono { get; set; }


        [GridMapping(Index = IndexBase, ResourceName = "Entities", VariableName = "PARENTI02", IncludeInSearch = false,Visible=false)]
        public string Base { get; set; }

        
        [GridMapping(Index = IndexTipo, ResourceName = "Entities", VariableName = "PARENTI17", IncludeInSearch = true)]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Labels", VariableName = "Interno", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Interno { get; set; }

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", AllowGroup = false, IncludeInSearch = true)]
        public string Patente { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", AllowGroup = false, IncludeInSearch = true)]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07")]
        public string Transportista { get; set; }
        
        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", AllowGroup = false, IncludeInSearch = true)]
        public string Dispositivo { get; set; }

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI37")]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexSubCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI99",Visible=false)]
        public string SubCentroDeCostos { get; set; }

        [GridMapping(Index = IndexDepartamento, ResourceName = "Entities", VariableName = "PARENTI04")]
        public string Departamento { get; set; }

        [GridMapping(Index = IndexReferencia, ResourceName = "Labels", VariableName = "REFFERENCE", IncludeInSearch = true)]
        public string Referencia { get; set; }

        [GridMapping(Index = IndexRendimiento, ResourceName = "Labels", VariableName = "RENDIMIENTO", IncludeInSearch = true)]
        public string Rendimiento { get; set; }

        [GridMapping(Index = IndexCostoKm, ResourceName = "Labels", VariableName = "COSTO_KM", IncludeInSearch = true)]
        public string CostoKm { get; set; }

        public int Estado { get; set; }

        public CocheVo(Coche coche)
        {
            Id = coche.Id;
            Interno = coche.Interno;
            Base = coche.Linea != null ? coche.Linea.Descripcion : "TODOS";
            Transportista = coche.Transportista != null ? coche.Transportista.Descripcion : string.Empty;
            Dispositivo = coche.Dispositivo != null ? coche.Dispositivo.Codigo : string.Empty;
            Estado = coche.Estado;
            Tipo = coche.TipoCoche != null ? coche.TipoCoche.Descripcion : string.Empty;
            Icono = coche.TipoCoche != null ? coche.TipoCoche.IconoDefault.PathIcono : null;
            Patente = coche.Patente;
            CentroDeCostos = coche.CentroDeCostos != null ? coche.CentroDeCostos.Descripcion : string.Empty;
            SubCentroDeCostos = coche.SubCentroDeCostos != null ? coche.SubCentroDeCostos.Descripcion : string.Empty;
            Departamento = coche.Departamento != null ? coche.Departamento.Descripcion : CultureManager.GetLabel("NINGUNO");
            Responsable = coche.Chofer != null ? coche.Chofer.Entidad.Descripcion : string.Empty;
            Referencia = coche.Referencia;
            Rendimiento = coche.CocheOperacion != null && coche.CocheOperacion.Rendimiento > 0.0
                              ? coche.CocheOperacion.Rendimiento.ToString("#0.00")
                              : coche.Modelo != null
                                    ? coche.Modelo.Rendimiento.ToString("#0.00")
                                    : "0,00";
            CostoKm = coche.CocheOperacion != null
                              ? coche.CocheOperacion.CostoKmUltimoMes.ToString("#0.00")
                              : "0,00";
        }
    }
}