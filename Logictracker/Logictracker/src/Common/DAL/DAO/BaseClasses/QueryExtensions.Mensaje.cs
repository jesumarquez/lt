using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterMensaje<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> mensajes)
            where TQuery : IHasMensaje
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterMensaje(q, session, empresas, lineas, mensajes, user);
        }
        public static IQueryable<TQuery> FilterMensaje<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> mensajes, Usuario user)
            where TQuery : IHasMensaje
        {
            var mensajesU = GetMensaje(session, empresas, lineas, mensajes, user);
            if (mensajesU != null) q = q.Where(c => c.Mensaje == null || mensajesU.Contains(c.Mensaje));

            return q;
        }

        public static IQueryable<TQuery> FilterMensajeCodes<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<string> mensajesCodes)
            where TQuery : IHasMensaje
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterMensajeCodes(q, session, empresas, lineas, mensajesCodes, user);
        }
        public static IQueryable<TQuery> FilterMensajeCodes<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<string> mensajesCodes, Usuario user)
            where TQuery : IHasMensaje
        {
            var mensajesU = GetMensajeByCodes(session, empresas, lineas, mensajesCodes, user);
            if (mensajesU != null) q = q.Where(c => c.Mensaje == null || mensajesU.Contains(c.Mensaje));

            return q;
        }


        private static List<Mensaje> GetMensaje(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> mensajes)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetMensaje(session, empresas, lineas, mensajes, user);
        }
        private static List<Mensaje> GetMensaje(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> mensajes, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(mensajes)) return null;

            var mensajesQ = session.Query<Mensaje>()
                                   .FilterEmpresa(session, empresas, user)
                                   .FilterLinea(session, empresas, lineas, user);

            var mensajesU = mensajesQ.Cacheable().ToList();

            if (!IncludesAll(mensajes)) mensajesU = mensajesU.Where(l => mensajes.Contains(l.Id)).ToList();

            return mensajesU;
        }

        private static List<Mensaje> GetMensajeByCodes(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<string> mensajesCodes)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetMensajeByCodes(session, empresas, lineas, mensajesCodes, user);
        }
        private static List<Mensaje> GetMensajeByCodes(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<string> mensajesCodes, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(mensajesCodes)) return null;

            var mensajesQ = session.Query<Mensaje>()
                                   .FilterEmpresa(session, empresas, user)
                                   .FilterLinea(session, empresas, lineas, user);

            var mensajesU = mensajesQ.Cacheable().ToList();

            if (!IncludesAll(mensajesCodes)) mensajesU = mensajesU.Where(l => mensajesCodes.Contains(l.Codigo)).ToList();

            return mensajesU;
        }
    }
}
