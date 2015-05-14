#region Usings

using System;

#endregion

#pragma warning disable 1574

namespace Logictracker.Model.IAgent
{
    /// <summary>
    /// Especifica que el componente necesita que este instanciado el 
    /// el tipo de servicio especificado y desencadena que el loader
    /// por medio de la interface <c>Logictracker.Model.ILoaderSetting</c>
    /// estableza la referencia entre servicios.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceRequiredAttribute : Attribute
    {
        /// <summary>
        /// Obtiene el tipo de servicio que se require.
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        /// Esteblece el atributo, en base al tipo.
        /// </summary>
        /// <param name="serviceType">Tipo de servicio requerido</param>
        /// 
        public ServiceRequiredAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }
    }
}
