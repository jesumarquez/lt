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
            if (!Odometro.EsIterativo && UltimoDisparo.HasValue) return true;
            return (Odometro.PorKm && Kilometros + AjusteKilometros > Odometro.ReferenciaKm)
                || (Odometro.PorTiempo && Dias + AjusteDias > Odometro.ReferenciaTiempo)
                || (Odometro.PorHoras && Horas + AjusteHoras > Odometro.ReferenciaHoras);
        }

        public virtual bool SuperoPrimerAviso()
        {
            if (FechaPrimerAviso.HasValue) return true;
            return (Odometro.PorKm && Kilometros + AjusteKilometros > Odometro.Alarma1Km)
                || (Odometro.PorTiempo && Dias + AjusteDias > Odometro.Alarma1Tiempo)
                || (Odometro.PorHoras && Horas + AjusteHoras > Odometro.Alarma1Horas);
        }

        public virtual bool SuperoSegundoAviso()
        {
            if (FechaSegundoAviso.HasValue) return true;
            return (Odometro.PorKm && Kilometros + AjusteKilometros > Odometro.Alarma2Km)
                || (Odometro.PorTiempo && Dias + AjusteDias > Odometro.Alarma2Tiempo)
                || (Odometro.PorHoras && Horas + AjusteHoras > Odometro.Alarma2Horas);
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