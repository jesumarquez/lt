using System;
using System.Linq;
using System.Web.Services;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.BaseClasses.Handlers;
using Geocoder.Core.VO;

namespace Logictracker
{
    /// <summary>
    /// Summary description for Geocoder
    /// </summary>
    [WebService(Namespace = "http://plataforma.logictracker.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class GeocoderWs : BaseWebService
    {
        [WebMethod]
        public Respuesta<ProvinciaVO[]> GetProvincias(string sessionId)
        {
            try
            {
                ValidateLoginInfo(sessionId, Securables.WebServiceGeocoder);
                var provincias = GeocoderHelper.BuscarProvincias().ToArray();

                return Respuesta<ProvinciaVO[]>.Create(provincias);
            }
            catch (Exception ex)
            {
                return Respuesta<ProvinciaVO[]>.CreateError(ex.Message);
            }
        }


        [WebMethod]
        public Respuesta<DireccionVO[]> GetDireccion(string sessionId, string calle, int altura, string esquina, string partido, int provincia)
        {
            try
            {
                ValidateLoginInfo(sessionId, Securables.WebServiceGeocoder);

                var direcciones = GeocoderHelper.GetDireccion(calle, altura, esquina, partido, provincia).ToArray();

                return Respuesta<DireccionVO[]>.Create(direcciones);
            }
            catch (Exception ex)
            {
                return Respuesta<DireccionVO[]>.CreateError(ex.Message);
            }
        }

        [WebMethod]
        public Respuesta<DireccionVO[]> GetSmartSearch(string sessionId, string frase)
        {
            try
            {
                ValidateLoginInfo(sessionId, Securables.WebServiceGeocoder);

                var direcciones = GeocoderHelper.GetDireccionSmartSearch(frase).ToArray();

                return Respuesta<DireccionVO[]>.Create(direcciones);
            }
            catch (Exception ex)
            {
                return Respuesta<DireccionVO[]>.CreateError(ex.Message);
            }
        }
    }
}

