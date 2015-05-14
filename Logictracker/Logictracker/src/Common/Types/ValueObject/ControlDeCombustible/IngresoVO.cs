#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObject.ControlDeCombustible
{
    [Serializable]
    public class IngresoVO
    {
        public IngresoVO(Movimiento mov)
        {
            Id = mov.Id;
            Fecha = mov.Fecha;
            Volumen = mov.Volumen;
            Tanque = mov.Tanque != null ? mov.Tanque.Descripcion : mov.Caudalimetro != null ? mov.Caudalimetro.Descripcion : "Sin Descripcion";
            NroRemito = mov.Observacion;
            DescriTipo = mov.TipoMovimiento.Descripcion;
            NombreEquipo = mov.Caudalimetro!= null? mov.Caudalimetro.Equipo != null ? mov.Caudalimetro.Equipo.Descripcion : "Sin Equipo" : "Sin Equipo";
            IDEquipo = mov.Caudalimetro != null ? mov.Caudalimetro.Equipo != null ? mov.Caudalimetro.Equipo.Id : 0 : 0;
        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public double Volumen { get; set; }
        public string Tanque { get; set; }
        public string NroRemito { get; set; }
        public string DescriTipo { get; set; }
        public string NombreEquipo { get; set; }
        public int IDEquipo { get; set; }
    }
}