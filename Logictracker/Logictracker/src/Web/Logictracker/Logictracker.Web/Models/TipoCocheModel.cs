using Logictracker.Types.BusinessObjects.Vehiculos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

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

            model.Contenedores = new List<ContenedorModel>(entity.Contenedores.Count);

            for (var i = 0; i < entity.Contenedores.Count; i++)
            {
                var toAdd = new ContenedorModel
                {
                    Orden = i+1,
                    Capacidad = entity.Contenedores[i].Capacidad,
                    Descripcion = entity.Contenedores[i].Descripcion
                };
                model.Contenedores.Add(toAdd);
            }
            // si no tiene contenedores, es como si tuviera uno solo de volumen uníco
            if (!model.Contenedores.Any())
            {
                model.Contenedores.Add( new ContenedorModel()
                    { Orden=1,Capacidad = entity.CapacidadCarga, Descripcion = "Espacio de carga"});
            }

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