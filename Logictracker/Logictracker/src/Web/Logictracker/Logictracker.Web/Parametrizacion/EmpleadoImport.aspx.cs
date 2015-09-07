using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Process.Import.EntityParser;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class EmpleadoImport : SecuredImportPage
    {
        protected override string RedirectUrl { get { return "EmpleadoLista.aspx"; } }
        protected override string VariableName { get { return "PAR_EMPLEADOS"; } }
        protected override string GetRefference() { return "EMPLEADO"; }

        protected override void ValidateFilters()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
        }

        protected override void Import(List<ImportRow> rows)
        {
            var empresa = cbEmpresa.Selected;
            var linea = cbLinea.Selected;

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var parser = new EmpleadoV1(DAOFactory);

                    foreach (var row in rows)
                    {
                        Data data = GetLogiclinkData(Entities.Empleado, row);
                        var empleado = parser.Parse(empresa, linea, data);
                        parser.Save(empleado, empresa, linea, data);
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
            return _default ?? (_default = new List<FieldValue>
                                               {
                                                   new FieldValue("Base", Properties.Empleado.Linea, Entities.Empleado),
                                                   new FieldValue("Tipo de Empleado", Properties.Empleado.TipoEmpleado, Entities.Empleado) {RequiredValue = true},
                                                   new FieldValue("Legajo", Properties.Empleado.Legajo, Entities.Empleado) { RequiredValue = true, RequiredMapping = true, DisableDefault = true },
                                                   new FieldValue("Apellido", Properties.Empleado.Apellido, Entities.Empleado) {RequiredValue = true},
                                                   new FieldValue("Nombre", Properties.Empleado.Nombre, Entities.Empleado),
                                                   new FieldValue("Tipo de Documento", Properties.Empleado.TipoDocumento, Entities.Empleado),
                                                   new FieldValue("Nro Documento", Properties.Empleado.NroDocumento, Entities.Empleado) {RequiredValue = true},
                                                   new FieldValue("Departamento", Properties.Empleado.Departamento, Entities.Empleado),
                                                   new FieldValue("Transportista", Properties.Empleado.Transportista, Entities.Empleado),
                                                   new FieldValue("Centro de costos", Properties.Empleado.CentroDeCosto, Entities.Empleado),
                                                   new FieldValue("Reporta 1", Properties.Empleado.Reporta1, Entities.Empleado),
                                                   new FieldValue("Reporta 2", Properties.Empleado.Reporta2, Entities.Empleado),
                                                   new FieldValue("Reporta 3", Properties.Empleado.Reporta3, Entities.Empleado),
                                                   new FieldValue("Categoria", Properties.Empleado.Categoria, Entities.Empleado),
                                                   new FieldValue("Tarjeta", Properties.Empleado.Tarjeta, Entities.Empleado),
                                                   new FieldValue("Antiguedad", Properties.Empleado.Antiguedad, Entities.Empleado),
                                                   new FieldValue("Art", Properties.Empleado.Art, Entities.Empleado), new FieldValue("Licencia", Properties.Empleado.Licencia, Entities.Empleado),
                                                   new FieldValue("Mail", Properties.Empleado.Mail, Entities.Empleado),
                                                   new FieldValue("Es Responsable", Properties.Empleado.EsResponsable, Entities.Empleado),
                                                   new FieldValue("Cuil", Properties.Empleado.Cuil, Entities.Empleado)
                                               });
        } 
    }
}