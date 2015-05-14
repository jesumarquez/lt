#region Usings

using System;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.ValueObject.Empleados
{
    [Serializable]
    public class TarjetaVo
    {
        public TarjetaVo(Tarjeta tarjeta, Empleado empleado)
        {
            Id = tarjeta.Id;
            Numero = tarjeta.Numero;
            Pin = tarjeta.Pin;
            Chofer = empleado != null ? empleado.Entidad != null ? empleado.Entidad.Descripcion : "Desconocido" : "Sin Asignar";
        }

        public int Id { get; set; }
        public string Numero { get; set; }
        public string Pin { get; set; }
        public string Chofer { get; set; }
    }
}