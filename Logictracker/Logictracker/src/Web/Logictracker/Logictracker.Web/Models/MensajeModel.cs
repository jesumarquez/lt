using Logictracker.Types.BusinessObjects.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logictracker.Web.Models
{
    public class MensajeModel
    {
    }
    public class MensajeMapper : EntityModelMapper<Mensaje, MensajeModel>
    {
        public override MensajeModel EntityToModel(Mensaje entity, MensajeModel model)
        {
            throw new NotImplementedException();
        }

        public override Mensaje ModelToEntity(MensajeModel model, Mensaje entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Mensaje entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }

}