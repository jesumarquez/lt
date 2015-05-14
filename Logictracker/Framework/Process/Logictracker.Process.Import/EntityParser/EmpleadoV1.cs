using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.Import.EntityParser
{
    public class EmpleadoV1: EntityParserBase
    {
        protected override string EntityName { get { return "Empleado"; } }

        public EmpleadoV1() {}
        public EmpleadoV1(DAOFactory daoFactory) : base(daoFactory) {}

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var empleado = GetEmpleado(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return empleado;

            if(empleado.Id == 0)
            {
                empleado.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                empleado.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
                empleado.Baja = false;
            }

            empleado.Legajo = data.AsString(Properties.Empleado.Legajo, 10);

            empleado.TipoEmpleado = DaoFactory.TipoEmpleadoDAO.FindByCodigo(empresa, linea, data[Properties.Empleado.TipoEmpleado]);
            if(empleado.TipoEmpleado == null) { ThrowProperty("TipoEmpleado"); }
            
            var departamento = data.AsString(Properties.Empleado.Departamento, 32);
            empleado.Departamento = departamento != null ? DaoFactory.DepartamentoDAO.FindByCodigo(empresa, linea, departamento) : null;

            var transportista = data.AsString(Properties.Empleado.Transportista, 64);
            empleado.Transportista = transportista != null ? DaoFactory.TransportistaDAO.FindByCodigo(empresa, linea, data[Properties.Empleado.Transportista]) : null;
            
            var centroDeCostos = data.AsString(Properties.Empleado.CentroDeCosto, 50);
            empleado.CentroDeCostos = centroDeCostos != null ? DaoFactory.CentroDeCostosDAO.FindByCodigo(empresa, linea, data[Properties.Empleado.CentroDeCosto]) : null;
            
            var categoria = data.AsString(Properties.Empleado.Categoria, 64);
            empleado.Categoria = categoria != null ? DaoFactory.CategoriaAccesoDAO.FindByName(new[] { empresa }, new[] { linea }, categoria) : null;

            var tarjeta = data.AsString(Properties.Empleado.Tarjeta, 50);
            empleado.Tarjeta = tarjeta != null ? DaoFactory.TarjetaDAO.FindByPin(new[] { empresa }, tarjeta) : null;

            var reporta1 = data.AsString(Properties.Empleado.Reporta1, 10);
            empleado.Reporta1 = categoria != null ? DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, reporta1) : null;

            var reporta2 = data.AsString(Properties.Empleado.Reporta2, 10);
            empleado.Reporta2 = categoria != null ? DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, reporta2) : null;

            var reporta3 = data.AsString(Properties.Empleado.Reporta3, 10);
            empleado.Reporta3 = categoria != null ? DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, reporta3) : null;

            empleado.Antiguedad = data.AsInt32(Properties.Empleado.Antiguedad) ?? 0;
            empleado.Art = data.AsString(Properties.Empleado.Art, 20) ?? string.Empty;
            empleado.Licencia = data.AsString(Properties.Empleado.Licencia, 15) ?? string.Empty;
            empleado.Mail = data.AsString(Properties.Empleado.Mail, 128) ?? string.Empty;
            empleado.EsResponsable = data.AsBool(Properties.Empleado.EsResponsable) ?? false;
            empleado.Entidad.Apellido = data.AsString(Properties.Empleado.Apellido, 255) ?? string.Empty;
            empleado.Entidad.Nombre = data.AsString(Properties.Empleado.Nombre, 128) ?? string.Empty;

            empleado.Entidad.TipoDocumento = data.AsString(Properties.Empleado.TipoDocumento, 10) ?? string.Empty;
            empleado.Entidad.NroDocumento = data.AsString(Properties.Empleado.NroDocumento, 13) ?? string.Empty;
            empleado.Entidad.Cuil = data.AsString(Properties.Empleado.Cuil, 13) ?? string.Empty;

            return empleado;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empleado;
            if(ValidateSaveOrUpdate(item)) DaoFactory.EmpleadoDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empleado;
            if(ValidateDelete(item)) DaoFactory.EmpleadoDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empleado;
            if(ValidateSave(item)) DaoFactory.EmpleadoDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empleado;
            if(ValidateUpdate(item)) DaoFactory.EmpleadoDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Empleado GetEmpleado(int empresa, int linea, IData data)
        {
            var legajo = data.AsString(Properties.Empleado.Legajo, 10);
            if(legajo == null) ThrowProperty("Legajo");

            var sameCode = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, legajo);
            return sameCode ?? new Empleado{Entidad = new Entidad()};
        }
    }

    
}
