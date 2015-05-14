using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Process.Import.EntityParser
{
    public class TipoInsumoV1: EntityParserBase
    {

        public TipoInsumoV1()
        {
        }

        public TipoInsumoV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }
        #region IEntityParser Members

        protected override string EntityName
        {
            get { return "Tipo de Insumo"; }
        }

        public override object Parse(int empresa, int linea, IData data)
        {
            var tipo = GetTipoInsumo(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return tipo;

            if(tipo.Id == 0)
            {
                tipo.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                tipo.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
            }

            for (var i = 0; i < data.Properties.Length; i++)
            {
                var property = data.Properties[i];
                var value = data.Values[i];
                
                switch(property)
                {
                    case Properties.TipoInsumo.Codigo: tipo.Codigo = value.Truncate(32); break;
                    case Properties.TipoInsumo.Descripcion: tipo.Descripcion = value.Truncate(50); break;
                    case Properties.TipoInsumo.DeCombustible: tipo.DeCombustible = value.AsBool(); break;
                    case Properties.TipoInsumo.Linea: tipo.Linea = GetLinea(empresa, value) ?? tipo.Linea; break;
                }
            }
            
            return tipo;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoInsumo;
            if(ValidateSaveOrUpdate(tipo)) DaoFactory.TipoInsumoDAO.SaveOrUpdate(tipo);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoInsumo;
            if(ValidateDelete(tipo)) DaoFactory.TipoInsumoDAO.Delete(tipo);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoInsumo;
            if(ValidateSave(tipo)) DaoFactory.TipoInsumoDAO.SaveOrUpdate(tipo);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as TipoInsumo;
            if(ValidateUpdate(tipo)) DaoFactory.TipoInsumoDAO.SaveOrUpdate(tipo);
        }

        #endregion

        protected virtual TipoInsumo GetTipoInsumo(int empresa, int linea, IData data)
        {
            string codigo = null;
            for(var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.TipoInsumo.Codigo) codigo = data.Values[i];

            if (codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.TipoInsumoDAO.FindByCode(new[] { empresa }, new[] { linea }, codigo);
            return sameCode ?? new TipoInsumo();
        }
    }

}
