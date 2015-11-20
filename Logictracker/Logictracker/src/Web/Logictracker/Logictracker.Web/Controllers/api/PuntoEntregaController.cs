using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Web.Http.ModelBinding;

namespace Logictracker.Web.Controllers.api
{
    public class PuntoEntregaController : EntityController<PuntoEntrega, PuntoEntregaDAO, PuntoEntregaModel, PuntoEntregaMapper>
    {
        [Route("api/PuntoEntrega/items")]
        public IEnumerable<ItemModel> GetComboItem()
        {
            return EntityDao.GetList(new int[] { }, new int[] { }, new int[] { }).Select(e => Mapper.ToItem(e));
        }
        
        [Route("api/distrito/{distritoId}/base/{baseId}/PuntoEntrega/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        {
            return EntityDao.GetList(new int[] { distritoId }, new int[] { baseId }, new int[] { }).Select(e => Mapper.ToItem(e));
        }

        [Route("api/distrito/{distritoId}/base/{baseId}/cliente/{clienteId}/PuntoEntregas/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId, int clienteId)
        {
            return EntityDao.GetList(new int[] { distritoId }, new int[] { baseId }, new int[] { clienteId }).Select(e => Mapper.ToItem(e));
        }

        [Route("api/distrito/{distritoId}/base/{baseId}/cliente/{clienteId}/PuntoEntrega/{puntoEntrega}/items")]
        public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId, int clienteId, string puntoEntrega)
        {
            return EntityDao.FindByCodes(new[] { distritoId }, new[] { baseId }, new[] { clienteId }, new string[] { puntoEntrega }).Select(e => Mapper.ToItem(e));
        }

        [HttpGet]
        [Route("api/distrito/{distritoId}/base/{baseId}/cliente/{clienteId}/PuntoEntrega/items")]
        public DataSourceResult GetDataSource([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest filter, int distritoId, int baseId, int clienteId)
        {
            var filterValue = GetFilterValue(filter.Filters, "Codigo");
            return filterValue != null ? EntityDao.FindByCodeLike(new[] { distritoId }, new[] { baseId }, new[] { clienteId }, filterValue.ToString()).ToList().ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new PuntoEntregaModel())) : EntityDao.GetList(new[] { distritoId }, new[] { baseId }, new[] { clienteId }).ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new PuntoEntregaModel()));
        }
    }
}