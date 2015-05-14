using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Process.Import.EntityParser
{
     public class TipoProveedorV1: EntityParserBase
    {
         protected override string EntityName
         {
             get { return "Tipo de Proveedor"; }
         }

         public TipoProveedorV1()
        {
        }

         public TipoProveedorV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var tipo = GetTipoProveedor(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return tipo;

            if(tipo.Id == 0)
            {
                tipo.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                tipo.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
                tipo.Baja = false;
            }

            for (var i = 0; i < data.Properties.Length; i++)
            {
                var property = data.Properties[i];
                var value = data.Values[i];
                
                switch(property)
                {
                    case Properties.TipoProveedor.Codigo: tipo.Codigo = value.Truncate(32); break;
                    case Properties.TipoProveedor.Descripcion: tipo.Descripcion = value.Truncate(64); break;
                    case Properties.TipoProveedor.Linea: tipo.Linea = GetLinea(empresa, value) ?? tipo.Linea; break;
                }
            }
            
            return tipo;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoProveedor;
            if(ValidateSaveOrUpdate(tipo)) DaoFactory.TipoProveedorDAO.SaveOrUpdate(tipo);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoProveedor;
            if(ValidateDelete(tipo)) DaoFactory.TipoProveedorDAO.Delete(tipo);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoProveedor;
            if(ValidateSave(tipo)) DaoFactory.TipoProveedorDAO.SaveOrUpdate(tipo);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoProveedor;
            if(ValidateUpdate(tipo)) DaoFactory.TipoProveedorDAO.SaveOrUpdate(tipo);
        }

        #endregion

        protected virtual TipoProveedor GetTipoProveedor(int empresa, int linea, IData data)
        {
            string codigo = null;
            for(var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.TipoProveedor.Codigo) codigo = data.Values[i];
            
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.TipoProveedorDAO.FindByCode(new[] { empresa }, new[] { linea }, codigo);
            return sameCode ?? new TipoProveedor();
        }
    }

}
