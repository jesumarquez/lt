using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.Models;
using System;

namespace Logictracker.Web.Controllers.api.Models
{
    public class CocheModel
    {
    }
    public class CocheMapper : EntityModelMapper<Coche, CocheModel>
    {
        public override CocheModel EntityToModel(Coche entity, CocheModel model)
        {
            throw new NotImplementedException();
        }

        public override Coche ModelToEntity(CocheModel model, Coche entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Coche entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Patente };
        }
    }
}