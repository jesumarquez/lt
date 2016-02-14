using Logictracker.Types.BusinessObjects.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logictracker.Web.Models
{
    public class InsumoModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
    }

    public class InsumoMapper : EntityModelMapper<Insumo, InsumoModel>
    {
        public override InsumoModel EntityToModel(Insumo entity, InsumoModel model)
        {
            model.Id = entity.Id;
            model.Descripcion = entity.Descripcion;
            model.Codigo = entity.Codigo;
            return model;
        }

        public override Insumo ModelToEntity(InsumoModel model, Insumo entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Insumo entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }

}