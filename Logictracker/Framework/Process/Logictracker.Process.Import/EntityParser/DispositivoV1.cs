using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Process.Import.EntityParser
{
    public class DispositivoV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Dispositivo"; }
        }
        public DispositivoV1()
        {
        }

        public DispositivoV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var dispositivo = GetDispositivo(empresa, data);
            if (data.Operation == (int)Operation.Delete) return dispositivo;

            if (dispositivo.Id == 0)
            {
                // valores default
                dispositivo.Tablas = string.Empty;
                dispositivo.Clave = string.Empty;
                dispositivo.Telefono = string.Empty;
            }
            var idlinea = data.AsInt32(Properties.Dispositivo.Base);
            if(idlinea.HasValue) linea = idlinea.Value;
            dispositivo.Empresa = empresa > 0 ? DaoFactory.EmpresaDAO.FindById(empresa) : null;
            dispositivo.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;

            var tipo = data.AsString(Properties.Dispositivo.TipoDispositivo, 50);
            
            dispositivo.Codigo = data.AsString(Properties.Dispositivo.Codigo, 64);
            dispositivo.Imei = data.AsString(Properties.Dispositivo.Imei, 32) ?? dispositivo.Imei;
            if (string.IsNullOrEmpty(dispositivo.Imei)) { ThrowProperty("Imei"); }
            var byImei = DaoFactory.DispositivoDAO.FindByImei(dispositivo.Imei);
            if(byImei != null && dispositivo.Id != byImei.Id)
            {
                ThrowMessage("Imei duplicado: {0}", dispositivo.Imei);
            }
            dispositivo.TipoDispositivo = DaoFactory.TipoDispositivoDAO.FindByModelo(tipo) ?? dispositivo.TipoDispositivo;
            if (dispositivo.TipoDispositivo == null) { ThrowProperty("TipoDispositivo"); }

            var estado = data.AsString(Properties.Dispositivo.Estado, 1);
            if(!string.IsNullOrEmpty(estado))
            {
                switch(estado.ToLower())
                {
                    case "o": dispositivo.Estado = Dispositivo.Estados.Activo; break;
                    case "i": dispositivo.Estado = Dispositivo.Estados.Inactivo; break;
                    case "m": dispositivo.Estado = Dispositivo.Estados.EnMantenimiento; break;
                }
            }

            var tel = data.AsString(Properties.Dispositivo.LineaTelefonica, 128);
            if(tel != null)
            {
                dispositivo.LineaTelefonica = DaoFactory.LineaTelefonicaDAO.FindByNumero(tel);
            }
            if (dispositivo.LineaTelefonica == null) { ThrowProperty("LineaTelefonica"); }
            dispositivo.Telefono = tel ?? string.Empty;
            return dispositivo;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Dispositivo;
            if (ValidateSaveOrUpdate(item)) DaoFactory.DispositivoDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Dispositivo;
            if (ValidateDelete(item)) DaoFactory.DispositivoDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Dispositivo;
            if (ValidateSave(item)) DaoFactory.DispositivoDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Dispositivo;
            if (ValidateUpdate(item)) DaoFactory.DispositivoDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Dispositivo GetDispositivo(int empresa, IData data)
        {
            var codigo = data[Properties.Dispositivo.Codigo];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.DispositivoDAO.FindByCode(codigo);
            return sameCode ?? new Dispositivo();
        }
    }

    
}
