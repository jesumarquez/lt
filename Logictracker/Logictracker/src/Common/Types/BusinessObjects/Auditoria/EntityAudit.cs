#region Usings

using System;
using System.Xml;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Auditoria
{
    /// <summary>
    /// Class that handles audit information about changes made into bussiness objects.
    /// </summary>
    [Serializable]
    public class EntityAudit : IAuditable
    {
        #region Public Properties

        public virtual Int32 Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual String Tabla { get; set; }
        public virtual Int32 AuditedId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual XmlDocument Data { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the type of the current object.
        /// </summary>
        /// <returns></returns>
        public virtual Type TypeOf() { return GetType(); }

        #endregion
    }
}
