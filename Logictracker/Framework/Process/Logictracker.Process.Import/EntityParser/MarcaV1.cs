using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Process.Import.EntityParser
{
    public class MarcaV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Marca"; }
        }
        public MarcaV1()
        {
        }

        public MarcaV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var marca = GetMarca(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return marca;

            if (marca.Id == 0)
            {
                // valores default
            }
            marca.Descripcion = data.AsString(Properties.Marca.Nombre, 64);

            var idlinea = data.AsInt32(Properties.Marca.Base);
            if(idlinea.HasValue) linea = idlinea.Value;
            marca.Empresa = empresa > 0 ? DaoFactory.EmpresaDAO.FindById(empresa) : null;
            marca.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;

            return marca;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Marca;
            if (ValidateSaveOrUpdate(item)) DaoFactory.MarcaDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Marca;
            if (ValidateDelete(item)) DaoFactory.MarcaDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Marca;
            if (ValidateSave(item)) DaoFactory.MarcaDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Marca;
            if (ValidateUpdate(item)) DaoFactory.MarcaDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Marca GetMarca(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.Marca.Nombre];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.MarcaDAO.GetByDescripcion(empresa, linea, codigo);
            return sameCode ?? new Marca();
        }
    }

    
}
