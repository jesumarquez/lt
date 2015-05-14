using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Utils;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobilePoiHistoricDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobilePoiHistoricDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        public IEnumerable<MobilePoiHistoric> GetMobileNearPois(IEnumerable<Coche> vehiculos, int idGeoRef, double distancia, DateTime desde, DateTime hasta)
        {
            var posicionesDao = DAOFactory.LogPosicionDAO;
            //var posicionesHistoricasDao = DAOFactory.LogPosicionHistoricaDAO;
            var mensajesDao = DAOFactory.LogMensajeDAO;
            var referenciasGeograficasDao = DAOFactory.ReferenciaGeograficaDAO;

            var results = new List<MobilePoiHistoric>();

            var referencia = referenciasGeograficasDao.FindById(idGeoRef);

            foreach (var vehicle in vehiculos)
            {
                // Si no son de la misma Empresa / Linea no los comparo.
                if (vehicle.Empresa != null && referencia.Empresa != null)
                {
                    if(vehicle.Empresa.Id != referencia.Empresa.Id) continue;
                    if(vehicle.Linea != null && referencia.Linea != null)
                    {
                        if(vehicle.Linea.Id != referencia.Linea.Id) continue;
                    }
                }

                var maxMonths = vehicle.Empresa != null ? vehicle.Empresa.MesesConsultaPosiciones : 3;

                var positions = posicionesDao.GetPositionsBetweenDates(vehicle.Id, desde, hasta, maxMonths);

                MobilePoiHistoric mobilePoi = null;

                if (positions.Count().Equals(0))
                {
                    //var positionsHist = posicionesHistoricasDao.GetPositionsBetweenDates(vehicle.Id, desde, hasta);

                    //if (positionsHist.Count <= 0) continue;

                    //mobilePoi = positionsHist.Where(pos => Distancias.Loxodromica(pos.Latitud, pos.Longitud, referencia.Latitude, referencia.Longitude) <= 
                    //    distancia).Select(pos => new MobilePoiHistoric(pos, referencia, pos.FechaMensaje.ToDisplayDateTime())).OrderBy(pos => pos.Distancia).FirstOrDefault();
                }
                else
                {
                    mobilePoi = positions.Where(pos => Distancias.Loxodromica(pos.Latitud, pos.Longitud, referencia.Latitude, referencia.Longitude) <= distancia)
                        .Select(pos => new MobilePoiHistoric(pos, referencia, pos.FechaMensaje.ToDisplayDateTime())).OrderBy(pos => pos.Distancia).FirstOrDefault();
                }

                if (mobilePoi == null) continue;

                var mensaje = mensajesDao.GetLastVehicleRfidEvent(vehicle.Id, mobilePoi.Fecha);

                mobilePoi.Chofer = mensaje != null ? mensaje.Chofer.Entidad.Descripcion : null;
                results.Add(mobilePoi);
            }

            return results.Count > 0 ? results.OrderBy(result => result.PuntoDeInteres).ThenBy(result => result.Distancia).ToList() : results;
        }

        #endregion
    }
}
