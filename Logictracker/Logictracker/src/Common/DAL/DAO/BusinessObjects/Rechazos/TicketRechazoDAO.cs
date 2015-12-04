using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Rechazos;

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
            var estadosActivos = new List<TicketRechazo.Estado> { TicketRechazo.Estado.Notificado1, TicketRechazo.Estado.Notificado2 };

            return Query.Where(t => t.Empresa.Id == idEmpresa && estadosActivos.Contains(t.UltimoEstado)).ToList();
        }
    }
}
