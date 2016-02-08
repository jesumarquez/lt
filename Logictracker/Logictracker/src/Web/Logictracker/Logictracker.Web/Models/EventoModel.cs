using Logictracker.Types.ReportObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logictracker.Web.Models
{
    public class EventoModel
    {
        public string Vehiculo { get; set; }
        public string Chofer { get; set; }
        public string Responsable { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Recepcion { get; set; }
        public TimeSpan Duracion { get; set; }
        public string Mensaje{ get; set; }

    }
    public class EventoMapper : EntityModelMapper<MobileEvent, EventoModel>
    {
        public override EventoModel EntityToModel(MobileEvent entity, EventoModel model)
        {
            model.Chofer = entity.Driver;
            model.Duracion = entity.Duration;
            model.Fecha = entity.Fecha;
            model.Mensaje = entity.Message;
            model.Recepcion = entity.Reception;
            model.Responsable = entity.Responsable;
            model.Vehiculo = entity.Intern;

            return model;
        }

        public override MobileEvent ModelToEntity(EventoModel model, MobileEvent entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(MobileEvent entity)
        {
            throw new NotImplementedException();
        }
    }
}