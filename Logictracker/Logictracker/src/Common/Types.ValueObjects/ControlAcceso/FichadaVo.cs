using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ControlAcceso
{
    [Serializable]
    public class FichadaVo
    {
        public const int IndexLegajo = 0;
        public const int IndexNombre = 1;
        public const int IndexTipoEmpleado = 2;
        public const int IndexDepartamento = 3;
        public const int IndexCentroDeCosto = 4;
        public const int IndexResponsable = 5;
        public const int IndexFecha = 6;
        public const int IndexPuertaEntrada = 7;
        public const int IndexHoraEntrada = 8;
        public const int IndexPuertaSalida = 9;
        public const int IndexHoraSalida = 10;
        public const int IndexDuracionJornada = 11;
        public const int IndexEdit = 12;

        public const int IndexKeyIdEntrada = 0;
        public const int IndexKeyIdSalida = 1;
        public const int IndexKeyEntradaDeleted = 2;
        public const int IndexKeySalidaDeleted = 3;
        public const int IndexKeyEntradaEdited = 4;
        public const int IndexKeySalidaEdited = 5;


        [GridMapping(IsDataKey = true, Index = IndexKeyIdEntrada)]
        public int IdEntrada { get; set; }
        [GridMapping(IsDataKey = true, Index = IndexKeyIdSalida)]
        public int IdSalida { get; set; }
        [GridMapping(IsDataKey = true, Index = IndexKeyEntradaDeleted)]
        public bool EntradaDeleted { get; set; }
        [GridMapping(IsDataKey = true, Index = IndexKeySalidaDeleted)]    
        public bool SalidaDeleted { get; set; }
        [GridMapping(IsDataKey = true, Index = IndexKeyEntradaEdited)]
        public bool EntradaEdited { get; set; }
        [GridMapping(IsDataKey = true, Index = IndexKeySalidaEdited)]
        public bool SalidaEdited { get; set; }

        [GridMapping(Index = IndexLegajo, ResourceName = "Labels", VariableName = "LEGAJO", AllowGroup = true, IncludeInSearch = true)]
        public string Legajo { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 0, IncludeInSearch = true)]
        public string Nombre { get; set; }

        [GridMapping(Index = IndexTipoEmpleado, ResourceName = "Entities", VariableName = "PARENTI43", AllowGroup = true)]
        public string TipoEmpleado { get; set; }

        [GridMapping(Index = IndexDepartamento, ResourceName = "Entities", VariableName = "PARENTI04", AllowGroup = true)]
        public string Departamento { get; set; }

        [GridMapping(Index = IndexCentroDeCosto, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true)]
        public string CentroDeCosto { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", AllowGroup = true, IncludeInSearch = true)]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DIA", AllowGroup = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexPuertaEntrada, ResourceName = "Labels", VariableName = "ENTRO_A", AllowGroup = true)]
        public string PuertaEntrada { get; set; }

        [GridMapping(Index = IndexHoraEntrada, IsTemplate = true, ResourceName = "Labels", VariableName = "DESDE", AllowGroup = false, InitialSortExpression = true, DataFormatString = "dd/MM/yyyy HH:mm")]
        public DateTime HoraEntrada { get; set; }

        [GridMapping(Index = IndexPuertaSalida, ResourceName = "Labels", VariableName = "SALIO_DE", AllowGroup = true)]
        public string PuertaSalida { get; set; }

        [GridMapping(Index = IndexHoraSalida, IsTemplate = true, ResourceName = "Labels", VariableName = "HASTA", AllowGroup = false, DataFormatString = "dd/MM/yyyy HH:mm")]
        public DateTime HoraSalida { get; set; }

        [GridMapping(Index = IndexDuracionJornada, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "custom",ExcelTextFormat="custom")]
        public TimeSpan Duracion { get; set; }

        [GridMapping(Index = IndexEdit, IsTemplate = true)]
        public string Edit { get; set; }

        public bool HasEntrada { get; set; }
        public bool HasSalida { get; set; }
        
        public FichadaVo(EventoAcceso evento)
        {
            SetEvento(evento);
        }
        public void SetEvento(EventoAcceso evento)
        {
            if (evento.Entrada)
            {
                IdEntrada = evento.Id;
                HoraEntrada = evento.Fecha.ToDisplayDateTime();
                PuertaEntrada = evento.Puerta.Descripcion;
                if (!HasSalida) HoraSalida = evento.Fecha.ToDisplayDateTime();
                HasEntrada = true;
                if(evento.Baja.HasValue) EntradaDeleted = true;
                if(evento.Modificado.HasValue) EntradaEdited = true;
            }
            else
            {
                IdSalida = evento.Id;
                HoraSalida = evento.Fecha.ToDisplayDateTime();
                PuertaSalida = evento.Puerta.Descripcion;
                if (!HasEntrada) HoraEntrada = evento.Fecha.ToDisplayDateTime();
                Duracion = HasEntrada ? HoraSalida.Subtract(HoraEntrada) : new TimeSpan(0,0,0);
                HasSalida = true;
                if (evento.Baja.HasValue) SalidaDeleted = true;
                if (evento.Modificado.HasValue) SalidaEdited = true;
            }
            Legajo = evento.Empleado.Legajo;
            Nombre = evento.Empleado.Entidad.Descripcion;
            TipoEmpleado = evento.Empleado.TipoEmpleado != null ? evento.Empleado.TipoEmpleado.Descripcion : string.Empty;
            Departamento = evento.Empleado.Departamento != null ? evento.Empleado.Departamento.Descripcion : string.Empty;
            CentroDeCosto = evento.Empleado.CentroDeCostos != null ? evento.Empleado.CentroDeCostos.Descripcion : string.Empty;
            Responsable = evento.Empleado.Reporta1 != null ? evento.Empleado.Reporta1.Entidad.Descripcion : string.Empty;

            Fecha = HoraEntrada.Date;
        }
    }
}
