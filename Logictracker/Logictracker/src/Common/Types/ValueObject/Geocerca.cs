using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;

namespace Logictracker.Types.ValueObject
{
    /// <summary>
    /// Class for caching geo refference info.
    /// </summary>
    [Serializable]
    public class Geocerca: IDataIdentify, IEquatable<Geocerca>
    {
        #region Constructors

        public Geocerca() { }

        public Geocerca(ReferenciaGeografica geocerca)
        {
            Id = geocerca.Id;
            TipoReferenciaGeograficaId = geocerca.TipoReferenciaGeografica.Id;
            Descripcion = geocerca.Descripcion;

            Inicio = geocerca.Vigencia != null ? geocerca.Vigencia.Inicio : null;
            Fin = geocerca.Vigencia != null ? geocerca.Vigencia.Fin : null;

            Latitude = geocerca.Latitude;
            Longitude = geocerca.Longitude;

            ControlaEntradaSalida = geocerca.TipoReferenciaGeografica.ControlaEntradaSalida;
            ControlaVelocidad = geocerca.TipoReferenciaGeografica.ControlaVelocidad;

            ControlaPermanencia = geocerca.TipoReferenciaGeografica.ControlaPermanencia;
            ControlaPermanenciaEntrega = geocerca.TipoReferenciaGeografica.ControlaPermanenciaEntrega;

            MaximaPermanencia = geocerca.TipoReferenciaGeografica.MaximaPermanencia;
            MaximaPermanenciaEntrega = geocerca.TipoReferenciaGeografica.MaximaPermanenciaEntrega;

            EsTaller = geocerca.TipoReferenciaGeografica.EsTaller;

            EsInicio = geocerca.EsInicio;
            EsIntermedio = geocerca.EsIntermedio;
            EsFin = geocerca.EsFin;

            var zona = geocerca.Zonas.Cast<ReferenciaZona>().FirstOrDefault(x => !x.Zona.Baja);
            ZonaManejo = zona != null ? zona.Zona.Id : 0;
            PrioridadZona = zona != null ? zona.Zona.Prioridad : 0;

            VelocidadesMaximas = CalculateMaxSpeeds(geocerca);
            VelocidadesMaximasTipo = CalculateTypeMaxSpeeds(geocerca);

            HasPoligono = geocerca.Poligono != null;

            if (geocerca.Poligono == null) return;

            Radio = geocerca.Poligono.Radio;

            Puntos = geocerca.Poligono.ToPointFList();

            MinY = geocerca.Poligono.MinY;
            MaxY = geocerca.Poligono.MaxY;
            MinX = geocerca.Poligono.MinX;
            MaxX = geocerca.Poligono.MaxX;

            CalculateBounds();
        }

        public void Calculate(Direccion dire, Poligono poly)
        {
            if (dire == null &&
                poly == null)
            {
                Latitude = 0;
                Longitude = 0;
                return;
            }

            if (dire != null)
            {
                Latitude = dire.Latitud;
                Longitude = dire.Longitud;               
            }

            HasPoligono = (poly != null);
            if (poly != null)
            {
                if (dire == null)
                {
                    Latitude = poly.Centro.Y;
                    Longitude = poly.Centro.X;
                }
                Puntos = poly.ToPointFList();
                Radio = poly.Radio;
                MinY = poly.MinY;
                MaxY = poly.MaxY;
                MinX = poly.MinX;
                MaxX = poly.MaxX;
                CalculateBounds();
            }            
        }
        #endregion

        #region Private Properties

        private int Radio { get; set; }

