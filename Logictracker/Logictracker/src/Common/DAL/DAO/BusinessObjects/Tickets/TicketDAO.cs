using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Tickets;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Tickets
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class TicketDAO : GenericDAO<Ticket>
    {
//        public TicketDAO(ISession session) : base(session) { }

        #region Find Methods

        public Ticket FindEnCurso(Dispositivo disp)
        {
            if (disp == null) return null;

            if (disp.KeyExists(Ticket.CurrentCacheKey))
            {
                var key = disp.Retrieve<object>(Ticket.CurrentCacheKey);
                return key == null ? null : FindById((int)key);
            }

            var current = Query.Where(t => t.Estado == Ticket.Estados.EnCurso
                                        && t.Dispositivo != null
                                        && t.Dispositivo.Id == disp.Id)
                               .OrderBy(t => t.FechaTicket)
                               .FirstOrDefault();

            StoreInCache(disp, current);

            return current;
        }

        public Ticket FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string codigo)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(t => t.Codigo == codigo && t.Estado != Ticket.Estados.Eliminado)
                .FirstOrDefault();
        }

        public IEnumerable<Ticket> FindByCocheYFecha(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> coches, DateTime fechaDesde, DateTime fechaHasta)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .FilterVehiculo(Session, empresas, lineas, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, coches)
                        .Where(t => t.FechaTicket >= fechaDesde 
                                 && t.FechaTicket <= fechaHasta
                                  && t.Estado != Ticket.Estados.Eliminado)
                        .ToList();
        }
        public List<Ticket> FindForExport(int empresa, int linea, int cantidad)
        {

            var q = Query.FilterEmpresa(Session, new[] { empresa }, null)
                .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                .Where(t => t.Pedido != null)
                .Where(t => t.ASincronizar);
            
            if(cantidad > 0) q = q.Take(cantidad);
            
            return q.ToList();
        }

        public Ticket FindByOrdenDiario(int empresa, int linea, DateTime dia, int orden)
        {
            return Query.FilterEmpresa(Session, new[]{empresa}, null)
                .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                .Where(t => (t.FechaTicket >= dia.Date) && (t.FechaTicket < dia.Date.AddDays(1)) && (t.OrdenDiario == orden) && t.Estado != Ticket.Estados.Eliminado)
                .FirstOrDefault();
        }

        public int FindNextOrdenDiario(int empresa, int linea, DateTime date)
        {
            var cacheKey = string.Format("DailyOrder[{0}][{1}]", empresa, linea);
            int maxId;
            if (LogicCache.KeyExists(typeof(Ticket), cacheKey))
            {
                maxId = (int)LogicCache.Retrieve<object>(typeof(Ticket), cacheKey);
            }
            else
            {
                var max = Query.FilterEmpresa(Session, new[] { empresa }, null)
                    .FilterLinea(Session, new[]{empresa}, new[]{linea}, null)
                    .Where(t => t.FechaTicket >= date.Date && t.FechaTicket < date.Date.AddDays(1) && t.OrdenDiario.HasValue && t.Estado != Ticket.Estados.Eliminado)
                    .Max(t => t.OrdenDiario);
                maxId = max.HasValue ? max.Value : 0;
            }

            maxId++;

            LogicCache.Store(typeof(Pedido), cacheKey, maxId, DateTime.Now.AddMinutes(5));

            return maxId;
        }

        /// <summary>
        /// Gets the total amount of tickets within the specified timespan for the current vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public Int32 FindTotalTickets(int vehicle, DateTime desde, DateTime hasta)
        {
            return Query.Count(t => t.FechaTicket >= desde && t.FechaTicket <= hasta && t.Vehiculo.Id == vehicle && t.Estado != Ticket.Estados.Eliminado);
        }

        public List<Ticket> FindList(int vehicle, DateTime desde, DateTime hasta)
        {
            return Query.Where(t => t.FechaTicket >= desde && t.FechaTicket <= hasta && t.Vehiculo.Id == vehicle && t.Estado != Ticket.Estados.Eliminado).ToList();
        }

        #endregion

        #region Get Methods

        public List<Ticket> GetByPedido(int pedido)
        {
            return Query.Where(t => t.Estado != Ticket.Estados.Eliminado && t.Pedido != null && t.Pedido.Id == pedido)
                        .ToList();
        }

        public List<Ticket> GetByPedido(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> pedidos)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterPedido(Session, empresas, lineas, pedidos, new[] {-1})
                        .Where(t => t.Estado != Ticket.Estados.Eliminado)
                        .ToList();
        }
        public List<Ticket> GetByTexto(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> coches, DateTime fechaDesde, DateTime fechaHasta, string texto)
        {
            var tickets = Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .FilterVehiculo(Session, empresas, lineas, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, coches)
                .Where(t => t.FechaTicket >= fechaDesde && t.FechaTicket < fechaHasta && t.Estado != Ticket.Estados.Eliminado)
                .ToList();

            if (string.IsNullOrEmpty(texto)) return tickets;

            var textoUi = texto.ToUpperInvariant();
            return tickets.Where(t => t.Cliente.Descripcion.ToUpperInvariant().Contains(textoUi) ||
                                    t.DescripcionProducto.ToUpperInvariant().Contains(textoUi) ||
                                    t.PuntoEntrega.Descripcion.ToUpperInvariant().Contains(textoUi))
                        .ToList();
        }
        public List<Ticket> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosdeCosto, IEnumerable<int> tiposVehiculo, IEnumerable<int> vehiculos, IEnumerable<int> estados, IEnumerable<int> clientes, IEnumerable<int> puntosEntrega, IEnumerable<int> bocasDeCarga, DateTime? desde, DateTime? hasta)
        {
            var estadosList = estados.ToList();
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas);
            if(vehiculos.Count() <= 100)
            {
                q = q.FilterVehiculo(Session, empresas, lineas, transportistas, departamentos, centrosdeCosto, tiposVehiculo, vehiculos);
            }
                                     
            q=q.Where(t => t.Estado != Ticket.Estados.Eliminado);

            if (!QueryExtensions.IncludesAll(estados) || estados.Contains(0)) q = q.Where(t => estadosList.Contains(t.Estado));
            if (desde.HasValue) q = q.Where(t => t.FechaTicket >= desde);
            if (hasta.HasValue) q = q.Where(t => t.FechaTicket < hasta);

            var list = q;
            if (!QueryExtensions.IncludesAll(clientes)) list = list.FilterCliente(Session, empresas, lineas, clientes);
            if (!QueryExtensions.IncludesAll(puntosEntrega)) list = list.FilterPuntoEntrega(Session, empresas, lineas, clientes, puntosEntrega);
            if (!QueryExtensions.IncludesAll(bocasDeCarga)) list = list.Where(x => (x.Pedido != null && bocasDeCarga.Contains(x.Pedido.BocaDeCarga.Id)));

            if (vehiculos.Count() > 100)
            {
                var all = QueryExtensions.IncludesAll(vehiculos);
                var veh = QueryExtensions.GetVehiculos(Session, empresas, lineas, transportistas, departamentos, centrosdeCosto, tiposVehiculo, vehiculos).Select(x=>x.Id);
                list = list.Where(x => (all && x.Vehiculo == null) || (x.Vehiculo != null && veh.Contains(x.Vehiculo.Id)));
            }
            return list.ToList();
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Saves or updates the specified ticket whithout openning a new transaction.
        /// </summary>
        /// <param name="obj"></param>
        public override void SaveOrUpdate(Ticket obj)
        {
            base.SaveOrUpdate(obj);
            UpdateCache(obj);
        }

        /// <summary>
        /// Disables the givenn Ticket.
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public override void Delete(Ticket ticket)
        {
            if (ticket == null) return;
            if (ticket.Estado == Ticket.Estados.Pendiente && ticket.Pedido != null) ticket.ASincronizar = true;
            ticket.Estado = Ticket.Estados.Eliminado;
            SaveOrUpdate(ticket);
            UpdateCache(ticket);
        }
        
        #endregion

        #region Other Methods

        private static void UpdateCache(Ticket ticket)
        {
            if (ticket == null || ticket.Dispositivo == null) return;
            if (ticket.Dispositivo.KeyExists(Ticket.CurrentCacheKey)) ticket.Dispositivo.Delete(Ticket.CurrentCacheKey);
            if (ticket.Estado == Ticket.Estados.EnCurso) StoreInCache(ticket.Dispositivo, ticket);
        }

        private static void StoreInCache(Dispositivo disp, Ticket ticket)
        {
            if (disp == null) return;
            disp.Store(Ticket.CurrentCacheKey, ticket == null ? null : (object)ticket.Id, DateTime.Now.AddHours(1));
        }

        #endregion
    }
}