#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible.Exportador
{
    [Serializable]
    public class DispatchsViewVO
    {
        public int IdMovimiento { get; set; }
        public int IdCoche { get; set; }
        public double Volumen { get; set; }
        public string Interno { get; set; }
        public DateTime Fecha { get; set; }
        public string Patente { get; set; }
        public string CentroDeCostos { get; set; }
        public string Linea { get; set; }
        public string Empleado { get; set; }
        
        public DispatchsViewVO(Movimiento m)
        {
            IdMovimiento = m.Id;
            IdCoche = m.Coche.Id;
            Fecha = m.Fecha;
            Linea = m.Tanque.Linea.DescripcionCorta;
            Interno = m.Coche.Interno;
            CentroDeCostos = m.Coche.CentroDeCostos != null ? m.Coche.CentroDeCostos.Descripcion : " - ";
            Patente = m.Coche.Patente;
            Volumen = m.Volumen;
            Empleado = m.Empleado != null ? m.Empleado.Entidad.Descripcion : "Sin Empleado";
        }
    }
}
