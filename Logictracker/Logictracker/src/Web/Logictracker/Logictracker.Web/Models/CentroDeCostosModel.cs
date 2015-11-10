using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class CentroDeCostosModel
    {
    }

    public class CentroDeCostosMapper : EntityModelMapper<CentroDeCostos, CentroDeCostosModel>
    {
        public override CentroDeCostosModel EntityToModel(CentroDeCostos entity, CentroDeCostosModel model)
        {
            throw new NotImplementedException();
        }

        public override CentroDeCostos ModelToEntity(CentroDeCostosModel model, CentroDeCostos entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(CentroDeCostos entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }
}