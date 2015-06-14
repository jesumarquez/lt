#region Usings

using System;
using System.Linq;
using System.Web.Services;
using Logictracker.Services.Helpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.Handlers;

#endregion

/// <summary>
/// Summary description for OperationService
/// </summary>
[WebService(Namespace = "http://plataforma.logictracker.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class OperationService : BaseWebService
{
    /// <summary>
    /// Devuelve la lista de móviles más cercanos al punto (latitud, longitud) 
    /// y la distancia en metros (vuelo de pájaro) a cada uno hasta “cantidad” de resultados.
    /// </summary>
    /// <param name="latitud">Latitud del punto</param>
    /// <param name="longitud">Longitud del punto</param>
    /// <param name="cantidad">Cantidad máxima de moviles a devolver</param>
    /// <returns>{string interno, double distancia}[cantidad]</returns>
    [WebMethod]
    public Respuesta<Distancia[]> GetMovilesCercanos(double latitud, double longitud, int cantidad, string empresa, string linea)
    {
        try
        {
            var emp = DAOFactory.EmpresaDAO.FindByCodigo(empresa);
            if (emp == null) return Respuesta<Distancia[]>.CreateError("Empresa " + empresa + " no encontrada");
            var lin = DAOFactory.LineaDAO.FindByCodigo(emp.Id, linea);
            if (lin == null) return Respuesta<Distancia[]>.CreateError("Linea " + linea + " no encontrada");
            if (!lin.Interfaceable) return Respuesta<Distancia[]>.CreateError("La Linea " + linea + " no acepta consultas de esta interface");
        
            var coches = DAOFactory.CocheDAO.FindList(new[]{emp.Id}, new[]{lin.Id});
            if (!coches.Any())
                return Respuesta<Distancia[]>.CreateError("No se encontraron móviles para " + empresa + " - " + linea);

            var pois = ReportFactory.MobilePoiDAO.GetMobilesNearPoint(coches, latitud, longitud, 10000);

            if (pois.Count > cantidad) pois.RemoveRange(cantidad, pois.Count - cantidad);

            var distancias = pois.Select(p => new Distancia { Interno = p.Interno, Metros = p.Distancia }).ToArray();
            return Respuesta<Distancia[]>.Create(distancias);
        }
        catch (Exception ex)
        {
            return Respuesta<Distancia[]>.CreateError(ex.Message);
        }
    }

    /// <summary>
    /// Devuelve un string con la dirección de la esquina más cercana a la última posición del móvil 
    /// con el interno dado o vacío si la misma no pudo ser encontrada. 
    /// </summary>
    /// <param name="interno">interno del movil</param>
    /// <param name="empresa"></param>
    /// <param name="linea"></param>
    /// <returns></returns>
    [WebMethod]
    public Respuesta<string> GetEsquinaMasCercana(string interno, string empresa, string linea)
    {
        try
        {
            var emp = DAOFactory.EmpresaDAO.FindByCodigo(empresa);
            if (emp == null) return Respuesta<string>.CreateError("Empresa " + empresa + " no encontrada");
            var lin = DAOFactory.LineaDAO.FindByCodigo(emp.Id, linea);
            if (lin == null) return Respuesta<string>.CreateError("Linea " + linea + " no encontrada");
            if (!lin.Interfaceable) return Respuesta<string>.CreateError("La Linea " + linea + " no acepta consultas de esta interface");

            var coche = DAOFactory.CocheDAO.FindByInterno(new[]{emp.Id}, new[]{lin.Id}, interno);
            if (coche == null) return Respuesta<string>.CreateError("No se encontraro el movil con interno " + interno);

            var pos = DAOFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);
            if (pos == null) return Respuesta<string>.CreateError("No se encontro posicion para el movil " + interno);
            var dir = GeocoderHelper.GetEsquinaMasCercana(pos.Latitud, pos.Longitud);
            return Respuesta<string>.Create(dir == null
                                                ? string.Format("({0}, {1})", pos.Latitud, pos.Longitud)
                                                : dir.Direccion);
        }
        catch (Exception ex)
        {
            return Respuesta<string>.CreateError(ex.Message);
        }
    }

    [WebMethod]
    public Respuesta<Posicion[]> GetLocationByCompany(string company)
    {
        return GetLocationByBranch(company, string.Empty);
    }
    [WebMethod]
    public Respuesta<Posicion[]> GetLocationByBranch(string company, string branch)
    {
        try
        {
            var time = CheckTime<Posicion[]>("GetPosiciones", company, branch);
            if (!time.RespuestaOk) return time;

            var emp = DAOFactory.EmpresaDAO.FindByCodigo(company);
            if (emp == null) return Respuesta<Posicion[]>.CreateError("Empresa " + company + " no encontrada");
            Linea lin = null;
            if (branch != string.Empty)
            {
                lin = DAOFactory.LineaDAO.FindByCodigo(emp.Id, branch);
                if (lin == null) return Respuesta<Posicion[]>.CreateError("Linea " + branch + " no encontrada");
                if (!lin.Interfaceable) return Respuesta<Posicion[]>.CreateError("La Linea " + branch + " no acepta consultas de esta interface");
            }

            var coches = DAOFactory.CocheDAO.FindList(new[]{emp.Id}, new[]{lin != null? lin.Id : -1})
                .Where(c => c.Dispositivo != null);

            var posiciones = coches.Select(coche => GetPosicion(coche));
            
            return Respuesta<Posicion[]>.Create(posiciones.Where(p=> p != null).ToArray());
        }
        catch (Exception ex)
        {
            return Respuesta<Posicion[]>.CreateError(ex.Message);
        }
    }

    [WebMethod]
    public Respuesta<Posicion> GetLocationByVehicle(string company, string branch, string vehicle)
    {
        try
        {
            var time = CheckTime<Posicion>("GetPosiciones", company, branch);
            if(!time.RespuestaOk) return time;

            var emp = DAOFactory.EmpresaDAO.FindByCodigo(company);
            if (emp == null) return Respuesta<Posicion>.CreateError("Empresa " + company + " no encontrada");
            Linea lin = null;
            if (branch != string.Empty)
            {
                lin = DAOFactory.LineaDAO.FindByCodigo(emp.Id, branch);
                if (lin == null) return Respuesta<Posicion>.CreateError("Linea " + branch + " no encontrada");
                if (!lin.Interfaceable) return Respuesta<Posicion>.CreateError("La Linea " + branch + " no acepta consultas de esta interface");
            }

            var coche = DAOFactory.CocheDAO.FindByInterno(new[] { emp.Id }, new[] { lin != null ? lin.Id : -1 }, vehicle);
            if(coche == null)return Respuesta<Posicion>.CreateError("No se encontro el vehiculo " + vehicle);

            return Respuesta<Posicion>.Create(GetPosicion(coche));
        }
        catch (Exception ex)
        {
            return Respuesta<Posicion>.CreateError(ex.Message);
        }
    
    }

    private Respuesta<T> CheckTime<T>(string function, string company, string branch)
    {
        var key = string.Concat("App_Services.",function,"[", company, ",", branch, "]");
        var lastTime = Application[key];

        if (lastTime != null)
        {
            var last = Convert.ToDateTime(lastTime);
            if (DateTime.UtcNow.Subtract(last) < TimeSpan.FromMinutes(1))
                return Respuesta<T>.CreateError("Ultima consulta: " + last.ToString("HH:mm:ss (UTC)"));
        }

        Application[key] = DateTime.UtcNow;
        return Respuesta<T>.Create(default(T));
    }
    private new Posicion GetPosicion(Coche coche)
    {
        var pos = DAOFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);
        if(pos == null) return null;
        return new Posicion
                   {
                       Fecha = pos.FechaMensaje,
                       Interno = coche.Interno,
                       Patente = coche.Patente,
                       Latitud = pos.Latitud,
                       Longitud = pos.Longitud,
                       Velocidad = pos.Velocidad
                   };
    }

    [Serializable]
    public new class Posicion
    {
        public string Interno { get; set; }
        public string Patente { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public int Velocidad { get; set; }
        public DateTime Fecha { get; set; }
    }
    [Serializable]
    public new class Distancia
    {
        public string Interno { get; set; }
        public double Metros { get; set; }
    }
}