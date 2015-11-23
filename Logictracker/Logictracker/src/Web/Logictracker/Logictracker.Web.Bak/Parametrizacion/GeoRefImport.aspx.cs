using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Services.Helpers;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;
using Geocoder.Core.VO;

namespace Logictracker.Parametrizacion
{
    public partial class GeoRefImport : SecuredImportPage
    {
        protected override string VariableName { get { return "PAR_POI"; } }
        protected override string RedirectUrl { get { return "GeoRefLista.aspx"; } }
        protected override string GetRefference() { return "DOMICILIO"; }

        private Dictionary<int, List<int>> _empresasLineas = new Dictionary<int, List<int>>();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override List<FieldValue> GetMappingFields()
        {
            return Fields.List;
        }

        protected override void ValidateMapping()
        {
            // El código es obligatorio
            if (!IsMapped(Fields.Codigo.Value)) throw new ApplicationException("Falta mapear " + Fields.Codigo.Name);

            // Una de las 3 variantes de direccion tiene que estar mapeada
            if (!IsMapped(Fields.Direccion.Value) 
                && (!IsMapped(Fields.Latitud.Value) || !IsMapped(Fields.Longitud.Value))
                && (!IsMapped(Fields.Calle.Value) || (!IsMapped(Fields.Altura.Value) || !IsMapped(Fields.Esquina.Value)) || (!IsMapped(Fields.Partido.Value) || !IsMapped(Fields.Provincia.Value))))
            {
                throw new ApplicationException("Falta mapear " + Fields.Direccion.Name);
            }

            // La Empresa es obligatoria
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");

            // El tipo de referencia geografica es obligatorio (por mapeo o seleccion)
            if(!IsMapped(Fields.TipoGeoRef.Value)) ValidateEntity(cbTipoGeoRef.Selected, "PARENTI10");
        }

        protected override bool ValidateRow(ImportRow row)
        {
            if (IsMapped(Fields.Latitud.Value) && IsMapped(Fields.Longitud.Value))
            {
                ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "LATITUD");
                ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "LONGITUD");
            }

            ValidateEmpty(GetColumnByValue(Fields.Codigo.Value), "CODE");
            if (IsMapped(Fields.Descripcion.Value)) ValidateEmpty(GetColumnByValue(Fields.Descripcion.Value), "DESCRIPTION");

            //if (IsMapped(Fields.TipoGeoRef.Value)) ValidateEmpty(GetColumnByValue(Fields.TipoGeoRef.Value), "Entities", "PARENTI10");

