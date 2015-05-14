using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Utils;

namespace Logictracker.Process.CicloLogistico.Distribucion
{
    public class Intercalador
    {
        protected DAOFactory DaoFactory { get; set; }
        public IList<ViajeDistribucion> Viajes { get; set; }
        public List<Intercalado> Intercalados { get; set; }
        public bool[] Horas { get; set; }

        public Intercalador(DAOFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        public void Load(int empresa, int linea, DateTime desde, DateTime hasta, bool[] horas)
        {
            Horas = horas;
            Viajes = DaoFactory.ViajeDistribucionDAO.GetListActivos(empresa, linea, desde, hasta);
        }

        public void Load(List<Intercalado> intercalados, bool[] horas)
        {
            Intercalados = intercalados;
            Horas = horas;
            Viajes = intercalados.Select(v => DaoFactory.ViajeDistribucionDAO.FindById(v.Id)).ToList();
        }

        public void CalcularCostos(PuntoEntrega nuevoPunto, int radio)
        {
            if (Viajes == null)
            {
                throw new ApplicationException("Antes de intercalar hay que cargar los datos con el método Load");
            }

            var cercanos = Viajes.Where(v => v.Detalles.Any(
                d => Distancias.Loxodromica(d.ReferenciaGeografica.Latitude,
                                            d.ReferenciaGeografica.Longitude,
                                            nuevoPunto.ReferenciaGeografica.Latitude,
                                            nuevoPunto.ReferenciaGeografica.Longitude) <= radio));

            var result = cercanos.Select(c => CalcularIntercalado(c, nuevoPunto))
                .OrderBy(i => i.CostoKmExtra);

            var inter = new List<Intercalado>();
            foreach (var intercalado in result)
            {
                var i = CalcularDirections(intercalado, nuevoPunto);
                if(IsInHoras(i))inter.Add(i);
                if(inter.Count > 5) break;
            }
            Intercalados = inter.OrderBy(i => i.CostoKmExtra).ToList();
        }

        private Intercalado CalcularIntercalado(ViajeDistribucion viaje, PuntoEntrega nuevoPunto)
        {
            var alPunto = viaje.Detalles.Select(e => Distancias.Loxodromica(e.ReferenciaGeografica.Latitude,
                                                                       e.ReferenciaGeografica.Longitude,
                                                                       nuevoPunto.ReferenciaGeografica.Latitude,
                                                                       nuevoPunto.ReferenciaGeografica.Longitude)).ToArray();
            var deViaje = new List<double>();
            for (int i = 1; i < viaje.EntregasTotalCountConBases; i++)
            {
                var e = viaje.Detalles[i];
                var p = viaje.Detalles[i - 1];
                var distancia = Distancias.Loxodromica(p.ReferenciaGeografica.Latitude,
                                                       p.ReferenciaGeografica.Longitude,
                                                       e.ReferenciaGeografica.Latitude,
                                                       e.ReferenciaGeografica.Longitude);
                deViaje.Add(distancia);
            }

            var costoTotal = deViaje.Any() ? deViaje.Sum() : 0;

            var menorCosto = double.MaxValue;
            var posicion = 0;
            for(var i = 0; i <= viaje.EntregasTotalCountConBases; i++)
            {
                var costo = 0.0;
                if (i == 0)
                {
                    costo += costoTotal;
                    costo += alPunto[0];
                }
                else if(i == viaje.EntregasTotalCountConBases)
                {
                    costo += costoTotal;
                    costo += alPunto[viaje.EntregasTotalCountConBases - 1];
                }
                else
                {
                    costo = costoTotal - deViaje[i - 1];
                    costo += alPunto[i - 1];
                    costo += alPunto[i];
                }
                
                if(costo < menorCosto)
                {
                    menorCosto = costo;
                    posicion = i;
                }
            }
            return new Intercalado {Id = viaje.Id, Index = posicion, CostoKm = menorCosto, CostoKmOld = costoTotal};
        }

        private Intercalado CalcularDirections(Intercalado intercalado, PuntoEntrega nuevoPunto)
        {
            var i = CalcularDirectionsOriginal(intercalado);
            i = CalcularDirectionsIntercalado(i, nuevoPunto);
            return i;
        }

        private Intercalado CalcularDirectionsOriginal(Intercalado intercalado)
        {
            var viaje = Viajes.FirstOrDefault(v => v.Id == intercalado.Id);
            var first = viaje.Detalles.First().ReferenciaGeografica;
            var last = viaje.Detalles.Last().ReferenciaGeografica;
            var origen = new LatLon(first.Latitude, first.Longitude);
            var destino = new LatLon(last.Latitude, last.Longitude);
            var waypoints = viaje.Detalles.Skip(1).Take(viaje.Detalles.Count - 2).Select(w => new LatLon(w.ReferenciaGeografica.Latitude, w.ReferenciaGeografica.Longitude)).ToList();
            var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving,
                                           string.Empty, waypoints.ToArray());

            intercalado.ViajeAnterior = directions;
            if (directions != null)
            {
                intercalado.CostoKmOld = directions.Distance;
            }
            var duracionAnterior = viaje.Detalles.Last().Programado.Subtract(viaje.Detalles.First().Programado);
            intercalado.CostoMinOld = duracionAnterior.TotalMinutes;

            return intercalado;
        }

