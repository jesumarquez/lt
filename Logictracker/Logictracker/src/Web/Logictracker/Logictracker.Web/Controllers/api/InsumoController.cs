using Kendo.Mvc.UI;
using Logictracker.DAL.DAO.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Kendo.Mvc.Extensions;

namespace Logictracker.Web.Controllers.api
{
    public class InsumoController : EntityController<Insumo, InsumoDAO, InsumoModel, InsumoMapper>
    {
        [Route("api/insumos/{distritoId}/base/{baseId}/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            return EntityDao.GetList(new[] { distritoId }, new[] { baseId }, new int[] {}).Select(d => Mapper.ToItem(d)).ToList();
        }

        [HttpGet]
        [Route("api/insumos/datasource")]
        public DataSourceResult GetDataSource([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            var insumos = EntityDao.FindAll();
            return insumos.ToDataSourceResult(request, i => Mapper.EntityToModel(i, new InsumoModel()));
        }
    }
}