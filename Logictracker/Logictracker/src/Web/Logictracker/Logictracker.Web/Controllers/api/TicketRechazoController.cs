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
using NHibernate.Criterion;

namespace Logictracker.Web.Controllers.api
{
    public class TicketRechazoController : EntityController<TicketRechazo, TicketRechazoDAO, TicketRechazoModel, TicketRechazoMapper>
    {

        [Route("api/ticketrechazo/{ticketId}/estado/next")]
        public IEnumerable<ItemModel> GetNextEstado(int ticketId)
        {
            var ticket = EntityDao.FindById(ticketId);
            var estados = TicketRechazo.Next(ticket.UltimoEstado);

            return estados.Select(e => new ItemModel { Key = (int)e, Value = Culture.CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(e)) });
        }

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
            var tickets = EntityDao.FindAll();
            
            if (Usuario.Empleado == null|| Usuario.Empleado.TipoEmpleado == null)
                return tickets.ToDataSourceResult(request, e => Mapper.EntityToModel(e, new TicketRechazoModel()));
            
            switch (Usuario.Empleado.TipoEmpleado.Codigo)
            {
                case "SR":
                    tickets = tickets.Where(t => t.SupervisorRuta.Id == Usuario.Empleado.Id);
                    break;
                case "JF":
                    tickets = tickets.Where(t => t.SupervisorVenta.Id == Usuario.Empleado.Id);
                    break;
                case "V":
                    tickets = tickets.Where(t => t.Vendedor.Id == Usuario.Empleado.Id);
                    break;
            }
            return tickets.ToDataSourceResult(request, e => Mapper.EntityToModel(e, new TicketRechazoModel()));
        }

        [Route("api/ticketrechazo/item/{id}")]
        public IHttpActionResult GetItem(int id)
        {
            var rechazo = EntityDao.FindById(id);
            var rechazoModel = new TicketRechazoModel();
            Mapper.EntityToModel(rechazo, rechazoModel);

            return Json(rechazoModel);
        }
        [Route("api/ticketrechazo/item")]
        public IHttpActionResult PostItem(TicketRechazoModel rechazoModel)
        {
            var rechazoEntity = new TicketRechazo(rechazoModel.Observacion, Usuario.Empleado, DateTime.UtcNow);

            Mapper.ModelToEntity(rechazoModel, rechazoEntity);

            EntityDao.Save(rechazoEntity);

            Mapper.EntityToModel(rechazoEntity, rechazoModel);

            return Created(string.Concat("api/ticketrechazo/item/{0}", rechazoEntity.Id), rechazoModel);
        }
        [Route("api/ticketrechazo/item/{id}")]
        public IHttpActionResult PutItem(int id, TicketRechazoModel rechazoModel)
        {
            var ticketEntity = EntityDao.FindById(id);

            ticketEntity.ChangeEstado((TicketRechazo.Estado)Enum.Parse(typeof(TicketRechazo.Estado), rechazoModel.Estado), rechazoModel.Observacion, Usuario.Empleado);
            EntityDao.SaveOrUpdate(ticketEntity);

            return Ok();
        }
        //[Route("api/ticketrechazo/cantidadesporestado/items")]
        //public IEnumerable<ItemModel> GetCantidadesPorEstado(int idEmpresa, int idLinea, DateTime desde, DateTime hasta)
        //{
        //    return EntityDao.GetCantidadesPorEstado(idEmpresa, idLinea, desde, hasta).Select(e => Mapper.ToItem(e));
        //}

        [Route("api/ticketrechazo/distrito/{distritoId}/base/{baseId}/estadisticas/rol")]
        public IHttpActionResult GetPromedioPorRol(int distritoId, int baseId)
        {
            var promedios = new
            {
                vendedor = 62,
                supervisorVentas = 25,
                jefeVentas = 20
            };

            return Json(promedios);
        }

        [Route("api/ticketrechazo/distrito/{distritoId}/base/{baseId}/estadisticas/estado")]
        public IHttpActionResult GetCantidadPorEstado(int distritoId, int baseId)
        {
            var list = new List<CantidadPorEstadoModel>()
            {
                new CantidadPorEstadoModel { Estado = "Pendiente", Cantidad = 6 },
                new CantidadPorEstadoModel { Estado = "Avisado", Cantidad = 11 },
                new CantidadPorEstadoModel { Estado = "Notificado", Cantidad = 20 }
            };

            return Json(list.ToArray());
        }

        [Route("api/ticketrechazo/estadisticas/promedio/porvendedor")]
        public DataSourceResult GetPromedioPorVendedor([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            var list = new List<PromedioPorVendedorModel>()
            {
                new PromedioPorVendedorModel
                {
                    Usuario = "Jose Gutierrez",
                    EstadoIngreso = "Pendiente",
                    EstadoEgreso = "Avisado",
                    Promedio = (float)5.2
                }
            };
            DataSourceResult r = new DataSourceResult();
            r.Data = list.ToArray();
            return r;
        }

        [Route("api/ticketrechazo/estadisticas/promedio/porestado")]
        public DataSourceResult GetPromedioPorEstado([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            var list = new List<PromedioPorEstadoModel>()
            {
                new PromedioPorEstadoModel
                {
                    Estado = "Pendiente",
                    Promedio = (float)4.2
                },
                new PromedioPorEstadoModel
                {
                    Estado = "Avisado",
                    Promedio = (float)10
                },
                new PromedioPorEstadoModel
                {
                    Estado = "Notificado",
                    Promedio = (float)10
                }

            };
            DataSourceResult r = new DataSourceResult();
            r.Data = list.ToArray();
            return r;
        }
    }
}
