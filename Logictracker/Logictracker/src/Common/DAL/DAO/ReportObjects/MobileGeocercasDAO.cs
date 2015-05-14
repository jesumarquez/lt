using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobileGeocercasDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileGeocercasDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the geocerca events for the givenn mobile and date.
        /// </summary>
        /// <param name="mobiles"></param>
        /// <param name="initDate"></param>
        /// <param name="endDate"></param>
        /// <param name="geocercasIds"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public List<MobileGeocerca> GetGeocercasEvent(IEnumerable<int> mobilesIds, List<Int32> geocercasIds, DateTime initDate, DateTime endDate, Double duration)
        {
            var results = new List<MobileGeocerca>();

            var geoRefDao = DAOFactory.ReferenciaGeograficaDAO;
            var events = DAOFactory.LogMensajeDAO.GetGeoRefferencesEvents(mobilesIds, initDate, endDate, geocercasIds, 3);

            var geocercas = geoRefDao.FindList(geocercasIds);

            foreach (var mobile in mobilesIds)
            {
                var logs = events.Where(e => e.Coche.Id == mobile);

                foreach (var log in logs)
                {
                    var geocerca = geocercas.FirstOrDefault(g => g.Id == log.IdPuntoDeInteres.Value);
                   
                    //Entrada a geocerca
                    if (log.Mensaje.Codigo.Equals(MessageCode.InsideGeoRefference.GetMessageCode()))
                    {
                        var result = new MobileGeocerca
                                         {
                                             Interno = log.Coche.Interno,
                                             Patente = log.Coche.Patente,
                                             Duracion = TimeSpan.MinValue,
                                             ProximaGeocerca = TimeSpan.MinValue,
                                             Recorrido = 0.0,
                                             Geocerca = geocerca.Descripcion,
                                             TipoGeocerca = geocerca.TipoReferenciaGeografica.Descripcion,
                                             IdMovil = mobile,
                                             Entrada = log.Fecha.ToDisplayDateTime(),
                                             Salida = DateTime.MinValue,
                                             IdGeo = log.IdPuntoDeInteres.Value
                                         };

                        results.Add(result);
                    }
                    //Salida de geocerca
                    else
                    {
                        var ultimo = results.FirstOrDefault(res => res.IdGeo == log.IdPuntoDeInteres && res.Salida.Equals(DateTime.MinValue) 
                                                                   && res.Entrada <= log.Fecha && res.IdMovil == log.Coche.Id);

                        if (ultimo != null)
                        {
                            ultimo.Salida = log.Fecha.ToDisplayDateTime();
                            ultimo.Duracion = ultimo.Entrada != DateTime.MinValue && ultimo.Salida != DateTime.MinValue ? ultimo.Salida - ultimo.Entrada : TimeSpan.MinValue;
                        }
                        else
                        {
                            var result = new MobileGeocerca
                                             {
                                                 Interno = log.Coche.Interno,
                                                 Patente = log.Coche.Patente,
                                                 Duracion = TimeSpan.MinValue,
                                                 ProximaGeocerca = TimeSpan.MinValue,
                                                 Recorrido = 0.0,
                                                 Geocerca = geocerca.Descripcion,
                                                 TipoGeocerca = geocerca.TipoReferenciaGeografica.Descripcion,
                                                 IdMovil = mobile,
                                                 Entrada = DateTime.MinValue,
                                                 Salida = log.Fecha.ToDisplayDateTime(),
                                                 IdGeo = log.IdPuntoDeInteres.Value
                                             };

                            results.Add(result);
                        }
                    }
                }
            }

            return results.Where(res => res.Duracion >= TimeSpan.FromSeconds(duration) || res.Duracion.Equals(TimeSpan.MinValue)).ToList();
        }

        #endregion
    }
}
