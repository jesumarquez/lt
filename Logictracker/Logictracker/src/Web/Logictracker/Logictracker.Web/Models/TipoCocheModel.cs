using Logictracker.Types.BusinessObjects.Vehiculos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logictracker.Web.Models
{
    public class TipoCocheModel
    {
    }

    public class TipoCocheMapper : EntityModelMapper<TipoCoche, TipoCocheModel>
    {
        public override TipoCocheModel EntityToModel(TipoCoche entity, TipoCocheModel model)
        {
            throw new NotImplementedException();
        }

        public override TipoCoche ModelToEntity(TipoCocheModel model, TipoCoche entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(TipoCoche entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }
}