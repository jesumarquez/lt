using System;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Messages.Saver;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Utils;

namespace Logictracker.Process.Import.EntityParser
{
    public class TicketV1 : EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Ticket"; }
        }

        public TicketV1()
        {
        }

        public TicketV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var ticket = GetTicket(empresa, linea, data);
            if (data.Operation == (int) Operation.Delete) return ticket;

            if (ticket.Id == 0)
            {
                ticket.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                ticket.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
                var pedido = GetPedido(empresa, linea, data);
                var cliente = pedido != null ? pedido.Cliente : GetCliente(empresa, linea, data);
                var punto = pedido != null ? pedido.PuntoEntrega : GetPuntoEntrega(empresa, linea, cliente.Id, data);
                ticket.Pedido = pedido;
                ticket.Cliente = cliente;
                ticket.PuntoEntrega = punto;
                ticket.OrdenDiario = -1;
                ticket.SourceStation = "LogicLink";
                ticket.SourceFile = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
            }

            ticket.Linea = GetLinea(empresa, data[Properties.Ticket.Linea]) ?? ticket.Linea;

            if (ticket.Linea == null)
            {
                throw new EntityParserException("No se especificó una base.");
            }
            var codigoDestino = data[Properties.Ticket.LineaDestino];
            var gmt = GetGmt(data);
            DateTime? fechaInicio = (data[Properties.Ticket.FechaInicio] ?? string.Empty).AsDateTime(gmt);
            DateTime? fechaEnObra = (data[Properties.Ticket.FechaEnObra] ?? string.Empty).AsDateTime(gmt);
            ticket.OrdenDiario = (data[Properties.Ticket.OrdenDiario]??string.Empty).AsInt() ?? 0;

            var codigoProducto = data[Properties.Ticket.Producto] ?? string.Empty;
            var producto = string.IsNullOrEmpty(codigoProducto) ? null
                : DaoFactory.ProductoDAO.FindByCodigo(empresa, linea, -1, codigoProducto.Truncate(10));
            ticket.CodigoProducto = producto != null ? producto.Codigo : codigoProducto.Truncate(10);
            ticket.DescripcionProducto = producto != null
                                             ? producto.Descripcion.Truncate(50)
                                             : codigoProducto.Truncate(50);
            ticket.CantidadPedido = (data[Properties.Ticket.CantidadPedido] ?? string.Empty).Truncate(12);
            ticket.CantidadCarga = (data[Properties.Ticket.CantidadCarga] ?? string.Empty).Truncate(12);
            ticket.CumulativeQty = (data[Properties.Ticket.CantidadAcumulada] ?? string.Empty).Truncate(12);
            ticket.Unidad = (data[Properties.Ticket.Unidad] ?? string.Empty).Truncate(5);
            ticket.UserField1 = (data[Properties.Ticket.Comentario1] ?? string.Empty).Truncate(50);
            ticket.UserField2 = (data[Properties.Ticket.Comentario2] ?? string.Empty).Truncate(50);
            ticket.UserField3 = (data[Properties.Ticket.Comentario3] ?? string.Empty).Truncate(50);

            var vehiculo = data[Properties.Ticket.Vehiculo];
            if(vehiculo != null)
            {
                ticket.Vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] {empresa}, new[] {linea}, vehiculo);
            }
            var chofer = data[Properties.Ticket.Chofer];
            if (!string.IsNullOrEmpty(chofer))
            {
                ticket.Empleado = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, chofer);
            }

            ticket.Detalles.Clear();
            var detalles = DaoFactory.EstadoDAO.GetByPlanta(ticket.Linea.Id);
            if (fechaEnObra.HasValue)
            {
				var enObra = detalles.Where(e => e.EsPuntoDeControl == Estado.Evento.LlegaAObra).SafeFirstOrDefault();
                if (enObra == null) throw new EntityParserException("No existe un estado de tipo Llegada a Obra");
                var index = detalles.IndexOf(enObra);
                var deltaInicio = detalles.Where((e, i) => i < index).Sum(e => e.Deltatime);
                fechaInicio = fechaEnObra.Value.AddMinutes(-deltaInicio);
            }
            if (fechaInicio.HasValue)
            {
                var hora = fechaInicio.Value;
                ticket.FechaTicket = hora;
                foreach (var detalle in detalles)
                {
                    var detalleTicket = new DetalleTicket
                    {
                        EstadoLogistico = detalle,
                        Ticket = ticket,
                        Programado = hora
                    };
                    ticket.Detalles.Add(detalleTicket);
                    hora = hora.AddMinutes(detalle.Deltatime);
                }
            }

            if (ticket.OrdenDiario == -1 && ticket.FechaTicket.HasValue)
            {
                ticket.OrdenDiario = DaoFactory.TicketDAO.FindNextOrdenDiario(empresa, linea, ticket.FechaTicket.Value);
            }
            return ticket;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Ticket;
            if(ValidateSaveOrUpdate(item)) DaoFactory.TicketDAO.SaveOrUpdate(item);
            var iniciar = data[Properties.Ticket.Iniciar];
            if (iniciar != null && iniciar.AsBool()) InitTicket(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Ticket;
            if(ValidateDelete(item)) DaoFactory.TicketDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Ticket;
            if(ValidateSave(item)) DaoFactory.TicketDAO.SaveOrUpdate(item);
            var iniciar = data[Properties.Ticket.Iniciar];
            if (iniciar != null && iniciar.AsBool()) InitTicket(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Ticket;
            if(ValidateUpdate(item)) DaoFactory.TicketDAO.SaveOrUpdate(item);
        }

        #endregion

        protected void InitTicket(Ticket ticket)
        {
            if (ticket.Estado != Ticket.Estados.Pendiente) return;
            if(ticket.Vehiculo == null)
            {
                throw new ApplicationException("No se puede iniciar el ticket porque no tiene un vehiculo asignado");
            }
            var ciclo = new CicloLogisticoHormigon(ticket, DaoFactory, new MessageSaver(DaoFactory));
            ciclo.ProcessEvent(new InitEvent(DateTime.UtcNow));
        }

        protected virtual Ticket GetTicket(int empresa, int linea, IData data)
        {
            string codigo = null;
            for (var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.Ticket.Codigo) codigo = data.Values[i];

            if (codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.TicketDAO.FindByCode(new[] { empresa }, new[] { linea }, codigo);
            return sameCode ?? new Ticket { Codigo = codigo };
        }
        protected virtual PuntoEntrega GetPuntoEntrega(int empresa, int linea, int cliente, IData data)
        {
            string codigo = null;
            for (var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.Ticket.PuntoEntrega) codigo = data.Values[i];

            if (codigo == null) throw new EntityParserException("No se encuentra el campo 'PuntoEntrega' para el ticket a importar");


            var puntoEntrega = DaoFactory.PuntoEntregaDAO.FindByCode(new[] { empresa }, new[] { linea }, new[] { cliente },  codigo);
            if (puntoEntrega == null) throw new EntityParserException("No se encuentra el Punto de Entrega " + codigo + " para el ticket a importar");

            return puntoEntrega;
        }
        protected virtual Cliente GetCliente(int empresa, int linea, IData data)
        {
            string codigo = null;
            for (var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.Ticket.Cliente) codigo = data.Values[i];

            if (codigo == null) throw new EntityParserException("No se encuentra el campo 'Cliente' para el ticket a importar");


            var client = DaoFactory.ClienteDAO.FindByCode(new[] { empresa }, new[] { linea }, codigo);
            if (client == null) throw new EntityParserException("No se encuentra el Cliente " + codigo + " para el ticket a importar");

            return client;
        }
        protected virtual Pedido GetPedido(int empresa, int linea, IData data)
        {
            string codigo = null;
            for (var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.Ticket.Pedido) codigo = data.Values[i];

            return codigo == null ? null : DaoFactory.PedidoDAO.FindByCode(empresa, codigo);
        }
        protected virtual int GetGmt(IData data)
        {
            string gmt = null;
            for (var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.Ticket.Gmt) gmt = data.Values[i];

            int igmt;
            int.TryParse(gmt, out igmt);
            return igmt;
        }
    }

}
