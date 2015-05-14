using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class DetalleValor : IHasEntidadPadre, IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual EntidadPadre Entidad { get; set; }
        public virtual Detalle Detalle { get; set; }

        public virtual double ValorNum { get; set; }
        public virtual string ValorStr { get; set; }
        public virtual DateTime? ValorDt { get; set; }
        public virtual bool Baja { get; set; }

        public override string ToString()
        {
            if (Detalle != null)
            {
                switch (Detalle.Tipo)
                {
                    case 1:
                        return ValorStr;
                    case 2:
                        return ValorNum.ToString();
                    case 3:
                        return ValorDt.HasValue ? ValorDt.Value.ToString() : string.Empty;
                }
            }

            return string.Empty;
        }
    }
}