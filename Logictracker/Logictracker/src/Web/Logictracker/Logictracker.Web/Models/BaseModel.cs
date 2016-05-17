using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.DAL.Factories;

namespace Logictracker.Web.Models
{
    public class BaseModel
    {
        public int BaseId { get; set; }
        public string Descripcion { get; set; }
        public int EmpresaId { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public int Key { get; set; }
        public string Value { get; set; }
    }

    public class LineaMapper : EntityModelMapper<Linea, BaseModel>
    {
        static DAOFactory daoF = new DAOFactory();

        public override BaseModel EntityToModel(Linea entity, BaseModel model)
        {
            model.Key = entity.Id;
            model.EmpresaId = entity.Empresa.Id;
            model.Value = entity.Descripcion;
            model.Latitud = entity.ReferenciaGeografica != null ? daoF.ReferenciaGeograficaDAO.FindById(entity.ReferenciaGeografica.Id).Latitude : 0;
            model.Longitud = entity.ReferenciaGeografica != null ? daoF.ReferenciaGeograficaDAO.FindById(entity.ReferenciaGeografica.Id).Longitude : 0;

            return model;
        }

        public override Linea ModelToEntity(BaseModel model, Linea entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Linea entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Descripcion };
        }
    }
}