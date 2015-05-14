using System.Linq;
using LogicOut.Server.Handlers;
using Logictracker.DAL.Factories;
using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Sync;

namespace LogicOut.Server
{
    public class QueryFactory
    {
        protected DAOFactory DaoFactory { get; set; }

        public QueryFactory(DAOFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        public OutData[] GetData(int empresa, int linea, string query, string parameters)
        {
            var q = query.ToLowerInvariant();
            var param = QueryParams.Create(parameters);
            switch(q)
            {
                case "betonmac": return GetBetonmac(empresa, linea, param);
                case "fichada": return new Fichada(DaoFactory).Process(empresa, linea, param);
                default: return GetNovedades(empresa, linea, q, param);
            }
        }
        private OutData[] GetNovedades(int empresa, int linea, string query, QueryParams parameters)
        {
            var server = parameters.ContainsKey("server") ? parameters["server"] : string.Empty;
            var novedades = DaoFactory.OutQueueDAO.GetNovedades(empresa, linea, query, server);

            return novedades
                .Select(nov => GetOutData(nov.Id, nov.Query, nov.Operacion, nov.Parametros))
                .Where(item => item != null)
                .ToArray();
        }
        public bool Done(int empresa, int linea, string query, string parameters, Usuario usuario)
        {
            var q = query.ToLowerInvariant();
            var param = QueryParams.Create(parameters);
            switch (q)
            {
                case "betonmac": return DoneBetonmac(empresa, linea, param);
                default: return Done(empresa, linea, q, param, usuario);
            }
        }
        private bool Done(int empresa, int linea, string query, QueryParams parameters, Usuario usuario)
        {
            try
            {
                if (!parameters.ContainsKey("id")) return false;

                var server = parameters.ContainsKey("server") ? parameters["server"] : string.Empty;
                int id;
                if (!int.TryParse(parameters["id"], out id)) return false;
                var ok = !parameters.ContainsKey("ok") || parameters["ok"] != "false";

                var item = DaoFactory.OutQueueDAO.FindById(id);
                var state = DaoFactory.OutStateDAO.GetItemState(id, server) ??
                            new OutState { OutQueue = item, Server = server };
                state.Sincronizado = true;
                state.Ok = ok;
                state.Fecha = DateTime.UtcNow;
                state.Usuario = usuario;

                DaoFactory.OutStateDAO.SaveOrUpdate(state);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Betonmac
        private OutData[] GetBetonmac(int empresa, int linea, QueryParams parameters)
        {
            var tickets = DaoFactory.TicketDAO.FindForExport(empresa, linea, 10);
            return tickets.Select(t => Convert.FromTicket(t.Id, t)).ToArray();
        }
        private bool DoneBetonmac(int empresa, int linea, QueryParams parameters)
        {
            try
            {
                int id;
                int.TryParse(parameters.Keys.First(), out id);
                var ticket = DaoFactory.TicketDAO.FindById(id);
                ticket.ASincronizar = false;
                ticket.FechaSincronizado = DateTime.UtcNow;
                DaoFactory.TicketDAO.SaveOrUpdate(ticket);
                return true;
            }
            catch
            {
                return false;
            }
        } 
        #endregion

        private OutData GetOutData(int itemId, string query, string operacion, string parametros)
        {
            switch(query)
            {
                case Convert.Queries.Betonmac:
                case Convert.Queries.Molinete:
                    var id = System.Convert.ToInt32(parametros);
                    switch (operacion)
                    {
                        case "empleado": return Convert.ToMolinete(itemId, DaoFactory.EmpleadoDAO.FindById(id));
                        case "tarjeta": return Convert.ToMolinete(itemId, DaoFactory.TarjetaDAO.FindById(id));
                        case "ticket": return Convert.FromTicket(itemId, DaoFactory.TicketDAO.FindById(id));
                        default: return null;
                    }
                default: return null;
            }
        }
    }
}
