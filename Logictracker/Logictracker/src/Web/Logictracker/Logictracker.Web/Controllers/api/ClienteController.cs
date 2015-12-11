using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class ClienteController : EntityController<Cliente, ClienteDAO, ClienteModel, ClienteMapper>
    {
        [HttpGet]
        [Route("api/distrito/{distritoId}/base/{baseId}/cliente/models")]
        public DataSourceResult GetDataSource([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest filter, int distritoId, int baseId)
        {
            var filterValue = GetFilterValue(filter.Filters,"Codigo");
            return filterValue != null ? EntityDao.FindByCodeLike(new[] { distritoId }, new[] { baseId }, filterValue.ToString()).ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new ClienteModel())) : EntityDao.GetList(new[] { distritoId }, new[] { baseId }).ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new ClienteModel()));
           
        }
        
    }

    public class ClienteModel
    {
        public string Codigo { get; set; }
        public string DireccionNomenclada { get; set; }
        public int ClienteId { get; set; }
        public string DescripcionCorta { get; set; }
    }

    public class ClienteMapper : EntityModelMapper<Cliente, ClienteModel>
    {
        public override ClienteModel EntityToModel(Cliente entity, ClienteModel model)
        {
            model.ClienteId = entity.Id;
            model.Codigo = entity.Codigo;
            model.DescripcionCorta = entity.DescripcionCorta;
            model.DireccionNomenclada = entity.DireccionNomenclada??string.Empty;
            return model;
        }

        public override Cliente ModelToEntity(ClienteModel model, Cliente entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(Cliente entity)
        {
            throw new NotImplementedException();
        }
    }


}