using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class EmpleadoModel
    {
        public int EmpleadoId { get; set; }
        public string Descripcion { get; set; }
        public string Legajo { get; set; }
        public int? BaseId { get; set; }
    }

    public class EmpleadoMapper : EntityModelMapper<Empleado, EmpleadoModel>
    {
        public override EmpleadoModel EntityToModel(Empleado entity, EmpleadoModel model)
        {
            model.EmpleadoId = entity.Id;
            model.Descripcion = entity.Entidad.Descripcion;
            model.Legajo = entity.Legajo;
            model.BaseId = entity.Linea != null? entity.Linea.Id:(int?) null;
            return model;
        }

        public override Empleado ModelToEntity(EmpleadoModel model, Empleado entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Empleado entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Entidad.Descripcion};
        }
    }
}