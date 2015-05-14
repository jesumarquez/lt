using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Auditoria;

namespace Logictracker.Types.ValueObjects.Auditoria
{
    [Serializable]
    public class LoginAuditVo
    {
        public const int IndexUsuario = 0;
        public const int IndexFechaInicio = 1;
        public const int IndexFechaFin = 2;
        public const int IndexIp = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexUsuario, ResourceName = "Entities", VariableName = "USUARIO", IsInitialGroup = true, InitialSortExpression = true, SortDirection = GridSortDirection.Ascending)]
        public string Usuario { get; set; }

        [GridMapping(Index = IndexFechaInicio, ResourceName = "Labels", VariableName = "INICIO", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime FechaInicio { get; set;}

        [GridMapping(Index = IndexFechaFin, ResourceName = "Labels", VariableName = "FIN", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime? FechaFin { get; set; }

        [GridMapping(Index = IndexIp, ResourceName = "Labels", VariableName = "IP", AllowGroup = true)]
        public string Ip { get; set;}

        public LoginAuditVo(LoginAudit audit)
        {
            Id = audit.Id;
            FechaInicio = audit.FechaInicio.ToDisplayDateTime();
            FechaFin = audit.FechaFin.HasValue ? (DateTime?)audit.FechaFin.Value.ToDisplayDateTime() : null;
            Ip = audit.IP;
            Usuario = audit.Usuario.NombreUsuario;
        }
    }
}
