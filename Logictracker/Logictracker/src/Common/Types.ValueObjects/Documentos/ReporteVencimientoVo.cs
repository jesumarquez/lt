using System;
using Logictracker.Types.BusinessObjects.Documentos;

namespace Logictracker.Types.ValueObjects.Documentos
{
    [Serializable]
    public class ReporteVencimientoVo
    {
        public const int IndexTipoDocumento = 0;
        public const int IndexVehiculo = 1;
        public const int IndexCodigo = 2;
        public const int IndexDescripcion = 3;
        public const int IndexVencimiento = 4;
        public const int IndexDiasAlVencimiento = 5;

        [GridMapping(Index = IndexTipoDocumento, ResourceName = "Entities", VariableName = "PARENTI25", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string TipoDocumento { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexVencimiento, ResourceName = "Labels", VariableName = "VENCIMIENTO", DataFormatString = "{0:d}", AllowGroup = true)]
        public DateTime Vencimiento { get; set; }

        [GridMapping(Index = IndexDiasAlVencimiento, ResourceName = "Labels", VariableName = "DIAS_AL_VENCIMIENTO", AllowGroup = true)]
        public int DiasAlVencimiento { get; set; }

        [GridMapping(IsDataKey = true)]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        
        public ReporteVencimientoVo(Documento doc) : this(doc, DateTime.Now) { }

        public ReporteVencimientoVo(Documento doc, DateTime fechaActual)
        {
            Id = doc.Id;
            TipoDocumento = doc.TipoDocumento.Descripcion;
            Vehiculo = doc.Vehiculo != null ? doc.Vehiculo.Interno : string.Empty;
            Codigo = doc.Codigo;
            Descripcion = doc.Descripcion;
            Fecha = doc.Fecha;
            if (doc.Vencimiento.HasValue)
            {
                Vencimiento = doc.Vencimiento.Value;
                DiasAlVencimiento = Convert.ToInt32(doc.Vencimiento.Value.Subtract(fechaActual).TotalDays);
            }
            else
            {
                Vencimiento = DateTime.MaxValue;
                DiasAlVencimiento = 9999;
            }
        }
    }
}