        private Intercalado CalcularDirectionsIntercalado(Intercalado intercalado, PuntoEntrega nuevoPunto)
        {
            var viaje = Viajes.FirstOrDefault(v => v.Id == intercalado.Id);

            var first = viaje.Detalles.First().ReferenciaGeografica;
            var origen = new LatLon(first.Latitude, first.Longitude);

            var last = viaje.Detalles.Last().ReferenciaGeografica;
            var destino = new LatLon(last.Latitude, last.Longitude);
            
            var waypoints = viaje.Detalles.Skip(1).Take(viaje.Detalles.Count - 2).Select(w => new LatLon(w.ReferenciaGeografica.Latitude, w.ReferenciaGeografica.Longitude)).ToList();

            var nuevaLatLon = new LatLon(nuevoPunto.ReferenciaGeografica.Latitude,
                                         nuevoPunto.ReferenciaGeografica.Longitude);

            if (intercalado.Index == 0)
            {
                waypoints.Insert(0, origen);
                origen = nuevaLatLon;
            }
            else if (intercalado.Index >= viaje.Detalles.Count)
            {
                waypoints.Add(destino);
                destino = nuevaLatLon;
            }
            else if (intercalado.Index == viaje.Detalles.Count - 1)
            {
                waypoints.Add(nuevaLatLon);
            }
            else
            {
                waypoints.Insert(intercalado.Index, nuevaLatLon);
            }
            
            var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving,
                                           string.Empty, waypoints.ToArray());

            intercalado.ViajeIntercalado = directions;
            if (directions != null)
            {
                intercalado.CostoKm = directions.Distance;
            }

            var nuevoDetalle = new EntregaDistribucion
                                   {
                                       PuntoEntrega = nuevoPunto,
                                       Cliente = nuevoPunto.Cliente,
                                       Descripcion = nuevoPunto.Descripcion
                                   };

            viaje.InsertarEntrega(intercalado.Index, nuevoDetalle);
            if (intercalado.Index > 0) viaje.CalcularHorario(intercalado.Index, intercalado.ViajeIntercalado);
            if (intercalado.Index < viaje.Detalles.Count) viaje.CalcularHorario(intercalado.Index, intercalado.ViajeIntercalado);

            var duracionNueva = viaje.Detalles.Last().Programado.Subtract(viaje.Detalles.First().Programado);

            intercalado.CostoMin = duracionNueva.TotalMinutes;

            intercalado.Hora = nuevoDetalle.Programado;

            viaje.RemoverEntrega(intercalado.Index);

            return intercalado;
        }

        private bool IsInHoras(Intercalado intercalado)
        {
            if(Horas.Count() != 24) return true;
            var h = intercalado.Hora.Hour;
            return Horas[h];
        }

        public Intercalado CambiarIndice(int idViaje, PuntoEntrega nuevoPunto, int nuevoIndice)
        {
            var viaje = Viajes.FirstOrDefault(v => v.Id == idViaje);
            if (viaje == null) return null;
            
            var intercalado = Intercalados.FirstOrDefault(i => i.Id == idViaje);
            if (intercalado == null) return null;

            intercalado.Index = nuevoIndice;
            CalcularDirectionsIntercalado(intercalado, nuevoPunto);

            return intercalado;
        }
    }
}
