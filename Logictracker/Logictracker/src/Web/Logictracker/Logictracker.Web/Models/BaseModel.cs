using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class BaseModel
    {
        public int BaseId { get; set; }
        public int EmpresaId  { get; set; }
    }

    public class LineaMapper : EntityModelMapper<Linea, BaseModel>
    {
        public override BaseModel EntityToModel(Linea entity, BaseModel model)
        {
            throw new NotImplementedException();
        }

        public override Linea ModelToEntity(BaseModel model, Linea entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Linea entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion};
        }
    }
}