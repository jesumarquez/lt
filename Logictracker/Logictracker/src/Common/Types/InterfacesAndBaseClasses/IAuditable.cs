#region Usings

using System;

#endregion

namespace Logictracker.Types.InterfacesAndBaseClasses
{
    /// <summary>
    /// Class that must be implemented in order to apply audit processes to any object.
    /// </summary>
    public interface IAuditable
    {
        #region Properties

        /// <summary>
        /// Database Id of the current Object.
        /// </summary>
        int Id { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Must return Class Name of the current object.
        /// NOTE: This option avoids Proxy Problems when using Nhibernate.
        /// </summary>
        /// <returns></returns>
        Type TypeOf();

        #endregion
    }
}
