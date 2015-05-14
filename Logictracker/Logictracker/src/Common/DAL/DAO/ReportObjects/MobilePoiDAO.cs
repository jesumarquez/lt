#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Utils;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Mobile Pois data access class.
    /// </summary>
    public class MobilePoiDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobilePoiDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        public IEnumerable<MobilePoi> GetMobileNearPois(IEnumerable<Coche> vehiculos, IEnumerable<int> idsReferenciasgeograficas, double distancia)
        {
            Watch.Start();
            var posicionesDao = DAOFactory.LogPosicionDAO;
            var mensajesDao = DAOFactory.LogUltimoLoginDAO;
            var referenciasGeograficasDao = DAOFactory.ReferenciaGeograficaDAO;

            var results = new List<MobilePoi>();
            
            var currentDrivers = mensajesDao.GetCurrentDrivers(vehiculos);

            foreach(var coche in vehiculos)
            {
                var posicion = posicionesDao.GetLastOnlineVehiclePosition(coche);
                if (posicion == null) continue;

                var idCoche = coche.Id;
                var referencias = referenciasGeograficasDao.GetGeocercasFor(coche);
                results.AddRange(referencias
                    .Where(g => Distancias.Loxodromica(posicion.Latitud, posicion.Longitud, g.Latitude, g.Longitude)<= distancia)
                    .Select(g => new MobilePoi(posicion, g, currentDrivers[idCoche]))
                    );
            }

            return results.OrderBy(result => result.PuntoDeInteres).ThenBy(result => result.Distancia).ToList();
        }
        /// <summary>
        /// Gets mobiles near the specified point
        /// </summary>
        /// <param name="coches"></param>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <param name="distancia"></param>
        /// <returns></returns>
        public List<MobilePoi> GetMobilesNearPoint(IEnumerable<Coche> coches, double latitud, double longitud, double distancia)
        {
            var lastPositions = DAOFactory.LogPosicionDAO.GetLastVehiclesPositions(coches);

            return (from position in lastPositions.Values where position != null &&
                        Distancias.Loxodromica(position.Latitud, position.Longitud, latitud, longitud) <= distancia
                    select new MobilePoi
                               {
                                   IdVehiculo = position.IdCoche,
                                   TipoVehiculo = position.TipoCoche,
                                   Interno = position.Coche,
                                   Distancia = Distancias.Loxodromica(position.Latitud, position.Longitud, latitud, longitud),
                                   Latitud = position.Latitud,
                                   Longitud = position.Longitud
                               }
                    ).OrderBy(position => position.Distancia).ToList();
        }

        #endregion
    }
}
