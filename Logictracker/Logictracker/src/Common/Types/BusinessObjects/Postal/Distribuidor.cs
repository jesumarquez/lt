#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Postal
{
    /// <summary>
    /// Class thar represents a mail distributor.
    /// </summary>
    [Serializable]
    public class Distribuidor : IAuditable
    {
        #region Public Properties

        public virtual int Id { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Clave { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Nombre { get; set; }
        public virtual DateTime FechaModificacion { get; set; }
        public virtual DateTime? FechaBaja { get; set; }

        #endregion

        #region Public Methods

        public virtual Type TypeOf() { return GetType(); }

        public override Boolean Equals(Object obj)
        {
            var distribuitor = obj as Distribuidor;

            return distribuitor != null && distribuitor.Id.Equals(Id);
        }

        public override int GetHashCode() { return Id.GetHashCode(); }

        public override String ToString() { return Usuario; }

        #endregion
    }
}
