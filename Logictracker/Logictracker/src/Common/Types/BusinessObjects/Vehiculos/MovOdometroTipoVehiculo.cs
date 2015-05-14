#region Usings

using System;

#endregion

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    /// <summary>
    /// Class that represents a relationship between a vehicle type and a odometer.
    /// </summary>
    [Serializable]
    public class MovOdometroTipoVehiculo
    {
        public virtual int Id { get; set; }

        public virtual TipoCoche TipoVehiculo { get; set; }
        public virtual Odometro Odometro { get; set; }
    }
}