            return true;
        }

        protected override void Import(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var tipoReferenciaDefault = cbTipoGeoRef.Selected > 0 ? DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoGeoRef.Selected) : null;

            var empresas = new [] { cbEmpresa.Selected };
            var lineas = new[] {cbLinea.Selected};

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                var list = new List<ReferenciaGeografica>(rows.Count);

                foreach (var row in rows)
                {
                    var codigo = row.GetString(GetColumnByValue(Fields.Codigo.Value));
                    var descripcion = IsMapped(Fields.Descripcion.Value) ? row.GetString(GetColumnByValue(Fields.Descripcion.Value)) : codigo;
                    var direccion = row.GetString(GetColumnByValue(Fields.Direccion.Value));
                    var calle = row.GetString(GetColumnByValue(Fields.Calle.Value));
                    var altura = row.GetInt32(GetColumnByValue(Fields.Altura.Value));
                    var esquina = row.GetString(GetColumnByValue(Fields.Esquina.Value));
                    var partido = row.GetString(GetColumnByValue(Fields.Partido.Value));
                    var provincia = row.GetString(GetColumnByValue(Fields.Provincia.Value));
                    var vigenciaDesde = row.GetDateTime(GetColumnByValue(Fields.VigenciaDesde.Value));
                    var vigenciaHasta = row.GetDateTime(GetColumnByValue(Fields.VigenciaHasta.Value));
                    var longitud = row.GetDouble(GetColumnByValue(Fields.Longitud.Value));
                    var latitud = row.GetDouble(GetColumnByValue(Fields.Latitud.Value));
                    var tipoGeoref = row.GetString(GetColumnByValue(Fields.TipoGeoRef.Value));

                    var tipoReferencia = tipoReferenciaDefault;
                    if (!string.IsNullOrEmpty(tipoGeoref))
                        DAOFactory.TipoReferenciaGeograficaDAO.GetByCodigo(empresas, lineas, tipoGeoref);
                    if (tipoReferencia == null) throw new ApplicationException("Tipo de Referencia Geografica invalido");

                    var geoRef = DAOFactory.ReferenciaGeograficaDAO.GetByCodigo(empresas, lineas, new[] {tipoReferencia.Id}, codigo);

                    var nomenclar = geoRef == null || geoRef.Direccion == null;

                    if (geoRef == null)
                    {
                        geoRef = new ReferenciaGeografica
                                 {
                                     Codigo = codigo,
                                     Empresa = empresa,
                                     Linea = linea,
                                     EsFin = tipoReferencia.EsFin,
                                     EsInicio = tipoReferencia.EsInicio,
                                     EsIntermedio = tipoReferencia.EsIntermedio,
                                     Icono = tipoReferencia.Icono,
                                     InhibeAlarma = tipoReferencia.InhibeAlarma,
                                     TipoReferenciaGeografica = tipoReferencia,
                                     Color = tipoReferencia.Color
                                 };
                        foreach (TipoReferenciaVelocidad maxima in tipoReferencia.VelocidadesMaximas)
                            geoRef.VelocidadesMaximas.Add(new ReferenciaVelocidad
                                                          {
                                                              ReferenciaGeografica = geoRef,
                                                              TipoVehiculo = maxima.TipoVehiculo,
                                                              VelocidadMaxima = maxima.VelocidadMaxima
                                                          });
                    }

                    geoRef.Baja = false;
                    geoRef.Descripcion = descripcion;
                    geoRef.Observaciones = string.Empty;

                    if (vigenciaDesde.HasValue &&
                        vigenciaDesde.Value.Equals(vigenciaHasta.Value))
                    {
                        vigenciaDesde = new DateTime(vigenciaDesde.Value.Year, vigenciaDesde.Value.Month, vigenciaDesde.Value.Day, 0, 0, 0);
                        vigenciaHasta = new DateTime(vigenciaDesde.Value.Year, vigenciaDesde.Value.Month, vigenciaDesde.Value.Day, 23, 59, 59);
                    }

                    UpdateVigencia(geoRef, vigenciaDesde, vigenciaHasta);

                    if (nomenclar)
                    {
                        IList<DireccionVO> nomencladas;

                        if (latitud.HasValue &&
                            longitud.HasValue)
                        {
                            nomencladas = new List<DireccionVO> {GeocoderHelper.GetEsquinaMasCercana(latitud.Value, longitud.Value)};
                            NomenclarByLatLon(latitud.Value, longitud.Value);
                        }
                        else if (!string.IsNullOrEmpty(calle) &&
                                 (!string.IsNullOrEmpty(esquina) || altura.HasValue) &&
                                 (!string.IsNullOrEmpty(partido) || !string.IsNullOrEmpty(provincia)))
                        {
                            nomencladas = NomenclarByCalle(calle, altura.HasValue ? altura.Value : -1, esquina, partido, provincia);
                        }
                        else if (!string.IsNullOrEmpty(direccion))
                        {
                            nomencladas = NomenclarByDireccion(direccion);
                        }
                        else
                        {
                            nomencladas = new List<DireccionVO>(0);
                        }

                        if (nomencladas.Count == 1)
                        {
                            var direccionNomenclada = nomencladas[0];

                            var dir = new Direccion
                                      {
                                          Altura = direccionNomenclada.Altura,
                                          Calle = direccionNomenclada.Calle,
                                          Descripcion = direccionNomenclada.Direccion,
                                          IdCalle = direccionNomenclada.IdPoligonal,
                                          IdEntrecalle = -1,
                                          IdEsquina = direccionNomenclada.IdEsquina,
                                          IdMapa = (short) direccionNomenclada.IdMapaUrbano,
                                          Latitud = direccionNomenclada.Latitud,
                                          Longitud = direccionNomenclada.Longitud,
                                          Pais = "Argentina",
                                          Partido = direccionNomenclada.Partido,
                                          Provincia = direccionNomenclada.Provincia,
                                          Vigencia = new Vigencia {Inicio = DateTime.Now}
                                      };

                            var pol = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.Now}};
                            pol.AddPoints(new[] {new PointF((float) direccionNomenclada.Longitud, (float) direccionNomenclada.Latitud)});


                            geoRef.AddHistoria(dir, pol, DateTime.UtcNow);
                        }
                    }

                    list.Add(geoRef);
                }

                foreach (var geoRef in list)
                {
                    DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(geoRef);
                    AddReferenciasGeograficas(geoRef);
                }

                transaction.Commit();
                DAOFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
            }
        }

        private void AddReferenciasGeograficas(ReferenciaGeografica rg)
        {
            if (rg == null)
                STrace.Error(GetType().FullName, "AddReferenciasGeograficas: rg is null");
            else if (rg.Empresa == null)
                STrace.Error(GetType().FullName, "AddReferenciasGeograficas: rg.Empresa is null");
            else
            {
                if (!_empresasLineas.ContainsKey(rg.Empresa.Id))
                    _empresasLineas.Add(rg.Empresa.Id, new List<int> { -1 });

                if (rg.Linea != null)
                {
                    if (!_empresasLineas[rg.Empresa.Id].Contains(rg.Linea.Id))
                        _empresasLineas[rg.Empresa.Id].Add(rg.Linea.Id);
                }
                else
                {
                    var todaslaslineas = DAOFactory.LineaDAO.GetList(new[] { rg.Empresa.Id });
                    foreach (var linea in todaslaslineas)
                    {
                        if (!_empresasLineas.ContainsKey(linea.Id))
                            _empresasLineas[rg.Empresa.Id].Add(linea.Id);
                    }
                }
            }
        }

        private void UpdateVigencia(ReferenciaGeografica referencia, DateTime? vigenciaDesde, DateTime? vigenciaHasta)
        {
            if (vigenciaDesde.HasValue || vigenciaHasta.HasValue)
            {
                if (referencia.Vigencia == null || !referencia.Vigencia.Vigente(DateTime.UtcNow))
                {
                    referencia.Vigencia = new Vigencia { Inicio = vigenciaDesde.HasValue 
                                                                    ? vigenciaDesde.Value.ToDataBaseDateTime()
                                                                    : vigenciaDesde, 
                                                         Fin = vigenciaHasta.HasValue 
                                                                    ? vigenciaHasta.Value.ToDataBaseDateTime() 
                                                                    : vigenciaHasta };
                }
                else
                {
                    var desde = vigenciaDesde.HasValue ? vigenciaDesde.Value : DateTime.MaxValue;
                    if (referencia.Vigencia.Inicio.HasValue && referencia.Vigencia.Inicio.Value < desde) desde = referencia.Vigencia.Inicio.Value;
                    if (desde == DateTime.MaxValue) desde = DateTime.UtcNow;

                    var hasta = vigenciaHasta.HasValue ? vigenciaHasta.Value : DateTime.MinValue;
                    if (referencia.Vigencia.Fin.HasValue && referencia.Vigencia.Fin.Value > hasta) hasta = referencia.Vigencia.Fin.Value;
                    if (hasta == DateTime.MinValue) hasta = DateTime.UtcNow;
                    referencia.Vigencia = new Vigencia { Inicio = desde.ToDataBaseDateTime(), Fin = hasta.ToDataBaseDateTime() };
                }
                DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(referencia);
            }
        }

        #region SubClasses

        private static class Fields
        {
            public static readonly FieldValue Descripcion = new FieldValue("Descripcion");
            public static readonly FieldValue Codigo = new FieldValue("Codigo");
            public static readonly FieldValue Direccion = new FieldValue("0.Direccion");
            public static readonly FieldValue Calle = new FieldValue("1.Calle");
            public static readonly FieldValue Altura = new FieldValue("1.Altura");
            public static readonly FieldValue Esquina = new FieldValue("1.Esquina");
            public static readonly FieldValue Partido = new FieldValue("1.Partido");
            public static readonly FieldValue Provincia = new FieldValue("1.Provincia");
            public static readonly FieldValue VigenciaDesde = new FieldValue("Vigencia Desde");
            public static readonly FieldValue VigenciaHasta = new FieldValue("Vigencia Hasta");
            public static readonly FieldValue Latitud = new FieldValue("2.Latitud");
            public static readonly FieldValue Longitud = new FieldValue("2.Longitud");
            public static readonly FieldValue TipoGeoRef = new FieldValue("Tipo Referencia Geográfica");

            public static List<FieldValue> List
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Descripcion,
                                   Codigo,
                                   Direccion,
                                   Calle,
                                   Altura,
                                   Esquina,
                                   Partido,
                                   Provincia,
                                   VigenciaDesde,
                                   VigenciaHasta,
                                   Longitud,
                                   Latitud,
                                   TipoGeoRef
                               };
                }
            }
        }

        #endregion
    }
}