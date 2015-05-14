using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.Import.EntityParser
{
    public class LineaTelefonicaV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "LineaTelefonica"; }
        }
        public LineaTelefonicaV1()
        {
        }

        public LineaTelefonicaV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var lineatel = GetLineaTelefonica(data);
            if (data.Operation == (int)Operation.Delete) return lineatel;          

            lineatel.NumeroLinea = data.AsString(Properties.LineaTelefonica.Numero, 128);
            lineatel.Imei = data.AsString(Properties.LineaTelefonica.Numero, 32) ?? lineatel.Imei;

            return lineatel;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as LineaTelefonica;
            if (ValidateSaveOrUpdate(item)) DaoFactory.LineaTelefonicaDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as LineaTelefonica;
            if (ValidateDelete(item)) DaoFactory.LineaTelefonicaDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as LineaTelefonica;
            if (ValidateSave(item)) DaoFactory.LineaTelefonicaDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as LineaTelefonica;
            if (ValidateUpdate(item)) DaoFactory.LineaTelefonicaDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual LineaTelefonica GetLineaTelefonica(IData data)
        {
            var codigo = data[Properties.LineaTelefonica.Numero];
            if(codigo == null) ThrowProperty("Numero");

            var sameCode = DaoFactory.LineaTelefonicaDAO.FindByNumero(codigo);
            return sameCode ?? new LineaTelefonica();
        }
    }

    
}
