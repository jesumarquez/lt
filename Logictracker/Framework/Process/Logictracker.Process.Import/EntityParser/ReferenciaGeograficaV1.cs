using System;
using System.Collections.Generic;
using System.Drawing;
using Geocoder.Core.VO;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using System.Globalization;

namespace Logictracker.Process.Import.EntityParser
{
    public class ReferenciaGeograficaV1 : EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Referencia Geografica"; }
        }

        public ReferenciaGeograficaV1()
        {
        }

        public ReferenciaGeograficaV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var georef = GetReferenciaGeografica(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return georef;

            if (georef.Id == 0)
            {
                georef.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                georef.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
            }

            georef.Codigo = data.AsString(Properties.ReferenciaGeografica.Codigo, 32);
            var descripcion = data.AsString(Properties.ReferenciaGeografica.Descripcion, 128);
            georef.Descripcion = string.IsNullOrEmpty(descripcion) ? georef.Codigo : descripcion;

            var lineaC = data[Properties.ReferenciaGeografica.Linea];
            if(lineaC != null) georef.Linea = GetLinea(empresa, lineaC) ?? georef.Linea;

            var vigenciaDesde = data.AsDateTime(Properties.ReferenciaGeografica.VigenciaDesde);
            var vigenciaHasta = data.AsDateTime(Properties.ReferenciaGeografica.VigenciaHasta);
            Revivir(georef, vigenciaDesde, vigenciaHasta);

            var tipoGeoRef = data.AsString(Properties.ReferenciaGeografica.TipoReferenciaGeografica, 50);
            if (!string.IsNullOrEmpty(tipoGeoRef))
            {
                georef.TipoReferenciaGeografica = DaoFactory.TipoReferenciaGeograficaDAO.GetByCodigo(new[] { empresa }, new[] { linea }, tipoGeoRef);
            }
            if (georef.TipoReferenciaGeografica == null)
            {
                ThrowProperty("TipoReferenciaGeografica");
            }
            if (georef.Id == 0)
            {
                georef.EsFin = georef.TipoReferenciaGeografica.EsFin;
                georef.EsInicio = georef.TipoReferenciaGeografica.EsInicio;
                georef.EsIntermedio = georef.TipoReferenciaGeografica.EsIntermedio;
                georef.Icono = georef.TipoReferenciaGeografica.Icono;
                georef.InhibeAlarma = georef.TipoReferenciaGeografica.InhibeAlarma;
                georef.TipoReferenciaGeografica = georef.TipoReferenciaGeografica;
                georef.Color = georef.TipoReferenciaGeografica.Color;

                foreach (TipoReferenciaVelocidad maxima in georef.TipoReferenciaGeografica.VelocidadesMaximas)
                    georef.VelocidadesMaximas.Add(new ReferenciaVelocidad
                    {
                        ReferenciaGeografica = georef,
                        TipoVehiculo = maxima.TipoVehiculo,
                        VelocidadMaxima = maxima.VelocidadMaxima
                    });
            }
            if (georef.Icono == null)
            {
                georef.Icono = georef.TipoReferenciaGeografica.Icono;
            }
            var color = data.AsString(Properties.ReferenciaGeografica.Color, 7);
            if(!string.IsNullOrEmpty(color))
            {
                georef.Color = new RGBColor { HexValue = color };    
            }

            if (!georef.IgnoraLogiclink)
            {
                var latitud = (data[Properties.ReferenciaGeografica.Latitud] ?? string.Empty).AsDouble();
                var longitud = (data[Properties.ReferenciaGeografica.Longitud] ?? string.Empty).AsDouble();
                var byLatLon = latitud.HasValue && longitud.HasValue;

                var direccion = (data[Properties.ReferenciaGeografica.Direccion] ?? string.Empty);
                var byDireccion = !string.IsNullOrEmpty(direccion.Trim());

                var provincia = (data[Properties.ReferenciaGeografica.Provincia] ?? string.Empty);
                var partido = (data[Properties.ReferenciaGeografica.Partido] ?? string.Empty);
                var calle = (data[Properties.ReferenciaGeografica.Calle] ?? string.Empty);
                var esquina = data[Properties.ReferenciaGeografica.Esquina];
                var altura = (data[Properties.ReferenciaGeografica.Altura] ?? string.Empty).AsInt() ?? -1;
                var radio = (data[Properties.ReferenciaGeografica.Radio] ?? string.Empty).AsInt() ?? 100;
                var byParts = !string.IsNullOrEmpty(calle.Trim());

                if (byLatLon)
                {
                    georef = GetReferenciaGeografica(georef.TipoReferenciaGeografica, georef, georef.Empresa,
                                                     georef.Linea,
                                                     georef.Codigo, descripcion, radio, vigenciaDesde, vigenciaHasta,
                                                     latitud.Value, longitud.Value);
                    georef.Observaciones = string.Format("({0}, {1})", latitud, longitud);
                }
                else if (byParts)
                {
                    georef = GetReferenciaGeografica(georef.TipoReferenciaGeografica, georef, georef.Empresa,
                                                     georef.Linea,
                                                     georef.Codigo, descripcion, radio,
                                                     vigenciaDesde, vigenciaHasta, calle, altura, esquina, partido,
                                                     provincia);

                    georef.Observaciones = string.Format("({0} {1}, {2})", calle,
                                                         altura > 0 ? altura.ToString() : " y " + esquina,
                                                         string.IsNullOrEmpty(partido) ? provincia : partido);
                }
                else if (byDireccion)
                {
                    georef = GetReferenciaGeografica(georef.TipoReferenciaGeografica, georef, georef.Empresa,
                                                     georef.Linea,
                                                     georef.Codigo, descripcion, radio, vigenciaDesde, vigenciaHasta,
                                                     direccion);

                    georef.Observaciones = direccion;
                }
            }
            return georef;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ReferenciaGeografica;
            if (ValidateSaveOrUpdate(item))
            {
                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(item);
                STrace.Trace("QtreeReset", "ReferenciaGeograficaV1 1");
            }
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ReferenciaGeografica;
            if (ValidateDelete(item)) DaoFactory.ReferenciaGeograficaDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ReferenciaGeografica;
            if (ValidateSave(item))
            {
                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(item);
                STrace.Trace("QtreeReset", "ReferenciaGeograficaV1 2");
            }
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ReferenciaGeografica;
            if (ValidateUpdate(item))
            {
                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(item);
                STrace.Trace("QtreeReset", "ReferenciaGeograficaV1 3");
            }
        }

        #endregion

        protected virtual ReferenciaGeografica GetReferenciaGeografica(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.ReferenciaGeografica.Codigo];
            if (codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.ReferenciaGeograficaDAO.GetByCodigo(new[] { empresa }, new[] { linea }, new[] { -1 }, codigo);
            return sameCode ?? new ReferenciaGeografica();
        }


        public ReferenciaGeografica GetReferenciaGeografica(TipoReferenciaGeografica tipoReferencia, ReferenciaGeografica geoRef, Empresa empresa, Linea linea, string codigo, string descripcion, int radio, DateTime? vigenciaDesde, DateTime? vigenciaHasta, double latitud, double longitud)
        {
            if (geoRef.Direccion == null || Math.Abs(geoRef.Direccion.Latitud - latitud) > 0.00005 || Math.Abs(geoRef.Direccion.Longitud - longitud) > 0.00005)
            {
                var nomencladas = NomenclarByLatLon(latitud, longitud);
                SetDireccion(geoRef, nomencladas, radio);
            }
            return geoRef;
        }
        public ReferenciaGeografica GetReferenciaGeografica(TipoReferenciaGeografica tipoReferencia, ReferenciaGeografica geoRef, Empresa empresa, Linea linea, string codigo, string descripcion, int radio, DateTime? vigenciaDesde, DateTime? vigenciaHasta, string direccion)
        {
            if (geoRef.Direccion == null)
            {
                var nomencladas = NomenclarByDireccion(direccion);
                SetDireccion(geoRef, nomencladas, radio);
            }
            return geoRef;
        }
        public ReferenciaGeografica GetReferenciaGeografica(TipoReferenciaGeografica tipoReferencia, ReferenciaGeografica geoRef, Empresa empresa, Linea linea, string codigo, string descripcion, int radio, DateTime? vigenciaDesde, DateTime? vigenciaHasta, string calle, int altura, string esquina, string partido, string provincia)
        {
            if (geoRef.Direccion == null)
            {
                var nomencladas = NomenclarByCalle(calle, altura, esquina, partido, provincia);
                SetDireccion(geoRef, nomencladas, radio);
            }
            return geoRef;
        }

        public ReferenciaGeografica Revivir(ReferenciaGeografica referencia, DateTime? vigenciaDesde, DateTime? vigenciaHasta)
        {
            referencia.Baja = false;
            if (vigenciaDesde.HasValue || vigenciaHasta.HasValue)
            {
                if (referencia.Vigencia == null || !referencia.Vigencia.Vigente(DateTime.UtcNow))
                {
                    referencia.Vigencia = new Vigencia { Inicio = vigenciaDesde, Fin = vigenciaHasta };
                }
                else
                {
                    var desde = vigenciaDesde.HasValue ? vigenciaDesde.Value : DateTime.MaxValue;
                    if (referencia.Vigencia.Inicio.HasValue && referencia.Vigencia.Inicio.Value < desde) desde = referencia.Vigencia.Inicio.Value;
                    if (desde == DateTime.MaxValue) desde = DateTime.UtcNow;

                    var hasta = vigenciaHasta.HasValue ? vigenciaHasta.Value : DateTime.MinValue;
                    if (referencia.Vigencia.Fin.HasValue && referencia.Vigencia.Fin.Value > hasta) hasta = referencia.Vigencia.Fin.Value;
                    if (hasta == DateTime.MinValue) hasta = DateTime.UtcNow;
                    referencia.Vigencia = new Vigencia { Inicio = desde, Fin = hasta };
                }
            }
            return referencia;
        }
        private void SetDireccion(ReferenciaGeografica geoRef, IList<DireccionVO> nomencladas, int radio)
        {
            if (nomencladas.Count > 0)
            {
                var direccionNomenclada = nomencladas[0];


                var dir = new Direccion
                {
                    Altura = direccionNomenclada.Altura,
                    Calle = direccionNomenclada.Calle,
                    Descripcion = direccionNomenclada.Direccion.Truncate(128),
                    IdCalle = direccionNomenclada.IdPoligonal,
                    IdEntrecalle = -1,
                    IdEsquina = direccionNomenclada.IdEsquina,
                    IdMapa = (short)direccionNomenclada.IdMapaUrbano,
                    Latitud = direccionNomenclada.Latitud,
                    Longitud = direccionNomenclada.Longitud,
                    Pais = "Argentina",
                    Partido = direccionNomenclada.Partido,
                    Provincia = direccionNomenclada.Provincia,
                    Vigencia = new Vigencia { Inicio = DateTime.Now }
                };

                var pol = new Poligono
                {
                    Radio = radio,
                    Vigencia = new Vigencia { Inicio = DateTime.Now }
                };
                pol.AddPoints(new[]
                                  {
                                      new PointF((float) direccionNomenclada.Longitud,
                                                 (float) direccionNomenclada.Latitud)
                                  });


                geoRef.AddHistoria(dir, pol, DateTime.UtcNow);
            }
        }

        protected static IList<DireccionVO> NomenclarByLatLon(double latitud, double longitud)
        {
            var dir = GeocoderHelper.Cleaning.GetDireccionMasCercana(latitud, longitud)
                ?? new DireccionVO
                {
                    Altura = -1,
                    Calle = string.Empty,
                    Direccion = string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture), longitud.ToString(CultureInfo.InvariantCulture)),
                    IdEsquina = -1,
                    IdMapaUrbano = -1,
                    IdPoligonal = -1,
                    IdProvincia = -1,
                    Latitud = latitud,
                    Longitud = longitud,
                    Partido = string.Empty,
                    Provincia = string.Empty
                };
            dir.Latitud = latitud;
            dir.Longitud = longitud;
            return new List<DireccionVO> { dir };
        }
        protected static IList<DireccionVO> NomenclarByCalle(string calle, int altura, string esquina, string partido, string provincia)
        {
            return GeocoderHelper.Cleaning.NomenclarDireccion(calle, altura, esquina ?? string.Empty, partido ?? string.Empty, provincia ?? string.Empty);
        }
        protected static IList<DireccionVO> NomenclarByDireccion(string direccion)
        {
            return GeocoderHelper.Cleaning.GetSmartSearch(direccion);
        }

    }
}
