using System.Collections;

namespace Urbetrack.QuadTree
{
    public interface ICustomIndex
    {
        /// <summary>
        /// Carga en la cache de nivel 1 la informacion asociada al punto dado.
        /// </summary>
        /// <param name="lat">Latitud</param>
        /// <param name="lon">Longitud</param>
        void LoadLevel1Cache(float lat, float lon);
        
        /// <summary>
        /// Descarga la cache de nivel 1.
        /// </summary>
        /// <remarks>Este metodo es llamado cuando el Repositorio libera todos los recursos en uso por
        /// el mismo.</remarks>
        void CommitLevel1Cache();

        TYPE GetReference<TYPE>(float lat, float lon, string name);

        void SetReference<TYPE>(float lat, float lon, string name, TYPE value);
        
        int GetPositionClass(float lat, float lon);

        void SetPositionClass(float lat, float lon, int value);

        IEnumerable Elements(float lat, float lon);


    
    }
}