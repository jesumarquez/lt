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


          /// <summary>
        /// Gets the geocerca events for the givenn mobile and date.
        /// PAGINADO
        /// </summary>
        /// <param name="mobiles"></param>
        /// <param name="initDate"></param>
        /// <param name="endDate"></param>
        /// <param name="geocercasIds"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public List<MobileGeocerca> GetGeocercasEvent(IEnumerable<int> mobilesIds, List<Int32> geocercasIds, DateTime initDate, DateTime endDate, Double duration, int page, int pageSize, ref int totalRows, bool reCount, string SearchString, bool calcularKms, DAOFactory daoFactory, TimeSpan tpEnMarcha)
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

            for (var i = 0; i < results.Count - 1; i++)
            {
                if (!results[i].Interno.Equals(results[i + 1].Interno)) continue;

                var salida = results[i].Salida;
                var entrada = results[i + 1].Entrada;

                if (salida.Equals(DateTime.MinValue) || entrada.Equals(DateTime.MinValue))
                {
                    results[i].ProximaGeocerca = TimeSpan.MinValue;
                    results[i].Recorrido = 0.0;
                }
                else
                {
                    results[i].ProximaGeocerca = entrada.Subtract(salida);
                    results[i].Recorrido = calcularKms ? daoFactory.CocheDAO.GetDistance(results[i].IdMovil, salida.ToDataBaseDateTime(), entrada.ToDataBaseDateTime()) : 0.0;
                }

                results[i].ProximaGeocerca = salida.Equals(DateTime.MinValue) || entrada.Equals(DateTime.MinValue) ? TimeSpan.MinValue : entrada.Subtract(salida);
            }


            var results2 = new List<MobileGeocerca>(results.Count);

            for (var i = 0; i < results.Count; i++)
            {
                if (!results[i].ProximaGeocerca.Equals(TimeSpan.MinValue) && results[i].ProximaGeocerca < tpEnMarcha) continue;

                results2.Add(results[i]);

                if (i >= results.Count - 1) continue;

                if (!results[i].Interno.Equals(results[i + 1].Interno)) continue;

                results2.Add(results[i + 1]);

                i++;
            }

            if (string.IsNullOrEmpty(SearchString))
            {
                if (reCount)
                {
                    int count = results2.Where(res => res.Duracion >= TimeSpan.FromSeconds(duration) || res.Duracion.Equals(TimeSpan.MinValue))
                        .ToList().Count();
                    if (!totalRows.Equals(count))
                    {
                        totalRows = count;
                    }
                }

                return results2.Where(res => res.Duracion >= TimeSpan.FromSeconds(duration) || res.Duracion.Equals(TimeSpan.MinValue))
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
            else
            {
                if (reCount)
                {
                    int count = results2.Where(res => res.Duracion >= TimeSpan.FromSeconds(duration) || res.Duracion.Equals(TimeSpan.MinValue)
                        && (res.Geocerca.ToUpper().Contains(SearchString.ToUpper()) || 
                            res.TipoGeocerca.ToUpper().Contains(SearchString.ToUpper()) ||
                            res.Patente.ToUpper().Contains(SearchString.ToUpper())))
                            .ToList().Count();
                    if (!totalRows.Equals(count))
                    {
                        totalRows = count;
                    }
                }

                return results2.Where(res => res.Duracion >= TimeSpan.FromSeconds(duration) || res.Duracion.Equals(TimeSpan.MinValue) 
                        && (res.Geocerca.ToUpper().Contains(SearchString.ToUpper()) ||
                            res.TipoGeocerca.ToUpper().Contains(SearchString.ToUpper()) ||
                            res.Patente.ToUpper().Contains(SearchString.ToUpper())))
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
           
        }

        #endregion
    }
}
