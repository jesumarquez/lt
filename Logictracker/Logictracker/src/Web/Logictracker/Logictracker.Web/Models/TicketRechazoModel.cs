using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
  
    public class TicketRechazoDetalleModel
    {
        public int TicketRechazoDetalleId { get; set; }
        public string Observacion { get; set; }
        public string UsuarioNombre { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; }
    }
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
        public TicketRechazo.MotivoRechazo Motivo { get; set; }
        public int Bultos { get; set; }
        public int VendedorId { get; set; }
        public string Observacion { get; set; }
        public bool EnHorario { get; set; }
        public IList<TicketRechazoDetalleModel> Detalle { get; set; }

        public int EntregaId { get; set; }
        public string EntregaDesc { get; set; }
        public string MotivoDesc { get; set; }
        public int TransportistaId { get; set; }
        public string TransportistaDesc { get; set; }
    }

    public class TicketRechazoDetalleMapper : EntityModelMapper<DetalleTicketRechazo, TicketRechazoDetalleModel>
    {
        public override TicketRechazoDetalleModel EntityToModel(DetalleTicketRechazo entity,
            TicketRechazoDetalleModel model)
        {
            model.TicketRechazoDetalleId = entity.Id;
            model.FechaHora = entity.FechaHora;
            model.UsuarioNombre = entity.Usuario.NombreUsuario;
            model.Observacion = entity.Observacion;
            model.Estado = CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(entity.Estado));
            
            return model;
        }

        public override DetalleTicketRechazo ModelToEntity(TicketRechazoDetalleModel model, DetalleTicketRechazo entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(DetalleTicketRechazo entity)
        {
            throw new NotImplementedException();
        }
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
            model.Cliente = entity.Cliente.Codigo;
            model.SupVenDesc = entity.SupervisorVenta.Entidad.Descripcion;
            model.SupRutDesc = entity.SupervisorRuta.Entidad.Descripcion;
            model.Estado = CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(entity.UltimoEstado));
            model.Territorio = entity.Territorio;
            model.Motivo = entity.Motivo;
            model.MotivoDesc = CultureManager.GetLabel(TicketRechazo.GetMotivoLabelVariableName(entity.Motivo));
            model.Bultos = entity.Bultos;
            model.Observacion = entity.Detalle.First().Observacion;
            model.EnHorario = entity.EnHorario;
            var mt = new TicketRechazoDetalleMapper();
            model.Detalle = new List<TicketRechazoDetalleModel>(entity.Detalle.Select(d => mt.EntityToModel(d, new TicketRechazoDetalleModel())));
            if (entity.Entrega != null && entity.Entrega.Id != 0)
            {
                model.EntregaId = entity.Entrega.Id;
                model.EntregaDesc = entity.Entrega.Descripcion;
                return model;
            }

            if (entity.Transportista != null && entity.Transportista.Id != 0)
            {
                model.TransportistaId = entity.Transportista.Id;
                model.TransportistaDesc = entity.Transportista.Descripcion;
            }
            return model;
        }

        public override TicketRechazo ModelToEntity(TicketRechazoModel model, TicketRechazo entity)
        {
            entity.Id = model.TicketRechazoId;
            entity.Empresa = DAOFactory.GetDao<EmpresaDAO>().FindById(model.DistritoId);
            entity.Linea = DAOFactory.GetDao<LineaDAO>().FindById(model.LineaId);
            entity.Cliente = DAOFactory.GetDao<ClienteDAO>().FindById(model.ClienteId);
            entity.Territorio = model.Territorio;
            entity.Bultos = model.Bultos;
            entity.Vendedor = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.VendedorId);
            entity.SupervisorRuta = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.SupRutId);
            entity.SupervisorVenta = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.SupVenId);
            entity.Motivo = model.Motivo;
            entity.EnHorario = model.EnHorario;
            entity.Entrega = DAOFactory.GetDao<EntregaDistribucionDAO>().FindById(model.EntregaId);
            entity.Transportista = DAOFactory.GetDao<TransportistaDAO>().FindById(model.TransportistaId);
            return entity;
        }

        public override ItemModel ToItem(TicketRechazo entity)
        {
            throw new NotImplementedException();
        }
    }
}