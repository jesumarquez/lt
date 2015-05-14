using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Process.Import.EntityParser
{
    public class InsumoV1: EntityParserBase
    {
         protected override string EntityName
         {
             get { return "Insumo"; }
         }

        public InsumoV1()
        {
        }

        public InsumoV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var item = GetInsumo(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return item;

            if(item.Id == 0)
            {
                item.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                item.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
            }

            for (var i = 0; i < data.Properties.Length; i++)
            {
                var property = data.Properties[i];
                var value = data.Values[i];
                
                switch(property)
                {
                    case Properties.Insumo.Codigo: item.Codigo = value.Truncate(32); break;
                    case Properties.Insumo.Descripcion: item.Descripcion = value.Truncate(50); break;
                    case Properties.Insumo.Linea: item.Linea = GetLinea(empresa, value) ?? item.Linea; break;
                    case Properties.Insumo.TipoInsumo: item.TipoInsumo = DaoFactory.TipoInsumoDAO.FindByCode(new[] { empresa }, new[] { linea }, value) ?? item.TipoInsumo; break;
                    case Properties.Insumo.UnidadMedida: item.UnidadMedida = DaoFactory.UnidadMedidaDAO.FindByCode(value) ?? item.UnidadMedida; break;
                }
            }
            return item;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as Insumo;
            if (ValidateSaveOrUpdate(tipo)) DaoFactory.InsumoDAO.SaveOrUpdate(tipo);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as Insumo;
            if (ValidateDelete(tipo)) DaoFactory.InsumoDAO.Delete(tipo);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as Insumo;
            if (ValidateSave(tipo)) DaoFactory.InsumoDAO.SaveOrUpdate(tipo);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as Insumo;
            if (ValidateUpdate(tipo)) DaoFactory.InsumoDAO.SaveOrUpdate(tipo);
        }

        #endregion

        protected virtual Insumo GetInsumo(int empresa, int linea, IData data)
        {
            string codigo = null;
            for(var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.Insumo.Codigo) codigo = data.Values[i];
            
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.InsumoDAO.FindByCode(new[] { empresa }, new[] { linea }, codigo);
            return sameCode ?? new Insumo();
        }
    }
}
