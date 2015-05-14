#region Usings

using System;
using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class NivelesTanqueDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public NivelesTanqueDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public IEnumerable<NivelesTanque> GetVolumesByDate(int tanque, DateTime desde, DateTime hasta,int groupByXMinutes)
        {
            var daoFactory = new DAOFactory();

            var capacidad = daoFactory.TanqueDAO.FindById(tanque).Capacidad;

            var actualDate = desde;
            var endDate = actualDate.AddMinutes(groupByXMinutes);
            var results = new List<NivelesTanque>();

            var initialVolume = DAOFactory.VolumenHistoricoDAO.FindInitialRealVolume(tanque, actualDate, endDate);

            if (initialVolume != null) results.Add(new NivelesTanque
                                          {
                                              Volumen = initialVolume.Volumen,
                                              Fecha = initialVolume.Fecha,
                                              IdTanque = initialVolume.Tanque.Id,
                                              TanqueDescri = initialVolume.Tanque.Descripcion,
                                              PorcentajeLlenado = capacidad > 0 ? initialVolume.Volumen*100/capacidad : -1
                                          });
            
            while (actualDate <= hasta)
            {
                var volume = DAOFactory.VolumenHistoricoDAO.FindInitialRealVolume(tanque, actualDate, endDate);

                if (volume != null) results.Add( new NivelesTanque
                                  {
                                      Volumen = volume.Volumen,
                                      Fecha = endDate <= DateTime.Now ? endDate : DateTime.Now,
                                      IdTanque = volume.Tanque.Id,
                                      TanqueDescri = volume.Tanque.Descripcion,
                                      PorcentajeLlenado = capacidad > 0 ? volume.Volumen*100/capacidad : -1
                                  });

                actualDate = endDate;
                endDate = actualDate.AddMinutes(groupByXMinutes);
            }

            return results;
        }
    }
}
