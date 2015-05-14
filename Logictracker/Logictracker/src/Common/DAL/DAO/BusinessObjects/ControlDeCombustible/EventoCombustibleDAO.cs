#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible
{
    public class EventoCombustibleDAO : GenericDAO<EventoCombustible>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public EventoCombustibleDAO(ISession session) : base(session) { }

        #endregion

        public IEnumerable<EventoCombustible> GetByEnginesAndCodes(List<String> mensajes, List<Int32> motores, DateTime startDate, DateTime endDate)
        {
            return Session.Query<EventoCombustible>().Where(e => mensajes.Contains(e.Mensaje.Codigo) && motores.Contains(e.Motor.Id) && e.Fecha >= startDate && e.Fecha <= endDate).ToList();
        }

        public IEnumerable<EventoCombustible> GetByTanksAndCodes(List<String> mensajes, List<Int32> tanques, DateTime startDate, DateTime endDate)
        {
            return Session.Query<EventoCombustible>().Where(e => mensajes.Contains(e.Mensaje.Codigo) && tanques.Contains(e.Tanque.Id) && e.Fecha >= startDate && e.Fecha <= endDate).ToList();
        }

        public IEnumerable<EventoCombustible> FindByEquipo(int equipo, DateTime startDate, DateTime endDate)
        {
            var caudalimeterEvents = Session.Query<EventoCombustible>()
                        .Where(e => e.Motor != null && e.Motor.Equipo != null && e.Motor.Equipo.Id == equipo 
                            && e.Fecha >= startDate && e.Fecha <= endDate);

            var tankEvents = Session.Query<EventoCombustible>()
                        .Where(e => e.Tanque != null && e.Tanque.Equipo != null && e.Tanque.Equipo.Id == equipo
                                    && e.Fecha >= startDate && e.Fecha <= endDate);

            return caudalimeterEvents.Union(tankEvents).ToList();
        }
    }
}