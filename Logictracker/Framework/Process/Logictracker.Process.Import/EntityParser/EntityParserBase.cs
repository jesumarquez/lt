using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Process.Import.EntityParser
{
    public abstract class EntityParserBase : IEntityParser
    {
        private DAOFactory _daoFactory;
        protected DAOFactory DaoFactory
        {
            get { return _daoFactory ?? (_daoFactory = new DAOFactory()); }
        }

        protected EntityParserBase()
        {}
        protected EntityParserBase(DAOFactory daoFactory)
        {
            _daoFactory = daoFactory;
        }

        protected abstract string EntityName { get;}


        #region IEntityParser Members

        public abstract object Parse(int empresa, int linea, IData data);
        public abstract void SaveOrUpdate(object parsedData, int empresa, int linea, IData data);
        public abstract void Delete(object parsedData, int empresa, int linea, IData data);
        public abstract void Save(object parsedData, int empresa, int linea, IData data);
        public abstract void Update(object parsedData, int empresa, int linea, IData data);

        #endregion

        protected Linea GetLinea(int empresa, string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return DaoFactory.LineaDAO.FindByCodigo(empresa, value);
        }       
        
        protected void ThrowSave()
        {
            throw new EntityParserException("No se puede actualizar " + EntityName + " porque es NULL.");
        }
        protected void ThrowCodigo()
        {
            ThrowProperty("Codigo");
        }
        protected void ThrowProperty(string property)
        {
            throw new EntityParserException("No se encuentra el campo '" + property + "' para el elemento de tipo " + EntityName);
        }
        protected void ThrowMessage(string message, params object[] par)
        {
            throw new EntityParserException(" [Tipo " + EntityName + "] "+string.Format(message, par));
        }
        public bool ValidateSaveOrUpdate<T>(T parsedData) where T : class, IAuditable
        {
            if (parsedData == null) ThrowSave();
            return true;
        }

        public bool ValidateDelete<T>(T parsedData) where T : class,IAuditable
        {
            if (parsedData == null) ThrowSave();
            return parsedData.Id != 0;
        }

        public bool ValidateSave<T>(T parsedData) where T : class, IAuditable
        {
            if (parsedData == null) ThrowSave();
            return parsedData.Id == 0;
        }

        public bool ValidateUpdate<T>(T parsedData) where T : class, IAuditable
        {
            if (parsedData == null) ThrowSave();
            return parsedData.Id != 0;
        }

    }
}
