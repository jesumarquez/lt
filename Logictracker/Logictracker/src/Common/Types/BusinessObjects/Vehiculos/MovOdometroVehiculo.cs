using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    [Serializable]
    public class MovOdometroVehiculo : IAuditable
    {
        public virtual int Id { get; set; }

        public virtual Coche Vehiculo { get; set; }
        public virtual Odometro Odometro { get; set; }

        public virtual double Kilometros { get; set; }
        public virtual double AjusteKilometros { get; set; }
        public virtual int Dias { get; set; }
        public virtual int AjusteDias { get; set; }
        public virtual double Horas { get; set; }
        public virtual double AjusteHoras { get; set; }

        public virtual DateTime? UltimoUpdate { get; set; }
        public virtual DateTime? UltimoDisparo { get; set; }

        public virtual DateTime? FechaPrimerAviso { get; set; }
        public virtual DateTime? FechaSegundoAviso { get; set; }
        
        public virtual bool Vencido()
        {
            if (!Odometro.EsIterativo && UltimoDisparo.HasValue) return false;
            return (Odometro.PorKm && Kilometros > Odometro.ReferenciaKm + AjusteKilometros) 
                || (Odometro.PorTiempo && Dias > Odometro.ReferenciaTiempo + AjusteDias)
                || (Odometro.PorHoras && Horas > Odometro.ReferenciaHoras + AjusteHoras);
        }

        public virtual bool SuperoPrimerAviso()
        {
            if (FechaPrimerAviso.HasValue) return false;
            return (Odometro.PorKm && Kilometros > Odometro.Alarma1Km + AjusteKilometros) 
                || (Odometro.PorTiempo && Dias > Odometro.Alarma1Tiempo + AjusteDias)
                || (Odometro.PorHoras && Horas > Odometro.Alarma1Horas + AjusteHoras);
        }

        public virtual bool SuperoSegundoAviso()
        {
            if (FechaSegundoAviso.HasValue) return false;
            return (Odometro.PorKm && Kilometros > Odometro.Alarma2Km + AjusteKilometros)
                || (Odometro.PorTiempo && Dias > Odometro.Alarma2Tiempo + AjusteDias)
                || (Odometro.PorHoras && Horas > Odometro.Alarma2Horas + AjusteHoras);
        }

        public virtual void ResetOdometerValues()
        {
            Kilometros = AjusteKilometros = Horas = AjusteHoras = Dias = AjusteDias =  0;
            FechaPrimerAviso = null;
            FechaSegundoAviso = null;
        }
        
        public override bool Equals(object obj)
        {
            var odometro = obj as MovOdometroVehiculo;

            if (odometro == null) return false;

            var equalsVehicles = Vehiculo == null ? odometro.Vehiculo == null : odometro.Vehiculo != null && Vehiculo.Id.Equals(odometro.Vehiculo.Id);

            return equalsVehicles && Odometro.Id.Equals(odometro.Odometro.Id);
        }

        public override int GetHashCode() { return (Vehiculo == null ? 0 : Vehiculo.Id.GetHashCode()) + Odometro.Id.GetHashCode(); }

        public virtual Type TypeOf() { return GetType(); }
    }
}