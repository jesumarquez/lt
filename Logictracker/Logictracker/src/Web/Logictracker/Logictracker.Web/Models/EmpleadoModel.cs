using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class EmpleadoModel
    {
        public int EmpleadoId { get; set; }
        public string Nombre { get; set; }
        public string Legajo { get; set; }
    }

    public class EmpleadoMapper : EntityModelMapper<Empleado, EmpleadoModel>
    {
        public override EmpleadoModel EntityToModel(Empleado entity, EmpleadoModel model)
        {
            model.EmpleadoId = entity.Id;
            model.Nombre = entity.Entidad.Nombre;
            model.Legajo = entity.Legajo;
            return model;
        }

        public override Empleado ModelToEntity(EmpleadoModel model, Empleado entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Empleado entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Entidad.Nombre};
        }
    }
}