using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;
using System;

namespace Logictracker.Process.Import.EntityParser
{
    public class BaseV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Base"; }
        }
        public BaseV1()
        {
        }

        public BaseV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }
        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var baze = GetBase(empresa, data);
            if (data.Operation == (int)Operation.Delete) return baze;

            if (baze.Id == 0)
            {
                baze.Baja = false;
            }

            if(empresa <= 0)
            {
                ThrowProperty("Distrito");
            }
            baze.Descripcion = data.AsString(Properties.Base.Descripcion, 255) ??
                               data.AsString(Properties.Base.Codigo, 255);
            baze.DescripcionCorta = data.AsString(Properties.Base.Codigo, 8);
            baze.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
            baze.Mail = data.AsString(Properties.Base.Mail, 64) ?? string.Empty;
            baze.Telefono = data.AsString(Properties.Base.Telefono, 32) ?? string.Empty;
            var georef = data.AsString(Properties.Base.ReferenciaGeografica, 32);
            if(!string.IsNullOrEmpty(georef))
            {
                baze.ReferenciaGeografica = DaoFactory.ReferenciaGeograficaDAO.FindByCodigo(new[] { empresa }, new[] { -1 }, new[] { -1 }, georef);
            }
            var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => Convert.ToInt32(x.BaseUtcOffset.TotalHours) == data.AsInt32(Properties.Distrito.Gmt));
            if (timeZoneInfo != null) baze.TimeZoneId = timeZoneInfo.Id;
            return baze;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Linea;
            if (ValidateSaveOrUpdate(item)) DaoFactory.LineaDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Linea;
            if (ValidateDelete(item)) DaoFactory.LineaDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Linea;
            if (ValidateSave(item)) DaoFactory.LineaDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Linea;
            if (ValidateUpdate(item)) DaoFactory.LineaDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Linea GetBase(int empresa, IData data)
        {
            var codigo = data[Properties.Base.Codigo];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.LineaDAO.FindByCodigo(empresa, codigo);
            return sameCode ?? new Linea();
        }
    }

    
}
