#region Usings

using System;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class ReferenciaGeografica : IAuditable, ISecurable, IHasTipoReferenciaGeografica
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }
        public virtual TipoReferenciaGeografica TipoReferenciaGeografica { get; set; }
        public virtual Icono Icono { get; set; }
        public virtual RGBColor Color { get; set; }
        public virtual Vigencia Vigencia { get; set; }
        public virtual bool InhibeAlarma { get; set; }
        public virtual bool EsInicio { get; set; }
        public virtual bool EsFin { get; set; }
        public virtual bool EsIntermedio { get; set; }
        public virtual bool Baja { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual bool IgnoraLogiclink { get; set; }

        private ISet _zonas;
        private ISet _historia;
        private ISet _velocidades;

        public virtual ISet Zonas { get { return _zonas ?? (_zonas = new ListSet()); } }

        public virtual ISet Historia { get { return _historia ?? (_historia = new ListSet()); } }

        public ReferenciaGeografica()
        {
            Color = new RGBColor();
            Vigencia = new Vigencia();
        }

        public virtual ISet VelocidadesMaximas { get { return _velocidades ?? (_velocidades = new ListSet()); } }

        public virtual HistoriaGeoRef GetHistoria(DateTime date)
        {
            return Historia.Count == 0 ? null : Historia.Cast<HistoriaGeoRef>().LastOrDefault(h => h.Vigencia == null || h.Vigencia.Vigente(date));
        }

        public virtual Direccion Direccion
        {
            get
            {
                if (_historiaActual == null) _historiaActual = GetHistoria(DateTime.UtcNow);

                return _historiaActual != null ? _historiaActual.Direccion : null;
            }
        }

        public virtual double Latitude { get { return Direccion != null ? Direccion.Latitud : Poligono != null ? Poligono.Centro.Y : 0; } }

        public virtual double Longitude { get { return Direccion != null ? Direccion.Longitud : Poligono != null ? Poligono.Centro.X : 0; } }

        private HistoriaGeoRef _historiaActual;

        public virtual Poligono Poligono
        {
            get
            {
                if (_historiaActual == null) _historiaActual = GetHistoria(DateTime.UtcNow);

                return _historiaActual != null ? _historiaActual.Poligono : null;
            }
        }

        public virtual void AddHistoria(Direccion dir, Poligono pol, DateTime? desde)
        {
            AddHistoria(dir, pol, desde, null);
        }
        public virtual void AddHistoria(Direccion dir, Poligono pol, DateTime? desde, DateTime? hasta)
        {
            var vigencia = new Vigencia();
            if (desde.HasValue) vigencia.Inicio = desde.Value;
            if (hasta.HasValue) vigencia.Fin = hasta.Value;
            
            var historia = from HistoriaGeoRef h in Historia orderby h.Vigencia != null ? h.Vigencia.Inicio : DateTime.MinValue select h;

            foreach (var h in historia)
            {
                if (h.Vigencia == null) h.Vigencia = new Vigencia();
                if (desde.HasValue && h.Vigencia.Vigente(desde.Value)) h.Vigencia.Fin = desde.Value;
                else if (hasta.HasValue && h.Vigencia.Vigente(hasta.Value)) h.Vigencia.Inicio = hasta.Value;
            }

            var newHistoria = new HistoriaGeoRef { ReferenciaGeografica = this, Direccion = dir, Poligono = pol, Vigencia = vigencia };

            Historia.Add(newHistoria);
        }

        public virtual int GetVelocidadMaxima(TipoCoche tipoCoche) { return GetVelocidadMaxima(tipoCoche.Id); }

        protected virtual int GetVelocidadMaxima(int tipoCoche)
        {
            foreach (var maxima in VelocidadesMaximas.Cast<ReferenciaVelocidad>().Where(maxima => maxima.TipoVehiculo.Id.Equals(tipoCoche))) return maxima.VelocidadMaxima;

            var referencias = TipoReferenciaGeografica != null ? TipoReferenciaGeografica.VelocidadesMaximas : null;

            return referencias == null ? 0 : (from TipoReferenciaVelocidad referencia in referencias where referencia.TipoVehiculo.Id.Equals(tipoCoche) select referencia.VelocidadMaxima).FirstOrDefault();
        }
    }
}
