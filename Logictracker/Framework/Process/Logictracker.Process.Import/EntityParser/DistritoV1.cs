using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.Import.EntityParser
{
    public class DistritoV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Distrito"; }
        }
        public DistritoV1()
        {
        }

        public DistritoV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }
        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var distrito = GetDistrito(data);
            if (data.Operation == (int)Operation.Delete) return distrito;

            if (distrito.Id == 0)
            {
                distrito.Baja = false;
            }

            distrito.Codigo = (data[Properties.Distrito.Codigo] ?? string.Empty).Truncate(50);
            distrito.RazonSocial = (data[Properties.Distrito.Descripcion] ?? string.Empty).Truncate(255);
            var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => Convert.ToInt32(x.BaseUtcOffset.TotalHours) == data.AsInt32(Properties.Distrito.Gmt));
            if (timeZoneInfo != null) distrito.TimeZoneId = timeZoneInfo.Id;
            return distrito;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empresa;
            if(ValidateSaveOrUpdate(item)) DaoFactory.EmpresaDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empresa;
            if (ValidateDelete(item)) DaoFactory.EmpresaDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empresa;
            if (ValidateSave(item)) DaoFactory.EmpresaDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Empresa;
            if (ValidateUpdate(item)) DaoFactory.EmpresaDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Empresa GetDistrito(IData data)
        {
            var codigo = data[Properties.Distrito.Codigo];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.EmpresaDAO.FindByCodigo(codigo);
            return sameCode ?? new Empresa();
        }
    }

    
}
