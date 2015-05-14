using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;
using NHibernate.SqlCommand;
using NHibernate.Criterion;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public partial class LogMensajeDAO
    {
        public IList<LogMensaje> GetTourEvents(IEnumerable<int> vehiculos, string codigoInicio, string codigoFin, DateTime inicio, DateTime fin)
        {
            var vehiculosList = vehiculos.ToList();
            var mensajesList = new List<string> { codigoInicio, codigoFin };
            Mensaje m = null;
            return Session.QueryOver<LogMensaje>()
                .JoinAlias(ev => ev.Mensaje, () => m, JoinType.InnerJoin)
                .Where(ev => ev.Fecha >= inicio && ev.Fecha < fin)
                .And(Restrictions.InG(Projections.Property<LogMensaje>(ev => ev.Coche.Id), vehiculosList))
                .And(Restrictions.InG(Projections.Property<Mensaje>(ev => m.Codigo), mensajesList))
                .OrderBy(Projections.Property<LogMensaje>(ev => ev.Coche.Id)).Asc
                .ThenBy(Projections.Property<LogMensaje>(ev => ev.Fecha)).Asc
                .List<LogMensaje>();
        }
        public List<MobileTour> GetMobileTour(IEnumerable<int> vehiculos, string codigoInicio, string codigoFin, DateTime inicio, DateTime fin, TimeSpan duracion)
        {

            var results = GetTourEvents(vehiculos, codigoInicio, codigoFin, inicio, fin);

            var list = new List<MobileTour>(results.Count/2);

            var cocheDao = new CocheDAO();
            MobileTour tour = null;
            foreach(var result in results)
            {
                if (result.Mensaje.Codigo == codigoInicio && (tour == null || tour.IdMovil != result.Coche.Id))
                {
                    tour = new MobileTour
                    {
                        Entrada = result.Fecha,
                        IdMovil = result.Coche.Id,
                        Interno = cocheDao.FindById(result.Coche.Id).Interno,
                        LatitudInicio = result.Latitud,
                        LongitudInicio = result.Longitud
                    };
                }
                else if (result.Mensaje.Codigo == codigoFin && (tour != null && tour.IdMovil == result.Coche.Id))
                {
                    tour.Salida = result.Fecha;
                    tour.LatitudFin = result.Latitud;
                    tour.LongitudFin = result.Longitud;
                    if(tour.Duracion >= duracion) list.Add(tour);
                    tour = null;
                }
            }

            return list;
        }
        public List<MobileTour> GetMobileTour(IEnumerable<int> vehiculos, string codigoInicio, string codigoFin, DateTime inicio, DateTime fin, int radio, bool ocultarHuerfanos)
        {
            var results = GetTourEvents(vehiculos, codigoInicio, codigoFin, inicio, fin);

            var list = new List<MobileTour>(results.Count / 2);

            var cocheDao = new CocheDAO();
            MobileTour tour = null;
            foreach (var result in results)
            {
                var distancia = tour != null ? Distancias.Loxodromica(tour.LatitudInicio, tour.LongitudInicio, result.Latitud, result.Longitud) : 0;
                var dentro = distancia <= radio;
                if (tour == null || tour.IdMovil != result.Coche.Id || !dentro)
                {
                    if (result.Mensaje.Codigo == codigoInicio)
                    {
                        tour = new MobileTour
                        {
                            Entrada = result.Fecha.ToDisplayDateTime(),
                            IdMovil = result.Coche.Id,
                            Interno = cocheDao.FindById(result.Coche.Id).Interno,
                            LatitudInicio = result.Latitud,
                            LongitudInicio = result.Longitud
                        };
                        list.Add(tour);
                    }
                    else if (result.Mensaje.Codigo == codigoFin)
                    {
                        tour = new MobileTour
                        {
                            Salida = result.Fecha.ToDisplayDateTime(),
                            IdMovil = result.Coche.Id,
                            Interno = cocheDao.FindById(result.Coche.Id).Interno,
                            LatitudFin = result.Latitud,
                            LongitudFin = result.Longitud
                        };
                        list.Add(tour);
                    }
                }
                else if (result.Mensaje.Codigo == codigoFin && (tour.IdMovil == result.Coche.Id))
                {
                    tour.Salida = result.Fecha.ToDisplayDateTime();
                    tour.LatitudFin = result.Latitud;
                    tour.LongitudFin = result.Longitud;
                }
            }

            if (ocultarHuerfanos)
            {
                list = list.Where(t => t.Entrada != DateTime.MinValue && t.Salida != DateTime.MinValue).ToList();
            }

            return list;
        }

        [Serializable]
        public class MobileTour
        {
            public string Interno { get; set; }
            public DateTime Entrada { get; set; }
            public DateTime Salida { get; set; }
            public TimeSpan Duracion { get { return Salida.Subtract(Entrada); } }
            public int IdMovil { get; set; }
            public double LatitudInicio { get; set; }
            public double LongitudInicio { get; set; }
            public double LatitudFin { get; set; }
            public double LongitudFin { get; set; }
        }
    }
}
