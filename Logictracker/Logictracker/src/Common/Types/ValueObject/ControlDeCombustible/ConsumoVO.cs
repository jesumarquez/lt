#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObject.ControlDeCombustible
{
    [Serializable]
    public class ConsumoVO
    {
        public ConsumoVO(Movimiento mov)
        {
            Id = mov.Id;
            Fecha = mov.Fecha;
            Volumen = mov.Volumen;
            Motor = mov.Caudalimetro.Descripcion;
            CentroDeCostos = mov.Caudalimetro.Equipo.Descripcion;
            Caudal = mov.Caudal;
            RPM = mov.RPM;
            HsEnMarcha = mov.HsEnMarcha;
            IDCaudalimetro = mov.Caudalimetro.Id;
        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public double Volumen { get; set; }
        public string Motor { get; set; }
        public string CentroDeCostos { get; set; }
        public Double Caudal { get; set; }
        public Double RPM { get; set; }
        public Double HsEnMarcha { get; set; }
        public int IDCaudalimetro { get; set; }
    }
}