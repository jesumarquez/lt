#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObject.ControlDeCombustible
{
    [Serializable]
    public class DespachoVO
    {
        public DespachoVO(Movimiento mov)
        {
            Id = mov.Id;
            Fecha = mov.Fecha;
            Volumen = mov.Volumen;
            Tanque = mov.Tanque.Descripcion;
            Patente = mov.Coche.Patente;
            CentroDeCostos = mov.Coche.CentroDeCostos != null ? mov.Coche.CentroDeCostos.Descripcion : "Sin Centro Asignado";
            Operador = mov.Empleado != null ? mov.Empleado.Entidad.Descripcion : "No Identificado";
            InternoVehiculo = mov.Coche != null ? mov.Coche.Interno : "Sin Asignar";
            LineaVehiculo = mov.Coche != null ? mov.Coche.Linea != null ? mov.Coche.Linea.Descripcion : "Sin Base" : "Sin Base";
            EmpresaVehiculo = mov.Coche != null
                                  ? mov.Coche.Empresa != null ? mov.Coche.Empresa.RazonSocial : "Sin Distrito"
                                  : "Sin Empresa";
            Vehiculo = String.Concat(InternoVehiculo, " (Distrito: ",EmpresaVehiculo," - Base: ", LineaVehiculo,")");
            CentroDeCarga = mov.Tanque != null
                                ? mov.Tanque.Linea != null ? mov.Tanque.Linea.Descripcion : "Sin Centro"
                                : "Sin Centro";
        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public double Volumen { get; set; }
        public string Tanque { get; set; }
        public string Operador { get; set; }
        public string InternoVehiculo { get; set; }
        public string LineaVehiculo { get; set; }
        public string EmpresaVehiculo { get; set; }
        public string Patente { get ;set; }
        public string CentroDeCostos { get; set; }
        public string Vehiculo { get; set; }
        public string CentroDeCarga { get; set; }
    }
}