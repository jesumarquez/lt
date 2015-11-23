using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Web.Models;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.Factories;

namespace Logictracker.Web.Controllers.api
{
    public class TicketRechazoModel
    {
        public int DistritoId { get; set; }
        public int LineaId { get; set; }
        public DateTime FechaHora { get; set; }
        public int TicketRechazoId { get; set; }
        public int ClienteId { get; set; }
        public string CodCliente { get; set; }
        public string Cliente { get; set; }
        public int SupVenId { get; set; }
        public string SupVenDesc { get; set; }
        public int SupRutId { get; set; }
        public string SupRutDesc { get; set; }
        public string Estado { get; set; }
        public string Territorio { get; set; }
        public string Motivo { get; set; }
        public int Bultos { get; set; }
        public int VendedorId { get; set; }
        public string Observaciones { get; set; }
        public bool EnHorario { get; set; }
    }

    public class TicketRechazoMapper : EntityModelMapper<TicketRechazo, TicketRechazoModel>
    {
        public override TicketRechazoModel EntityToModel(TicketRechazo entity, TicketRechazoModel model)
        {
            model.TicketRechazoId = entity.Id;
            model.DistritoId = entity.Empresa.Id;
            model.LineaId = entity.Linea.Id;
            model.FechaHora = entity.FechaHora;
            model.ClienteId = entity.Cliente.Id;
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
            entity.Id = model.TicketRechazoId;
            entity.Empresa = DAOFactory.GetDao<EmpresaDAO>().FindById(model.DistritoId);
            entity.Linea = DAOFactory.GetDao<LineaDAO>().FindById(model.LineaId);
            entity.FechaHora = DateTime.UtcNow;//model.FechaHora;
            entity.Cliente = DAOFactory.GetDao<ClienteDAO>().FindById(model.ClienteId);
            entity.Territorio = model.Territorio;
            entity.Bultos = model.Bultos;
            entity.Vendedor = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.VendedorId);
            entity.SupervisorRuta = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.SupRutId);
            entity.SupervisorVenta = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.SupVenId);

            return entity;
        }

        public override ItemModel ToItem(TicketRechazo entity)
        {
            throw new NotImplementedException();
        }
    }
}