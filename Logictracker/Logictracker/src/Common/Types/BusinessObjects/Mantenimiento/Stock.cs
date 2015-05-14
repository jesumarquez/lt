using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Mantenimiento
{
    [Serializable]
    public class Stock : IAuditable, IHasInsumo, IHasDeposito
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Deposito Deposito { get; set; }
        public virtual Insumo Insumo { get; set; }

        public virtual double Cantidad { get; set; }
        public virtual double CapacidadMaxima { get; set; }
        public virtual double PuntoReposicion { get; set; }
        public virtual double StockCritico { get; set; }
        public virtual bool AlarmaActiva { get; set; }
    }
}