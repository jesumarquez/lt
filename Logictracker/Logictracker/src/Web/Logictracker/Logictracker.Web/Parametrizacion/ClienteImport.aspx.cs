using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

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

                try
                {
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
                        var telefono = row.GetString(GetColumnByValue(Fields.Telefono.Value));


                        var createPuntoEntrega = false;

                        var cliente = DAOFactory.ClienteDAO.GetByCode(empresas, lineas, codigo);

                        if (cliente == null)
                        {
                            cliente = new Cliente
                                      {
                                          Codigo = codigo.Truncate(32),
                                          Empresa = empresa,
                                          Linea = linea,
                                          Telefono = telefono.Truncate(32),
                                          Baja = false,
                                          Nomenclado = false,
                                          DireccionNomenclada = string.Empty
                                      };

                            createPuntoEntrega = true;
                        }

                        cliente.Baja = false;
                        cliente.Descripcion = descripcion.Truncate(40);
                        cliente.DescripcionCorta = descripcion.Truncate(17);

                        if (!cliente.Nomenclado)
                        {
                            tipoGeoref = ValidateEmpty(tipoGeoref, "Entities", "PARENTI10");
                            var tipoReferencia = DAOFactory.TipoReferenciaGeograficaDAO.GetByCodigo(empresas, lineas, tipoGeoref) ?? tipoReferenciaDefault;
                            if (tipoReferencia == null) throw new ApplicationException("Tipo de Referencia Geografica invalido");


                            if (latitud.HasValue &&
                                longitud.HasValue)
                            {
                                cliente.ReferenciaGeografica = GetReferenciaGeografica(tipoReferencia, cliente.ReferenciaGeografica, empresa, linea, codigo,
                                    descripcion, vigenciaDesde, vigenciaHasta, latitud.Value, longitud.Value);
                                cliente.DireccionNomenclada = string.Format("({0}, {1})", latitud.Value, longitud.Value);
                            }
                            else if (!string.IsNullOrEmpty(calle) &&
                                     (!string.IsNullOrEmpty(esquina) || altura.HasValue) &&
                                     (!string.IsNullOrEmpty(partido) || !string.IsNullOrEmpty(provincia)))
                            {
                                cliente.ReferenciaGeografica = GetReferenciaGeografica(tipoReferencia, cliente.ReferenciaGeografica, empresa, linea, codigo,
                                    descripcion, vigenciaDesde, vigenciaHasta, calle, altura.HasValue ? altura.Value : -1, esquina, partido, provincia);
                                cliente.DireccionNomenclada = string.Format("({0} {1}, {2})", calle,
                                    altura.HasValue ? altura.Value.ToString("#0") : " y " + esquina, string.IsNullOrEmpty(partido) ? provincia : partido);
                            }
                            else if (!string.IsNullOrEmpty(direccion))
                            {
                                cliente.ReferenciaGeografica = GetReferenciaGeografica(tipoReferencia, cliente.ReferenciaGeografica, empresa, linea, codigo,
                                    descripcion, vigenciaDesde, vigenciaHasta, direccion);
                                cliente.DireccionNomenclada = direccion;
                            }
                            else
                            {
                                cliente.ReferenciaGeografica = GetReferenciaGeografica(tipoReferencia, cliente.ReferenciaGeografica, empresa, linea, codigo,
                                    descripcion, vigenciaDesde, vigenciaHasta);
                            }
                            cliente.Nomenclado = cliente.ReferenciaGeografica.Direccion != null;
                            cliente.DireccionNomenclada = cliente.DireccionNomenclada.Truncate(255);

                            DAOFactory.ClienteDAO.SaveOrUpdate(cliente);
                            if (createPuntoEntrega) CreatePuntoEntrega(cliente);
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

        private void CreatePuntoEntrega(Cliente cliente)
        {
            var punto = new PuntoEntrega
            {
                Cliente = cliente,
                Codigo = cliente.Codigo.Truncate(32),
                Descripcion = cliente.Descripcion.Truncate(40),
                Telefono = cliente.Telefono,
                Baja = false,
                ReferenciaGeografica = cliente.ReferenciaGeografica,
                Nomenclado = cliente.Nomenclado,
                DireccionNomenclada = cliente.DireccionNomenclada
            };
            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(punto);
        }

        #region SubClasses
        private static class Fields
        {
           public static readonly FieldValue Descripcion = new FieldValue("Descripcion");
           public static readonly FieldValue Codigo = new FieldValue("Codigo");
           public static readonly FieldValue Calle = new FieldValue("Calle");
           public static readonly FieldValue Direccion = new FieldValue("Direccion");
           public static readonly FieldValue Altura = new FieldValue("Altura");
           public static readonly FieldValue Esquina = new FieldValue("Esquina");
           public static readonly FieldValue Partido = new FieldValue("Partido");
           public static readonly FieldValue Provincia = new FieldValue("Provincia");
           public static readonly FieldValue VigenciaDesde = new FieldValue("Vigencia Desde");
           public static readonly FieldValue VigenciaHasta = new FieldValue("Vigencia Hasta");
           public static readonly FieldValue Latitud = new FieldValue("Latitud");
           public static readonly FieldValue Longitud = new FieldValue("Longitud");
           public static readonly FieldValue TipoGeoRef = new FieldValue("Tipo Referencia Geográfica");
           public static readonly FieldValue Telefono = new FieldValue("Telefono");

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
                                           TipoGeoRef,
                                           Telefono
                                       };
                }
            }
        }
        #endregion
    }
}