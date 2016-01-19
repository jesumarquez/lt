using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Rechazos;

namespace Logictracker.Web.Models
{

    public class TicketRechazoDetalleModel
    {
        public int TicketRechazoDetalleId { get; set; }
        public string Observacion { get; set; }
        public string EmpleadoDesc { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; }
    }

    public class TicketRechazoModel
    {
        public int DistritoId { get; set; }
        public int LineaId { get; set; }
        public DateTime FechaHora { get; set; }
        public int TicketRechazoId { get; set; }
        public int ClienteId { get; set; }
        public string ClienteCodigo { get; set; }
        public string ClienteDesc { get; set; }
        public int SupVenId { get; set; }
        public string SupVenDesc { get; set; }
        public int SupRutId { get; set; }
        public string SupRutDesc { get; set; }
        public string Estado { get; set; }
        public string Territorio { get; set; }
        public TicketRechazo.MotivoRechazo Motivo { get; set; }
        public int Bultos { get; set; }
        public int VendedorId { get; set; }
        public string Observacion { get; set; }
        public bool EnHorario { get; set; }
        public IList<TicketRechazoDetalleModel> Detalle { get; set; }

        public int EntregaId { get; set; }
        public string EntregaDesc { get; set; }
        public string MotivoDesc { get; set; }
        public int TransportistaId { get; set; }
        public string TransportistaDesc { get; set; }
        public string EntregaCodigo { get; set; }
        public string VendedorDesc { get; set; }
        public DateTime FechaHoraEstado { get; set; }

        public int ChoferId { get; set; }
        public string ChoferDesc { get; set; }
        public TicketRechazo.EstadoFinal EstadoFinal { get; set; }
        public TicketRechazo.Estado UltimoEstado { get; set; }

        private String GetSemaforoGyo(double transcurrido)
        {
            return transcurrido < 5 ? ColorTranslator.ToHtml(Color.Gray) : ColorTranslator.ToHtml(transcurrido < 10 ? Color.Yellow : Color.Orange);
        }

        public string Semaforo
        {

            get
            {
                switch (EstadoFinal)
                {
                    case TicketRechazo.EstadoFinal.SolucionPendiente:

                        var transcurrido = DateTime.UtcNow.Subtract(FechaHoraEstado).TotalMinutes;

                        switch (UltimoEstado)
                        {
                            case TicketRechazo.Estado.Pendiente:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.Notificado:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.Alertado:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.Resuelto:
                                return ColorTranslator.ToHtml(Color.Green);
                            case TicketRechazo.Estado.Anulado:
                                return ColorTranslator.ToHtml(Color.Black);
                            case TicketRechazo.Estado.Avisado:
                                return ColorTranslator.ToHtml(Color.Purple);
                            case TicketRechazo.Estado.Entregado:
                                return ColorTranslator.ToHtml(Color.Green);
                            case TicketRechazo.Estado.SinAviso:
                                return ColorTranslator.ToHtml(Color.Red);
                            case TicketRechazo.Estado.NoResuelta:
                                return ColorTranslator.ToHtml(Color.Red);
                            case TicketRechazo.Estado.AltaErronea:
                                return ColorTranslator.ToHtml(Color.Green);
                            case TicketRechazo.Estado.Duplicado:
                                return ColorTranslator.ToHtml(Color.Black);
                            case TicketRechazo.Estado.NotificadoAutomatico:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.AlertadoAutomatico:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.Notificado1:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.Notificado2:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.Notificado3:
                                return GetSemaforoGyo(transcurrido);
                            case TicketRechazo.Estado.RespuestaExitosa:
                                return ColorTranslator.ToHtml(Color.LawnGreen);
                            case TicketRechazo.Estado.RespuestaConRechazo:
                                return ColorTranslator.ToHtml(Color.Purple);
                            case TicketRechazo.Estado.Rechazado:
                                return ColorTranslator.ToHtml(Color.Red);
                        }

                        return ColorTranslator.ToHtml(Color.Red);
                    case TicketRechazo.EstadoFinal.RechazoDuplicado:
                        return ColorTranslator.ToHtml(Color.Black);
                    case TicketRechazo.EstadoFinal.RechazoErroneo:
                        return ColorTranslator.ToHtml(Color.Red);
                    case TicketRechazo.EstadoFinal.ResueltoEntregado:
                        return ColorTranslator.ToHtml(Color.Green);
                    case TicketRechazo.EstadoFinal.ResueltoSinEntrega:
                        return ColorTranslator.ToHtml(Color.Red);
                    case TicketRechazo.EstadoFinal.Anulado:
                        return ColorTranslator.ToHtml(Color.Black);
                    default:
                        return ColorTranslator.ToHtml(Color.Red);
                }
            }
        }

        public string UltimoEstadoDesc { get; set; }
    }

