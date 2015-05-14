#region Usings

using System.Xml.Linq;
using Logictracker.Model.EnumTypes;

#endregion

namespace Logictracker.Model.IAgent
{
    /// <summary>
    /// Define un mecanismo para establecer la configuracion del componente 
    /// durante la carga inicial y un para actualizar dicha configuracion
    /// durante su funcion principal.
    /// </summary>
    public interface ILoaderSettings
    {
    	/// <summary>
    	/// Configura un parametro.
    	/// </summary>
    	/// <param name="xElement">Instancia de <c>Setting</c> con la informacion del parametro.</param>
    	/// <param name="object"></param>
        /// <returns>Retorna el resultado de la carga segun la enumeracion <c>Logictracker.LoadResults</c></returns>
    	LoadResults LoadSetting(XElement xElement, object @object);
    }
}
