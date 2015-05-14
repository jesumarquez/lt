using System;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class TicketMantenimientoVo
    {
        public const int IndexId = 0;
        public const int IndexFecha = 1;
        public const int IndexCodigo = 2;
        public const int IndexTaller = 3;
        public const int IndexEmpresa = 4;
        public const int IndexVehiculo = 5;
        public const int IndexEstado = 6;
        public const int IndexTurno = 7;
        public const int IndexIngreso = 8;
        public const int IndexEgreso = 9;
        public const int IndexDescripcion = 10;
        
        [GridMapping(Index = IndexId, ResourceName = "Labels", VariableName = "ID", AllowGroup = false, IncludeInSearch = true)]
        public int Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", AllowGroup = false, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexTaller, ResourceName = "Entities", VariableName = "PARENTI35", AllowGroup = true)]
        public string Taller { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", AllowGroup = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true)]
        public string Estado { get; set; }

        [GridMapping(Index = IndexTurno, ResourceName = "Labels", VariableName = "TURNO", AllowGroup = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? Turno { get; set; }

        [GridMapping(Index = IndexEgreso, ResourceName = "Labels", VariableName = "EGRESO", AllowGroup = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime ? Egreso { get; set; }

        [GridMapping(Index = IndexIngreso, ResourceName = "Labels", VariableName = "INGRESO", AllowGroup = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime ? Ingreso { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public TicketMantenimientoVo(TicketMantenimiento ticket)
        {
            Id = ticket.Id;
            Fecha = ticket.FechaSolicitud.ToDisplayDateTime();
            Codigo = ticket.Codigo;
            Descripcion = ticket.Descripcion;
            Estado = CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(ticket.Estado));
            Empresa = ticket.Empresa.RazonSocial;
            Vehiculo = ticket.Vehiculo != null ? ticket.Vehiculo.Interno : string.Empty;
            Taller = ticket.Taller != null ? ticket.Taller.Descripcion : string.Empty;
            Turno = ticket.FechaTurno.HasValue ? ticket.FechaTurno.Value.ToDisplayDateTime() : (DateTime?) null;
            Egreso = ticket.Salida.HasValue ? ticket.Salida.Value.ToDisplayDateTime() : (DateTime ?) null;
            Ingreso = ticket.Entrada.HasValue ? ticket.Entrada.Value.ToDisplayDateTime() : (DateTime?) null;
        }
    }
}
