using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.DAL.DAO.BusinessObjects.Rechazos;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Web.Models;
using WebGrease.Css.Extensions;

namespace Logictracker.Web.Controllers.api
{
    public class TicketRechazoController : EntityController<TicketRechazo, TicketRechazoDAO, TicketRechazoModel, TicketRechazoMapper>
    {
        [Route("api/ticketrechazo/{ticketId}/estado/next")]
        public IEnumerable<ItemModel> GetNextEstado(int ticketId)
        {
            var ticket = EntityDao.FindById(ticketId);
            var estados = TicketRechazo.Next(ticket.UltimoEstado);

            return estados.Select(e => new ItemModel { Key = (int)e, Value = CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(e)) });
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
            var transaction = SessionHelper.Current.BeginTransaction();
            try
            {
                var tickets = EntityDao.FindAll();

                if (Usuario.Empleado == null || Usuario.Empleado.TipoEmpleado == null)
                    return GetResult(tickets, request);

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
                return GetResult(tickets, request);

            }
            finally
            {
                transaction.Rollback();
            }
        }

        private DataSourceResult GetResult(IQueryable<TicketRechazo> data, DataSourceRequest r)
        {
            if (r.Groups == null)
                return data.ToDataSourceResult(r, e => Mapper.EntityToModel(e, new TicketRechazoModel()));

            // Workaround por como resuelve linq y nhibernate con los filter y haciendo el group del lado del server
            // Guardo valores originales
            var grouping = r.Groups;
            var page = r.Page;
            var pageSize = r.PageSize;

            // Limpio Group y Paging
            r.Page = 0;
            r.PageSize = 0;
            r.Groups = null;

            var result = data.ToDataSourceResult(r);

            // Reanudo los valores originales
            r.Groups = grouping;
            r.PageSize = pageSize;
            r.Page = page;

            //// Se acomodan los nombres de los members
            //r.Groups.ForEach(gd => ReplaceGroupDescriptorMember(gd));

            return result.Data.Cast<TicketRechazo>().ToDataSourceResult(r, e => Mapper.EntityToModel(e, new TicketRechazoModel()));
        }
    

    //private void ReplaceGroupDescriptorMember(GroupDescriptor gd)
    //{
    //    switch (gd.Member)
    //    {
    //        case "Estado":
    //            gd.Member = "UltimoEstado";
    //             break;
    //        case "MotivoDesc":
    //            gd.Member = "Motivo";
    //            break;
    //        case "EntregaCodigo":
    //            gd.Member = "Entrega.Id";
    //            break;
    //        case "VendedorDesc":
    //            gd.Member = "Vendedor.Id";
    //            break;
    //        case "SupVenDesc":
    //            gd.Member = "SupervisorVenta.Id";
    //            break;
    //        case "SupRutDesc":
    //            gd.Member = "SupervisorRuta.Id";
    //            break;
    //    }
    //}

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
            var transacion = SessionHelper.Current.BeginTransaction();

