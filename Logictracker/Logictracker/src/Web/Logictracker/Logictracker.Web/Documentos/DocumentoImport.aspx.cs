using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.DAL.NHibernate;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Process.Import.EntityParser;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos
{
    public partial class DocumentoImport : SecuredImportPage
    {
        protected override string RedirectUrl { get { return "DocumentoLista.aspx"; } }
        protected override string VariableName { get { return "DOC_DOCUMENTOS"; } }
        protected override string GetRefference() { return "DOCUMENTO"; }

        protected override void ValidateFilters()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbTipoDocumento.Selected, "TIPO_DOCUMENTO");
        }

        protected override void Import(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected;
            var linea = cbLinea.Selected;
            var tipoDocumento = cbTipoDocumento.Selected;

            var nombreTipo = tipoDocumento > 0 ? DAOFactory.TipoDocumentoDAO.FindById(tipoDocumento).Nombre : null;

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var parser = new DocumentoV1(DAOFactory);

                    foreach (var row in rows)
                    {
                        var data = GetLogiclinkData(Entities.Documento, row);
                        var tipo = data[Properties.Documento.TipoDocumento];
                        if (string.IsNullOrEmpty(tipo))
                        {
                            data.Add(Properties.Documento.TipoDocumento, nombreTipo);
                        }
                        data.Add(Properties.Documento.Gmt, Usuario.GmtModifier.ToString(CultureInfo.InvariantCulture));
                        data.Pack();
                        var documento = parser.Parse(empresa, linea, data);
                        parser.Save(documento, empresa, linea, data);
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

        private static List<FieldValue> _default;
        protected override List<FieldValue> GetMappingFields()
        {
            if (_default == null)
            {
                var tipo = DAOFactory.TipoDocumentoDAO.FindById(cbTipoDocumento.Selected);

                _default = new List<FieldValue>
                               {
                                   new FieldValue("Base", Properties.Documento.Linea, Entities.Documento),
                                   new FieldValue("Codigo", Properties.Documento.Codigo, Entities.Documento) {RequiredMapping = true, DisableDefault = true},
                                   new FieldValue("Descripcion", Properties.Documento.Descripcion, Entities.Documento) {RequiredValue = true},
                                   new FieldValue("Fecha", Properties.Documento.Fecha, Entities.Documento) {RequiredValue = true},
                                   new FieldValue("FechaPresentacion", Properties.Documento.FechaPresentacion, Entities.Documento) {RequiredValue = tipo.RequerirPresentacion},
                                   new FieldValue("FechaVencimiento", Properties.Documento.FechaVencimiento, Entities.Documento) {RequiredValue = tipo.RequerirVencimiento},
                                   new FieldValue("FechaCierre", Properties.Documento.FechaCierre, Entities.Documento)
                               };

                if (tipo.AplicarAVehiculo)
                {
                    _default.Add(new FieldValue("Vehiculo", Properties.Documento.Vehiculo, Entities.Documento){RequiredValue = true});
                }
                if (tipo.AplicarAEmpleado)
                {
                    _default.Add(new FieldValue("Empleado", Properties.Documento.Empleado, Entities.Documento) { RequiredValue = true });
                }
                if (tipo.AplicarATransportista)
                {
                    _default.Add(new FieldValue("Transportista", Properties.Documento.Transportista, Entities.Documento) { RequiredValue = true });
                }
                if (tipo.AplicarAEquipo)
                {
                    _default.Add(new FieldValue("Equipo", Properties.Documento.Equipo, Entities.Documento) { RequiredValue = true });
                }

                var d = tipo.Parametros.OfType<TipoDocumentoParametro>()
                    .GroupBy(x => (int) Math.Truncate(x.Orden));

                foreach(var o in d)
                {
                    var rep = Math.Max(1.0, o.First().Repeticion);
                    for (var i = 0; i < rep; i++)
                    {
                        var repNo = i;
                        foreach (var par in o)
                        {
                            var nombre = par.Repeticion > 1 ? string.Format("[{0}] {1}",repNo, par.Nombre) : par.Nombre;
                            var field = new FieldValue(nombre, Properties.Documento.None, Entities.Documento)
                                            {RequiredValue = par.Obligatorio};
                            _default.Add(field);
                        }
                    }
                }
            }
            return _default;
        }

        protected void CbTipoDocumentoSelectedIndexChanged(object sender, EventArgs e)
        {
            _default = null;
            UpdateGrid();
        }
    }
}