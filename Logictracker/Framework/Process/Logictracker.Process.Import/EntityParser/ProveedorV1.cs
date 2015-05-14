using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Process.Import.EntityParser
{
    public class ProveedorV1 : EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Proveedor"; }
        }

        public ProveedorV1()
        {
        }

        public ProveedorV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var item = GetProveedor(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return item;

            if (item.Id == 0)
            {
                item.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                item.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
            }

            for (var i = 0; i < data.Properties.Length; i++)
            {
                var property = data.Properties[i];
                var value = data.Values[i];

                switch (property)
                {
                    case Properties.Proveedor.Codigo: item.Codigo = value.Truncate(32); break;
                    case Properties.Proveedor.Descripcion: item.Descripcion = value.Truncate(50); break;
                    case Properties.Proveedor.Linea: item.Linea = GetLinea(empresa, value) ?? item.Linea; break;
                    case Properties.Proveedor.TipoProveedor: item.TipoProveedor = DaoFactory.TipoProveedorDAO.FindByCode(new[] { empresa }, new[] { linea }, value) ?? item.TipoProveedor; break;
                }
            }

            return item;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Proveedor;
            if (ValidateSaveOrUpdate(item)) DaoFactory.ProveedorDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Proveedor;
            if (ValidateDelete(item)) DaoFactory.ProveedorDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Proveedor;
            if (ValidateSave(item)) DaoFactory.ProveedorDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Proveedor;
            if (ValidateUpdate(item)) DaoFactory.ProveedorDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Proveedor GetProveedor(int empresa, int linea, IData data)
        {
            string codigo = null;
            for (var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.Proveedor.Codigo) codigo = data.Values[i];

            if (codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.ProveedorDAO.FindByCode(new[] { empresa }, new[] { linea }, codigo);
            return sameCode ?? new Proveedor();
        }
    }
}
