#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObject.ControlDeCombustible
{
     [Serializable]
    public class ConciliacionVO
    {
         public ConciliacionVO(Movimiento mov)
            {
             Id = mov.Id;
             Fecha = mov.Fecha;
             Descripcion = mov.Observacion;
             TipoMov = mov.TipoMovimiento.Descripcion;
             Volumen = mov.Volumen;
            }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public double Volumen { get; set; }
        public string TipoMov { get; set; }
       
    }
}

