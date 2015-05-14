#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class ReferenciaVelocidad : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual TipoCoche TipoVehiculo { get; set; }
        public virtual int VelocidadMaxima { get; set; }
    }
}