            try
            {
                var rechazoEntity = new TicketRechazo(rechazoModel.Observacion, Usuario.Empleado, DateTime.UtcNow);

                Mapper.ModelToEntity(rechazoModel, rechazoEntity);

                EntityDao.Save(rechazoEntity);

                Mapper.EntityToModel(rechazoEntity, rechazoModel);

                var empleado = rechazoEntity.Entrega.Responsable;
                if (empleado != null)
                {
                    var cocheDao = DAOFactory.GetDao<CocheDAO>();
                    var mensajeDao = DAOFactory.GetDao<MensajeDAO>();
                    var logMensajeDao = DAOFactory.GetDao<LogMensajeDAO>();

                    var coche = cocheDao.FindByChofer(empleado.Id);
                    if (coche != null)
                    {
                        var mensajeVO = mensajeDao.GetByCodigo(TicketRechazo.GetCodigoMotivo(rechazoEntity.Motivo),
                            coche.Empresa, coche.Linea);
                        var mensaje = mensajeDao.FindById(mensajeVO.Id);
                        if (mensaje != null)
                        {
                            var newEvent = new LogMensaje
                            {
                                Coche = coche,
                                Chofer = empleado,
                                CodigoMensaje = mensaje.Codigo,
                                Dispositivo = coche.Dispositivo,
                                Expiracion = DateTime.UtcNow.AddDays(1),
                                Fecha = DateTime.UtcNow,
                                FechaAlta = DateTime.UtcNow,
                                FechaFin = DateTime.UtcNow,
                                IdCoche = coche.Id,
                                Latitud = 0,
                                LatitudFin = 0,
                                Longitud = 0,
                                LongitudFin = 0,
                                Mensaje = mensaje,
                                Texto =
                                    "INFORME DE RECHAZO NRO " + rechazoEntity.Id + ": " + mensaje.Descripcion + " -> " +
                                    rechazoEntity.Entrega.Descripcion,
                                Usuario = Usuario
                            };

                            logMensajeDao.Save(newEvent);
                        }
                    }
                    else
                    {
                        STrace.Warning(STrace.Module, new Exception("Vendedor sin choche asociado"));
                    }
                }

                transacion.Commit();

                return Created(string.Concat("api/ticketrechazo/item/{0}", rechazoEntity.Id), rechazoModel);

            }
            catch (Exception ex)
            {
                STrace.Exception(STrace.Module, ex);
                transacion.Rollback();
                return InternalServerError(ex);
            }
        }

        [Route("api/ticketrechazo/item/{id}")]
        public IHttpActionResult PutItem(int id, TicketRechazoModel rechazoModel)
        {
            var ticketEntity = EntityDao.FindById(id);

            ticketEntity.ChangeEstado((TicketRechazo.Estado)Enum.Parse(typeof(TicketRechazo.Estado), rechazoModel.Estado), rechazoModel.Observacion, Usuario.Empleado);
            EntityDao.SaveOrUpdate(ticketEntity);

            return Ok();
        }

        [Route("api/ticketrechazo/distrito/{distritoId}/base/{baseId}/estadisticas/rol")]
        public IHttpActionResult GetPromedioPorRol(int distritoId, int baseId)
        {
            var w = EntityDao.GetPromedioPorRol(distritoId, baseId);

            var vend = w.FirstOrDefault(e => e.TipoEmpleado == "V");
            var sup = w.FirstOrDefault(e => e.TipoEmpleado == "SR");
            var jef = w.FirstOrDefault(e => e.TipoEmpleado == "JF");
            var otr = w.Where(e => e.TipoEmpleado != "V" && e.TipoEmpleado != "SR" && e.TipoEmpleado != "JF");

            var promedios = new
            {
                vendedor = (vend == null ? 0 : vend.Promedio) / 60,
                supervisorVentas = (sup == null ? 0 : sup.Promedio) / 60,
                jefeVentas = (jef == null ? 0 : jef.Promedio) / 60,
                otros = (otr.Sum(e => e.Promedio) / 60) / Math.Max(1, otr.Count())
            };

            return Json(promedios);
        }

        [Route("api/ticketrechazo/distrito/{distritoId}/base/{baseId}/estadisticas/estado")]
        public IHttpActionResult GetCantidadPorEstado(int distritoId, int baseId)
        {
            var w = EntityDao.FindAll();

            if (distritoId != -1)
                w = w.Where(t => t.Empresa.Id == distritoId);
            if (baseId != -1)
                w = w.Where(t => t.Linea.Id == baseId);

            w = w.Where(t => t.FechaHoraEstado >= DateTime.UtcNow.AddHours(-24));

            var list = w.ToList().GroupBy(t => t.UltimoEstado)
                 .Select(
                     g =>
                         new CantidadPorEstadoModel
                         {
                             Cantidad = g.Count(),
                             Estado = CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(g.Key))
                         });


            return Json(list.ToArray());
        }

        [Route("api/ticketrechazo/estadisticas/promedio/porvendedor")]
        public DataSourceResult GetPromedioPorVendedor([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            var list = EntityDao.GetPromedioPorVendedor(-1, -1);
            return list.ToDataSourceResult(request);
        }

        [Route("api/ticketrechazo/estadisticas/promedio/porestado")]
        public DataSourceResult GetPromedioPorEstado([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            var list = EntityDao.GetPromedioPorEstado(-1, -1);
            list.ForEach(e => e.Promedio = e.Promedio / 60);
            //var r = new DataSourceResult {Data = list.ToArray()};
            return list.ToDataSourceResult(request);
        }
    }
}
