#region Usings

using Logictracker.Utils;

#endregion

namespace Logictracker.Model
{
    /// <summary>
    /// Agrega la caracteristica de incluir un punto GPS.
    /// </summary>
	public interface IGeoPoint : IMessage
    {
        /// <summary>
        /// Posicion GPS
        /// </summary>
        GPSPoint GeoPoint { get; set; }
    }
}