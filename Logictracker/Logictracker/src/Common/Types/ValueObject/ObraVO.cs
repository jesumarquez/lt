#region Usings

using System;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.ValueObject
{
    [Serializable]
    public class ObraVO
    {
        #region Constructors

        /// <summary>
        /// Constructor basen on bussiness object.
        /// </summary>
        /// <param name="obra"></param>
        public ObraVO(Equipo obra)
        {
            Id = obra.Id;
            Codigo = obra.Codigo;
            Cliente = string.Concat(obra.Cliente.Codigo, " - ", obra.Cliente.Descripcion);
            Descripcion = obra.Descripcion;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Bussiness object id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Object code.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Client information.
        /// </summary>
        public string Cliente { get; set; }

        /// <summary>
        /// Object description.
        /// </summary>
        public string Descripcion { get; set; }

        #endregion
    }
}