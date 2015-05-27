using System;
using System.Globalization;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    [Serializable]
    public class Odometro : IAuditable, ISecurable, IHasInsumo
    {
        private ISet<TipoCoche> _tiposDeVehiculo;
        
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; } 

        #endregion

        #region IHasInsumo Members

        public virtual Insumo Insumo { get; set; }

        #endregion

        public virtual string Descripcion { get; set; }

        public virtual bool PorKm { get; set; }
        public virtual double ReferenciaKm { get; set; }
        public virtual bool PorTiempo { get; set; }
        public virtual int ReferenciaTiempo { get; set; }
        public virtual bool PorHoras { get; set; }
        public virtual double ReferenciaHoras { get; set; }
        public virtual bool EsIterativo { get; set; }
        public virtual double Alarma1Km { get; set; }
        public virtual int Alarma1Tiempo { get; set; }
        public virtual double Alarma1Horas { get; set; }

        public virtual byte Alarma1Red { get; set; }
        public virtual byte Alarma1Green { get; set; }
        public virtual byte Alarma1Blue { get; set; }
        public virtual string Alarma1RGB
        {
            get
            {
                var rr = String.Format("{0:x}", Alarma1Red);
                var gg = String.Format("{0:x}", Alarma1Green);
                var bb = String.Format("{0:x}", Alarma1Blue);

                var result = rr.Length == 2 ? rr : "0" + rr;

                result += gg.Length == 2 ? gg : "0" + gg;
                result += bb.Length == 2 ? bb : "0" + bb;

                return result;
            }
            set
            {
                var rrggbb = value;

                if (rrggbb.StartsWith("#")) rrggbb = rrggbb.Substring(1, rrggbb.Length - 1);

                if (value.Length != 6) return;

                Alarma1Red = byte.Parse(rrggbb.Substring(0, 2), NumberStyles.HexNumber);
                Alarma1Green = byte.Parse(rrggbb.Substring(2, 2), NumberStyles.HexNumber);
                Alarma1Blue = byte.Parse(rrggbb.Substring(4, 2), NumberStyles.HexNumber);
            }
        }

        public virtual double Alarma2Km { get; set; }
        public virtual int Alarma2Tiempo { get; set; }
        public virtual double Alarma2Horas { get; set; }

        public virtual byte Alarma2Red { get; set; }
        public virtual byte Alarma2Green { get; set; }
        public virtual byte Alarma2Blue { get; set; }
        public virtual string Alarma2Rgb
        {
            get
            {
                var rr = String.Format("{0:x}", Alarma2Red);
                var gg = String.Format("{0:x}", Alarma2Green);
                var bb = String.Format("{0:x}", Alarma2Blue);

                var result = rr.Length == 2 ? rr : "0" + rr;

                result += gg.Length == 2 ? gg : "0" + gg;
                result += bb.Length == 2 ? bb : "0" + bb;

                return result;
            }
            set
            {
                var rrggbb = value;

                if (rrggbb.StartsWith("#")) rrggbb = rrggbb.Substring(1, rrggbb.Length - 1);

                if (value.Length != 6) return;

                Alarma2Red = byte.Parse(rrggbb.Substring(0, 2), NumberStyles.HexNumber);
                Alarma2Green = byte.Parse(rrggbb.Substring(2, 2), NumberStyles.HexNumber);
                Alarma2Blue = byte.Parse(rrggbb.Substring(4, 2), NumberStyles.HexNumber);
            }
        }

        public virtual bool EsReseteable { get; set; }

        public virtual ISet<TipoCoche> TiposDeVehiculo { get { return _tiposDeVehiculo ?? (_tiposDeVehiculo = new HashSet<TipoCoche>()); } }

        public virtual void ClearVehicleTypes() { TiposDeVehiculo.Clear(); }

        public virtual void AddVehicleType(TipoCoche tipo) { TiposDeVehiculo.Add(tipo); }
    }
}
