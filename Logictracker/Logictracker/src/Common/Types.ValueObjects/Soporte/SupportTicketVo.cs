using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Support;

namespace Logictracker.Types.ValueObjects.Soporte
{
    [Serializable]
    public class SupportTicketVo
    {
        public const int IndexId = 0;
        public const int IndexFecha = 1;
        public const int IndexTipoProblema = 2;
        public const int IndexDescripcion = 3;
        public const int IndexNombre = 4;
        public const int IndexTelefono = 5;
        public const int IndexMail = 6;
        public const int IndexCurrentState = 7;
        public const int IndexEmpresa = 8;
        public const int IndexVehiculo = 10;
        public const int IndexDispositivo = 11;

        [GridMapping(Index = IndexId, ResourceName = "Entities", VariableName = "OPETICK01", AllowGroup = false, IncludeInSearch = true)]
        public Int32 Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:G}", InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexTipoProblema, ResourceName = "Labels", VariableName = "INCIDENCE", IncludeInSearch = false)]
        public short TipoProblema { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", IncludeInSearch = true, AllowGroup = false)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "CONTACTO", AllowGroup = false)]
        public String Nombre { get; set; }

        [GridMapping(Index = IndexTelefono, ResourceName = "Labels", VariableName = "MDN", AllowGroup = false)]
        public String Telefono { get; set; }

        [GridMapping(Index = IndexMail, ResourceName = "Labels", VariableName = "MAIL", AllowGroup = false)]
        public String Mail { get; set; }

        [GridMapping(Index = IndexCurrentState, ResourceName = "Labels", VariableName = "STATE", IncludeInSearch = true)]
        public short CurrentState { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", IncludeInSearch = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI08", IncludeInSearch = true)]
        public string Dispositivo { get; set; }

        public short Categoria { get; set; }

        public SupportTicketVo(SupportTicket supportTicket)
        {
            Id = supportTicket.Id;
            Fecha = supportTicket.Fecha.ToDisplayDateTime();
            TipoProblema = supportTicket.TipoProblema;
            Descripcion = supportTicket.Descripcion;
            Nombre = supportTicket.Nombre;
            Descripcion = supportTicket.Descripcion.Replace("<", "&lt;").Replace(">", "&gt;");
            Telefono = supportTicket.Telefono;
            Mail = supportTicket.Mail;
            CurrentState = supportTicket.CurrentState;
            Categoria = (short)(supportTicket.CategoriaObj != null 
                                    ? supportTicket.CategoriaObj.Id > 7
                                        ? supportTicket.CategoriaObj.Id - 9
                                        : supportTicket.CategoriaObj.Id - 1
                                    : 0);
            Empresa = supportTicket.Empresa != null ? supportTicket.Empresa.RazonSocial : null;
            Vehiculo = supportTicket.Vehiculo != null ? supportTicket.Vehiculo.Interno : null;
            Dispositivo = supportTicket.Dispositivo != null ? supportTicket.Dispositivo.Codigo : null;
        }
    }
}
