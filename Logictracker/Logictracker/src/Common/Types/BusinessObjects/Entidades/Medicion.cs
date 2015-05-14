using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class Medicion : IAuditable, IHasSensor, IHasTipoMedicion, IHasSubEntidad, IHasDispositivo
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime FechaAlta { get; set; }
        public virtual DateTime FechaMedicion { get; set; }
        public virtual string Valor { get; set; }
        public virtual double ValorNum1 { get; set; }
        public virtual double ValorNum2 { get; set; }
        public virtual TipoMedicion TipoMedicion { get; set; }
        public virtual SubEntidad SubEntidad { get; set; }
        public virtual Sensor Sensor { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }

        public override string ToString() { return Valor; }

        public virtual double ValorDouble
        {
            get { return ValorNum1; }
            set { ValorNum1 = value; }
        }
    }
}