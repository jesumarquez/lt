using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public partial class LogMensajeDAO
    {
        public int Count(int vehiculo, string messageCode, DateTime desde, DateTime hasta, int maxMonths)
        {
            Mensaje mensaje = null;

            var minDesde = DateTime.UtcNow.AddMonths(-maxMonths);
            desde = desde > minDesde ? desde : minDesde;

            return Session.QueryOver<LogMensaje>()
                .JoinAlias(dm => dm.Mensaje, () => mensaje, JoinType.InnerJoin)
                .Where(m => m.Coche.Id == vehiculo)
                .And(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(m => mensaje.Codigo == messageCode)
                .Select(Projections.RowCount())
                .SingleOrDefault<int>();
        }

        public IEnumerable<LoginLogout> GetDrivers(int vehiculo, DateTime desde, DateTime hasta, int maxMonths)
        {
            Mensaje mensaje = null;
            var loginCode = MessageCode.RfidDriverLogin.GetMessageCode();
            var logoutCode = MessageCode.RfidDriverLogout.GetMessageCode();

            var minDesde = DateTime.UtcNow.AddMonths(-maxMonths);
            desde = desde > minDesde ? desde : minDesde;

            var mensajes = Session.QueryOver<LogMensaje>()
                .JoinAlias(dm => dm.Mensaje, () => mensaje, JoinType.InnerJoin)
                .Where(m => m.Coche.Id == vehiculo)
                .And(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(m => mensaje.Codigo == loginCode || mensaje.Codigo == logoutCode)
                .OrderBy(m=>m.Fecha).Asc
                .List<LogMensaje>();

            LoginLogout last = null;
            foreach (var logMensaje in mensajes)
            {
                if (logMensaje.Mensaje.Codigo == loginCode)
                {
                    last = new LoginLogout {LoginDate = logMensaje.Fecha, Empleado = logMensaje.Chofer};
                    yield return last;
                }
                else if(logMensaje.Mensaje.Codigo == logoutCode && last != null)
                {
                    last.LogoutDate = logMensaje.Fecha;
                }
            }
        }

        public IList<LogMensaje> GetEventos(int vehiculo, DateTime desde, DateTime hasta, int maxMonths, params string[] codigos)
        {
            var mensajesList = codigos.ToList();
            Mensaje m = null;

            var minDesde = DateTime.UtcNow.AddMonths(-maxMonths);
            desde = desde > minDesde ? desde : minDesde;

            return Session.QueryOver<LogMensaje>()
                .JoinAlias(ev => ev.Mensaje, () => m, JoinType.InnerJoin)
                .Where(ev => ev.Fecha >= desde && ev.Fecha < hasta)
                .And(x => x.Coche.Id == vehiculo)
                .And(Restrictions.InG(Projections.Property<Mensaje>(ev => m.Codigo), mensajesList))
                .OrderBy(Projections.Property<LogMensaje>(ev => ev.Coche.Id)).Asc
                .ThenBy(Projections.Property<LogMensaje>(ev => ev.Fecha)).Asc
                .List<LogMensaje>();
        }
    }

    public class LoginLogout
    {
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public Empleado Empleado { get; set; }
    }
}