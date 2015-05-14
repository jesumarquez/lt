using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.Import.EntityParser
{
    public class UnidadMedidaV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Unidad de Medida"; }
        }
        public UnidadMedidaV1()
        {
        }

        public UnidadMedidaV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var unidad = GetUnidadMedida(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return unidad;

            for (var i = 0; i < data.Properties.Length; i++)
            {
                var property = data.Properties[i];
                var value = data.Values[i];
                
                switch(property)
                {
                    case Properties.UnidadMedida.Codigo: unidad.Codigo = value.Truncate(64); break;
                    case Properties.UnidadMedida.Descripcion: unidad.Descripcion = value.Truncate(64); break;
                    case Properties.UnidadMedida.Simbolo: unidad.Simbolo = value.Truncate(64); break;
                }
            }           

            return unidad;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var unidad = parsedData as UnidadMedida;
            if(ValidateSaveOrUpdate(unidad)) DaoFactory.UnidadMedidaDAO.SaveOrUpdate(unidad);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var unidad = parsedData as UnidadMedida;
            if(ValidateDelete(unidad)) DaoFactory.UnidadMedidaDAO.Delete(unidad);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var unidad = parsedData as UnidadMedida;
            if(ValidateSave(unidad)) DaoFactory.UnidadMedidaDAO.SaveOrUpdate(unidad);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var unidad = parsedData as UnidadMedida;
            if(ValidateUpdate(unidad)) DaoFactory.UnidadMedidaDAO.SaveOrUpdate(unidad);
        }

        #endregion

        protected virtual UnidadMedida GetUnidadMedida(int empresa, int linea, IData data)
        {
            string codigo = null;
            for(var i = 0; i < data.Properties.Length; i++)
                if (data.Properties[i] == Properties.UnidadMedida.Codigo) codigo = data.Values[i];
            
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.UnidadMedidaDAO.FindByCode(codigo);
            return sameCode ?? new UnidadMedida();
        }
    }

}
