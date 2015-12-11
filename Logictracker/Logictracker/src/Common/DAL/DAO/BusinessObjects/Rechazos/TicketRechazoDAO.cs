using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Types.BusinessObjects;

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
            return Query.Where(t => t.Entrega.Id == idPuntoEntrega 
                                 && t.FechaHora > desde 
                                 && t.FechaHora < hasta)
                        .FirstOrDefault();
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

            return q.ToList();
        }
    }
}
