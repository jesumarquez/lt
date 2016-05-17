using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    [Serializable]
    public class Contenedor: IAuditable, IHasTipoVehiculo
    {
        #region IAuditable
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        #endregion
        public virtual string Descripcion { get; set; }
        public virtual double Capacidad { get; set; }
        public virtual TipoCoche TipoCoche { get; set; }        
    }
}
