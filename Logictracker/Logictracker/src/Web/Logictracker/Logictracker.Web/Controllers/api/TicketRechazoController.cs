using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Logictracker.DAL.DAO.BusinessObjects.Rechazos;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Web.Models;
using System;

namespace Logictracker.Web.Controllers.api
{
    public class TicketRechazoController : EntityController<TicketRechazo, TicketRechazoDAO, TicketRechazoModel, TicketRechazoMapper>
    {
        [Route("api/ticketrechazo/estado/items")]
        public IEnumerable<ItemModel> GetEstado()
        {
            return EntityDao.GetEstados().Select(e => new ItemModel { Key = (int)e.Key, Value = e.Value });
        }
        [Route("api/ticketrechazo/motivo/items")]
        public IEnumerable<ItemModel> GetMotivo()
        {
            return EntityDao.GetMotivos().Select(e => new ItemModel { Key = (int)e.Key, Value = e.Value });
        }

        [Route("api/ticketrechazo/datasource")]
        public DataSourceResult GetDataSource(
                [ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            return EntityDao.FindAll().ToDataSourceResult(request, e => Mapper.EntityToModel(e, new TicketRechazoModel()));
        }

        //[Route("api/ticketrechazo/cantidadesporestado/items")]
        //public IEnumerable<ItemModel> GetCantidadesPorEstado(int idEmpresa, int idLinea, DateTime desde, DateTime hasta)
        //{
        //    return EntityDao.GetCantidadesPorEstado(idEmpresa, idLinea, desde, hasta).Select(e => Mapper.ToItem(e));
        //}
    }

}
