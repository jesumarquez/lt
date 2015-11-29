using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class PuntoEntregaController : EntityController<PuntoEntrega, PuntoEntregaDAO, PuntoEntregaModel, PuntoEntregaMapper>
    {
        [Route("api/PuntoEntrega/items")]
        public IEnumerable<ItemModel> GetComboItem()
        {
            return EntityDao.GetList(new int[] { }, new int[] { }, new int[] { }).Select(e => Mapper.ToItem(e));
        }
        
        //[Route("api/distrito/{distritoId}/base/{baseId}/PuntoEntrega/items")]
        //public IEnumerable<ItemModel> GetComboItem(int distritoId, int baseId)
        //{
        //    return EntityDao.GetList(new int[] { distritoId }, new int[] { baseId }, new int[] { }).Select(e => Mapper.ToItem(e));
        //}

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

        //[Route("api/distrito/{distritoId}/base/{baseId}/viajeDistribucion/{viajeDistribucionId}/PuntoEntrega/items")]
        //public IEnumerable<ItemModel> GetComboItem2(int distritoId, int baseId, int viajeDistribucionId)
        //{
        //    return DAOFactory.GetDao<ViajeDistribucionDAO>().GetById(new[] { distritoId }, new[] { baseId }, viajeDistribucionId).Detalles.Select(d => Mapper.ToItem(d.PuntoEntrega));
        //}

        //[HttpGet]
        //[Route("api/distrito/{distritoId}/base/{baseId}/cliente/{clienteId}/PuntoEntrega/items")]
        //public DataSourceResult GetDataSource([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest filter, int distritoId, int baseId, int clienteId)
        //{
        //    var filterValue = GetFilterValue(filter.Filters, "Codigo");
        //    return filterValue != null ? EntityDao.FindByCodeLike(new[] { distritoId }, new[] { baseId }, new[] { clienteId }, filterValue.ToString()).ToList().ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new PuntoEntregaModel())) : EntityDao.GetList(new[] { distritoId }, new[] { baseId }, new[] { clienteId }).ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new PuntoEntregaModel()));
        //}

        [HttpGet]
        [Route("api/distrito/{distritoId}/base/{baseId}/PuntoEntrega/items")]
        public DataSourceResult GetDataSource([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest filter, int distritoId, int baseId)
        {
            var filterValue = GetFilterValue(filter.Filters, "Codigo");
            return filterValue != null ? EntityDao.FindByCodeLike(new[] { distritoId }, new[] { baseId }, new int[] { }, filterValue.ToString()).ToList().ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new PuntoEntregaModel())) : EntityDao.GetList(new[] { distritoId }, new[] { baseId }, new int[] { }).ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new PuntoEntregaModel()));
        }

        [HttpGet]
        [Route("api/distrito/{distritoId}/base/{baseId}/viajeDistribucion/{viajeDistribucionId}/PuntoEntrega/items")]
        public DataSourceResult GetDataSourceByDistribucion([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest filter, int distritoId, int baseId, int viajeDistribucionId)
        {
            var entregas = new List<PuntoEntrega>();
            DAOFactory.GetDao<ViajeDistribucionDAO>().FindById(viajeDistribucionId).Detalles.Where(d => d.PuntoEntrega != null).ToList().ForEach(d => { entregas.Add(d.PuntoEntrega); });
            return entregas.ToDataSourceResult(filter, e => Mapper.EntityToModel(e, new PuntoEntregaModel()));
       }
    }
}