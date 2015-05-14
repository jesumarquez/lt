using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Utils;
using System;

namespace Logictracker.Process.CicloLogistico.Distribucion
{
    public static class Tools
    {
        public static ViajeDistribucion InsertarEntrega(this ViajeDistribucion distribucion, int index, EntregaDistribucion entrega)
        {
            if (index < 0) index = 0;
            if (index < distribucion.Detalles.Count)
            {
                distribucion.Detalles.Insert(index, entrega);
            }
            else
            {
                distribucion.Detalles.Add(entrega);
            }
            entrega.Viaje = distribucion;
            return distribucion.Renumerar();
        }

        public static ViajeDistribucion RemoverEntrega(this ViajeDistribucion distribucion, int index)
        {
            if (index < 0 || index >= distribucion.Detalles.Count) return distribucion;
            distribucion.Detalles.RemoveAt(index);
            return distribucion.Renumerar();
        }

        public static ViajeDistribucion Reordenar(this ViajeDistribucion distribucion, int indexFrom, int indexTo)
        {
            var it = distribucion.Detalles[indexFrom];
            distribucion.Detalles.RemoveAt(indexFrom);
            distribucion.Detalles.Insert(indexTo, it);
            return distribucion.Renumerar();
        }
        public static ViajeDistribucion Renumerar(this ViajeDistribucion distribucion)
        {
            for (var i = 0; i < distribucion.Detalles.Count; i++)
            {
                distribucion.Detalles[i].Orden = i;
            }
            return distribucion;
        }
        public static ViajeDistribucion CalcularHorario(this ViajeDistribucion distribucion, int index)
        {
            return CalcularHorario(distribucion, index, null);
        }
        public static ViajeDistribucion CalcularHorario(this ViajeDistribucion distribucion, int index, Directions directions)
        {
            if (index < 0 || index >= distribucion.Detalles.Count || distribucion.Detalles.Count <= 1) return distribucion;

            var entregas = distribucion.Detalles;

            var previo = entregas[index == 0 ? 0 : index - 1];
            var entrega = entregas[index == 0 ? 1 : index];
            
            TimeSpan tiempoViaje;
            if (directions == null)
            {
                // si no tengo el recorrido hago distancia x velocidad promedio
                var distancia = Distancias.Loxodromica(previo.PuntoEntrega.ReferenciaGeografica.Latitude,
                                                       previo.PuntoEntrega.ReferenciaGeografica.Longitude,
                                                       entrega.PuntoEntrega.ReferenciaGeografica.Latitude,
                                                       entrega.PuntoEntrega.ReferenciaGeografica.Longitude);
                var velocidad = distribucion.GetVelocidadPromedio();

                tiempoViaje = TimeSpan.FromHours(distancia/velocidad);
            }
            else
            {
                // si tengo el recorrido tomo la duración
                tiempoViaje = directions.Legs[index == 0 ? 0 : index - 1].Duration;
            }
            if(index == 0)
            {
                previo.Programado = entrega.Programado;
            }
            entrega.Programado = previo.Programado.Add(tiempoViaje);

            // muevo todos las entregas posteriores
            for (var i = index; i < entregas.Count; i++)
            {
                entregas[i].Programado = entregas[i].Programado.Add(tiempoViaje);
            }

            return distribucion;
        }

        public static int GetVelocidadPromedio(this ViajeDistribucion distribucion)
        {
            if (distribucion.Vehiculo != null)
            {
                if (distribucion.Vehiculo.VelocidadPromedio > 0)
                {
                    return distribucion.Vehiculo.VelocidadPromedio;
                }
                if (distribucion.Vehiculo.TipoCoche.VelocidadPromedio > 0)
                {
                    return distribucion.Vehiculo.TipoCoche.VelocidadPromedio;
                }
            }
            else if (distribucion.Empresa != null && distribucion.Empresa.VelocidadPromedio > 0)
            {
                return distribucion.Empresa.VelocidadPromedio;
            }
            
            return 20;
            
        }
    }
}
