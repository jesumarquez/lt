#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObject.ControlDeCombustible
{
    [Serializable]
    public class RemitoVO
    {
        public RemitoVO(Movimiento mov)
        {
            Id = mov.Id;
            Fecha = mov.Fecha;
            Volumen = mov.Volumen;
            Tanque = mov.Tanque.Descripcion;
            CentroDeCostos = mov.Tanque.Linea.Descripcion;
            NroRemito = mov.Observacion;
            DescriTipo = mov.TipoMovimiento.Descripcion;
        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public double Volumen { get; set; }
        public string Tanque { get; set; }
        public string CentroDeCostos { get; set; }
        public string NroRemito { get; set; }
// ReSharper disable UnusedAutoPopertyAccessor
        public string DescriTipo { get; set; }
// ReSharper restore UnusedAutoPopertyAccessor
    }
}