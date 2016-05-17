using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class DepartamentoModel
    {
    }

    public class DepartamentoMapper : EntityModelMapper<Departamento,DepartamentoModel>
    {
        public override DepartamentoModel EntityToModel(Departamento entity, DepartamentoModel model)
        {
            throw new NotImplementedException();
        }

        public override Departamento ModelToEntity(DepartamentoModel model, Departamento entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Departamento entity)
        {
            return new ItemModel {Key = entity.Id, Value = entity.Descripcion};
        }
    }
}