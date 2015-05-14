using System.Web.Services;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.Handlers;

namespace Logictracker.Web.BaseClasses.BasePages
{
    public class BaseWebService: WebService
    {
        protected BaseService BaseService { get; private set; }
        public DAOFactory DAOFactory { get { return BaseService.DAOFactory; } }
        public ReportFactory ReportFactory { get { return BaseService.ReportFactory; } }

        public Usuario Usuario { get { return BaseService.Usuario; } }


        public BaseWebService()
        {
            BaseService = new BaseService();
        }

        [WebMethod]
        public Respuesta<string> Login(string username, string password)
        {
            return BaseService.Login(username, password);
        }
        [WebMethod]
        public Respuesta<bool> IsActive(string sessionId)
        {
            return BaseService.IsActive(sessionId);
        }
        [WebMethod]
        public Respuesta<bool> Logout(string sessionId)
        {
            return BaseService.Logout(sessionId);
        }
        
        
        public LoginInfo GetLoginInfo(string sessionId)
        {
            return BaseService.GetLoginInfo(sessionId);
        }

        public LoginInfo ValidateLoginInfo(string sessionId, string securable)
        {
            return BaseService.ValidateLoginInfo(sessionId, securable);
        }
        public LoginInfo ValidateLoginInfo(string sessionId)
        {
            return BaseService.ValidateLoginInfo(sessionId);
        }

        public void ValidateExpiration(string uniqueKey, int seconds)
        {
            BaseService.ValidateExpiration(uniqueKey, seconds);
        }

        public Empresa GetEmpresaByCode(string company)
        {
            return BaseService.GetEmpresaByCode(company);
        }

        public Linea GetLineaByCode(string branch, Empresa company)
        {
            return BaseService.GetLineaByCode(branch, company);
        }

        public Coche GetCocheByInterno(Empresa empresa, Linea linea, string interno)
        {
            return BaseService.GetCocheByInterno(empresa, linea, interno);
        }
        public Coche GetCocheByPatente(Empresa empresa, Linea linea, string patente)
        {
            return BaseService.GetCocheByPatente(empresa, linea, patente);
        }
        public Coche GetCocheByPatente(int empresa, string patente)
        {
            return BaseService.GetCocheByPatente(empresa, patente);
        }
        public Cliente GetClienteByCode(string codigo)
        {
            return BaseService.GetClienteByCode(codigo);
        }

        public PuntoEntrega GetPuntoEntregaByCode(string codigo, Cliente client)
        {
            return BaseService.GetPuntoEntregaByCode(codigo, client);
        }

        public Posicion GetPosicion(Coche coche)
        {
            return BaseService.GetPosicion(coche);
        }
    }
}
