using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Support;

namespace Logictracker.Types.ValueObjects.Soporte
{
    [Serializable]
    public class ReporteTicketVo
    {
        public const int IndexCheck = 0;
        public const int IndexEmpresa = 1;
        public const int IndexId = 2;
        public const int IndexFecha = 3;
        public const int IndexCategoria = 4;
        public const int IndexSubcategoria = 5;
        public const int IndexNivel = 6;
        public const int IndexEstadoActual = 7;
        public const int IndexTiempoTotal = 8;
        public const int IndexOpen = 9;
        public const int IndexWorking = 10;
        public const int IndexWaitingUser = 11;
        public const int IndexSolved = 12;
        public const int IndexApproved = 13;
        public const int IndexRejected = 14;
        public const int IndexClosed = 15;
        public const int IndexInvalid = 16;

        [GridMapping(Index = IndexCheck, IsTemplate = true, Width = "32px", AllowGroup = false, AllowMove = false)]
        public bool Check { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", IncludeInSearch = true, AllowGroup = true, AllowMove = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexId, ResourceName = "Labels", VariableName = "TICKET_NUMBER", AllowGroup = false, IncludeInSearch = true, InitialSortExpression = true, SortDirection = GridSortDirection.Ascending)]
        public Int32 Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:G}")]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexCategoria, ResourceName = "Entities", VariableName = "AUDSUP03", IncludeInSearch = false, AllowGroup = true, AllowMove = true)]
        public string Categoria { get; set; }

        [GridMapping(Index = IndexSubcategoria, ResourceName = "Entities", VariableName = "AUDSUP04", IncludeInSearch = false, AllowGroup = true, AllowMove = true)]
        public string Subcategoria { get; set; }

        [GridMapping(Index = IndexNivel, ResourceName = "Entities", VariableName = "AUDSUP05", IncludeInSearch = false, AllowGroup = true, AllowMove = true)]
        public string Nivel { get; set; }

        [GridMapping(Index = IndexEstadoActual, ResourceName = "Labels", VariableName = "STATE", IncludeInSearch = true, AllowGroup = true, AllowMove = true)]
        public string EstadoActual { get; set; }

        [GridMapping(Index = IndexTiempoTotal, ResourceName = "Labels", VariableName = "TIEMPO_TOTAL")]
        public string TiempoTotal { get; set; }

        [GridMapping(Index = IndexOpen, ResourceName = "Labels", VariableName = "SUPPORT_STATE_1_OPEN")]
        public string Open { get; set; }

        [GridMapping(Index = IndexWorking, ResourceName = "Labels", VariableName = "SUPPORT_STATE_2_WORKING")]
        public string Working { get; set; }

        [GridMapping(Index = IndexWaitingUser, ResourceName = "Labels", VariableName = "SUPPORT_STATE_3_WAITING_USER")]
        public string WaitingUser { get; set; }

        [GridMapping(Index = IndexSolved, ResourceName = "Labels", VariableName = "SUPPORT_STATE_4_SOLVED")]
        public string Solved { get; set; }

        [GridMapping(Index = IndexApproved, ResourceName = "Labels", VariableName = "SUPPORT_STATE_5_APPROVED")]
        public string Approved { get; set; }

        [GridMapping(Index = IndexRejected, ResourceName = "Labels", VariableName = "SUPPORT_STATE_6_REJECTED")]
        public string Rejected { get; set; }

        [GridMapping(Index = IndexClosed, ResourceName = "Labels", VariableName = "SUPPORT_STATE_7_CLOSED")]
        public string Closed { get; set; }

        [GridMapping(Index = IndexInvalid, ResourceName = "Labels", VariableName = "SUPPORT_STATE_8_INVALID")]
        public string Invalid { get; set; }

