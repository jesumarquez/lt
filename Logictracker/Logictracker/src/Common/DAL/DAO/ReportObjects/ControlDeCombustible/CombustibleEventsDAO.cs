#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class CombustibleEventsDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public CombustibleEventsDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public List<CombustibleEvent> FindByEquipoAndDate(int equipo, DateTime startDate, DateTime endDate)
        {
            return DAOFactory.EventoCombustibleDAO.FindByEquipo(equipo, startDate, endDate)
                                                  .Select(e => new CombustibleEvent
                                                    {
                                                        Fecha = e.Fecha,
                                                        MotorDescri = e.Motor.Descripcion,
                                                        Mensaje = e.MensajeDescri,
                                                        IDAccion = e.Accion.Id
                                                    }).ToList();
        }

        public IEnumerable FindByMotoresMensajesAndFecha(List<int> motores, List<int> tanques, List<string> mensajes, DateTime startDate, DateTime endDate)
        {
            var listMotores = DAOFactory.EventoCombustibleDAO.GetByEnginesAndCodes(mensajes, motores, startDate, endDate)
                .Select(e => new CombustibleEvent
                                 {
                                      Fecha = e.Fecha,
                                      MotorDescri = e.Motor.Descripcion,
                                      Mensaje = e.MensajeDescri,
                                      IDAccion = e.Accion.Id
                                 }).ToList();

            var listTanques = DAOFactory.EventoCombustibleDAO.GetByTanksAndCodes(mensajes, tanques, startDate, endDate)
                .Select(e => new CombustibleEvent
                                    {
                                        Fecha = e.Fecha,
                                        MotorDescri = e.Tanque.Descripcion,
                                        Mensaje = e.MensajeDescri,
                                        IDAccion = e.Accion.Id
                                    }).ToList();

            return listMotores.Union(listTanques).OrderBy(e => e.Fecha);
        }
    }
}
