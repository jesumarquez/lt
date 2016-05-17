using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logictracker.Web.Models
{
    public class TipoCicloLogisticoModel
    {
    }

    public class TipoCicloLogisticoMapper : EntityModelMapper<TipoCicloLogistico,TipoCicloLogisticoModel>
    {
        public override TipoCicloLogisticoModel EntityToModel(TipoCicloLogistico entity, TipoCicloLogisticoModel model)
        {
            throw new NotImplementedException();
        }

        public override TipoCicloLogistico ModelToEntity(TipoCicloLogisticoModel model, TipoCicloLogistico entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(TipoCicloLogistico entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }
}