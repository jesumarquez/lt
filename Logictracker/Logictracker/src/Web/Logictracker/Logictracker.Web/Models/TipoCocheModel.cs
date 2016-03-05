using Logictracker.Types.BusinessObjects.Vehiculos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logictracker.Web.Models
{
    public class TipoCocheModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public IList<ContenedorModel> Contenedores { get; set; }
    }

    public class TipoCocheMapper : EntityModelMapper<TipoCoche, TipoCocheModel>
    {
        public override TipoCocheModel EntityToModel(TipoCoche entity, TipoCocheModel model)
        {
            model.Id = entity.Id;
            model.Descripcion = entity.Descripcion;
            // if (entity.Contenedores.Count > 0) model.Contenedores = new List<ContenedorModel>(entity.Contenedores.Select(c => new ContenedorModel() { Descripcion = c.Descripcion, Capacidad = c.Capacidad }));

            //Duro
            model.Contenedores = new List<ContenedorModel>();
            model.Contenedores.Add(new ContenedorModel() { Orden = 1, Capacidad = 6000, Descripcion = "Cuaderna 1" });
            model.Contenedores.Add(new ContenedorModel() { Orden = 2, Capacidad = 6000, Descripcion = "Cuaderna 2" });
            model.Contenedores.Add(new ContenedorModel() { Orden = 3, Capacidad = 6000, Descripcion = "Cuaderna 3" });
            model.Contenedores.Add(new ContenedorModel() { Orden = 4, Capacidad = 6000, Descripcion = "Cuaderna 4" });

            return model;
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

    public class ContenedorModel
    {
        public virtual int Orden { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual double Capacidad { get; set; }
    }
}