        private List<PointF> Puntos { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
       
        public double MinY { get; set; }

        public double MaxY { get; set; }

        public double MinX { get; set; }

        public double MaxX { get; set; }

        private double InnerMinY { get; set; }
        private double InnerMaxY { get; set; }
        private double InnerMinX { get; set; }
        private double InnerMaxX { get; set; }

        public Dictionary<int, int> VelocidadesMaximas { get; set; }
        public Dictionary<int, int> VelocidadesMaximasTipo { get; set; }

        #endregion

        #region Public Properties

        public int Id { get; set; }
        public int TipoReferenciaGeograficaId { get; private set; }
        public int? HistoriaGeoId { get; private set; }
        public int? PoligonoId { get; private set; }
        public int? DireccionId { get; private set; }

        public string Descripcion { get; private set; }

        public bool ControlaEntradaSalida { get; private set; }
        public bool ControlaVelocidad { get; private set; }
        public bool ControlaPermanencia { get; private set; }
        public bool ControlaPermanenciaEntrega { get; private set; }
        public int MaximaPermanencia { get; private set; }
        public int MaximaPermanenciaEntrega { get; private set; }
        public bool EsTaller { get; private set; }
        public bool EsInicio { get; private set; }
        public bool EsIntermedio { get; private set; }
        public bool EsFin { get; private set; }
        public int ZonaManejo { get; private set; }
        public int PrioridadZona { get; private set; }

        public bool HasPoligono { get; private set; }
        public DateTime? Inicio { get; private set; }
        public DateTime? Fin { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the max speed by vehicle type associated to the geo reference.
        /// </summary>
        /// <param name="geocerca"></param>
        /// <returns></returns>
        private static Dictionary<int, int> CalculateMaxSpeeds(ReferenciaGeografica geocerca)
        {
            return geocerca.VelocidadesMaximas.OfType<ReferenciaVelocidad>().ToDictionary(velocidad => velocidad.TipoVehiculo.Id, velocidad => velocidad.VelocidadMaxima);
        }

        /// <summary>
        /// Gets the max speed by vehicle type associated to the geo reference type.
        /// </summary>
        /// <param name="geocerca"></param>
        /// <returns></returns>
        private static Dictionary<int, int> CalculateTypeMaxSpeeds(ReferenciaGeografica geocerca)
        {
            return geocerca.TipoReferenciaGeografica.VelocidadesMaximas.OfType<TipoReferenciaVelocidad>()
                .ToDictionary(velocidad => velocidad.TipoVehiculo.Id, velocidad => velocidad.VelocidadMaxima);
        }        

        /// <summary>
        /// Calculetas the current reference inner bounds.
        /// </summary>
        private void CalculateBounds()
        {
            if (Radio <= 0) return;

            var box = Radio/((111200*1.414235)/110000.0);

            InnerMinX = Puntos[0].X - box;
            InnerMaxX = Puntos[0].X + box;
            InnerMinY = Puntos[0].Y - box;
            InnerMaxY = Puntos[0].Y + box;
        }

        /// <summary>
        /// Determines if the specified point is within the inner bounds of the geo refference.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private bool IsInInnerBounds(PointF pt)
        {
            if (Radio <= 0) return false;

            return pt.X > InnerMinX && pt.X < InnerMaxX && pt.Y > InnerMinY && pt.Y < InnerMaxY;
        }

        /// <summary>
        /// Determines if the geo reference contains the givenn point.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private bool Contains(PointF pt)
        {
            var isIn = false;

            if (!HasPoligono) return false;

            if (IsInBounds(pt))
            {
                if (Radio > 0)
                {
                    if (IsInInnerBounds(pt)) return true;

                    var distancia = Distancias.Loxodromica(Puntos[0].Y, Puntos[0].X, pt.Y, pt.X);

                    isIn = distancia <= Radio;
                }
                else
                {

                    var pts = Puntos;

                    int i, j;

                    for (i = 0, j = pts.Count - 1; i < pts.Count; j = i++)
                    {
                        if ((((pts[i].Y <= pt.Y) && (pt.Y < pts[j].Y)) || ((pts[j].Y <= pt.Y) && (pt.Y < pts[i].Y)) ) &&
                            (pt.X < (pts[j].X - pts[i].X)*(pt.Y - pts[i].Y)/(pts[j].Y - pts[i].Y) + pts[i].X))
                        {
                            isIn = !isIn;
                        }
                    }
                }
            }

            return isIn;
        }

        #endregion

        #region Public Methods

        public bool IsVigente()
        {
            return IsVigente(DateTime.UtcNow);
        }

        public bool IsVigente(DateTime fecha)
        {
            if (Inicio.HasValue && fecha < Inicio.Value) return false;

            if (Fin.HasValue && fecha > Fin.Value) return false;

            return true;
        }

        /// <summary>
        /// Determines if the givenn point is within the bounds of the reference.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsInBounds(PointF pt)
        {
            if (MinX == MaxX || MinY == MaxY) return true;
            if (pt.X < MinX || pt.X > MaxX) return false;

            return pt.Y >= MinY && pt.Y <= MaxY;
        }

        /// <summary>
        /// Determines if the geoference contains the specified latitude and longitude.
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        public bool Contains(double latitud, double longitud) { return Contains(new PointF((float) longitud, (float) latitud)); }

        /// <summary>
        /// Gets the max speed associated to the givenn vehicle type id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetVelocidadMaxima(int id)
        {
            if (VelocidadesMaximas != null && VelocidadesMaximas.ContainsKey(id)) return VelocidadesMaximas[id];

            if (VelocidadesMaximasTipo != null && VelocidadesMaximasTipo.ContainsKey(id)) return VelocidadesMaximasTipo[id];

            return 0;
        }

        #endregion

        public bool Equals(Geocerca other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Geocerca)) return false;
            return Equals((Geocerca) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Geocerca left, Geocerca right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Geocerca left, Geocerca right)
        {
            return !Equals(left, right);
        }
    }
}