    public class TicketRechazoDetalleMapper : EntityModelMapper<DetalleTicketRechazo, TicketRechazoDetalleModel>
    {
        public override TicketRechazoDetalleModel EntityToModel(DetalleTicketRechazo entity,
            TicketRechazoDetalleModel model)
        {
            model.TicketRechazoDetalleId = entity.Id;
            model.FechaHora = DateTime.SpecifyKind(entity.FechaHora, DateTimeKind.Utc);
            model.EmpleadoDesc = entity.Empleado.Entidad.Descripcion;
            model.Observacion = entity.Observacion;
            model.Estado = CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(entity.Estado));

            return model;
        }

        public override DetalleTicketRechazo ModelToEntity(TicketRechazoDetalleModel model, DetalleTicketRechazo entity)
        {
            throw new NotImplementedException();
        }

        public override ItemModel ToItem(DetalleTicketRechazo entity)
        {
            throw new NotImplementedException();
        }
    }

    public class TicketRechazoMapper : EntityModelMapper<TicketRechazo, TicketRechazoModel>
    {
        public override TicketRechazoModel EntityToModel(TicketRechazo entity, TicketRechazoModel model)
        {
            model.TicketRechazoId = entity.Id;
            model.DistritoId = entity.Empresa.Id;
            model.LineaId = entity.Linea.Id;
            model.FechaHora = DateTime.SpecifyKind(entity.FechaHora, DateTimeKind.Utc);
            model.ClienteId = entity.Cliente.Id;
            model.ClienteCodigo = entity.Cliente.Codigo;
            model.ClienteDesc = entity.Cliente.Descripcion;
            model.SupVenDesc = entity.SupervisorVenta.Entidad.Descripcion;
            model.SupRutDesc = entity.SupervisorRuta.Entidad.Descripcion;
            model.Estado = CultureManager.GetLabel(TicketRechazo.GetEstadoLabelVariableName(entity.Detalle.OrderByDescending(e => e.FechaHora).First().Estado));
            model.Territorio = entity.Territorio;
            model.Motivo = entity.Motivo;
            model.MotivoDesc = CultureManager.GetLabel(TicketRechazo.GetMotivoLabelVariableName(entity.Motivo));
            model.Bultos = entity.Bultos;
            model.Observacion = entity.Detalle.First().Observacion;
            model.EnHorario = entity.EnHorario;
            model.EstadoFinal = entity.Final;
            model.UltimoEstado = entity.UltimoEstado;
            model.UltimoEstadoDesc = CultureManager.GetLabel(TicketRechazo.GetEstadoFinalVariableName(entity.Final));

            var mt = new TicketRechazoDetalleMapper();
            model.Detalle = new List<TicketRechazoDetalleModel>(entity.Detalle.Select(d => mt.EntityToModel(d, new TicketRechazoDetalleModel())));
            if (entity.Entrega != null)
            {
                model.EntregaId = entity.Entrega.Id;
                model.EntregaDesc = entity.Entrega.Descripcion;
                model.EntregaCodigo = entity.Entrega.Codigo;

            }

            if (entity.Transportista != null)
            {
                model.TransportistaId = entity.Transportista.Id;
                model.TransportistaDesc = entity.Transportista.Descripcion;
            }

            if (entity.Vendedor != null)
            {
                model.VendedorId = entity.Vendedor.Id;
                model.VendedorDesc = entity.Vendedor.Entidad.Descripcion;
            }

            model.FechaHoraEstado = DateTime.SpecifyKind(entity.Detalle.OrderByDescending(e => e.FechaHora).First().FechaHora, DateTimeKind.Utc);

            if (entity.Chofer != null)
            {
                model.ChoferId = entity.Chofer.Id;
                model.ChoferDesc = entity.Chofer.Entidad.Descripcion;
            }

            return model;
        }

        public override TicketRechazo ModelToEntity(TicketRechazoModel model, TicketRechazo entity)
        {
            entity.Id = model.TicketRechazoId;
            entity.Empresa = DAOFactory.GetDao<EmpresaDAO>().FindById(model.DistritoId);
            entity.Linea = DAOFactory.GetDao<LineaDAO>().FindById(model.LineaId);
            entity.Cliente = DAOFactory.GetDao<ClienteDAO>().FindById(model.ClienteId);
            entity.Territorio = model.Territorio;
            entity.Bultos = model.Bultos;
            entity.Vendedor = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.VendedorId);
            entity.SupervisorRuta = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.SupRutId);
            entity.SupervisorVenta = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.SupVenId);
            entity.Motivo = model.Motivo;
            entity.EnHorario = model.EnHorario;
            entity.Entrega = DAOFactory.GetDao<PuntoEntregaDAO>().FindById(model.EntregaId);
            entity.Transportista = DAOFactory.GetDao<TransportistaDAO>().FindById(model.TransportistaId);
            entity.Chofer = DAOFactory.GetDao<EmpleadoDAO>().FindById(model.ChoferId);
            return entity;
        }

        public override ItemModel ToItem(TicketRechazo entity)
        {
            throw new NotImplementedException();
        }
    }
}