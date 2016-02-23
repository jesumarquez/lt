using Logictracker.Types.BusinessObjects.Ordenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Web.Models
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Insumo { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }
    }

    public class OrdenDetallesMapper : EntityModelMapper<OrderDetail, OrderDetailModel>
    {
        public override OrderDetailModel EntityToModel(OrderDetail entity, OrderDetailModel model)
        {
            model.Id = entity.Id;
            model.OrderId = entity.Order.Id;
            if (entity.Insumo != null) model.Insumo = entity.Insumo.Descripcion;
            model.PrecioUnitario = entity.PrecioUnitario;
            model.Cantidad = entity.Cantidad;
            model.Descuento = entity.Descuento;

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
}
