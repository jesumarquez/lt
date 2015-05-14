using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class RankingVehiculosDAO : ReportDAO
    {
        public RankingVehiculosDAO(DAOFactory daoFactory) : base(daoFactory) {}

        private ReportFactory _reportFactory;
        private ReportFactory ReportFactory { get { return _reportFactory ?? (_reportFactory = new ReportFactory(DAOFactory)); } }

        public List<RankingVehiculos> GetRanking(List<Int32> distritos, List<Int32> bases, List<Int32> transportistas, List<Int32> tipos, List<Int32> centros, DateTime from, DateTime to)
        {
            var ranking = new List<RankingVehiculos>();

            var vehicles = DAOFactory.CocheDAO.GetList(distritos, bases, tipos, transportistas, centros);
            //var mensajes = new List<string> {MessageCode.SpeedingTicket.GetMessageCode()};
            
            //var vehiclesMessages = DAOFactory.LogMensajeDAO.GetByVehiclesAndCodes(vehicles.Select(v => v.Id).ToList(), mensajes, from, to);
            var infracciones = DAOFactory.InfraccionDAO.GetByVehiculos(vehicles.Select(v => v.Id), from, to);

            foreach (var vehicle in vehicles)
            {
                var km = DAOFactory.CocheDAO.GetDistance(vehicle.Id, from, to);
                var hs = DAOFactory.CocheDAO.GetRunningHours(vehicle.Id, from, to);

                var vehicleRanking = new RankingVehiculos
                                        {
                                            IdVehiculo = vehicle.Id,
                                            Patente = vehicle.Patente,
                                            Vehiculo = vehicle.Interno,
                                            Kilometros = km,
                                            Hours = hs
                                        };

                var messages = infracciones.Where(message => message.Vehiculo.Id.Equals(vehicle.Id)).ToList();

                foreach (var infraction in messages)
                {
                    var gravedad = GetGravedadInfraccion(infraction);

                    if (gravedad.Equals(0)) continue;

                    if (gravedad.Equals(1)) vehicleRanking.InfraccionesLeves++;
                    else if (gravedad.Equals(2)) vehicleRanking.InfraccionesMedias++;
                    else vehicleRanking.InfraccionesGraves++;

                    vehicleRanking.Puntaje += GetPonderacionInfraccion(infraction);
                }

                ranking.Add(vehicleRanking);
            }

            return ranking;
        }

        private double GetPonderacionInfraccion(Infraccion infraction)
        {
            return (ReportFactory.PuntajeExcesoVelocidadDAO.GetSpeedingPoints(infraction)*ReportFactory.PuntajeExcesoTiempoDAO.GetDurationPoints(infraction))/1000.0;
        }

        private static int GetGravedadInfraccion(Infraccion infraction)
        {
            if (infraction.Permitido == 0 || infraction.Alcanzado == 0) return 0;

            var difference = infraction.Alcanzado - infraction.Permitido;

            return difference <= 9 ? 1 : difference > 9 && difference <= 17 ? 2 : 3;
        }
    }
}