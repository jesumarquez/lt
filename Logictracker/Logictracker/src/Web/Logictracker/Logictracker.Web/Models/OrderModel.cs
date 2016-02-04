using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Ordenes;

namespace Logictracker.Web.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public int IdEmpresa { get; set; }
        public int BaseId { get; set; }
        public string Empleado { get; set; }
        public int IdEmpleado { get; set; }
        public string Transportista { get; set; }
        public int IdTransportista { get; set; }
        public string PuntoEntrega { get; set; }
        public int IdPuntoEntrega { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string InicioVentana { get; set; }
        public string FinVentana { get; set; }
    }

    public class OrdenesMapper : EntityModelMapper<Order, OrderModel>
    {
        public override OrderModel EntityToModel(Order entity, OrderModel model)
        {
            model.Id = entity.Id;
            model.CodigoPedido = entity.CodigoPedido;
            if (entity.Empleado != null) model.Empleado = entity.Empleado.Entidad.Descripcion;
            if (entity.Empleado != null) model.IdEmpleado = entity.Empleado.Entidad.Id;
            if (entity.Empresa != null) model.Empresa = entity.Empresa.RazonSocial;
            if (entity.Empresa != null) model.IdEmpresa = entity.Empresa.Id;

            model.BaseId = entity.Linea.Id;
            model.FechaAlta = entity.FechaAlta;
            if (entity.FechaEntrega != null)
                model.FechaEntrega = entity.FechaEntrega.Value;
            else
                model.FechaEntrega = null;
            model.FechaPedido = entity.FechaPedido;
            model.FinVentana = entity.FinVentana;
            model.InicioVentana = entity.InicioVentana;
            model.Id = entity.Id;
            if (entity.PuntoEntrega != null) model.PuntoEntrega = entity.PuntoEntrega.Descripcion;
            if (entity.PuntoEntrega != null) model.IdPuntoEntrega = entity.PuntoEntrega.Id;
            if (entity.Transportista != null) model.Transportista = entity.Transportista.Descripcion;
            if (entity.Transportista != null) model.IdTransportista = entity.Transportista.Id;
            
            return model;
        }

        public override Order ModelToEntity(OrderModel model, Order entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Order entity)
        {
            throw new NotImplementedException();
        }
    }
}
