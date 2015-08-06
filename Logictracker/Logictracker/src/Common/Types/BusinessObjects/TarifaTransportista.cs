#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class TarifaTransportista : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Transportista Transportista { get; set; }
        public virtual Cliente Cliente { get; set; }
        private Double _TarifaTramoCorto { get; set; }
        private Double _TarifaTramoLargo { get; set; }        
        private Double _CostoPorBulto { get; set; }
        private Double _CostoPorHora { get; set; }
        private Double _CostoPorKm { get; set; }

        public virtual Double TarifaTramoCorto
        {
            get { return Math.Round(_TarifaTramoCorto, 2); }
            set { _TarifaTramoCorto = value; }
        }
        public virtual Double TarifaTramoLargo
        {
            get { return Math.Round(_TarifaTramoLargo, 2); }
            set { _TarifaTramoLargo = value; }
        }
        public virtual Double CostoPorBulto
        {
            get { return Math.Round(_CostoPorBulto, 2); }
            set { _CostoPorBulto = value; }
        }
        public virtual Double CostoPorHora
        {
            get { return Math.Round(_CostoPorHora, 2); }
            set { _CostoPorHora = value; }
        }
        public virtual Double CostoPorKm
        {
            get { return Math.Round(_CostoPorKm, 2); }
            set { _CostoPorKm = value; }
        }
    }
}
