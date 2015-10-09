using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Components;
using System.Drawing;

namespace Logictracker.Parametrizacion
{
    public partial class ClienteImport : SecuredImportPage
    {
        protected override string VariableName { get { return "PAR_CLIENTES"; } }
        protected override string RedirectUrl { get { return "ClienteLista.aspx"; } }
        protected override string GetRefference() { return "RAMAL"; }

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
           if (!IsMapped(Fields.Latitud.Value) || !IsMapped(Fields.Longitud.Value))
           {
              throw new ApplicationException("Falta mapear latitud/longitud ");
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
            var tipoGeoref = cbTipoGeoRef.Selected > 0 ? DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoGeoRef.Selected) : null;

            var empresas = new [] { cbEmpresa.Selected };
            var lineas = new[] {cbLinea.Selected};

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    foreach (var row in rows)
                    {
                        var codigo = row.GetString(GetColumnByValue(Fields.Codigo.Value));
                        var descripcion = IsMapped(Fields.Descripcion.Value) ? row.GetString(GetColumnByValue(Fields.Descripcion.Value)) : codigo;
                        var vigenciaDesde = DateTime.UtcNow;
                        var vigenciaHasta = DateTime.UtcNow.AddDays(1);
                        var longitud = row.GetDouble(GetColumnByValue(Fields.Longitud.Value));
                        var latitud = row.GetDouble(GetColumnByValue(Fields.Latitud.Value));
                        

                        var createPuntoEntrega = false;

                        var cliente = DAOFactory.ClienteDAO.GetByCode(empresas, lineas, codigo);

                        if (cliente == null)
                        {
                            cliente = new Cliente
                                      {
                                          Codigo = codigo.Truncate(32),
                                          Empresa = empresa,
                                          Linea = linea,
                                          Baja = false,
                                          Nomenclado = false
                                      };

                            createPuntoEntrega = true;
                        }

                        cliente.Baja = false;
                        cliente.Descripcion = descripcion.Truncate(40);
                        cliente.DescripcionCorta = descripcion.Truncate(17);

                        if (!cliente.Nomenclado)
                        {
                            if (latitud.HasValue &&
                                longitud.HasValue)
                            {
                                

                                var geoRef = new ReferenciaGeografica
                             {
                                 Codigo = codigo,
                                 Empresa = empresa,
                                 Linea = linea,
                                 EsFin = tipoGeoref.EsFin,
                                 EsInicio = tipoGeoref.EsInicio,
                                 EsIntermedio = tipoGeoref.EsIntermedio,
                                 Icono = tipoGeoref.Icono,
                                 InhibeAlarma = tipoGeoref.InhibeAlarma,
                                 TipoReferenciaGeografica = tipoGeoref,
                                 Color = tipoGeoref.Color,
                                 Baja = false,
                                 Descripcion = descripcion
            
                             };

                                geoRef.Vigencia = new Vigencia { Inicio = vigenciaDesde, Fin = vigenciaHasta };



                                var dir = new Direccion
                                {
                                    Altura = 0,
                                    Calle = String.Empty,
                                    Descripcion = descripcion,
                                    IdCalle = 0,
                                    IdEntrecalle = -1,
                                    IdEsquina = 0,
                                    IdMapa = 0,
                                    Latitud = latitud.Value,
                                    Longitud = longitud.Value,
                                    Pais = "Argentina",
                                    Partido = String.Empty,
                                    Provincia = String.Empty,
                                    Vigencia = new Vigencia { Inicio = DateTime.Now }
                                };

                                var pol = new Poligono
                                {
                                    Radio = 1,
                                    Vigencia = new Vigencia { Inicio = DateTime.Now }
                                };
                                pol.AddPoints(new[]
                                  {
                                      new PointF((float) longitud.Value,
                                                 (float) latitud.Value)
                                  });


                                geoRef.AddHistoria(dir, pol, DateTime.UtcNow);

                                DAOFactory.ReferenciaGeograficaDAO.SaveOrUpdate(geoRef);

                                cliente.ReferenciaGeografica = geoRef;

                            }
                         
                            DAOFactory.ClienteDAO.SaveOrUpdate(cliente);
                            
                        }
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

        

        #region SubClasses
        private static class Fields
        {
           public static readonly FieldValue Descripcion = new FieldValue("Descripcion");
           public static readonly FieldValue Codigo = new FieldValue("Codigo");
           public static readonly FieldValue Latitud = new FieldValue("Latitud");
           public static readonly FieldValue Longitud = new FieldValue("Longitud");
           public static readonly FieldValue TipoGeoRef = new FieldValue("Tipo Referencia Geográfica");

           public static List<FieldValue> List
           {
              get
              {
                 return new List<FieldValue>
                                       {
                                           Descripcion,
                                           Codigo,
                                           Latitud,
                                           Longitud,
                                           TipoGeoRef
                                           
                                       };
                }
            }
        }
        #endregion
    }
}