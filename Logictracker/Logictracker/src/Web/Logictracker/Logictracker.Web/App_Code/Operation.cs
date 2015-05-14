using System;
using System.Linq;
using System.Web.Services;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.Handlers;

/// <summary>
/// Summary description for Operation
/// </summary>
[WebService(Namespace = "http://plataforma.logictracker.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Operation : BaseWebService
{

    public Operation()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public Respuesta<Distancia[]> GetVehiclesNear(string sessionId, string company, string branch, double latitude, double longitude, int count)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceOperation);

            var empresa = GetEmpresaByCode(company);
            var linea = GetLineaByCode(branch, empresa);
       
            var coches = DAOFactory.CocheDAO.GetList(new[]{empresa.Id}, new[]{linea.Id});

            if (coches.Count == 0) throw new ApplicationException("No se encontraron móviles para " + empresa + " - " + linea);

            var pois = ReportFactory.MobilePoiDAO.GetMobilesNearPoint(coches, latitude, longitude, 10000);

            if (pois.Count > count) pois.RemoveRange(count, pois.Count - count);

            var distancias = pois.Select(p => new Distancia { Interno = p.Interno, Latitud = p.Latitud, Longitud = p.Longitud, Esquina = p.Esquina, Metros = p.Distancia }).ToArray();
            return Respuesta<Distancia[]>.Create(distancias);
        }
        catch (Exception ex)
        {
            return Respuesta<Distancia[]>.CreateError(ex.Message);
        }
    }


    [WebMethod]
    public Respuesta<Posicion[]> GetLocationByCompany(string sessionId, string company)
    {
        return GetLocationByBranch(sessionId, company, string.Empty);
    }
    [WebMethod]
    public Respuesta<Posicion[]> GetLocationByBranch(string sessionId, string company, string branch)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceOperation);
            ValidateExpiration(string.Concat("GetLocation[",company,",",branch,"]"), 60);

            var empresa = GetEmpresaByCode(company);
            var linea = string.IsNullOrEmpty(branch) ? null : GetLineaByCode(branch, empresa);

            var coches = DAOFactory.CocheDAO.GetList(new[] { empresa.Id }, new[] { linea != null ? linea.Id : -1 });

            if (coches.Count == 0) throw new ApplicationException("No se encontraron móviles para " + empresa + " - " + linea);

            var posiciones = coches.Select(coche => GetPosicion(coche)).Where(p => p != null).ToArray();

            return Respuesta<Posicion[]>.Create(posiciones);
        }
        catch (Exception ex)
        {
            return Respuesta<Posicion[]>.CreateError(ex.Message);
        }
    }

    [WebMethod]
    public Respuesta<Posicion> GetLocationByVehicle(string sessionId, string company, string branch, string vehicle)
    {
        try
        {
            ValidateLoginInfo(sessionId, Securables.WebServiceOperation);
            ValidateExpiration(string.Concat("GetLocation[", company, ",", branch, "]"), 60);

            var empresa = GetEmpresaByCode(company);
            var linea = GetLineaByCode(branch, empresa);

            var coche = DAOFactory.CocheDAO.GetList(new[] { empresa.Id }, new[] { linea.Id }).FirstOrDefault(c => c.Interno == vehicle);

            if (coche == null) throw new ApplicationException("No se encontro el vehiculo " + vehicle);

            return Respuesta<Posicion>.Create(GetPosicion(coche));
        }
        catch (Exception ex)
        {
            return Respuesta<Posicion>.CreateError(ex.Message);
        }
    }
}