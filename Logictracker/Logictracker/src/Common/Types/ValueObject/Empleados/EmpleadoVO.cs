#region Usings

using System;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.ValueObject.Empleados
{
    [Serializable]
    public class EmpleadoVO
    {
        public EmpleadoVO(Empleado empleado)
        {
            Id = empleado.Id;
            Empresa = empleado.Empresa != null ? empleado.Empresa.RazonSocial : empleado.Linea != null ? empleado.Linea.Empresa.RazonSocial : string.Empty;
            Linea = empleado.Linea != null ? empleado.Linea.Descripcion : string.Empty;
            Legajo = empleado.Legajo;
            Descripcion = (empleado.Entidad != null)? empleado.Entidad.Descripcion: string.Empty;
            Cuil = (empleado.Entidad != null)?empleado.Entidad.Cuil: string.Empty;
            TipoDocumento = (empleado.Entidad != null)? empleado.Entidad.TipoDocumento : string.Empty;
            NroDocumento = (empleado.Entidad != null)?empleado.Entidad.NroDocumento : string.Empty;
            Antiguedad = empleado.Antiguedad;
            Art = empleado.Art;
            DomicilioCalle = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Descripcion :  string.Empty : string.Empty;
            DomicilioAltura = (empleado.Entidad != null)?(empleado.Entidad.Direccion != null)?empleado.Entidad.Direccion.Altura : 0 : 0;
            DomicilioPartido = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Partido :string.Empty: string.Empty;
            DomicilioProvincia = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Provincia :  string.Empty : string.Empty;
            DomicilioPais = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Pais : string.Empty : string.Empty;
            Falta = empleado.Falta;
            Licencia = (empleado.Licencia) ?? string.Empty;
            NroTarjeta = (empleado.Tarjeta != null)? empleado.Tarjeta.Numero : string.Empty;
            Transportista = empleado.Transportista != null ? empleado.Transportista.Descripcion : string.Empty;
        }

        public string Empresa { get; set; }
        public string Linea { get; set; }
        public string Legajo { get; set; }
        public string Descripcion { get; set; }
        public string Cuil { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public int Antiguedad { get; set; }
        public string Art { get; set; }
        public string DomicilioCalle { get; set; }
        public int DomicilioAltura { get; set; }
        public string DomicilioEsquina { get; set; }
        public string DomicilioEntreCalle { get; set; }
        public string DomicilioPartido { get; set; }
        public string DomicilioProvincia { get; set; }
        public string DomicilioPais { get; set; }
        public DateTime? Falta { get; set; }
        public int Id { get; set; }
        public string Licencia { get; set; }
        public string NroTarjeta { get; set; }
        public string Transportista { get; set; }
    }
}