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
        public int Ajuste { get; set; }
        public int ChocheId { get; set; }
        public int Cuaderna { get; set; }
        public string Descripcion { get; set; }
        public string EstadoDescripcion { get; set; }
        public string ClienteDescripcion { get; set; }
        public string ClienteLocalidad { get; set; }
        public double PuntoEntregaLatitud { get; set; }
        public double PuntoEntregaLongitud { get; set; }
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
            model.Cuaderna = entity.Cuaderna;
            model.Ajuste = entity.Ajuste;
            model.EstadoDescripcion = entity.Estado.ToString();
            model.Descripcion = string.Format("{0} | {1}", model.OrderId.ToString(), model.Insumo);
            
            if (entity.Order.PuntoEntrega != null)
            {
                model.ClienteDescripcion = entity.Order.PuntoEntrega.Descripcion;
                if (entity.Order.PuntoEntrega.ReferenciaGeografica != null)
                {
                    model.ClienteLocalidad = entity.Order.PuntoEntrega.ReferenciaGeografica.Direccion.Partido;
                    model.PuntoEntregaLatitud = entity.Order.PuntoEntrega.ReferenciaGeografica.Latitude;
                    model.PuntoEntregaLongitud = entity.Order.PuntoEntrega.ReferenciaGeografica.Longitude;
                }
            }
            
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
