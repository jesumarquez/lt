using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Mantenimiento
{
    [Serializable]
    public class ConsumoDetalle : IAuditable, IHasConsumoCabecera, IHasInsumo
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual double Cantidad { get; set; }
        public virtual double ImporteUnitario { get; set; }
        public virtual double ImporteTotal { get; set; }

        public virtual Insumo Insumo { get; set; }
        public virtual ConsumoCabecera ConsumoCabecera { get; set; }
    }
}