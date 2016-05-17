using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Services.Helpers;
using System.Globalization;
using Logictracker.Types.BusinessObjects.Components;
using System.Drawing;

namespace Logictracker.Parametrizacion
{
    public partial class PtoEntregaImport : SecuredImportPage
    {
        protected override string VariableName { get { return "PTOS_ENTREGA"; } }
        protected override string RedirectUrl { get { return "PtoEntregaLista.aspx"; } }
        protected override string GetRefference() { return "PTO_ENTREGA"; }

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
           if (!IsMapped(Fields.Codigo.Value)) 
               throw new ApplicationException("Falta mapear " + Fields.Codigo.Name);

           
           // La Empresa es obligatoria
           ValidateEntity(cbEmpresa.Selected, "PARENTI01");


           // El Cliente es obligatorio
           if(!IsMapped(Fields.TipoCliente.Value))
               throw new ApplicationException("Falta mapear " + Fields.TipoCliente.Name);
        }

        protected override bool ValidateRow(ImportRow row)
        {
           if (IsMapped(Fields.Latitud.Value) && IsMapped(Fields.Longitud.Value))
           {
              ValidateDouble(row[GetColumnByValue(Fields.Latitud.Value)], "LATITUD");
              ValidateDouble(row[GetColumnByValue(Fields.Longitud.Value)], "LONGITUD");
           }

           ValidateEmpty(GetColumnByValue(Fields.Codigo.Value), "CODE");
           if (IsMapped(Fields.Descripcion.Value)) 
               ValidateEmpty(GetColumnByValue(Fields.Descripcion.Value), "DESCRIPTION");

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

                try
                {
                    foreach (var row in rows)
                    {
                        var tipoCliente = row.GetString(GetColumnByValue(Fields.TipoCliente.Value));
                        var cliente = DAOFactory.ClienteDAO.FindByDescripcion(new[] { empresa.Id }, new[] { -1 }, tipoCliente);
                        var codigo = row.GetString(GetColumnByValue(Fields.Codigo.Value));
                                                  
                        var pto = DAOFactory.PuntoEntregaDAO.FindByCode(new[] { cliente.Empresa.Id }, new[] { cliente.Linea != null ? cliente.Linea.Id : -1 }, new[] { cliente.Id }, codigo);
                                            
                        if (pto == null)
                        {
                            var descripcion = IsMapped(Fields.Descripcion.Value) ? row.GetString(GetColumnByValue(Fields.Descripcion.Value)) : codigo;
                            var longitud = row.GetDouble(GetColumnByValue(Fields.Longitud.Value));
                            var latitud = row.GetDouble(GetColumnByValue(Fields.Latitud.Value));
                            var vigenciaDesde = DateTime.Now;
                            var direccionNomenclada = row.GetString(GetColumnByValue(Fields.Direccion.Value));
                            var geoRef = new ReferenciaGeografica();
                            var telefono = "";

                            var tipoReferencia = tipoReferenciaDefault;
                            if (tipoReferencia == null) throw new ApplicationException("Tipo de Referencia Geográfica inválido");


                            if (latitud.HasValue &&
                                longitud.HasValue)
                            {
                                if (direccionNomenclada == null)
                                {
                                    direccionNomenclada = string.Format("({0}, {1})", latitud.Value, longitud.Value);
                                }
                                geoRef = GetReferenciaGeografica(tipoReferencia, null, empresa, linea, codigo, descripcion, vigenciaDesde, null,
                                    latitud.Value, longitud.Value);
                               
                            }

                            pto = CreatePuntoEntrega(cliente, codigo, descripcion, telefono, geoRef);

                        }
                        else
                        {
                            var descripcion = IsMapped(Fields.Descripcion.Value) ? row.GetString(GetColumnByValue(Fields.Descripcion.Value)) : codigo;
                            var longitud = row.GetDouble(GetColumnByValue(Fields.Longitud.Value));
                            var latitud = row.GetDouble(GetColumnByValue(Fields.Latitud.Value));

                            pto.Descripcion = descripcion;
                            pto.Nombre = descripcion;

                            if (pto.ReferenciaGeografica.Latitude != latitud ||
                                pto.ReferenciaGeografica.Longitude != longitud)
                            {
                                pto.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                                pto.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                                #region var posicion = new Direccion()

                                var direccion = GeocoderHelper.GetEsquinaMasCercana(latitud.Value, longitud.Value);

                                var posicion = new Direccion
                                {
                                    Altura = direccion != null ? direccion.Altura : -1,
                                    IdMapa = (short)(direccion != null ? direccion.IdMapaUrbano : -1),
                                    Provincia = direccion != null ? direccion.Provincia : string.Empty,
                                    IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                                    IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                                    IdEntrecalle = -1,
                                    Latitud = latitud.Value,
                                    Longitud = longitud.Value,
                                    Partido = direccion != null ? direccion.Partido : string.Empty,
                                    Pais = string.Empty,
                                    Calle = direccion != null ? direccion.Calle : string.Empty,
                                    Descripcion =
                                        direccion != null
                                            ? direccion.Direccion
                                            : string.Format("({0}, {1})", latitud.Value.ToString(CultureInfo.InvariantCulture),
                                                longitud.Value.ToString(CultureInfo.InvariantCulture)),
                                    Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                                };

                                #endregion

                                #region var poligono = new Poligono()

                                var poligono = new Poligono { Radio = 100, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                                poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

                                #endregion

                                pto.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);

                                DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(pto.ReferenciaGeografica);
                            }

                        }
                                                                                              
                        DAOFactory.PuntoEntregaDAO.SaveOrUpdate(pto);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private PuntoEntrega CreatePuntoEntrega(Cliente cliente, string codigo, string descripcion, string telefono, ReferenciaGeografica rg)
        {
            var pto = new PuntoEntrega();

            pto.Cliente = cliente;
            pto.Codigo = codigo;
            pto.Descripcion = descripcion;
            pto.Baja = false;
            pto.ReferenciaGeografica = rg;
            pto.Nomenclado = true;
            pto.Nombre = descripcion;

            return pto;
        }

        #region SubClasses
        
        private static class Fields
        {
            public static readonly FieldValue Codigo = new FieldValue("Codigo Cliente");
            public static readonly FieldValue Latitud = new FieldValue("Latitud");
            public static readonly FieldValue Longitud = new FieldValue("Longitud");
            public static readonly FieldValue Descripcion = new FieldValue("Descripcion cliente");
            public static readonly FieldValue TipoCliente = new FieldValue("Tipo de Cliente");
            public static readonly FieldValue Direccion = new FieldValue("Direccion");
            
            public static List<FieldValue> List
            {
                get
                {
                    return new List<FieldValue> {
                                                    Codigo,
                                                    Latitud,
                                                    Longitud,
                                                    Descripcion,
                                                    TipoCliente,
                                                    Direccion
                                                };
                }
            }
        }
        
        #endregion
    }
}