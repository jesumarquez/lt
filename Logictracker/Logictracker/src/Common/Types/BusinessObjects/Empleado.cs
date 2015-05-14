using System;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.BusinessObjects.ControlAcceso;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Empleado : IAuditable, IDataIdentify, ISecurable, IHasTipoEmpleado, IHasTransportista, IHasCentroDeCosto, IHasDepartamento, IHasCategoriaAcceso
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        public virtual string Telefono { get; set; }
        public virtual string Licencia { get; set; }
        public virtual DateTime? Vencimiento { get; set; }
        
        public virtual string Legajo { get; set; }
        public virtual DateTime? Falta { get; set; }
        public virtual int Antiguedad { get; set; }
        public virtual string Art { get; set; }
        public virtual Entidad Entidad { get; set; }
        public virtual bool Baja { get; set; }
        public virtual string Pin { get; set; }
        public virtual Tarjeta Tarjeta { get; set; }
        public virtual string Mail { get; set; }
        public virtual bool EsResponsable { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual TipoEmpleado TipoEmpleado { get; set; }
        public virtual Empleado Reporta1 { get; set; }
        public virtual Empleado Reporta2 { get; set; }
        public virtual Empleado Reporta3 { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual CentroDeCostos CentroDeCostos { get; set; }
        public virtual CategoriaAcceso Categoria { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var castObj = obj as Empleado;

            return (castObj != null) && (Id == castObj.Id) && (Id != 0);
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        #endregion

        [Serializable]
        public class VerificadorEmpleado : IHasZonaAcceso, IHasPuertaAcceso
        {   
            public enum TipoDeFichada
            {
                Entrada = 1,
                Salida = 2,
                SinFichar = 3
            }

            public ZonaAcceso ZonaAcceso { get; set; }
            public Empleado Empleado { get; set; }
            public PuertaAcceso PuertaAcceso { get; set; }
            public DateTime? Fecha { get; set; }
            public TipoDeFichada TipoFichada { get; set; }
        }
    }
}