using System;
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
        public string CodigoPuntoEntrega { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string InicioVentana { get; set; }
        public string FinVentana { get; set; }
        public double PuntoEntregaLatitud { get; set; }
        public double PuntoEntregaLongitud { get; set; }
    }

    public class OrdenesDetailMapper : EntityModelMapper<OrderDetail, OrderDetailModel>
    {
        public override OrderDetailModel EntityToModel(OrderDetail entity, OrderDetailModel model)
        {
            model.Id = entity.Id;
            model.OrderId = entity.Order.Id;
            model.PrecioUnitario = entity.PrecioUnitario;
            model.Cantidad = entity.Cantidad;
            model.Descuento = entity.Descuento;
            model.Ajuste = entity.Ajuste;
            model.ChocheId = 0;
            model.Cuaderna = entity.Cuaderna;
            if (entity.Insumo != null) model.Insumo = entity.Insumo.Descripcion;
            model.EstadoDescripcion = entity.Estado.ToString();
            return model;
        }

        public override OrderDetail ModelToEntity(OrderDetailModel model, OrderDetail entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(OrderDetail entity)
        {
            throw new NotImplementedException();
        }
    }

    public class OrdenesMapper : EntityModelMapper<Order, OrderModel>
    {
        public override OrderModel EntityToModel(Order entity, OrderModel model)
        {
            model.CodigoPedido = entity.CodigoPedido;
            model.FechaPedido = entity.FechaPedido;
            model.FinVentana = entity.FinVentana;
            model.InicioVentana = entity.InicioVentana;
            model.Id = entity.Id;
            model.BaseId = entity.Linea.Id;
            model.FechaAlta = entity.FechaAlta;
            model.FechaEntrega = entity.FechaEntrega;
            
            if (entity.Empleado != null)
            {
                model.Empleado = entity.Empleado.Entidad.Descripcion;
                model.IdEmpleado = entity.Empleado.Entidad.Id;
            }

            if (entity.Empresa != null)
            {
                model.Empresa = entity.Empresa.RazonSocial;
                model.IdEmpresa = entity.Empresa.Id;
            }

            if (entity.Transportista != null)
            {
                model.Transportista = entity.Transportista.Descripcion;
                model.IdTransportista = entity.Transportista.Id;
            }

            if (entity.PuntoEntrega != null)
            {
                model.PuntoEntrega = entity.PuntoEntrega.Descripcion;
                model.IdPuntoEntrega = entity.PuntoEntrega.Id;
                model.PuntoEntregaLatitud = entity.PuntoEntrega.ReferenciaGeografica.Latitude;
                model.PuntoEntregaLongitud = entity.PuntoEntrega.ReferenciaGeografica.Longitude;
            }

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
