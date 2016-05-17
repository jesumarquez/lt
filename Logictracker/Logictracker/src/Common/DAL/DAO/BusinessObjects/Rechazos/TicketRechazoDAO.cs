using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Rechazos;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects.Rechazos
{
    public class TicketRechazoDAO : GenericDAO<TicketRechazo>
    {
        public IEnumerable<KeyValuePair<TicketRechazo.MotivoRechazo, string>> GetMotivos()
        {
            return Enum.GetValues(typeof(TicketRechazo.MotivoRechazo))
                .Cast<TicketRechazo.MotivoRechazo>()
                .Select(k => new KeyValuePair<TicketRechazo.MotivoRechazo, string>(k, CultureManager.GetLabel(TicketRechazo.GetMotivoLabelVariableName(k))));
        }

        public IEnumerable<KeyValuePair<TicketRechazo.Estado, string>> GetEstados()
        {
            return Enum.GetValues(typeof(TicketRechazo.Estado))
                .Cast<TicketRechazo.Estado>()
                .Select(k => new KeyValuePair<TicketRechazo.Estado, string>(k, CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(k))));
        }

        public TicketRechazo GetByPuntoEntregaYFecha(int idPuntoEntrega, DateTime desde, DateTime hasta)
        {
            return Query.FirstOrDefault(t => t.Entrega.Id == idPuntoEntrega
                                             && t.FechaHora > desde
                                             && t.FechaHora < hasta);
        }

        public IEnumerable<TicketRechazo> GetActivos(int idEmpresa)
        {
            var estadosActivos = new List<TicketRechazo.Estado> { TicketRechazo.Estado.Notificado1, TicketRechazo.Estado.Notificado2, TicketRechazo.Estado.Notificado3 };

            return Query.Where(t => t.Empresa.Id == idEmpresa && estadosActivos.Contains(t.UltimoEstado)).ToList();
        }

        public IEnumerable<TicketRechazo> GetByEmpleadoAndFecha(Empleado empleado, DateTime desde, DateTime hasta)
        {
            var q = Query.Where(t => t.Empresa.Id == empleado.Empresa.Id 
                                 && t.FechaHora > desde 
                                 && t.FechaHora < hasta);


            if (empleado == null || empleado.TipoEmpleado == null)
                return q.ToList();

            if (empleado.TipoEmpleado != null)
            {
                switch (empleado.TipoEmpleado.Codigo)
                {
                    case "SR":
                        q = q.Where(t => t.SupervisorRuta.Id == empleado.Id);
                        break;
                    case "JF":
                        q = q.Where(t => t.SupervisorVenta.Id == empleado.Id);
                        break;
                    case "V":
                        q = q.Where(t => t.Vendedor.Id == empleado.Id);
                        break;
                }
            }

            return q.ToList();
        }

        public IEnumerable<RechazoPromedioRolModel> GetPromedioPorRol(int distritoId, int baseId)
        {
            RechazoMov mov = null;
            Empleado empEgreso = null;
            TipoEmpleado tEmpleado = null;
            TicketRechazo ticket = null;

            var q = Session
                .QueryOver(() => mov)
                .Inner.JoinAlias(() => mov.EmpledoEgreso, () => empEgreso)
                .Left.JoinAlias(() => empEgreso.TipoEmpleado, () => tEmpleado)
                .Inner.JoinAlias(() => mov.Ticket, () => ticket)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Count(() => mov.Id).As("Cantidad"))
                    .Add(Projections.Avg(() => mov.Lapso).As("Promedio"))
                    .Add(Projections.Group(() => tEmpleado.Codigo).As("TipoEmpleado"))
                );

            if (distritoId != -1)
                q = q.Where(m => ticket.Empresa.Id == distritoId);

            if (baseId != -1)
                q = q.Where(m => ticket.Linea.Id == baseId);

            q = q.TransformUsing(Transformers.AliasToBean<RechazoPromedioRolModel>());

            return q.Future<RechazoPromedioRolModel>();
        }

        public IEnumerable<PromedioPorEstadoModel> GetPromedioPorEstado(int distritoId, int baseId)
        {
            RechazoMov mov = null;
            TicketRechazo ticket = null;

            var q = Session
                .QueryOver(() => mov)
                .Inner.JoinAlias(() => mov.Ticket, () => ticket)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Count(() => mov.Id).As("Cantidad"))
                    .Add(Projections.Avg(() => mov.Lapso).As("Promedio"))
                    .Add(Projections.Group(() => mov.EstadoEgreso).As("EstadoEnum"))
                    .Add(Projections.Group(() => ticket.Empresa.Id).As("EmpresaId"))
                    .Add(Projections.Group(() => ticket.Linea.Id).As("BaseId"))
                ).OrderBy(Projections.Avg(() => mov.Lapso).As("Promedio")).Desc;

            if (distritoId != -1)
                q = q.Where(m => ticket.Empresa.Id == distritoId);

            if (baseId != -1)
                q = q.Where(m => ticket.Linea.Id == baseId);

            q = q.TransformUsing(Transformers.AliasToBean<PromedioPorEstadoModel>());

            return q.Future<PromedioPorEstadoModel>();
        }

        public IEnumerable<PromedioPorVendedorModel> GetPromedioPorVendedor(int distritoId, int baseId)
        {
            RechazoMov mov = null;
            TicketRechazo ticket = null;
            Empleado empEgreso = null;
            TipoEmpleado tEmpleado = null;
            Entidad entidad = null;

            var q = Session
                .QueryOver(() => mov)
                .Inner.JoinAlias(() => mov.EmpledoEgreso, () => empEgreso)
                .Left.JoinAlias(() => empEgreso.TipoEmpleado, () => tEmpleado)
                .Inner.JoinAlias(() => mov.Ticket, () => ticket)
                .Inner.JoinAlias(() => empEgreso.Entidad, () => entidad)
                .Where(() => tEmpleado.Codigo == "V")
                .Select(Projections.ProjectionList()
                    .Add(Projections.Count(() => mov.Id).As("Cantidad"))
                    .Add(Projections.Avg(() => mov.Lapso).As("Promedio"))
                    .Add(Projections.Group(() => mov.EstadoEgreso).As("EstadoEnum"))
                    .Add(Projections.Group(() => ticket.Empresa.Id).As("EmpresaId"))
                    .Add(Projections.Group(() => ticket.Linea.Id).As("BaseId"))
                    .Add(Projections.Group(() => entidad.Nombre).As("EntidadNombre"))
                    .Add(Projections.Group(() => entidad.Apellido).As("EntidadApellido"))
                ).OrderBy(Projections.Avg(() => mov.Lapso).As("Promedio")).Desc;

            if (distritoId != -1)
                q = q.Where(m => ticket.Empresa.Id == distritoId);

            if (baseId != -1)
                q = q.Where(m => ticket.Linea.Id == baseId);

            q = q.TransformUsing(Transformers.AliasToBean<PromedioPorVendedorModel>());

            return q.Future<PromedioPorVendedorModel>();
        }

        public IEnumerable<TicketPorHoraModel> GetCantidadTicketPorHora(int distritoId, int baseId)
        {
            RechazoMov mov = null;
            TicketRechazo ticket = null;
            List<TicketPorHoraModel> result = new List<TicketPorHoraModel>();

            TicketPorHoraModel item = null;

            var hourNow = DateTime.UtcNow.Hour;

            for (int hour = 0; hour <= hourNow; hour++)
            {
                // Obtengo un subQuery con la lista de id de movimientos para una hora agrupados por idTicket
                var subQ = Session
                    .QueryOver<RechazoMov>(() => mov)
                    .Inner.JoinAlias(() => mov.Ticket, () => ticket)
                    .Where(() => mov.Egreso.Hour < hour)
                    .Select(Projections.ProjectionList()
                        .Add(Projections.Max(() => mov.Id))
                        .Add(Projections.Group(() => ticket.Id)));

                var l = subQ.List<object[]>().Select(i => i[0]).ToArray();

                var q = Session
                .QueryOver(() => mov)
                .Inner.JoinAlias(() => mov.Ticket, () => ticket)
                .Where(() => mov.Id.IsIn(l))
                .Select(Projections.ProjectionList()
                    .Add(Projections.Count(() => mov.Ticket.Id).As("Cantidad"))
                    .Add(Projections.Group(() => ticket.Final).As("Estado"))
                     );

                if (distritoId != -1)
                    q = q.Where(m => ticket.Empresa.Id == distritoId);

                if (baseId != -1)
                    q = q.Where(m => ticket.Linea.Id == baseId);

                q = q.TransformUsing(Transformers.AliasToBean<TicketEstadoPorHoraModel>());

                item = new TicketPorHoraModel();
                item.Hora = hour;

                var ticketEstado = q.Future<TicketEstadoPorHoraModel>().ToList();
                ticketEstado.ForEach(t =>
                {
                    switch (t.Estado)
                    {
                        case TicketRechazo.EstadoFinal.Anulado:
                            item.CantidadAnulado = t.Cantidad;
                            break;
                        case TicketRechazo.EstadoFinal.RechazoDuplicado:
                            item.CantidadDuplicado = t.Cantidad;
                            break;
                        case TicketRechazo.EstadoFinal.RechazoErroneo:
                            item.CantidadErroneo = t.Cantidad;
                            break;
                        case TicketRechazo.EstadoFinal.ResueltoEntregado:
                            item.CantidadResueltoEntregado = t.Cantidad;
                            break;
                        case TicketRechazo.EstadoFinal.ResueltoSinEntrega:
                            item.CantidadSinEntrega = t.Cantidad;
                            break;
                        case TicketRechazo.EstadoFinal.SolucionPendiente:
                            item.CantidadPendiente = t.Cantidad;
                            break;
                    }
                });

                result.Add(item);
            }

            return result;
        }
    }

    public class TicketEstadoPorHoraModel
    {
        public int Cantidad { get; set; }
        public TicketRechazo.EstadoFinal Estado { get; set; }
    }

    public class TicketPorHoraModel
    {
        public int CantidadPendiente { get; set; }
        public int CantidadDuplicado { get; set; }
        public int CantidadErroneo { get; set; }
        public int CantidadSinEntrega { get; set; }
        public int CantidadAnulado { get; set; }
        public int CantidadResueltoEntregado { get; set; }
        public int Hora { get; set; }
    }

    public class RechazoPromedioRolModel
    {
        public int EmpresaId { get; set; }
        public int BaseId { get; set; }
        public int Cantidad { get; set; }
        public double Promedio { get; set; }
        public string TipoEmpleado { get; set; }
    }

    public class PromedioPorEstadoModel
    {
        public int EmpresaId { get; set; }
        public int BaseId { get; set; }
        public int Cantidad { get; set; }
        public int EstadoEnum { get; set; }

        public string Estado
        {
            get { return CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName((TicketRechazo.Estado)EstadoEnum)); }
        }

        public double Promedio { get; set; }

        public string PromedioMinutos
        {
            get
            {
                return TimeSpan.FromSeconds(this.Promedio).ToString(@"hh\:mm");
            }
        }
    }

    public class PromedioPorVendedorModel
    {
        public int EmpresaId { get; set; }
        public int BaseId { get; set; }
        public string Usuario
        {
            get
            {
                if (EntidadNombre == null) return EntidadApellido.Trim();

                return EntidadApellido.Trim() + ", " + EntidadNombre.Trim();
            }
        }
        public int EstadoEnum { get; set; }
        public string EntidadNombre { get; set; }
        public string EntidadApellido { get; set; }
        public string EstadoEgreso
        {
            get { return CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName((TicketRechazo.Estado)EstadoEnum)); }
        }
        public double Promedio { get; set; }
        public string PromedioMinutos
        {
            get
            {
                return TimeSpan.FromSeconds(this.Promedio).ToString(@"hh\:mm");
            }
        }
        public int Cantidad { get; set; }
    }
}
