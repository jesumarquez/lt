using System;
using System.Web.Services;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.Handlers;
using Logictracker.Web.BaseClasses.BasePages;
using LogicOut.Server;

/// <summary>
/// Summary description for Export
/// </summary>
[WebService(Namespace = "http://plataforma.logictracker.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Export : BaseWebService
{

    public Export()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public Respuesta<OutData[]> ExportData(string sessionId, string company, string branch, string query, string parameters)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceExport);

            var emp = GetEmpresaByCode(company);
            var empresa = emp.Id;
            var linea = string.IsNullOrEmpty(branch) ? -1 : GetLineaByCode(branch, emp).Id;

            var factory = new QueryFactory(DAOFactory);
            var data = factory.GetData(empresa, linea, query, parameters);

            return Respuesta<OutData[]>.Create(data);
        }
        catch (Exception ex)
        {
            return Respuesta<OutData[]>.CreateError(ex.ToString());
        }
    }
    [WebMethod]
    public Respuesta<bool> Done(string sessionId, string company, string branch, string query, string parameters)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceExport);

            var emp = GetEmpresaByCode(company);
            var empresa = emp.Id;
            var linea = string.IsNullOrEmpty(branch) ? -1 : GetLineaByCode(branch, emp).Id;

            var factory = new QueryFactory(DAOFactory);
            var data = factory.Done(empresa, linea, query, parameters, Usuario);

            return Respuesta<bool>.Create(data);
        }
        catch (Exception ex)
        {
            return Respuesta<bool>.CreateError(ex.ToString());
        }
    }
}