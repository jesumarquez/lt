using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class PuntoEntregaModel
    {
        public string Codigo { get; set; }
        public int PuntoEntregaId { get; set; }
        public string Descripcion { get; set; }
    }

    public class PuntoEntregaMapper : EntityModelMapper<PuntoEntrega, PuntoEntregaModel>
    {
        public override PuntoEntregaModel EntityToModel(PuntoEntrega entity, PuntoEntregaModel model)
        {
            model.Codigo = entity.Codigo;
            model.PuntoEntregaId = entity.Id;
            model.Descripcion = entity.Descripcion;
            return model;
        }

        public override PuntoEntrega ModelToEntity(PuntoEntregaModel model, PuntoEntrega entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(PuntoEntrega entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion};
        }
    }
}