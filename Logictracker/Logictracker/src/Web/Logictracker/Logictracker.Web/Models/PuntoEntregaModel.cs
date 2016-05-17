using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class PuntoEntregaModel
    {
        public string Codigo { get; set; }
        public int PuntoEntregaId { get; set; }
        public string Descripcion { get; set; }
        public int ResponsableId { get; set; }
        public string ClienteCodigo { get; set; }
        public int ClienteId { get; set; }
        public string ClienteDesc { get; set; }
    }

    public class PuntoEntregaMapper : EntityModelMapper<PuntoEntrega, PuntoEntregaModel>
    {
        public override PuntoEntregaModel EntityToModel(PuntoEntrega entity, PuntoEntregaModel model)
        {
            model.Codigo = entity.Codigo;
            model.PuntoEntregaId = entity.Id;
            model.Descripcion = entity.Descripcion;
            if (entity.Responsable != null) model.ResponsableId = entity.Responsable.Id;
            if (entity.Cliente != null)
            {
                model.ClienteCodigo = entity.Cliente.Codigo;
                model.ClienteId = entity.Cliente.Id;
                model.ClienteDesc = entity.Cliente.Descripcion;
            }
            return model;
        }

        public override PuntoEntrega ModelToEntity(PuntoEntregaModel model, PuntoEntrega entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(PuntoEntrega entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }
}