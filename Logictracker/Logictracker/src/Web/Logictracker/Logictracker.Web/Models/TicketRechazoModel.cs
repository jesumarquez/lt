using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class TicketRechazoModel
    {
        public int DistritoId { get; set; }
        public int LineaId { get; set; }
        public DateTime FechaHora { get; set; }
        public int TicketRechazoId { get; set; }
        public string CodCliente { get; set; }
        public string Cliente { get; set; }
        public string SupVenDesc { get; set; }
        public string SupRutDesc { get; set; }
        public string Estado { get; set; }
        public string Territorio { get; set; }
        public string Motivo { get; set; }
        public int Bultos { get; set; }
    }

    public class TicketRechazoMapper : EntityModelMapper<TicketRechazo, TicketRechazoModel>
    {
        public override TicketRechazoModel EntityToModel(TicketRechazo entity, TicketRechazoModel model)
        {
            model.TicketRechazoId = entity.Id;
            model.DistritoId = entity.Empresa.Id;
            model.LineaId = entity.Linea.Id;
            model.FechaHora = entity.FechaHora;
            model.CodCliente = entity.Cliente.Codigo;
            model.Cliente = entity.Cliente.Descripcion;
            model.SupVenDesc = entity.SupervisorVenta.Entidad.Descripcion;
            model.SupRutDesc = entity.SupervisorRuta.Entidad.Descripcion;
            model.Estado = CultureManager.GetLabel( TicketRechazo.GetEstadoLabelVariableName(entity.UltimoEstado));
            model.Territorio = entity.Territorio;
            model.Motivo = CultureManager.GetLabel(TicketRechazo.GetMotivoLabelVariableName(entity.Motivo));
            model.Bultos = entity.Bultos;
            return model;
        }

        public override TicketRechazo ModelToEntity(TicketRechazoModel model, TicketRechazo entity)
        {
            throw  new NotImplementedException();
        }

        public override ItemModel ToItem(TicketRechazo entity)
        {
            throw new NotImplementedException();
        }
    }
}