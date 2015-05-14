using System;
using Logictracker.Process.Geofences.Classes;

namespace Logictracker.Process.CicloLogistico
{
    [Serializable]
    public class EstadosEntrega
    {
        public EstadosEntrega(EstadosGeocerca estado)
        {
            Estado = estado;
        }
        /// <summary>
        /// The current state of the vehicle within the geo reference.
        /// </summary>
        public EstadosGeocerca Estado { get; private set; }

        /// <summary>
        /// Updates vehicle state.
        /// </summary>
        /// <param name="estado"></param>
        public void UpdateState(EstadosGeocerca estado) { Estado = estado; }
    }
}
