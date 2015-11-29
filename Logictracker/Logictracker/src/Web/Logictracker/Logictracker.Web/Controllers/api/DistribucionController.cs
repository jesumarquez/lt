using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class DistribucionController : EntityController<ViajeDistribucion, ViajeDistribucionDAO, ViajeDistribucionModel, ViajeDistribucionMapper>
    {
        [Route("api/distrito/{distritoId}/base/{baseId}/distribucion/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            return EntityDao.GetList(new[] { distritoId }, new[] { baseId }, new[] { -1 }, new[] { -1 }).Select(c => Mapper.ToItem(c)).ToList();
        }

        [Route("api/distrito/{distritoId}/base/{baseId}/distribucion/models")]
        public DataSourceResult GetDataSource(
            [ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest filter, int distritoId,
            int baseId)
        {
            var filterValue = GetFilterValue(filter.Filters, "Codigo");
            return EntityDao.FindByCodeLike(distritoId, baseId, filterValue.ToString()).ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new ViajeDistribucionModel()));
        }
        

    }

    public class ViajeDistribucionMapper : EntityModelMapper<ViajeDistribucion, ViajeDistribucionModel>
    {
        public override ViajeDistribucionModel EntityToModel(ViajeDistribucion entity, ViajeDistribucionModel model)
        {
            model.Id = entity.Id;
            model.Codigo = entity.Codigo;
            model.DistritoId = entity.Empresa.Id;
            model.BaseId = entity.Linea.Id;
            model.Inicio = entity.Inicio;

            if (entity.Vehiculo != null)
            {
                model.Patente = entity.Vehiculo.Patente;
                model.VehiculoId = entity.Vehiculo.Id;
            }
            return model;
        }

        public override ViajeDistribucion ModelToEntity(ViajeDistribucionModel model, ViajeDistribucion entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(ViajeDistribucion entity)
        {
            return new ItemModel { Key = entity.Id, Value = entity.Codigo };
        }
    }

    public class ViajeDistribucionModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int DistritoId { get; set; }
        public int BaseId { get; set; }
        public string Patente { get; set; }
        public int VehiculoId { get; set; }
        public DateTime Inicio { get; set; }
    }
}