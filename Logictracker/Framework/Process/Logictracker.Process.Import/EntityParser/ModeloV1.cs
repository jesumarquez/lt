using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Process.Import.EntityParser
{
    public class ModeloV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Modelo"; }
        }
        public ModeloV1()
        {
        }

        public ModeloV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var modelo = GetModelo(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return modelo;

            if (modelo.Id == 0)
            {
                // valores default
            }
            modelo.Codigo = data.AsString(Properties.Modelo.Codigo, 10);

            var marca = data.AsString(Properties.Modelo.Marca, 64);
            if (marca != null)
            {
                modelo.Marca = DaoFactory.MarcaDAO.GetByDescripcion(empresa, linea, marca);
            }
            if (modelo.Marca == null) { ThrowProperty("Marca"); }
            modelo.Empresa = modelo.Marca.Empresa;
            modelo.Linea = modelo.Marca.Linea;

            modelo.Descripcion = data.AsString(Properties.Modelo.Descripcion, 50) ?? modelo.Descripcion;
            if (modelo.Descripcion == null) { ThrowProperty("Descripcion"); }

            modelo.Rendimiento = data.AsDouble(Properties.Modelo.Rendimiento) ?? modelo.Rendimiento;
            modelo.Capacidad = data.AsDouble(Properties.Modelo.Capacidad) ?? modelo.Capacidad;
            modelo.Costo = data.AsInt32(Properties.Modelo.Costo) ?? modelo.Costo;
            modelo.VidaUtil = data.AsInt32(Properties.Modelo.VidaUtil) ?? modelo.VidaUtil;

            return modelo;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Modelo;
            if (ValidateSaveOrUpdate(item)) DaoFactory.ModeloDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Modelo;
            if (ValidateDelete(item)) DaoFactory.ModeloDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Modelo;
            if (ValidateSave(item)) DaoFactory.ModeloDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Modelo;
            if (ValidateUpdate(item)) DaoFactory.ModeloDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Modelo GetModelo(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.Modelo.Codigo];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.ModeloDAO.FindByCodigo(empresa, linea, codigo);
            return sameCode ?? new Modelo();
        }
    }

    
}
