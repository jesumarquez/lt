using Iesi.Collections;
using System;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class TipoReferenciaGeografica : IAuditable, ISecurable
    {
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
        public virtual Icono Icono { get; set; }
        public virtual RGBColor Color { get; set; }
        public virtual Vigencia Vigencia { get; set; }
        public virtual bool ControlaVelocidad { get; set; }
        public virtual bool ControlaEntradaSalida { get; set; }
        public virtual bool EsZonaDeRiesgo { get; set; }
        public virtual bool InhibeAlarma { get; set; }
        public virtual bool EsInicio { get; set; }
        public virtual bool EsFin { get; set; }
        public virtual bool EsIntermedio { get; set; }
        public virtual bool ControlaPermanencia { get; set; }
        public virtual bool ControlaPermanenciaEntrega { get; set; }
        public virtual int MaximaPermanencia { get; set; }
        public virtual int MaximaPermanenciaEntrega { get; set; }
        public virtual bool Baja { get; set; }
        public virtual bool ExcluirMonitor { get; set; }
        public virtual bool EsTaller { get; set; }
        public virtual bool EsControlAcceso { get; set; }

        private ISet<TipoReferenciaVelocidad> _velocidades;

        public TipoReferenciaGeografica()
        {
            Color = new RGBColor();
            Vigencia = new Vigencia();
        }

        public virtual ISet<TipoReferenciaVelocidad> VelocidadesMaximas
        {
            get { return _velocidades ?? (_velocidades = new HashSet<TipoReferenciaVelocidad>()); }
        }

        public virtual int GetVelocidadMaxima(TipoCoche tipoCoche) { return GetVelocidadMaxima(tipoCoche.Id); }

        protected virtual int GetVelocidadMaxima(int tipoCoche)
        {
            foreach (TipoReferenciaVelocidad maxima in VelocidadesMaximas) if (maxima.TipoVehiculo.Id == tipoCoche) return maxima.VelocidadMaxima;

            return 0;
        }
    }
}