        public ReporteTicketVo (SupportTicket supportTicket)
        {
            Empresa = supportTicket.Empresa != null ? supportTicket.Empresa.RazonSocial : CultureManager.GetLabel("TODOS");
            Id = supportTicket.Id;
            Fecha = supportTicket.Fecha.ToDisplayDateTime();
            Categoria = supportTicket.CategoriaObj != null ? supportTicket.CategoriaObj.Descripcion : string.Empty;
            Subcategoria = supportTicket.Subcategoria != null ? supportTicket.Subcategoria.Descripcion : string.Empty;
            Nivel = supportTicket.NivelObj != null ? supportTicket.NivelObj.Descripcion : string.Empty;

            var estadoActual = string.Empty;

            switch (supportTicket.CurrentState)
            {
                case 0: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_1_OPEN"); break;
                case 1: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_2_WORKING"); break;
                case 2: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_3_WAITING_USER"); break;
                case 3: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_4_SOLVED"); break;
                case 4: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_5_APPROVED"); break;
                case 5: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_6_REJECTED"); break;
                case 6: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_7_CLOSED"); break;
                case 7: estadoActual = CultureManager.GetLabel("SUPPORT_STATE_8_INVALID"); break;
            }

            EstadoActual = estadoActual;

            var tiempoTotal = new TimeSpan(0,0,0);
            var open = new TimeSpan(0,0,0);
            var working = new TimeSpan(0,0,0);
            var waitingUser = new TimeSpan(0,0,0);
            var solved = new TimeSpan(0,0,0);
            var approved = new TimeSpan(0,0,0);
            var rejected = new TimeSpan(0,0,0);
            var closed = new TimeSpan(0,0,0);
            var invalid = new TimeSpan(0,0,0);

            var estados = supportTicket.States.OfType<SupportTicketDetail>().OrderByDescending(d => d.Fecha);

            SupportTicketDetail last = null;
            foreach (var estado in estados)
            {
                var ts = GetTiempo(last, estado.Fecha, estado.SupportTicket.Empresa, estado.SupportTicket.Linea);
                switch (estado.Estado)
                {
                    case 0: open = open.Add(ts); break;
                    case 1: working = working.Add(ts); break;
                    case 2: waitingUser = waitingUser.Add(ts); break;
                    case 3: solved = solved.Add(ts); break;
                    case 4: approved = approved.Add(ts); break;
                    case 5: rejected = rejected.Add(ts); break;
                    case 6: closed = closed.Add(ts); break;
                    case 7: invalid = invalid.Add(ts); break;
                }
                tiempoTotal = tiempoTotal.Add(ts);
                last = estado;
            }

            Open = GetString(open);
            Working = GetString(working);
            WaitingUser = GetString(waitingUser);
            Solved = GetString(solved);
            Approved = GetString(approved);
            Rejected = GetString(rejected);
            Closed = GetString(closed);
            Invalid = GetString(invalid);
            TiempoTotal = GetString(tiempoTotal);
        }

        private static TimeSpan GetTiempo(SupportTicketDetail last, DateTime fecha, Empresa empresa, Linea linea)
        {
            var ts = new TimeSpan();
            var desde = fecha.ToDisplayDateTime();
            var hasta = last != null ? last.Fecha.ToDisplayDateTime() : DateTime.UtcNow.ToDisplayDateTime();

            var codigoTurno = empresa.GetParameter(BusinessObjects.Empresa.Params.TurnoSoporte);
            var dao = new DAOFactory();
            var turno = dao.ShiftDAO.FindByCode(new[] { empresa.Id }, new[] { linea != null ? linea.Id : -1 }, codigoTurno);
            var feriados = dao.FeriadoDAO.FindAll().Select(f => f.Fecha.DayOfYear);

            var dias = GetDias(desde, hasta);

            for (var i = 0; i < dias.Count; i++)
            {
                var dia = dias[i.ToString()];
                var maxDesde = turno != null && turno.Inicio > dia[0].TimeOfDay.TotalHours ? turno.Inicio : dia[0].TimeOfDay.TotalHours;
                var minHasta = turno != null && turno.Fin < dia[1].TimeOfDay.TotalHours ? turno.Fin : dia[1].TimeOfDay.TotalHours;

                if (turno == null && feriados.Contains(dia[0].DayOfYear))
                    continue;
                if (turno != null && !turno.AppliesToDate(dia[0], feriados))
                    continue;
                
                if (minHasta > maxDesde)
                    ts = ts.Add(TimeSpan.FromHours(minHasta - maxDesde));
            }

            return ts;
        }

        private static Dictionary<string, DateTime[]> GetDias(DateTime desde, DateTime hasta)
        {
            var dias = new Dictionary<string, DateTime[]>();
            var i = 0;

            if (desde.Date == hasta.Date)
            {
                dias.Add(i.ToString(), new[] {desde, hasta});
            }
            else
            {
                var inicio = desde;
                while (inicio.Date < hasta.Date)
                {
                    dias.Add(i.ToString(), new[] { inicio, inicio.Date.AddDays(1).AddSeconds(-1) });
                    inicio = inicio.Date.AddDays(1);
                    i++;
                }
                if (inicio.Date == hasta.Date)
                {
                    dias.Add(i.ToString(), new[] { inicio, hasta });
                }
            }

            return dias;
        }

        private static string GetString(TimeSpan ts)
        {
            var str = new StringBuilder();

            if (ts.TotalDays > 1)
                str.Append((int)ts.TotalDays + " d ");

            ts = ts.Subtract(new TimeSpan((int) ts.TotalDays));

            str.Append(ts.Hours + " hs " + ts.Minutes + " min");

            return str.ToString();
        }
    }
}
