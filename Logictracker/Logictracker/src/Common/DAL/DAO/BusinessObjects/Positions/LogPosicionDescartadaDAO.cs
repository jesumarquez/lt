#region Usings

using System;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Positions;
using NHibernate;
using System.Collections.Generic;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Positions
{
    /// <summary>
    /// LogPosicionDescartada data access class.
    /// </summary>
    public class LogPosicionDescartadaDAO : MaintenanceDAO<LogPosicionDescartada>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public LogPosicionDescartadaDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        public IList<LogPosicionDescartada> GetLastPositions(int dispositivo, int maxresults)
        {
            return Session.QueryOver<LogPosicionDescartada>()
                .And(l => l.Dispositivo.Id == dispositivo)
                .OrderBy(l => l.Id).Desc
                .Take(maxresults)
                .List<LogPosicionDescartada>();
        }

        /// <summary>
        /// Saves discarted position into database.
        /// </summary>
        /// <param name="obj"></param>
        public new void Save(LogPosicionDescartada obj) { Session.Save(obj); }


        public string GetMotivoDescarte(int motivo)
        {

            // TODO: CULTURE
            switch (motivo)
            {
                case 1: return "Sin movil asignado";
                case 2: return "Fecha invalida";
                case 3: return "Fuera del mapa";
                case 4: return "Velocidad invalida";
                case 5: return "Distancia invalida";
                case 6: return "Bajo nivel de señal";
                case 7: return "Excepcion";
                case 8: return "Descartado por Datamart";
                case 9: return "Descarte Manual";
                case 10: return "Dispositivo no asignado";
                case 11: return "Mensaje no encontrado";
                case 12: return "Dentro de inhibidor";
                case 13: return "Posiciones perdidas";
                case 14: return "Duracion Invalida";
                case 17: return "Existe una mejor posición en velocidad 0";
                default: return "Sin Definir";
            }
        }            
        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the positions deletion sql command.
        /// </summary>
        /// <returns></returns>
        protected override String GetDeleteCommand() { return "delete top(:n) from opeposi08 where opeposi08_fechora <= :date ; select @@ROWCOUNT as count;"; }

        #endregion
    }
}