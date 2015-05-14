#region Usings

using System.Collections.Generic;
using Logictracker.Utils;

#endregion

namespace Logictracker.Model
{
    /// <summary>
    /// El IMessage incluye una lista de puntos GPS.
    /// </summary>
    public interface IGeoMultiPoint : IMessage
    {
        /// <summary>
        /// Lista de posiciones GPS.
        /// </summary>
        List<GPSPoint> GeoPoints { get; }
    }
}