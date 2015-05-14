using System;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    [Serializable]
    public class CocheOperacion : IDataIdentify, IAuditable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual DateTime FechaInicio { get; set; }
        public virtual double CostoKmHistorico { get; set; }
        public virtual double CostoKmUltimoMes { get; set; }
        public virtual double CostoKmUltimoAnio { get; set; }
        public virtual double CostoDiaHistorico { get; set; }
        public virtual double CostoUltimoMes { get; set; }
        public virtual double CostoMesHistorico { get; set; }
        public virtual double Rendimiento { get; set; }
        public virtual DateTime? UltimoControlConsumo { get; set; }
    }
}