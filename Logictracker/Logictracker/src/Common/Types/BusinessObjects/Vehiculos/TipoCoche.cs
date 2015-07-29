#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

#endregion

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    [Serializable]
    public class TipoCoche : IAuditable, ISecurable
    {
        #region Private Properties

        private ISet<Odometro> _odometros;

        #endregion

        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion
        
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
        
        public virtual Icono IconoNormal { get; set; }
        public virtual Icono IconoAtraso { get; set; }
        public virtual Icono IconoAdelanto { get; set; }
        public virtual Icono IconoDefault { get; set; }

        public virtual int MaximaVelocidadAlcanzable { get; set; }
        public virtual double KilometrosDiarios { get; set; }
        public virtual int VelocidadPromedio { get; set; }

        public virtual bool ControlaKilometraje { get; set; }
        public virtual bool ControlaTurno { get; set; }
        public virtual bool SeguimientoPersona { get; set; }

        public virtual bool NoEsVehiculo { get; set; }

        public virtual int Capacidad { get; set; }
        public virtual int CapacidadCarga { get; set; }
        public virtual int DesvioMinimo { get; set; }
        public virtual int DesvioMaximo { get; set; }
        public virtual bool AlarmaConsumo { get; set; }

        public virtual ISet<Odometro> Odometros { get { return _odometros ?? (_odometros = new HashSet<Odometro>()); } }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var tipoCoche = obj as TipoCoche;

            if (tipoCoche == null) return false;

            return Id == tipoCoche.Id;
        }

        public override int GetHashCode() { return Id; }

        public override string ToString() { return Descripcion; }

        #endregion
    }
}
