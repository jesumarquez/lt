using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ReportObjects;
using Logictracker.DAL.DAO.BusinessObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class RankingTransportistasDAO : ReportDAO
    {
        public RankingTransportistasDAO(DAOFactory daoFactory) : base(daoFactory) { }

        private ReportFactory _reportFactory;
        private ReportFactory ReportFactory { get { return _reportFactory ?? (_reportFactory = new ReportFactory(DAOFactory)); } }

        public List<RankingTransportistas> GetRanking(List<Int32> distritos, List<Int32> bases, List<Int32> transportistas, DateTime from, DateTime to)
        {
            var ranking = new List<RankingTransportistas>();

            var vehicles = DAOFactory.CocheDAO.GetList(distritos, bases, new[] { -1 }, transportistas, new[] { -1 });
            var infracciones = DAOFactory.InfraccionDAO.GetByVehiculos(vehicles.Select(v => v.Id), from, to);

            var kms = new List<MobilesKilometers>();
            var times = new List<MobilesTime>();

            if (to < DateTime.Today.ToDataBaseDateTime())
            {
                var dmDAO = new DatamartDAO();
                kms = dmDAO.GetMobilesKilometers(from, to, vehicles.Select(v => v.Id).ToList()).ToList();
                times = dmDAO.GetMobilesTimes(from, to, vehicles.Select(v => v.Id).ToList()).ToList();
            }

            foreach (var vehicle in vehicles)
            {
                var dmKm = kms.FirstOrDefault(dm => dm.Movil == vehicle.Id);
                var dmTime = times.FirstOrDefault(dm => dm.Movil == vehicle.Id);

                var km = dmKm != null ? dmKm.Kilometers : DAOFactory.CocheDAO.GetDistance(vehicle.Id, from, to);
                var hs = dmTime != null ? dmTime.ElapsedTime : DAOFactory.CocheDAO.GetRunningHours(vehicle.Id, from, to);

                var idTransportista = vehicle.Transportista != null ? vehicle.Transportista.Id : 0;
                var rankingTransportista = ranking.FirstOrDefault(d => d.IdTransportista == idTransportista);

                if (rankingTransportista != null)
                {
                    ranking.Remove(rankingTransportista);

                    rankingTransportista.Vehiculos++;
                    rankingTransportista.Kilometros += km;
                    rankingTransportista.Hours += hs;
                }
                else
                {
                    rankingTransportista = new RankingTransportistas
                                        {
                                            IdTransportista = idTransportista,
                                            Transportista = vehicle.Transportista != null ? vehicle.Transportista.Descripcion : "Sin Transportista",
                                            Kilometros = km,
                                            Hours = hs,
                                            Vehiculos = 1
                                        };
                }

                var messages = infracciones.Where(message => message.Vehiculo.Id.Equals(vehicle.Id)).ToList();

                foreach (var infraction in messages)
                {
                    var gravedad = GetGravedadInfraccion(infraction);

                    if (gravedad.Equals(0)) continue;

                    if (gravedad.Equals(1)) rankingTransportista.InfraccionesLeves++;
                    else if (gravedad.Equals(2)) rankingTransportista.InfraccionesMedias++;
                    else rankingTransportista.InfraccionesGraves++;

                    rankingTransportista.Puntaje += GetPonderacionInfraccion(infraction);
                }

                ranking.Add(rankingTransportista);
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