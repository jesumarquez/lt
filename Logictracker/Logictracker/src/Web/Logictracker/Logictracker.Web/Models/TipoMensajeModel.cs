using Logictracker.Types.BusinessObjects.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logictracker.Web.Models
{
    public class TipoMensajeModel
    {
    }
    public class TipoMensajeMapper : EntityModelMapper<TipoMensaje, TipoMensajeModel>
    {
        public override TipoMensajeModel EntityToModel(TipoMensaje entity, TipoMensajeModel model)
        {
            throw new NotImplementedException();
        }

        public override TipoMensaje ModelToEntity(TipoMensajeModel model, TipoMensaje entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(TipoMensaje entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }
}