using System;
using System.Web.Services;
using Logictracker.Process.Import.Service;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.Handlers;
using Logictracker.Web.BaseClasses.BasePages;

/// <summary>
/// Summary description for Import
/// </summary>
[WebService(Namespace = "http://web.logictracker.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Import : BaseWebService
{

    public Import()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public Respuesta<bool> ImportData(string sessionId, string company, string branch, string data, int version)
    {
        try
        {
            //BaseService.ValidateLoginInfo(sessionId, Securables.WebServiceImport);

            var empresa = BaseService.GetEmpresaByCode(company);
            var linea = string.IsNullOrEmpty(branch) ? null : BaseService.GetLineaByCode(branch, empresa);

            var server = new Server();
            server.Import(empresa.Id, linea != null ? linea.Id: -1, data, version);

            return Respuesta<bool>.Create(true);
        }
        catch (Exception ex)
        {
            return Respuesta<bool>.CreateError(ex.ToString());
        }
    }
}
